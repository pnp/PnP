import * as React from "react";
import IDiscussionBoardProps from "./IDiscussionBoardProps";
import IDiscussionBoardState from "./IDiscussionBoardState";
import SocialModule from "../../../modules/SocialModule";
import IDiscussion from "../../../models/IDiscussion";
import { Web, PermissionKind } from "sp-pnp-js";
import DiscussionReply from "../DiscussionReply/DiscussionReply";
import { IDiscussionReply, DiscussionPermissionLevel } from "../../../models/IDiscussionReply";
import * as immutability from "immutability-helper";
import ContentEditable = require('react-contenteditable');
import * as i18n from "i18next";

// Needed to get it work at runtime
const update = immutability as any;

class DiscussionBoard extends React.Component<IDiscussionBoardProps, IDiscussionBoardState> {

    private _socialModule: SocialModule;
    private _associatedPageId: number;
    private _parentId: number;
    private _dicussionBoardListRelativeUrl: string;

    private _refInput: any;

    public constructor() {

        super();

        this.state = {
            discussion: null,
            userPermissions: [],
            inputValue: "",
            inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
            isLoading: false
        };

        this._dicussionBoardListRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/Lists/Comments`;
        this._socialModule = new SocialModule(this._dicussionBoardListRelativeUrl);

        // Handlers
        this.addNewComment = this.addNewComment.bind(this);
        this.deleteReply = this.deleteReply.bind(this);
        this.updateReply = this.updateReply.bind(this);
        this.toggleLikeReply = this.toggleLikeReply.bind(this);
        this.onValueChange = this.onValueChange.bind(this);
        this.onFocus = this.onFocus.bind(this);
        this.onBlur = this.onBlur.bind(this);
    }

    public render() {

        let renderPageComments = null;
        let renderNewReply = null;
        let renderIsLoading = null;
        let discussion = this.state.discussion;

        if (this.state.isLoading) {
            renderIsLoading = <div className="reply--loading"><i className="fa fa-spinner fa-spin"/></div>;
        }

        // Render comments as tree
        if (discussion) {
            const discussionTree = this.SetDiscussionFeedAsTree(discussion.Replies, discussion.Id);
            discussion = update(discussion, { Replies: {$set: discussionTree }});

            if (discussion.Replies.length > 0) {
                renderPageComments = discussion.Replies.map((reply, index) => {
                    return <DiscussionReply key={ reply.Id } 
                                            addNewReply= { this.addNewComment } 
                                            deleteReply={ this.deleteReply } 
                                            updateReply={ this.updateReply } 
                                            toggleLikeReply={ this.toggleLikeReply }
                                            reply={ reply }
                                            isLikeEnabled={ this.state.discussion.AreLikesEnabled }
                                            replyLevel={ 0 }
                                            />;  
                });
            }

            // If the current user can add list item to the list, it means he can comment
            if (this.state.userPermissions.indexOf(DiscussionPermissionLevel.Add) !== -1) {
                renderNewReply = <div className="reply main">
                                    <ContentEditable
                                        html={ this.state.inputValue } 
                                        disabled={ false }      
                                        onChange={ this.onValueChange }
                                        data-placeholder={ this.state.inputPlaceHolderValue }
                                        className="input"
                                        role="textbox"
                                        onFocus={ this.onFocus }
                                        onBlur={ this.onBlur }
                                    />
                                    { renderIsLoading }
                                    <button type="button" className="btn" onClick={ () => { 
                    
                                        let parentId = null;
                                        if (this.state.discussion) {
                                            parentId = this.state.discussion.Id;
                                        }

                                        this.addNewComment(parentId, this.state.inputValue);
                                    
                                }}>{ i18n.t("comments_post") }</button>
                                </div>;
            }

        } else {
            renderPageComments =    <div>
                                        { renderIsLoading }
                                        <div>{ i18n.t("comments_loading")}</div>
                                    </div>;
        }
            
        return  <div id="page-comments">
                    { renderNewReply }
                    { renderPageComments }                                       
                </div>;
    }

    /**
     * Event handlers
     */
    public onValueChange(e: any) {
        this.setState({ inputValue: e.target.value });
    }

    public onFocus() {
        if (!this.state.inputValue || this.state.inputValue.localeCompare("</br>") === 0) {
            this.setState({
                inputPlaceHolderValue: "",
                inputValue: "<show-placeholder>" // This is just to re-render the <ContentEditable/> component
            });
        }
    }

    public onBlur() {
        if (!$(this.state.inputValue).text()) {
            this.setState({
                inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
                inputValue: "",
            });
        }
    }

    /**
     * React lifecycle
     */
    public async componentDidMount() {

        this._associatedPageId = _spPageContextInfo.pageItemId;
        
        // Load JSOM dependencies before playing with the discussion board
        await this._socialModule.init();

        // Retrieve the discussion for this page
        let discussion = await this.getPageDiscussion(this._associatedPageId);
                
        // Get current user permissions
        const userListPermissions = await this._socialModule.getCurrentUserPermissionsOnList(this._dicussionBoardListRelativeUrl);

        this.setState({
            userPermissions: userListPermissions,
            discussion: discussion,
            inputValue: ""
        });
    }

    public async addNewComment(parentId: number, replyBody: string) {

        if (!replyBody) {
            alert("You can't post an empty comment");
        } else {
            let currentDiscussion = this.state.discussion;

            // First comment will create a new discussion
            if (!parentId) {
                const newDiscussion = await this.createNewDiscussion($("#title").text(), window.location.href);
                currentDiscussion = update(currentDiscussion, { $set: newDiscussion});

                // Set the new parent Id
                parentId = newDiscussion.Id;
            }

            let isLoading = false;
            if (currentDiscussion) {
                if (parentId === currentDiscussion.Id) {
                    isLoading = true;
                }
            }

            this.setState({
                isLoading: isLoading,
            });

            // Create reply to the discussion and and it to the state
            // Set the content as HTML (default field type)
            const reply = await this.createNewDiscussionReply(parentId, `<div>${replyBody}</div>`);
            currentDiscussion = update(currentDiscussion, { Replies: { $push: [reply]} });

            // Update the discussion
            this.setState({
                discussion: currentDiscussion,
                isLoading: false,
                inputValue: "",
                inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
            });
        }
    }

    public async deleteReply(reply: IDiscussionReply): Promise<void> {

        let hasBeenDeleted: boolean = false;
        let deletedIds: number[] = [];

        if (reply.Children.length > 0) {

            // Delete the root reply
            await this._socialModule.deleteReply(reply.Id);

            // Delete children replies
            deletedIds = await this._socialModule.deleteRepliesHierachy(reply, deletedIds);
        } else {
            await this._socialModule.deleteReply(reply.Id);
        }

        // Update the state
        const updatedReplies = this.state.discussion.Replies.filter((currentReply) => {
            let shouldReturn = true;
            if (currentReply.Id === reply.Id) {
                shouldReturn = false;
            } else {
                if (deletedIds.indexOf(currentReply.Id) !== -1) {
                    shouldReturn = false;
                }
            }
            return shouldReturn;
        });

        // Update state
        this.setState({
            discussion: update(this.state.discussion, { Replies: { $set: updatedReplies }}),
        });
        
    }

    public async updateReply(replyToUpdate: IDiscussionReply) {
    
        if (!$(replyToUpdate.Body).text()) {
            alert("You can't post an empty comment");
        } else {

            try {
                await this._socialModule.updateReply(replyToUpdate.Id, replyToUpdate.Body);

                const updatedReplies = this.state.discussion.Replies.map((currentReply) => {

                    let updatedReply = currentReply;
                    if (currentReply.Id === replyToUpdate.Id) {
                        updatedReply.Body = replyToUpdate.Body;
                        updatedReply.Edited = new Date();
                    }
                    return updatedReply;
                });

                // Update state
                this.setState({
                    discussion: update(this.state.discussion, { Replies: { $set: updatedReplies }}),
                });
                
            } catch (error){
                // TODO: Set state error
            }
        }
    }

    private async createNewDiscussion(title: string, body: string): Promise<IDiscussion> {
        return await this._socialModule.createNewDiscussion(this._associatedPageId, title, body);
    }

    private async toggleLikeReply(reply: IDiscussionReply, isLiked: boolean): Promise<void> {

        try {
            const updatdeLikesCount = await this._socialModule.toggleLike(reply.Id, reply.ParentListId,isLiked);

            const updatedReplies = this.state.discussion.Replies.map((currentReply) => {

                let updatedReply = currentReply;
                const userId = _spPageContextInfo.userId.toString();
                if (currentReply.Id === reply.Id) {
                    updatedReply.LikesCount = updatdeLikesCount;
                    updatedReply.LikedBy = isLiked ? 
                        update(updatedReply.LikedBy, {$push: [_spPageContextInfo.userId.toString()]}) : 
                        update(updatedReply.LikedBy, {$splice: [[updatedReply.LikedBy.indexOf(userId),1]]}) ;
                }
                return updatedReply;
            });

            // Update state
            this.setState({
                discussion: update(this.state.discussion, { Replies: { $set: updatedReplies }}),
            });
        } catch (error) {

        }
    }

    private async getPageDiscussion(associatedPageId: number): Promise<IDiscussion> {
        return await this._socialModule.getDiscussionById(associatedPageId);
    }

    private async createNewDiscussionReply(parentId: number, replyBody: string): Promise<IDiscussionReply> {
        return await this._socialModule.createNewDiscussionReply(parentId, replyBody);
    }

    private SetDiscussionFeedAsTree(list: any[], rootParentID: number, idAttr?, parentAttr?, childrenAttr?): any[] {
        if (!idAttr) idAttr = 'Id';
        if (!parentAttr) parentAttr = 'ParentItemID';
        if (!childrenAttr) childrenAttr = 'Children';
        var treeList = [];
        var lookup = {};
        list.forEach(function(obj) {
            lookup[obj[idAttr]] = obj;
            obj[childrenAttr] = [];
        });
        list.forEach(function(obj) {
            if (obj[parentAttr] != rootParentID) {
                if (lookup[obj[parentAttr]]) {
                    lookup[obj[parentAttr]][childrenAttr].push(obj);
                }                
            } else {
                treeList.push(obj);
            }
        });
        return treeList;
    };
}

export default DiscussionBoard;