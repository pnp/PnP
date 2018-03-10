import * as i18n from "i18next";
import * as immutability from "immutability-helper";
import * as React from "react";
import ContentEditable = require("react-contenteditable");
import { PermissionKind, Web } from "sp-pnp-js";
import IDiscussion from "../../../models/IDiscussion";
import { DiscussionPermissionLevel, IDiscussionReply } from "../../../models/IDiscussionReply";
import SocialModule from "../../../modules/SocialModule";
import DiscussionReply from "../DiscussionReply/DiscussionReply";
import IDiscussionBoardProps from "./IDiscussionBoardProps";
import IDiscussionBoardState from "./IDiscussionBoardState";

// Needed to get it work at runtime
const update = immutability as any;

class DiscussionBoard extends React.Component<IDiscussionBoardProps, IDiscussionBoardState> {

    private socialModule: SocialModule;
    private associatedPageId: number;
    private parentId: number;
    private dicussionBoardListRelativeUrl: string;

    public constructor(props: IDiscussionBoardProps) {

        super(props);

        this.state = {
            areCommentsLoading: true,
            discussion: null,
            inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
            inputValue: "",
            isAdding: false,
            userPermissions: [],
        };

        this.dicussionBoardListRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/Lists/${props.listRootFolderUrl}`; // You can parametrized the list URL if you want
        this.socialModule = new SocialModule(this.dicussionBoardListRelativeUrl);

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
        let renderIsAdding = null;
        let renderCommentsAreLoading = null;
        let renderCommentsCount = null;
        let discussion = this.state.discussion;

        if (this.state.isAdding) {
            renderIsAdding = <div className="reply--loading"><i className="fa fa-spinner fa-spin"/></div>;
        }

        if (this.state.areCommentsLoading) {
            renderCommentsAreLoading = <div className="loading"><i className="fa fa-spinner fa-spin"/><span>{ i18n.t("comments_loading")}</span></div>;
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
                                { renderIsAdding }
                                <button type="button" className="btn" onClick={ () => {

                                    let parentId = null;

                                    if (this.state.discussion) {
                                        parentId = this.state.discussion.Id;
                                    }

                                    this.addNewComment(parentId, this.state.inputValue);
                                }}>{ i18n.t("comments_post") }</button>
                            </div>;
        }

        // Get the number of comments
        const commentsCount = !discussion ? 0 : discussion.Replies.length;
        renderCommentsCount = !this.state.areCommentsLoading ? <div className="count">{`${commentsCount} ${i18n.t("comments_commentsLabel")}`}</div> : null;

        // Render comments as tree
        if (discussion) {

            const discussionTree = this.setDiscussionFeedAsTree(discussion.Replies, discussion.Id);
            discussion = update(discussion, { Replies: {$set: discussionTree }});

            if (discussion.Replies.length > 0) {
                renderPageComments = discussion.Replies.map((reply, index) => {
                    return <DiscussionReply key={ reply.Id }
                                            id={ reply.Id.toString() }
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

        } else {
            renderPageComments =    <div>
                                       { renderCommentsAreLoading }
                                    </div>;
        }

        return  <div id="page-comments">
                    { renderCommentsCount }
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
                inputValue: "<show-placeholder>", // This is just to re-render the <ContentEditable/> component by faking a new value
            });
        }
    }

    public onBlur() {
        if (!$(`<div>${this.state.inputValue}</div>`).text()) {
            this.setState({
                inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
                inputValue: "",
            });
        }
    }

    /**
     * React component lifecycle
     */
    public async componentDidMount() {

        this.associatedPageId = _spPageContextInfo.pageItemId;

        // Load JSOM dependencies before playing with the discussion board
        await this.socialModule.init();

        // Retrieve the discussion for this page
        const discussion = await this.getPageDiscussion(this.associatedPageId);

        // Get current user permissions
        const userListPermissions = await this.socialModule.getCurrentUserPermissionsOnList(this.dicussionBoardListRelativeUrl);

        this.setState({
            areCommentsLoading: false,
            discussion,
            inputValue: "",
            userPermissions: userListPermissions,
        });
    }

    /**
     * Adde a new comment and create the discussion if doesn't exist
     * @param parentId the reply parent item id
     * @param replyBody the reply body text
     */
    public async addNewComment(parentId: number, replyBody: string) {

        if (!replyBody) {
            alert(i18n.t("comments_empty"));
        } else {

            let currentDiscussion = this.state.discussion;

            // First comment will create a new discussion and a reply
            if (!parentId) {

                this.setState({
                    isAdding: true,
                });

                const newDiscussion = await this.createNewDiscussion($("#title").text(), window.location.href);
                currentDiscussion = update(currentDiscussion, { $set: newDiscussion});

                // Set the new parent Id
                parentId = newDiscussion.Id;

            } else {
                if (parentId === currentDiscussion.Id) {
                    this.setState({
                        isAdding: true,
                    });
                }
            }

            // Create reply to the discussion and and it to the state
            // Set the content as HTML (default field type)
            const reply = await this.createNewDiscussionReply(parentId, `<div>${replyBody}</div>`);
            currentDiscussion = update(currentDiscussion, { Replies: { $push: [reply]} });

            // Update the discussion
            this.setState({
                discussion: currentDiscussion,
                inputPlaceHolderValue: i18n.t("comments_new_placeholder"),
                inputValue: "",
                isAdding: false,
            });
        }
    }

    /**
     * Deletes a reply
     * @param reply the reply object to delete
     */
    public async deleteReply(reply: IDiscussionReply): Promise<void> {

        const hasBeenDeleted: boolean = false;
        let deletedIds: number[] = [];

        if (reply.Children.length > 0) {

            // Delete the root reply
            await this.socialModule.deleteReply(reply.Id);

            // Delete children replies
            deletedIds = await this.socialModule.deleteRepliesHierachy(reply, deletedIds);
        } else {
            await this.socialModule.deleteReply(reply.Id);
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

    /**
     * Updates a reply
     * @param reply the reply object to update
     */
    public async updateReply(replyToUpdate: IDiscussionReply) {

        if (!$(replyToUpdate.Body).text()) {
            alert(i18n.t("comments_empty"));
        } else {

            await this.socialModule.updateReply(replyToUpdate.Id, replyToUpdate.Body);

            const updatedReplies = this.state.discussion.Replies.map((currentReply) => {

                const updatedReply = currentReply;
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
        }
    }

    private async createNewDiscussion(title: string, body: string): Promise<IDiscussion> {
        return this.socialModule.createNewDiscussion(this.associatedPageId, title, body);
    }

    private async toggleLikeReply(reply: IDiscussionReply, isLiked: boolean): Promise<void> {

        const updatdeLikesCount = await this.socialModule.toggleLike(reply.Id, reply.ParentListId, isLiked);

        const updatedReplies = this.state.discussion.Replies.map((currentReply) => {

            const updatedReply = currentReply;
            const userId = _spPageContextInfo.userId.toString();
            if (currentReply.Id === reply.Id) {
                updatedReply.LikesCount = updatdeLikesCount;
                updatedReply.LikedBy = isLiked ?
                    update(updatedReply.LikedBy, {$push: [_spPageContextInfo.userId.toString()]}) :
                    update(updatedReply.LikedBy, {$splice: [[updatedReply.LikedBy.indexOf(userId), 1]]}) ;
            }
            return updatedReply;
        });

        // Update state
        this.setState({
            discussion: update(this.state.discussion, { Replies: { $set: updatedReplies }}),
        });
    }

    private async getPageDiscussion(associatedPageId: number): Promise<IDiscussion> {
        return this.socialModule.getDiscussionById(associatedPageId);
    }

    private async createNewDiscussionReply(parentId: number, replyBody: string): Promise<IDiscussionReply> {
        return this.socialModule.createNewDiscussionReply(parentId, replyBody);
    }

    private setDiscussionFeedAsTree(list: any[], rootParentID: number, idAttr?, parentAttr?, childrenAttr?): any[] {

        if (!idAttr) {
            idAttr = "Id";
        }

        if (!parentAttr) {
            parentAttr = "ParentItemID";
        }

        if (!childrenAttr) {
            childrenAttr = "Children";
        }

        const treeList = [];
        const lookup = {};

        list.forEach((obj) => {
            lookup[obj[idAttr]] = obj;
            obj[childrenAttr] = [];
        });

        list.forEach((obj) => {
            if (obj[parentAttr] !== rootParentID) {
                if (lookup[obj[parentAttr]]) {
                    lookup[obj[parentAttr]][childrenAttr].push(obj);
                }
            } else {
                treeList.push(obj);
            }
        });
        return treeList;
    }
}

export default DiscussionBoard;
