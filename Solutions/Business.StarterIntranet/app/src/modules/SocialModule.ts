import { Web, ODataEntityArray, CamlQuery} from "sp-pnp-js";
import IDiscussion from "../models/IDiscussion";
import IDiscussionReply from "../models/IDiscussionReply";

class SocialModule {

    private _discussionBoardlistName: string;

    public constructor() {
        
    }

    /**
     * Create a new discussion
     * @param discussion the discussion properties
     */
    public async createNewDiscussion(discussion: IDiscussion): Promise<number> {

        const p = new Promise<number>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().get_lists().getByTitle("Comments");

            const reply = SP.Utilities.Utility.createNewDiscussion(context, list, discussion.Title); 
            reply.set_item("Body", discussion.Body);
            reply.update();
            context.load(reply);

            context.executeQueryAsync(async () => {
                    resolve(reply.get_id());
            }, (sender, args) => {
                return;
            });    

        });
        
        return p;
    }

    /**
     * Add a reply to a discussion
     * @param parentItemId the parent discussion id
     */
    public async createNewDiscussionReply(parentFolderId: number, parentItemId: number, replyBody: string): Promise<void>{

        const p = new Promise<void>((resolve, reject) => {

            const context = SP.ClientContext.get_current();
            const list = context.get_web().get_lists().getByTitle("Comments");
            const parentItem = list.getItemById(parentItemId);
            
            const reply = SP.Utilities.Utility.createNewDiscussionReply(context, parentItem);
            reply.set_item("Body", replyBody);
            reply.update();
            context.load(reply);
            context.executeQueryAsync(() => {
                resolve();
            });
            
        });

        
        return p;
    }

    public async getDiscussionById(id: number): Promise<IDiscussion> {

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);
        const listUrl = _spPageContextInfo.webServerRelativeUrl + "/Lists/Comments";
        const discussion = await web.getList(listUrl).items.getById(id).expand("Folder").get();
        
        if (discussion) {
    
            // Get replies from this discussion (i.e. folder)
            const query: CamlQuery = {
                'ViewXml': '<View><Query/></View>',
                'FolderServerRelativeUrl': `${listUrl}/${discussion.Folder.Name}`
            };
        
            const replies = await web.lists.getByTitle("Comments").getItemsByCAMLQuery(query);

            const discussionReplies = replies.map((reply) => {
                return {
                    Id: reply.Id,
                    ParentItemID: reply.ParentItemID,
                    AuthorId: reply.AuthorId,
                    Body: reply.Body
                } as IDiscussionReply;
            });
            
            return {
                Title: discussion.Title,
                Id: discussion.Id,
                Replies: discussionReplies
            } as IDiscussion;
        } else {
            return null;
        }
    }

    public async deleteReply(replyId: number): Promise<void>{

        try {
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);
            const listUrl = _spPageContextInfo.webServerRelativeUrl + "/Lists/Comments";
            const discussion = await web.getList(listUrl).items.getById(replyId).delete();

            return;

        } catch (error) {
            throw error;
        }
    }
}

export default SocialModule;