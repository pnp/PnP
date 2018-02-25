import * as React from "react";
import IDiscussionReplyProps from "./IDiscussionReplyProps";
import { IDiscussionReplyState, EditMode } from "./IDiscussionReplyState";
import { PermissionKind } from "sp-pnp-js";
import { IDiscussionReply, DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

class DiscussionReply extends React.Component<IDiscussionReplyProps, IDiscussionReplyState> {

    public constructor() {
        super();

        this.state = {
            showInput: false,
            editMode: EditMode.NewComment,
            inputValue: "",
        };

        this.toggleInput = this.toggleInput.bind(this);
        this.onValueChange = this.onValueChange.bind(this);
    }

    public render() {

        let renderEdit = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.EditAsAuthor ) !== -1 || 
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.ManageLists ) !== -1) {
            renderEdit = <a onClick={ () => {
                this.toggleInput(true, EditMode.UpdateComment);
            }}>Edit</a>;
        }

        let renderDelete = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.EditAsAuthor) !== -1 || 
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.ManageLists ) !== -1) {
            renderDelete = <a onClick={ () => { this.props.deleteReply(this.props.reply.Id) }}>Delete</a>;
        }

        let renderReply = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.Add) !== -1) {
            renderReply = <a onClick={ () => {
                this.toggleInput(true, EditMode.NewComment);
            }}>Reply</a>;
        }
            
        return  <div>
                    <img src={ this.props.reply.Author.PictureUrl}/>
                    <div>{ this.props.reply.Author.DisplayName }</div>
                    <div dangerouslySetInnerHTML= {{__html: $(this.props.reply.Body).text() }}></div>
                    { renderEdit }                   
                    { renderDelete }
                    { renderReply }
                    { this.state.showInput ? 
                        <div>
                            <textarea   defaultValue={ this.state.editMode === EditMode.UpdateComment ? $(this.props.reply.Body).text() : "" }
                                        placeholder="Add your comment..."
                                        onChange={ this.onValueChange }></textarea>
                            <button type="button" onClick={ () => {

                                switch (this.state.editMode) {
                                    case EditMode.NewComment:
                                        this.props.addNewReply(this.props.reply.Id, this.state.inputValue);
                                        break;

                                    case EditMode.UpdateComment:
                                        const reply: IDiscussionReply = {
                                            Id: this.props.reply.Id,
                                            Body: `<div>${this.state.inputValue}</div>`, // Set as HTML
                                        };

                                        this.props.updateReply(reply);
                                        break;
                                }

                                this.toggleInput(false, null);
                            }}>{ this.state.editMode === EditMode.UpdateComment ? "Update" : "Post" }</button>
                        </div>
                        : 
                            null
                    }
                </div>
    }

    public toggleInput(isVisible: boolean, editMode: EditMode) {

        this.setState({
            showInput: isVisible,
            editMode: editMode,
        });
    }

    public onValueChange(e: any) {
        this.setState({ inputValue: e.target.value });
    }
}

export default DiscussionReply;