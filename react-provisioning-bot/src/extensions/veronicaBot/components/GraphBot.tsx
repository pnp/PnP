import * as React from "react";
import IGraphBotProps from "./IGraphBotProps";
import { ActionButton } from "office-ui-fabric-react/lib/Button";
import { Panel, PanelType } from "office-ui-fabric-react/lib/Panel";
import { Spinner, SpinnerSize } from "office-ui-fabric-react/lib/spinner";
import { Overlay } from "office-ui-fabric-react/lib/overlay";
import * as ReactDOM from 'react-dom';
import { Chat, DirectLine, DirectLineOptions, ConnectionStatus } from 'botframework-webchat';
import IGraphBotState from "./IGraphBotState";
require("botframework-webchat/botchat.css");
import pnp, { Logger, LogLevel } from "sp-pnp-js";
import { Text } from "@microsoft/sp-core-library";
import styles from "./GraphBot.module.scss";
import { SPHttpClient } from "@microsoft/sp-http";
import IGraphBotSettings from "./IGraphBotSettings";
import * as strings from "VeronicaBotApplicationCustomizerStrings";

class GraphBot extends React.Component<IGraphBotProps, IGraphBotState> {

  private _botConnection: DirectLine;
  private _botId: string;
  private _directLineSecret: string;

  // Local storage keys
  private readonly ENTITYKEY_BOTID = "PnPGraphBot_BotId";
  private readonly ENTITYKEY_DIRECTLINESECRET = "PnPGraphBot_BotDirectLineSecret";
  private readonly CONVERSATION_ID_KEY = "PnPGraphBot_ConversationId";

  constructor(props: IGraphBotProps) {
    super(props);

    this._login = this._login.bind(this);

    this.state = {
      showPanel: false,
      isBotInitializing: false
    };

    // Enable sp-pnp-js session storage wrapper
    pnp.storage.local.enabled = true;
  }

  public render() {

    // Be careful, the user Id is mandatory to be able to use the bot state service (i.e privateConversationData)
    return (
      <div className={styles.banner}>
        <ActionButton onClick={this._login} checked={true} iconProps={{ iconName: "Robot", className: styles.banner__chatButtonIcon }} className={styles.banner__chatButton}>
          {strings.GraphBotButtonLabel}
        </ActionButton>
        <Panel
          isOpen={this.state.showPanel}
          type={PanelType.medium}
          isLightDismiss={true}
          onDismiss={() => this.setState({ showPanel: false })}
        >
          {this.state.isBotInitializing ?
            <Overlay className={styles.overlayList} >
              <Spinner size={SpinnerSize.large} label={strings.GraphBotInitializationMessage} />
            </Overlay>
            :
            <Chat
              botConnection={this._botConnection}
              bot={
                {
                  id: this._botId,
                }
              }
              user={
                {
                  // IMPORTANT (2 of 2): USE THE SAME USER ID FOR BOT STATE TO BE ABLE TO GET USER SPECIFIC DATA
                  id: this.props.context.pageContext.user.email,
                  name: this.props.context.pageContext.user.displayName,
                }
              }
              locale={this.props.context.pageContext.cultureInfo.currentCultureName}
              formatOptions={
                {
                  showHeader: false,
                }
              }
            />
          }
        </Panel>
      </div>
    );
  }

  public async componentDidMount() {

    // Delete expired local storage items (conversation id, etc.)
    pnp.storage.local.deleteExpired();

    // Read the bot settings from the tenant property bag or local storage if available
    const settings = await this._getGraphBotSettings(this.props);

    // Note: no need to store these informations in state because they are never updated after that
    this._botId = settings.BotId;
    this._directLineSecret = settings.DirectLineSecret;
  }

  /**
   * Login the current user
   */
  private async _login() {

    this.setState({
      isBotInitializing: true,
      showPanel: true,
    });

    // Get the conversation id if there is one. Otherwise, a new one will be created
    const conversationId = pnp.storage.local.get(this.CONVERSATION_ID_KEY);

    // Initialize the bot connection direct line
    this._botConnection = new DirectLine({
      secret: this._directLineSecret,
      webSocket: false, // Needed to be able to retrieve history
      conversationId: conversationId ? conversationId : null,
    });

    this._botConnection.connectionStatus$
      .subscribe((connectionStatus) => {
        switch (connectionStatus) {
          // Successfully connected to the converstaion.
          case ConnectionStatus.Online:
            if (!conversationId) {
              // Store the current conversation id in the browser session storage
              // with 15 minutes expiration
              pnp.storage.local.put(
                this.CONVERSATION_ID_KEY, this._botConnection["conversationId"],
                pnp.util.dateAdd(new Date(), "minute", 15)
              );
            }
            break;
          case ConnectionStatus.Uninitialized:
            this.setState({
              isBotInitializing: false,
            });
            break;
        }
      });

  }

  /**
   * Read the bot settings in the tenant property bag or local storage
   * @param props the component properties
   */
  private async _getGraphBotSettings(props: IGraphBotProps): Promise<IGraphBotSettings> {

    // Read these values from the local storage first
    let botId = pnp.storage.local.get(this.ENTITYKEY_BOTID);
    let directLineSecret = pnp.storage.local.get(this.ENTITYKEY_DIRECTLINESECRET);

    const expiration = pnp.util.dateAdd(new Date(), "day", 1);

    try {

      if (!botId) {
        botId = await this.getTenantPropertyValue(this.ENTITYKEY_BOTID);
        pnp.storage.local.put(this.ENTITYKEY_BOTID, botId, expiration);
      }

      if (!directLineSecret) {
        directLineSecret = await this.getTenantPropertyValue(this.ENTITYKEY_DIRECTLINESECRET);
        pnp.storage.local.put(this.ENTITYKEY_DIRECTLINESECRET, directLineSecret, expiration);
      }

      return {
        BotId: botId,
        DirectLineSecret: directLineSecret,
      } as IGraphBotSettings;

    } catch (error) {
      Logger.write(Text.format("[GraphBot_getGraphBotSettings]: Error: {0}", error));
    }
  }

  /**
   * Get the value of a tenant property bag property
   * @param key the property bag key
   */
  public async getTenantPropertyValue(key: string): Promise<any> {
    // Get settings from tenant properties
    try {
      pnp.sp.web.getStorageEntity(key).then(r => {
        console.log(r);
        return r;
      });
    } catch (error) {
      Logger.write(Text.format("[getTenantProperty]: Error: {0}", error));
    }
  }
}

export default GraphBot;
