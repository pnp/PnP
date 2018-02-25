import * as React from "react";
import IDiscussionReplyProps from "./IDiscussionReplyProps";
import { IDiscussionReplyState, EditMode } from "./IDiscussionReplyState";
import { PermissionKind } from "sp-pnp-js";
import { IDiscussionReply, DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

class DiscussionReply extends React.Component<IDiscussionReplyProps, IDiscussionReplyState> {

    private _replyBodyInputRef: any;

    public constructor() {
        super();

        this.state = {
            showInput: false,
            editMode: EditMode.NewComment,
        };

        this.toggleInput = this.toggleInput.bind(this);
    }

    public render() {

        let renderEdit = null;
        if (this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.EditAsAuthor ) !== -1 || 
            this.props.reply.UserPermissions.indexOf(DiscussionPermissionLevel.ManageLists ) !== -1) {
            renderEdit = <a onClick={ () => {
                this.toggleInput(EditMode.UpdateComment);
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
                this.toggleInput(EditMode.NewComment);
            }}>Reply</a>;
        }
            
        return  <div>
                    <div dangerouslySetInnerHTML= {{__html: this.props.reply.Body } }></div>
                    { renderEdit }                   
                    { renderDelete }
                    { renderReply }
                    { this.state.showInput ? 
                        <div>
                            <textarea ref={ (input) => { this._replyBodyInputRef = input; }} value={ this.state.editMode === EditMode.UpdateComment ? this.props.reply.Body: "" }></textarea>
                            <button type="button" onClick={ () => {

                                switch (this.state.editMode) {
                                    case EditMode.NewComment:
                                        this.props.addNewReply(this.props.reply.Id, this._replyBodyInputRef.value);
                                        break;

                                    case EditMode.UpdateComment:
                                        const reply: IDiscussionReply = {
                                            Id: this.props.reply.Id,
                                            Body: this._replyBodyInputRef.value,
                                        };

                                        this.props.updateReply(reply);
                                        break;
                                }

                                this.toggleInput(null);
                            }}>Reply</button>
                        </div>
                        : 
                            null
                    }
                </div>
    }

    public toggleInput(editMode: EditMode) {
        this.setState({
            showInput: !this.state.showInput,
            editMode: editMode,
        });
    }
}

export default DiscussionReply;