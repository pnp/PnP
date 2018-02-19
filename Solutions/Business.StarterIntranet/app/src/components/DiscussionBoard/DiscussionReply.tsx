import * as React from "react";
import IDiscussionReplyProps from "./IDiscussionReplyProps";
import IDiscussionReplyState from "./IDiscussionReplyState";

class DiscussionReply extends React.Component<IDiscussionReplyProps, IDiscussionReplyState> {

    private _replyBodyInputRef: any;

    public constructor() {
        super();

        this.state = {
            showInput: false,
        };

        this.toggleInput = this.toggleInput.bind(this);
    }

    public render() {
        return <div>
                <div dangerouslySetInnerHTML= {{__html: this.props.reply.Body } }>
                </div>
                <a onClick={ () => { this.props.deleteReply(this.props.reply.Id) }}>Delete</a>
                <a onClick={ this.toggleInput }>Reply</a>
                { this.state.showInput ? 
                    <div>
                        <textarea ref={ (input) => { this._replyBodyInputRef = input; }}></textarea>
                        <button type="button" onClick={ () => { 
                            this.props.addNewReply(this.props.reply.Id, this._replyBodyInputRef.value);
                            this.toggleInput();
                        }}>Reply</button>
                    </div>
                    : 
                        null
                }
        </div>
    }

    public toggleInput() {
        this.setState({
            showInput: !this.state.showInput,
        });
    }
}

export default DiscussionReply;