import pnp, { Web, ODataEntityArray, CamlQuery, PermissionKind } from "sp-pnp-js";
import IDiscussion from "../models/IDiscussion";
import { IDiscussionReply, DiscussionPermissionLevel } from "../models/IDiscussionReply";

class SocialModule {

    private _discussionListServerRelativeUrl: string;

    /**
     * Initialize a new social module
     * @param listServerRelativeUrl the discussion board list server relative URL (e.g. '/sites/mysite/Lists/MyList')
     */
    public constructor(listServerRelativeUrl: string) {
        this._discussionListServerRelativeUrl = listServerRelativeUrl; 
    }

    /**
     * Ensure all script dependencies are loaded before using the taxonomy SharePoint CSOM functions
     * @return {Promise<void>}       A promise allowing you to execute your code logic.
     */
    public init(): Promise<void>  {

        // Initialize SharePoint script dependencies
        SP.SOD.registerSod("sp.runtime.js", "/_layouts/15/sp.runtime.js");
        SP.SOD.registerSod("sp.js", "/_layouts/15/sp.js");
        SP.SOD.registerSodDep("sp.js", "sp.runtime.js");

        const p = new Promise<void>((resolve) => {

            SP.SOD.loadMultiple(["sp.runtime.js", "sp.js"], () => {
                resolve();
            });
        });

        return p;
    }

