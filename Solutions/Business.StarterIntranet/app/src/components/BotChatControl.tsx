import * as React from 'react';
import { Link } from 'office-ui-fabric-react/lib/Link';
import { Panel } from 'office-ui-fabric-react/lib/Panel';
import { Chat } from 'botframework-webchat';
import { Site } from 'sp-pnp-js';
import * as i18n from 'i18next';

export class BotChatControl extends React.Component<any, any> {

  private configListName: string; 
  private currentLanguage: string;
  private botId: string;
  private botHandle: string;
  private botDirectLineSecretKey: string;
  private botLinkLabel: string;

  constructor() {
    super();

    this.configListName = "Configuration";
    this.state = { showPanel: false, isBotDisabled: true };
    this.currentLanguage = i18n.t("LanguageLabel");
    this.botLinkLabel = i18n.t("chatWithBot");

    // This binding is necessary to make `this` work in the callback
    this.handleClick = this.handleClick.bind(this);
    
  }

  public handleClick(e) {

    e.preventDefault(); // Prevent the whole page to refresh

    this.setState(prevState => ({
      showPanel: true
    }));
  }

  public componentDidMount() {

      let site = new Site(_spPageContextInfo.siteAbsoluteUrl);
      let filterQuery: string = "IntranetContentLanguage eq '" + this.currentLanguage + "'";

      site.rootWeb.lists.getByTitle(this.configListName).items.filter(filterQuery).top(1).get().then((item) => {

        let botId = item[0].BotId;
        let botHandle = item[0].BotHandle;
        let botDirectLineSecretKey = item[0].BotDirectLineSecretKey;

        if (botId && botHandle && botDirectLineSecretKey) {

          this.botId = botId;
          this.botHandle = botHandle;
          this.botDirectLineSecretKey = botDirectLineSecretKey;

          this.setState({ isBotDisabled : false });
        }
      });
  }

  public render() {

    return (

      <div> 
        <div className={ this.state.isBotDisabled ? "is-botdisabled": ""}>
          <i className="fa fa-comments" aria-hidden="true"></i>
          <Link
            disabled = { this.state.isBotDisabled }
            onClick= { this.handleClick }
            href="#">{ this.botLinkLabel }</Link>   

        </div>   
        <Panel
          isOpen={ this.state.showPanel }
          isLightDismiss={ true }
          headerText=''
          onDismiss={ () => this.setState({ showPanel: false }) }
        >
          <Chat 
            bot={{id: this.botId , name: this.botHandle }}
            directLine={{ secret: this.botDirectLineSecretKey }}
            user={{ id: 'user_id', name: 'Guest' }}
            locale={ this.currentLanguage.toLowerCase() }
            sendTyping= { true }/>
        </Panel>
      </div>
    );
  }
}