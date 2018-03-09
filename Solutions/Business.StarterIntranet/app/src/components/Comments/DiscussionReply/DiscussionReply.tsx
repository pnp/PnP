import * as React from "react";
import IDiscussionReplyProps from "./IDiscussionReplyProps";
import { IDiscussionReplyState, EditMode } from "./IDiscussionReplyState";
import { PermissionKind } from "sp-pnp-js";
import { IDiscussionReply, DiscussionPermissionLevel } from "../../../models/IDiscussionReply";
import * as moment from "moment";
import * as i18n from "i18next";
import ContentEditable = require('react-contenteditable');

class DiscussionReply extends React.Component<IDiscussionReplyProps, IDiscussionReplyState> {

    private readonly REPLY_NESTED_LEVEL_LIMIT = 3;
    private readonly CHILD_LEFT_PADDING_SIZE = 32;

    public constructor() {
        super();

        this.state = {
            showInput: false,
            editMode: EditMode.NewComment,
            inputValue: "",
            isLoading: false,
        };

        this.toggleInput = this.toggleInput.bind(this);
        this.onValueChange = this.onValueChange.bind(this);
        this.updateReply = this.updateReply.bind(this);
        this.addNewReply = this.addNewReply.bind(this);
        this.deleteReply = this.deleteReply.bind(this);
        this.toggleLikeReply = this.toggleLikeReply.bind(this);
    }

    public render() {

        let renderIsLoading = null;

        if (this.state.isLoading) {
            renderIsLoading = <i className="fa fa-spinner fa-spin"/>;
        }

        let renderEdit = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.EditAsAuthor ) !== -1 || 
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.ManageLists ) !== -1) {
            renderEdit = <div><i className="fa fa-pencil-alt"/><a href="#" onClick={ () => {
                this.toggleInput(true, EditMode.UpdateComment);
            }}>{ i18n.t("comments_edit") }</a></div>;
        }

        let renderDelete = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Delete) !== -1) {
            renderDelete = <div><i className="fa fa-trash"/><a href="#" onClick={ () => { 
                this.deleteReply(this.props.reply);
            }}>{ i18n.t("comments_delete") }</a></div>;
        }

        let renderReply = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Add) !== -1 && this.props.replyLevel < this.REPLY_NESTED_LEVEL_LIMIT) {
            renderReply = <div><i className="fa fa-reply"/><a href="#" onClick={ () => {
                this.toggleInput(true, EditMode.NewComment);
            }}>{ i18n.t("comments_reply") }</a></div>;
        }

        let renderChildren: JSX.Element[] = [];
        if (this.props.reply.Children) {
            this.props.reply.Children.map((childReply, index) => {
                renderChildren.push(
                    <DiscussionReply
                        key={ childReply.Id }
                        reply={ childReply }
                        isLikeEnabled={ this.props.isLikeEnabled }
                        addNewReply={this.props.addNewReply}
                        deleteReply={ this.props.deleteReply }
                        updateReply={ this.props.updateReply }
                        toggleLikeReply={ this.props.toggleLikeReply }
                        isChildReply={ true }
                        replyLevel={ this.props.replyLevel + 1 }
                    />
                )
            });
        }

        let renderLike: JSX.Element = null;

        if (this.props.isLikeEnabled) {
            let likeLabel = this.isReplyLikedByCurrentUser(this.props.reply) ? i18n.t("comments_unlike") : i18n.t("comments_like");
            renderLike = <div>
                            <i className="fa fa-heart"/>
                            <span>{this.props.reply.LikesCount}</span>
                            <a href="#" onClick={ () => { this.toggleLikeReply(this.props.reply); }}>{ likeLabel }</a>                        
                        </div>;
        }

        const posted = moment(this.props.reply.Posted);
        const modified = moment(this.props.reply.Edited);
        let isPosthasBeenEdited: JSX.Element = modified.diff(posted) > 0 ? <div><strong>{`Edited (Last update on ${moment(modified).format("LLL")})`}</strong></div> : null;
        const rootElementClassName = this.props.isChildReply ? "reply child" : "reply";    
        const paddingCalc = this.CHILD_LEFT_PADDING_SIZE * this.props.replyLevel;   
        
        return  <div>
                    <div className="reply" style={{ paddingLeft: `${paddingCalc}px`}} key= { this.props.reply.Id }>
                        <div>            
                            <img className="reply--user-avatar" src={ this.props.reply.Author.PictureUrl}/>
                        </div>
                        <div className="reply--content">
                            <div>
                                <div className="reply--content--user-name">{ this.props.reply.Author.DisplayName }</div>
                                <div dangerouslySetInnerHTML= {{__html: this.props.reply.Body }}></div>
                                <div>{ `${i18n.t("comments_postedOn")} ${moment(this.props.reply.Posted).format('LLL')}`}</div>
                                { isPosthasBeenEdited }
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

    public async updateReply(replyToUpdate: IDiscussionReply): Promise<void> {

        try {
            
            this.setState({
                isLoading: true,
            }); 

            const reply: IDiscussionReply = {
                Id: replyToUpdate.Id,
                Body: `<div>${this.state.inputValue}</div>`, // Set as HTML to be able to parse it easily afterward
            };

            await this.props.updateReply(reply);

            this.setState({
                isLoading: false,
            });

        } catch (error) {
            throw error;
        }
    }

    public async deleteReply(replyToDelete: IDiscussionReply): Promise<void> {

        try {
            // We make this verification in the reply component itself to avoid an issue when the user says 'No'. 
            // In this case, the state wouldn't be updated to false (isLoading).
            if (replyToDelete.Children.length > 0) {
                if (confirm('This comment has some sub comments. They will be also deleted. Are you sure?')) {

                    this.setState({
                        isLoading: true,
                    }); 

                    await this.props.deleteReply(replyToDelete);
                }
            } else {
                if (confirm('Are you sure you want to delete this comment?')) {

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

    public async toggleLikeReply(reply: IDiscussionReply) {

        this.setState({
            isLoading: true,
        }); 

        await this.props.toggleLikeReply(reply, !this.isReplyLikedByCurrentUser(reply));

        this.setState({
            isLoading: false,
        });
    }

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
            showInput: isVisible,
            editMode: editMode,
            inputValue: inputValue,
        });
    }

    public onValueChange(e: any) {

        this.setState({ 
            inputValue: e.target.value,
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
}

export default DiscussionReply;