    /**
     * Create a new discussion in a disucssion board list
     * @param discussion the discussion properties

     */
    public async createNewDiscussion(discussion: IDiscussion): Promise<number> {

        const p = new Promise<number>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().getList(this._discussionListServerRelativeUrl);

            const reply = SP.Utilities.Utility.createNewDiscussion(context, list, discussion.Title); 
            reply.set_item("Body", discussion.Body);
            reply.set_item("AssociatedPageId", discussion.AssociatedPageId);

            // Need to explicitly update the item to actually create it (doesn't work otherwise)
            reply.update();
            context.load(reply);

            context.executeQueryAsync(async () => {

                resolve(reply.get_id());
            }, (sender, args) => {
                reject(args.get_message());
            });
        });
        
        return p;
    }

    /**
     * Add a reply to an existing discussion
     * @param parentItemId the parent item id for this reply
     * @param replyBody the content of the reply
     */
    public async createNewDiscussionReply(parentItemId: number, replyBody: string): Promise<void>{

        const p = new Promise<void>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().getList(this._discussionListServerRelativeUrl);
            const parentItem = list.getItemById(parentItemId);
            
            const reply = SP.Utilities.Utility.createNewDiscussionReply(context, parentItem);
            reply.set_item("Body", replyBody);

            // Need to explicitly update the item to actually create it (doesn't work otherwise)
            reply.update();
            context.load(reply);
            context.executeQueryAsync(() => {
                resolve();
            }, (sender, args) => {
                reject(args.get_message());
            });
        });

        return p;
    }

    /**
     * Get a disucssion feed by id
     * @param id the id of discussion the root folder
     */
    public async getDiscussionById(associatedPageId: number): Promise<IDiscussion> {

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        try {

            const discussion = await web.getList(this._discussionListServerRelativeUrl).items.filter(`AssociatedPageId eq ${ associatedPageId }`).select("Id","Folder","AssociatedPageId").expand("Folder").top(1).get();
            if (discussion.length > 0) {
        
                // Get replies from this discussion (i.e. folder)
                const query: CamlQuery = {
                    'ViewXml': '<View><Query/></View>',
                    'FolderServerRelativeUrl': `${this._discussionListServerRelativeUrl}/${discussion[0].Folder.Name}`
                };
            
                const replies = await web.getList(this._discussionListServerRelativeUrl).getItemsByCAMLQuery(query);

                const discussionReplies: Promise<IDiscussionReply>[] = replies.map(async (reply) => {

                    

                    const userItemPermissions = await this.getCurrentUserPermissionsOnItem(reply.Id);
                
                    return {
                        Id: reply.Id,
                        ParentItemID: reply.ParentItemID,
                        AuthorId: reply.AuthorId,
                        Body: reply.Body,
                        UserPermissions: userItemPermissions
                    } as IDiscussionReply;
                });
       
                return {
                    AssociatedPageId: discussion[0].AssociatedPageId,
                    Title: discussion[0].Title,
                    Id: discussion[0].Id,
                    Replies: await Promise.all(discussionReplies),
                } as IDiscussion;

            } else {
                return null;
            }
        } catch (error) {
            throw error;
        }
    }

    /**
     * Delete a reply in an existing discussion
     * @param replyId the item id to delete
     */
    public async deleteReply(replyId: number): Promise<void>{

        try {
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);
            const discussion = await web.getList(this._discussionListServerRelativeUrl).items.getById(replyId).delete();
            return;

        } catch (error) {
            throw error;
        }
    }

    public async updateReply(reply: IDiscussionReply): Promise<void>{

        try {
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);
            const discussion = await web.getList(this._discussionListServerRelativeUrl).items.getById(reply.Id).update({
                "Body": reply.Body
            });
            
            return;

        } catch (error) {
            throw error;
        }
    }

    private async getCurrentUserPermissionsOnItem(itemId: number): Promise<DiscussionPermissionLevel[]> {

        let permissionsList = [];

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        const permissions = await web.getList(this._discussionListServerRelativeUrl).items.getById(itemId).getCurrentUserEffectivePermissions();

        const canAddListItems = web.hasPermissions(permissions, PermissionKind.AddListItems);
        const canEditListItems = web.hasPermissions(permissions, PermissionKind.EditListItems);
        const canDeleteListItems = web.hasPermissions(permissions, PermissionKind.DeleteListItems);
        const canManageLists = web.hasPermissions(permissions, PermissionKind.ManageLists);

        if (canManageLists)
            permissionsList.push(DiscussionPermissionLevel.ManageLists);

        if (canEditListItems && !canManageLists) {
            permissionsList.push(DiscussionPermissionLevel.Edit);

            // The "WriteSecurity" property isn't availabe through REST with SharePoint 2013. In this case, we need to get the whole list XML schema to extract this info
            // Not very efficient but we do not have any other option here
            // Not List Item Level Security is different than item permissions so we can rely on native REST methods
            // TODO: Implement specific behavior for SharePoint Online
            const list = await web.getList(this._discussionListServerRelativeUrl).select("SchemaXml").usingCaching({
                key: String.format("{0}_{1}", _spPageContextInfo.webServerRelativeUrl, "discussionBoardListSettings"),
                expiration: pnp.util.dateAdd(new Date(), "minute", 60),
                storeName: "local"})
                .get();
            const writeSecurity = /WriteSecurity="(\d)"/.exec(list.SchemaXml)[1];
            const currentUser =  await web.currentUser.select("LoginName").get();
            const item = await web.getList(this._discussionListServerRelativeUrl).items.getById(itemId).select("Author/Name").expand("Author/Name").get();

            if (writeSecurity.localeCompare("2") === 0) {
                // If the current user is the author of the comment
                if (item.Author.Name === currentUser.LoginName) {
                    permissionsList.push(DiscussionPermissionLevel.EditAsAuthor);
                }
            }
        }

        if (canDeleteListItems)
            permissionsList.push(DiscussionPermissionLevel.Delete);

        if (canAddListItems)
            permissionsList.push(DiscussionPermissionLevel.Add);

        return permissionsList;
    }

    public async getCurrentUserPermissionsOnList(listServerRelativeUrl: string): Promise<DiscussionPermissionLevel[]> {

        let permissionsList = [];

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);
        const permissions = await web.getList(listServerRelativeUrl).getCurrentUserEffectivePermissions();
        const canAddListItems = web.hasPermissions(permissions, PermissionKind.AddListItems);
        const canManageLists = web.hasPermissions(permissions, PermissionKind.ManageLists);

        if (canAddListItems)
            permissionsList.push(DiscussionPermissionLevel.Add);

        if (canManageLists)
            permissionsList.push(DiscussionPermissionLevel.ManageLists);
        
        return permissionsList;
    }
}

export default SocialModule;