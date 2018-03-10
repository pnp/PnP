interface IGraphBotSettings {
  /**
   * The bot application id
   */
  BotId: string;

  /**
   * The secret key for the bot "Direct Line" channel
   */
  DirectLineSecret: string;
}

export default IGraphBotSettings;
