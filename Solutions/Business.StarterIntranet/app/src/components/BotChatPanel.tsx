import * as React from 'react';
import { IconButton, IButtonProps } from 'office-ui-fabric-react/lib/Button';
import { Panel } from 'office-ui-fabric-react/lib/Panel';
import { Chat } from 'botframework-webchat';
import { Site } from 'sp-pnp-js';
import * as i18n from 'i18next';

export class BotChatControl extends React.Component<any, any> {

  private configListName: string; 
  private currentLanguage: string;

  constructor() {
    super();

    this.configListName = "Configuration";
    this.state = { showPanel: false, isBotEnabled: false };
    this.currentLanguage = i18n.t("LanguageLabel");

    // This binding is necessary to make `this` work in the callback
    this.handleClick = this.handleClick.bind(this);
  }

  public handleClick(e) {

    e.preventDefault(); // Prevent the whole page to refresh

    this.setState(prevState => ({
      showPanel: true
    }));
  }

  public componentWillMount() {

      let site = new Site(_spPageContextInfo.siteAbsoluteUrl);
      let filterQuery: string = "IntranetContentLanguage eq '" + this.currentLanguage + "'";

      site.rootWeb.lists.getByTitle(this.configListName).items.filter(filterQuery).top(1).get().then((item) => {
        let toto = item;
        this.setState({isBotEnabled : false});
      });
  }

  public render() {

    return (

      <div> 
        <IconButton
          disabled = { this.state.isBotEnabled }
          iconProps={ { iconName: 'Emoji2' } }
          title='Emoji'
          ariaLabel='Emoji' 
          onClick= { this.handleClick } />        
        <Panel
          isOpen={ this.state.showPanel }
          isLightDismiss={ true }
          headerText=''
          onDismiss={ () => this.setState({ showPanel: false }) }
        >
          <Chat 
            bot={{id: 'a0095b82-a596-450f-957a-a62b858b75cf', name: 'SharePointBot'}}
            directLine={{ secret: "0ZVQsoBm6F0.cwA.YdE.HF64soQxOy2ls_t2wKXiL4BKV0HTf1zjiIzUMG-rbzY" }}
            user={{ id: 'user_id', name: 'Guest' }}/>
        </Panel>
      </div>
    );
  }
}