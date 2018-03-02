import pnp, { Web, CamlQuery, PermissionKind, setup, ICachingOptions } from "sp-pnp-js";
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
     * @return {Promise<void>} A promise allowing you to execute your code logic.
     */
    public init(): Promise<void>  {

        // Initialize SharePoint script dependencies
        SP.SOD.registerSod("sp.runtime.js", "/_layouts/15/sp.runtime.js");
        SP.SOD.registerSod("sp.js", "/_layouts/15/sp.js");
        SP.SOD.registerSod("reputation.js", "/_layouts/15/reputation.js");
        SP.SOD.registerSodDep("reputation.js", "sp.js");
        SP.SOD.registerSodDep("sp.js", "sp.runtime.js");
        
        const p = new Promise<void>((resolve) => {

            SP.SOD.loadMultiple(["reputation.js","sp.runtime.js", "sp.js"], () => {
                resolve();
            });
        });

        return p;
    }

    /**
     * Create a new discussion in a disucssion board list
     * @param discussion the discussion properties
     */
    public async createNewDiscussion(associatedPageId: number, discussionTitle: string, discussionBody: string): Promise<IDiscussion> {

        const p = new Promise<IDiscussion>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().getList(this._discussionListServerRelativeUrl);

            const reply = SP.Utilities.Utility.createNewDiscussion(context, list, discussionTitle); 
            reply.set_item("Body", discussionBody);
            reply.set_item("AssociatedPageId", associatedPageId);

            // Need to explicitly update the item to actually create it (doesn't work otherwise)
            reply.update();
            context.load(reply, "Id","Author","Created","AssociatedPageId","Body","Title");

            context.executeQueryAsync(async () => {
                resolve({
                    AssociatedPageId: reply.get_item("AssociatedPageId"),
                    Body: reply.get_item("Body"),
                    Id: reply.get_id(),
                    Title: reply.get_item("Title"),
                    Created: reply.get_item("Created"),
                    Author: reply.get_item("Author"),
                    Replies: [],
                } as IDiscussion);
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
    public async createNewDiscussionReply(parentItemId: number, replyBody: string): Promise<IDiscussionReply>{

        const p = new Promise<IDiscussionReply>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().getList(this._discussionListServerRelativeUrl);
            const parentItem = list.getItemById(parentItemId);

            const web = context.get_web();
            const currentUser = web.get_currentUser();
            
            const reply = SP.Utilities.Utility.createNewDiscussionReply(context, parentItem);
            reply.set_item("Body", replyBody);

            // Need to explicitly update the item to actually create it (doesn't work otherwise)
            reply.update();
            context.load(currentUser);
            context.load(reply, "Id","Author","ParentItemID","Modified","Created","ParentList");
            context.executeQueryAsync(async () => {

                // Get user detail
                let authorProperties = await this.getUserProperties(currentUser.get_loginName());
                const PictureUrl = authorProperties["PictureUrl"] ? authorProperties["PictureUrl"] : "/_layouts/15/images/person.gif?rev=23";

                // Create a new dsicussion reply with initial property values
                resolve({
                    Body: replyBody,
                    Id: reply.get_id(),
                    ParentItemID: reply.get_item("ParentItemID"),
                    Posted: reply.get_item("Created"),
                    Edited: reply.get_item("Modified"),
                    Author: {
                        DisplayName: authorProperties["DisplayName"],
                        PictureUrl: PictureUrl,
                    },
                    UserPermissions: await this.getCurrentUserPermissionsOnItem(reply.get_id(), authorProperties["AccountName"]),
                    Children: [],
                    LikedBy: [],
                    LikesCount: 0,
                    ParentListId: reply.get_parentList().get_id().toString()
                } as IDiscussionReply);
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

            const discussion = await web.getList(this._discussionListServerRelativeUrl).items
                .filter(`AssociatedPageId eq ${ associatedPageId }`)
                .select("Id","Folder","AssociatedPageId")
                .expand("Folder")
                .top(1)
                .get();
            if (discussion.length > 0) {
        
                // Get replies from this discussion (i.e. folder)
                const query: CamlQuery = {
                    'ViewXml': `<View>
                                    <ViewFields>
                                        <FieldRef Name="Id"></FieldRef>
                                        <FieldRef Name="ParentItemID"></FieldRef>
                                        <FieldRef Name="Created"></FieldRef>
                                        <FieldRef Name="Modified"></FieldRef>
                                        <FieldRef Name="Body"></FieldRef>
                                        <FieldRef Name="ParenListId"></FieldRef>
                                        <FieldRef Name="LikedBy"></FieldRef>
                                        <FieldRef Name="LikesCount"></FieldRef>
                                    </ViewFields>
                                    <Query/>
                                </View>`,
                    'FolderServerRelativeUrl': `${this._discussionListServerRelativeUrl}/${discussion[0].Folder.Name}`
                };
            
                const replies = await web.getList(this._discussionListServerRelativeUrl).getItemsByCAMLQuery(query);

                // Batch are not supported on Sharepoint 2013
                // https://github.com/SharePoint/PnP-JS-Core/issues/492
                let batch = pnp.sp.createBatch();
                const isSPO = _spPageContextInfo["isSPO"];

                const discussionReplies: Promise<IDiscussionReply>[] = replies.map(async (reply) => {

                    const web = new Web(_spPageContextInfo.webAbsoluteUrl);
                    let item;
                    if (isSPO) {
                        item = await web.getList(this._discussionListServerRelativeUrl).items.getById(reply.Id).select("Author/Name","ParentList/Id").expand("Author/Name","ParentList/Id").inBatch(batch).get();
                    } else {
                        item = await web.getList(this._discussionListServerRelativeUrl).items.getById(reply.Id).select("Author/Name","ParentList/Id").expand("Author/Name","ParentList/Id").get();
                    }

                    let authorProperties = await this.getUserProperties(item.Author.Name);
                    const PictureUrl = authorProperties["PictureUrl"] ? authorProperties["PictureUrl"] : "/_layouts/15/images/person.gif?rev=23";

                    return {
                        Id: reply.Id,
                        ParentItemID: reply.ParentItemID,
                        Author: {
                            Id: item.Author.Id,
                            DisplayName: authorProperties["DisplayName"],
                            PictureUrl: PictureUrl,
                        },
                        Body: reply.Body,
                        Posted: reply.Created,
                        Edited: reply.Modified,
                        UserPermissions: await this.getCurrentUserPermissionsOnItem(reply.Id, item.Author.Name),
                        Children: [],
                        LikedBy: reply.LikedByStringId ? reply.LikedByStringId.results : [],
                        LikesCount: reply.LikesCount,
                        ParentListId: item.ParentList.Id,
                    } as IDiscussionReply;
                });

                if (isSPO) {
                    await batch.execute();
                }
       
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
    public async deleteReply(replyId: number): Promise<number>{

        try {
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);
            await web.getList(this._discussionListServerRelativeUrl).items.getById(replyId).delete();
            return replyId;

        } catch (error) {
            throw error;
        }
    }

    public async deleteRepliesHierachy(rootReply: IDiscussionReply, deletedIds: number[]): Promise<number[]> {
        
        if (rootReply.Children.length > 0) {
            // Delete children
            await Promise.all(rootReply.Children.map(async (currentReply) => {
                deletedIds.push(await this.deleteReply(currentReply.Id));
                await this.deleteRepliesHierachy(currentReply, deletedIds);
            }));
        }
        
        return deletedIds;
    }

    public async updateReply(replyId: number, replyBody: string): Promise<void>{

        try {
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);
            const result = await web.getList(this._discussionListServerRelativeUrl).items.getById(replyId).select("Modified").update({
                "Body": replyBody
            });
            
            return;

        } catch (error) {
            throw error;
        }
    }

    private async getCurrentUserPermissionsOnItem(itemId: number, replyAuthorLoginName: string): Promise<DiscussionPermissionLevel[]> {

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

            pnp.storage.local.deleteExpired();

            // The "WriteSecurity" property isn't availabe through REST with SharePoint 2013. In this case, we need to get the whole list XML schema to extract this info
            // Not very efficient but we do not have any other option here
            // Not List Item Level Security is different than item permissions so we can't rely on native REST methods (i.e. getCurrentUserEffectivePermissions())
            const writeSecurityStorageKey = String.format("{0}_{1}", _spPageContextInfo.webServerRelativeUrl, "commentsListWriteSecurity");
            let writeSecurity = pnp.storage.local.get(writeSecurityStorageKey);

            if (!writeSecurity) {
                const  list = await web.getList(this._discussionListServerRelativeUrl).select("SchemaXml").get();
                const writeSecurity = /WriteSecurity="(\d)"/.exec(list.SchemaXml)[1];
                pnp.storage.local.put(writeSecurityStorageKey, writeSecurity, pnp.util.dateAdd(new Date(), "minute", 60));
            }
                        
            if (writeSecurity.localeCompare("2") === 0) {

                const userLoginNameStorageKey = String.format("{0}_{1}", _spPageContextInfo.webServerRelativeUrl, "currentUserLoginName");
                let currentUserLoginName = pnp.storage.local.get(userLoginNameStorageKey);
                if (!currentUserLoginName) {
                    const currentUser = await web.currentUser.select("LoginName").get();
                    currentUserLoginName = currentUser.LoginName;
                    pnp.storage.local.put(userLoginNameStorageKey, currentUserLoginName, pnp.util.dateAdd(new Date(), "minute", 20));
                }

                // If the current user is the author of the comment
                if (replyAuthorLoginName === currentUserLoginName) {
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

    private async getUserProperties(accountName: string): Promise<any> {

        pnp.storage.local.deleteExpired();

        let authorPropertiesStorageKey = String.format("{0}_{1}", _spPageContextInfo.webServerRelativeUrl, accountName);
        let authorProperties = pnp.storage.local.get(authorPropertiesStorageKey);
        
        if (!authorProperties) {
            
            // Get user detail
            authorProperties = await pnp.sp.profiles.select("AccountName","PictureUrl","DisplayName","Email").getPropertiesFor(accountName);
            pnp.storage.local.put(authorPropertiesStorageKey, authorProperties, pnp.util.dateAdd(new Date(), "minute", 60));
        }

        return authorProperties;
    }

    public toggleLike(itemId: number, parentListId: string, isLiked: boolean): Promise<void> {

        const p = new Promise<void>((resolve, reject) => {
            const context = SP.ClientContext.get_current();
            Microsoft.Office.Server.ReputationModel.Reputation.setLike(context, parentListId, itemId, isLiked);
            context.executeQueryAsync(()=> {
                resolve();
            },(sender, args) => {
                reject(args.get_message());
            });
        });

        return p;
    };
}

export default SocialModule;