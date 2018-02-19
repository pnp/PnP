import * as React from "react";
import IDiscussionBoardProps from "./IDiscussionBoardProps";
import IDiscussionBoardState from "./IDiscussionBoardState";
import SocialModule from "../../modules/SocialModule";
import IDiscussion from "../../models/IDiscussion";
import { Web } from "sp-pnp-js";
import DiscussionReply from "./DiscussionReply";

class DiscussionBoard extends React.Component<IDiscussionBoardProps, IDiscussionBoardState> {

    private _socialModule: SocialModule;
    private _pageDiscussionId: number;
    private _commentInputRef: any;

    public constructor() {

        super();

        this.state = {
            discussion: null
        }

        this._socialModule = new SocialModule();
        this.addNewComment = this.addNewComment.bind(this);
        this.deleteReply = this.deleteReply.bind(this);
    }

    public render() {

        let renderPageComments = null;

        if (this.state.discussion) {
            renderPageComments = this.state.discussion.Replies.map((reply, index) => {
                return <DiscussionReply key={ index } addNewReply= { this.addNewComment } deleteReply={ this.deleteReply } reply= { reply }/>    
            });
        }
            
        let renderNewReply = 
        <div>
            <textarea ref={ (input) => {
                this._commentInputRef = input;
            }}></textarea>
            <button type="button" onClick={ () => { this.addNewComment(this._pageDiscussionId, this._commentInputRef.value) }}>Add new comment</button>
        </div>

        return <div>
            { renderPageComments }
            { renderNewReply }
        </div>
    }

    public componentDidMount() {

        // Get the page discussion main thread id present of the page(exposed via the page layout)
        if ($("#page-discussion-id").text().trim() !== "") {
            
            this._pageDiscussionId = parseInt($("#page-discussion-id").text());
            // Retrieve the discussion for this page
            this.getPageDiscussion(this._pageDiscussionId);
        }
    }

    public async addNewComment(parentId: number, replyBody: string) {

        // First comment will create the discussion
        if (!this.state.discussion) {
            parentId = await this.createNewDiscussion($("#title").text(), window.location.href);
            this._pageDiscussionId = parentId;
        }

        // Create reply to the discussion
        await this.createNewDiscussionReply(this._pageDiscussionId , parentId, replyBody);

        // Fetch the updated discussion
        await this.getPageDiscussion(this._pageDiscussionId);
    }

    public async deleteReply(replyId: number) {
        await this._socialModule.deleteReply(replyId);

        // Fetch the updated discussion
        await this.getPageDiscussion(this._pageDiscussionId);
    }

    private async createNewDiscussion(title: string, body: string): Promise<number>{

        const discussionId = await this._socialModule.createNewDiscussion({Title: title, Body: body} as IDiscussion);

        // Persist the discussion id in the page
        const web = new Web(_spPageContextInfo.webAbsoluteUrl);
        await web.lists.getById(_spPageContextInfo.pageListId).items.getById(_spPageContextInfo.pageItemId).update({
            IntranetPageDiscussionId: discussionId,
        });

        return discussionId;
    }

    private async getPageDiscussion(itemId: number) {
        // Check if there is arleady a discussion for this page
        const discussion = await this._socialModule.getDiscussionById(itemId);

        this.setState({
            discussion: discussion,
        });
    }

    private async createNewDiscussionReply(parentFolderId: number, parentId: number, replyBody: string): Promise<void> {
        return await this._socialModule.createNewDiscussionReply(parentFolderId, parentId, replyBody);
    }

}

export default DiscussionBoard;