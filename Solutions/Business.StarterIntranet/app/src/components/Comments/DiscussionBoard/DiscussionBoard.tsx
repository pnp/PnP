import * as React from "react";
import IDiscussionBoardProps from "./IDiscussionBoardProps";
import IDiscussionBoardState from "./IDiscussionBoardState";
import SocialModule from "../../../modules/SocialModule";
import IDiscussion from "../../../models/IDiscussion";
import { Web, PermissionKind } from "sp-pnp-js";
import DiscussionReply from "../DiscussionReply/DiscussionReply";
import { IDiscussionReply, DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

class DiscussionBoard extends React.Component<IDiscussionBoardProps, IDiscussionBoardState> {

    private _socialModule: SocialModule;
    private _associatedPageId: number;
    private _parentId: number;
    private _commentInputRef: any;
    private _dicussionBoardListRelativeUrl: string;

    public constructor() {

        super();

        this.state = {
            discussion: null,
            userPermissions: [],
        };

        this._dicussionBoardListRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/Lists/Comments`;
        this._socialModule = new SocialModule(this._dicussionBoardListRelativeUrl);

        // Handlers
        this.addNewComment = this.addNewComment.bind(this);
        this.deleteReply = this.deleteReply.bind(this);
        this.updateReply = this.updateReply.bind(this);
    }

    public render() {

        let renderPageComments = null;

        if (this.state.discussion) {
            renderPageComments = this.state.discussion.Replies.map((reply, index) => {
                return <DiscussionReply key={ index } addNewReply= { this.addNewComment } deleteReply={ this.deleteReply } updateReply={ this.updateReply } reply={ reply }/>    
            });
        }
            
        let renderNewReply = null;

        // If the current user can add list item to the list, it means he can comment
        if (this.state.userPermissions.indexOf(DiscussionPermissionLevel.Add) !== -1) {
            renderNewReply = <div>
                <textarea ref={ (input) => {
                    this._commentInputRef = input;
                }}></textarea>
                <button type="button" onClick={ () => { 

                    let parentId = null;
                    if (this.state.discussion) {
                        parentId = this.state.discussion.Id;
                    }

                    this.addNewComment(parentId, this._commentInputRef.value) 
                }}>Add new comment</button>
            </div>
        }

        return <div>
            { renderPageComments }
            { renderNewReply }
        </div>
    }

    public async componentDidMount() {

        this._associatedPageId = _spPageContextInfo.pageItemId;

        // Retrieve the discussion for this page
        await this.getPageDiscussion(this._associatedPageId);
        
        // Get current user permissions
        const userListPermissions = await this._socialModule.getCurrentUserPermissionsOnList(this._dicussionBoardListRelativeUrl);

        this.setState({
            userPermissions: userListPermissions,
        });
    }

    public async addNewComment(parentId: number, replyBody: string) {

        // First comment will create a new discussion
        if (!parentId) {
            parentId = await this.createNewDiscussion($("#title").text(), window.location.href);
        }

        // Create reply to the discussion
        await this.createNewDiscussionReply(parentId, replyBody);

        // Fetch the updated discussion
        await this.getPageDiscussion(this._associatedPageId);
    }

    public async deleteReply(replyId: number) {
        await this._socialModule.deleteReply(replyId);

        // Fetch the updated discussion
        await this.getPageDiscussion(this._associatedPageId);
    }

    public async updateReply(reply: IDiscussionReply) {
        await this._socialModule.updateReply(reply);

        // Fetch the updated discussion
        await this.getPageDiscussion(this._associatedPageId);
    }

    private async createNewDiscussion(title: string, body: string): Promise<number>{

        const newDiscussionInfo: IDiscussion = {
            AssociatedPageId: this._associatedPageId,
            Body: body,
            Title: title,
        };

        const discussionId = await this._socialModule.createNewDiscussion(newDiscussionInfo);

        return discussionId;
    }

    private async getPageDiscussion(associatedPageId: number) {
        // Check if there is arleady a discussion for this page
        const discussion = await this._socialModule.getDiscussionById(associatedPageId);

        this.setState({
            discussion: discussion,
        });
    }

    private async createNewDiscussionReply(parentId: number, replyBody: string): Promise<void> {
        return await this._socialModule.createNewDiscussionReply(parentId, replyBody);
    }

}

export default DiscussionBoard;