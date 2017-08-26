// ========================================
// Represents a configuration list item
// ========================================
import IConfigurationItem from "./IConfigurationItem";

class ConfigurationItem implements IConfigurationItem {

    public static SelectFields = [
            "Title",
            "ID",
            "IntranetContentLanguage",
            "ForceCacheRefresh",
            "FooterLinksTermSetId",
            "HeaderLinksTermSetId",
            "SiteMapTermSetId",
            "AppInsightsInstrumentationKey",
            "BotId",
            "BotHandle",
            "BotDirectLineSecretKey",
        ];

    public AppInsightsInstrumentationKey: string;
    public Title: string;
    public Id: number;
    public IntranetContentLanguage: string;
    public BotId: string;
    public BotHandle: string;
    public BotDirectLineSecretKey: string;
    public ForceCacheRefresh: boolean;
    public FooterLinksTermSetId: string;
    public HeaderLinksTermSetId: string;
    public SiteMapTermSetId: string;
}

export default ConfigurationItem;
