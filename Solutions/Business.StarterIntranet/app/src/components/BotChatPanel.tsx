import * as React from "react";
import { DefaultButton } from 'office-ui-fabric-react/lib/Button';
import { Panel } from 'office-ui-fabric-react/lib/Panel';
import { Chat } from 'botframework-webchat';

export class BotChatControl extends React.Component<any, any> {

  constructor() {
    super();

    this.state = { showPanel: false };

    // This binding is necessary to make `this` work in the callback
    this.handleClick = this.handleClick.bind(this);
  }

  public handleClick(e) {

    e.preventDefault(); // Prevent hte whole page to refresh

    this.setState(prevState => ({
      showPanel: true
    }));
  }

  public render() {
    return (
      <div>
        <DefaultButton
          text='Open panel'
          onClick={ this.handleClick }
        />
        <Panel
          isOpen={ this.state.showPanel }
          isLightDismiss={ true }
          headerText=''
          onDismiss={ () => this.setState({ showPanel: false }) }
        >
          <Chat bot={{id: 'a0095b82-a596-450f-957a-a62b858b75cf', name: 'SharePointBot'}} directLine={{ secret: "0ZVQsoBm6F0.cwA.YdE.HF64soQxOy2ls_t2wKXiL4BKV0HTf1zjiIzUMG-rbzY" }} user={{ id: 'user_id', name: 'user_name' }}/>
        </Panel>
      </div>
    );
  }
}