import * as i18n from "i18next";
import * as moment from "moment";
import * as React from "react";
import ContentEditable = require("react-contenteditable");
import { PermissionKind } from "sp-pnp-js";
import { DiscussionPermissionLevel, IDiscussionReply } from "../../../models/IDiscussionReply";
import IDiscussionReplyProps from "./IDiscussionReplyProps";
import { EditMode, IDiscussionReplyState } from "./IDiscussionReplyState";

class DiscussionReply extends React.Component<IDiscussionReplyProps, IDiscussionReplyState> {

    private readonly REPLY_NESTED_LEVEL_LIMIT = 3;
    private readonly CHILD_LEFT_PADDING_SIZE = 32;

    public constructor() {
        super();

        this.state = {
            editMode: EditMode.NewComment,
            inputValue: "",
            isLoading: false,
            showInput: false,
        };

        // Handlers
        this.toggleInput = this.toggleInput.bind(this);
        this.onValueChange = this.onValueChange.bind(this);
        this.updateReply = this.updateReply.bind(this);
        this.addNewReply = this.addNewReply.bind(this);
        this.deleteReply = this.deleteReply.bind(this);
        this.toggleLikeReply = this.toggleLikeReply.bind(this);
    }

    public render() {

        let renderIsLoading: JSX.Element = null;

        if (this.state.isLoading) {
            renderIsLoading = <i className="fa fa-spinner fa-spin"/>;
        }

        let renderEdit: JSX.Element = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.EditAsAuthor) !== -1 ||
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Edit) !== -1) {
            renderEdit = <div><i className="fa fa-pencil-alt"/><a href="#" onClick={ () => {
                this.toggleInput(true, EditMode.UpdateComment);
            }}>{ i18n.t("comments_edit") }</a></div>;
        }

        let renderDelete: JSX.Element = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.DeleteAsAuthor) !== -1 ||
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Delete) !== -1) {
            renderDelete = <div><i className="fa fa-trash"/><a href="#" onClick={ () => {
                this.deleteReply(this.props.reply);
            }}>{ i18n.t("comments_delete") }</a></div>;
        }

        let renderReply: JSX.Element = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Add) !== -1 && this.props.replyLevel < this.REPLY_NESTED_LEVEL_LIMIT) {
            renderReply = <div><i className="fa fa-reply"/><a href="#" onClick={ () => {
                this.toggleInput(true, EditMode.NewComment);
            }}>{ i18n.t("comments_reply") }</a></div>;
        }

        const renderChildren: JSX.Element[] = [];
        if (this.props.reply.Children) {
            this.props.reply.Children.map((childReply, index) => {
                renderChildren.push(
                    <DiscussionReply
                        key={ childReply.Id }
                        id={ `${this.props.id}${index}`}
                        reply={ childReply }
                        isLikeEnabled={ this.props.isLikeEnabled }
                        addNewReply={this.props.addNewReply}
                        deleteReply={ this.props.deleteReply }
                        updateReply={ this.props.updateReply }
                        toggleLikeReply={ this.props.toggleLikeReply }
                        isChildReply={ true }
                        replyLevel={ this.props.replyLevel + 1 }
                    />,
                );
            });
        }

        let renderLike: JSX.Element = null;

        if (this.props.isLikeEnabled) {
            const likeLabel = this.isReplyLikedByCurrentUser(this.props.reply) ? i18n.t("comments_unlike") : i18n.t("comments_like");
            renderLike = <div>
                            <i className="fa fa-heart"/>
                            <span>{this.props.reply.LikesCount}</span>
                            <a href="#" onClick={ () => { this.toggleLikeReply(this.props.reply); }}>{ likeLabel }</a>
                        </div>;
        }

        const posted = moment(this.props.reply.Posted);
        const modified = moment(this.props.reply.Edited);
        const isPosthasBeenEdited: JSX.Element = modified.diff(posted) > 0 ? <span>{`(${i18n.t("comments_edited")})`}</span> : null;
        const lastUpdate: JSX.Element = isPosthasBeenEdited ? <div>{`${i18n.t("comments_lastUpdate")} ${moment(modified).format("LLL")}`}</div> : null;
        const rootElementClassName = this.props.isChildReply ? "reply child" : "reply";
        const paddingCalc = this.CHILD_LEFT_PADDING_SIZE * this.props.replyLevel;

        return  <div>
                    <div className="reply" style={{ paddingLeft: `${paddingCalc}px`}} key= { this.props.reply.Id }>
                        <div>
                            <img className="reply--user-avatar" src={ this.props.reply.Author.PictureUrl}/>
                        </div>
                        <div className="reply--content">
                            <div>
                                <div className="reply--content--user-name">{ this.props.reply.Author.DisplayName } { isPosthasBeenEdited }</div>
                                <div className="reply--content--body" dangerouslySetInnerHTML= {{__html: this.props.reply.Body }}></div>
                                <div className="reply--content--date">
                                    <div>{ `${i18n.t("comments_postedOn")} ${moment(this.props.reply.Posted).format("LLL")}`}</div>
                                    { lastUpdate }
                                </div>
                            </div>
                            <div className="reply--content--actions">
                                { renderLike }
                                { renderReply }
                                { renderEdit }
                                { renderDelete }
                                <div>
                                    { renderIsLoading }
                                </div>
                            </div>
                            { this.state.showInput ?
                                <div className="reply--input-zone">
                                    <ContentEditable
                                        id={`reply-input-${this.props.id}`}
                                        html={ this.state.inputValue }
                                        disabled={ false }
                                        onChange={ this.onValueChange }
                                        className="input"
                                        role="textbox"
                                    />
                                    <button type="button" className="btn" onClick={ async () => {

                                        switch (this.state.editMode) {
                                            case EditMode.NewComment:
                                                await this.addNewReply(this.props.reply.Id, this.state.inputValue);
                                                break;

                                            case EditMode.UpdateComment:
                                                await this.updateReply(this.props.reply);
                                                break;
                                        }

                                        this.toggleInput(false, null);
                                    }}>{ this.state.editMode === EditMode.UpdateComment ? i18n.t("comments_update") : i18n.t("comments_post") }</button>
                                    <button className="btn" onClick={ () => { this.toggleInput(false, null); }} >{ i18n.t("comments_cancel") }</button>
                                </div>
                                :
                                    null
                            }
                        </div>
                    </div>
                    { renderChildren }
                </div>;
    }

    public onValueChange(e: any) {

        this.setState({
            inputValue: e.target.value,
         });
    }

    public componentDidUpdate() {

        // Set auto focus to input when replying or updating
        if (this.state.showInput) {
            switch (this.state.editMode) {
                case EditMode.NewComment:
                    if (!this.state.inputValue) {
                        this.setFocus(`reply-input-${this.props.id}`);
                    }
                    break;

                case EditMode.UpdateComment:
                    if (this.state.inputValue === this.props.reply.Body) {
                        this.setFocus(`reply-input-${this.props.id}`);
                        break;
                    }
            }
        }
    }

    /**
     * Adds a new reply
     * @param parentReplyId the parent reply item id
     * @param replyBody the reply body text
     */
    public async addNewReply(parentReplyId: number, replyBody: string) {

        try {

            this.setState({
                isLoading: true,
            });

            await this.props.addNewReply(this.props.reply.Id, this.state.inputValue);

            this.setState({
                isLoading: false,
            });

        } catch (error) {
            throw error;
        }
    }

    /**
     * Updates an existing reply
     * @param replyToUpdate replu object to update
     */
    public async updateReply(replyToUpdate: IDiscussionReply): Promise<void> {

        try {

            this.setState({
                isLoading: true,
            });

            const reply: IDiscussionReply = {
                Body: `<div>${this.state.inputValue}</div>`, // Set as HTML to be able to parse it easily afterward
                Id: replyToUpdate.Id,
            };

            await this.props.updateReply(reply);

            this.setState({
                isLoading: false,
            });

        } catch (error) {
            throw error;
        }
    }

    /**
     * Deletes a single or multipels replies
     * @param replyToDelete the reply to delete
     */
    public async deleteReply(replyToDelete: IDiscussionReply): Promise<void> {

        try {
            // We make this verification in the reply component itself to avoid an issue when the user says 'No'.
            // In this case, the state wouldn't be updated to false (isLoading).
            if (replyToDelete.Children.length > 0) {
                if (confirm(i18n.t("comments_delete_hierarchy"))) {

                    this.setState({
                        isLoading: true,
                    });

                    await this.props.deleteReply(replyToDelete);
                }
            } else {
                if (confirm(i18n.t("comments_delete_single"))) {

                    this.setState({
                        isLoading: true,
                    });

                    await this.props.deleteReply(replyToDelete);
                }
            }

            // After that the element is deleted in the DOM so we can't update the state anymore...

        } catch (error) {
            throw error;
        }
    }

    /**
     * Like or unlike a reply
     * @param reply the reply to like/unlike
     */
    public async toggleLikeReply(reply: IDiscussionReply) {

        this.setState({
            isLoading: true,
        });

        await this.props.toggleLikeReply(reply, !this.isReplyLikedByCurrentUser(reply));

        this.setState({
            isLoading: false,
        });
    }

    /**
     * Show or hide the input control
     * @param isVisible true if visible, false otherwise
     * @param editMode the current edit mode (UpdateComment or NewComment)
     */
    public toggleInput(isVisible: boolean, editMode: EditMode) {

        let inputValue;

        switch (editMode) {
            case EditMode.UpdateComment:
                inputValue = this.props.reply.Body;
                break;

            case EditMode.NewComment:
                inputValue = "";
                break;

            default:
                inputValue = "";
                break;
        }

        this.setState({
            editMode,
            inputValue,
            showInput: isVisible,
        });
    }

    /**
     * Indicates whether or not a reply is liked by the current user
     * @param reply the reply to check
     */
    private isReplyLikedByCurrentUser(reply: IDiscussionReply): boolean {

        // If the current user id is in the list ok "liked by" field
        let isLiked = false;
        if (reply.LikedBy.indexOf(_spPageContextInfo.userId.toString()) !== -1) {
            isLiked = true;
        }

        return isLiked;
    }

    /**
     * Sets the focus in the content editable div
     * @param eltId the DOM element id
     */
    private setFocus(eltId: string) {
        const p = document.getElementById(eltId);
        const s = window.getSelection();
        const r = document.createRange();
        r.setStart(p, p.childElementCount);
        r.setEnd(p, p.childElementCount);
        s.removeAllRanges();
        s.addRange(r);
    }
}

export default DiscussionReply;
