// ========================================
// Represents a configuration list item
// ========================================
interface IConfigurationItem {
    AppInsightsInstrumentationKey: string;
    Title: string;
    Id: number;
    IntranetContentLanguage: string;
    BotId: string;
    BotHandle: string;
    BotDirectLineSecretKey: string;
    ForceCacheRefresh: boolean;
    FooterLinksTermSetId: string;
    HeaderLinksTermSetId: string;
    SiteMapTermSetId: string;
}

export default IConfigurationItem;
