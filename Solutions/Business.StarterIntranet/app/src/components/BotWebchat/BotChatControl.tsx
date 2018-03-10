import { Chat } from "botframework-webchat";
import * as i18n from "i18next";
import * as moment from "moment";
import { Panel } from "office-ui-fabric-react";
import * as React from "react";
import UtilityModule from "../../modules/UtilityModule";

export class BotChatControl extends React.Component<any, any> {

  private currentLanguage: string;
  private botId: string;
  private botHandle: string;
  private botDirectLineSecretKey: string;
  private botLinkLabel: string;
  private utilityModule: UtilityModule;

  constructor(props: any) {
    super(props);

    this.utilityModule = new UtilityModule();

    this.state = { showPanel: false, isBotDisabled: true };
    this.currentLanguage = i18n.t("languageLabel");
    this.botLinkLabel = i18n.t("chatWithBot");

    // This binding is necessary to make `this` work in the callback
    this.handleClick = this.handleClick.bind(this);
  }

  public handleClick(e) {

    e.preventDefault(); // Prevent the whole page to refresh

    this.setState((prevState) => ({
      showPanel: true,
    }));
  }

  public componentDidMount() {

      this.utilityModule.getConfigurationListValuesForLanguage(this.currentLanguage).then((item) => {

          if (item) {

            const botId = item.BotId;
            const botHandle = item.BotHandle;
            const botDirectLineSecretKey = item.BotDirectLineSecretKey;

            if (botId && botHandle && botDirectLineSecretKey) {

              this.botId = botId;
              this.botHandle = botHandle;
              this.botDirectLineSecretKey = botDirectLineSecretKey;

              this.setState({ isBotDisabled : false });
            }
          }
      });
  }

  public render() {

    // Be careful, the user Id is mandatory to be able to use the bot state service (i.e privateConversationData)
    return (

      <div>
        <div className={ this.state.isBotDisabled ? "is-botdisabled" : ""}>
          <button onClick={ this.handleClick } type="button">
              <i className="fa fa-android" aria-hidden="true"></i>
          </button>
        </div>
        <Panel
          isOpen={ this.state.showPanel }
          isLightDismiss={ true }
          headerText=""
          onDismiss={ () => this.setState({ showPanel: false }) }
        >
        <Chat
            bot={
              {
                id: this.botId,
                name: this.botHandle,
              }
            }
            directLine={
              {
                secret: this.botDirectLineSecretKey,
              }
            }
            user={
              {
                id: _spPageContextInfo.userId.toString(),
                name: _spPageContextInfo["userDisplayName"],
              }
            }
            locale={ this.currentLanguage.toLowerCase() }
            sendTyping= { true }/>
        </Panel>
      </div>
    );
  }
}
