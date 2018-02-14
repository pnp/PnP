// ========================================
// Footer Links Component View Model
// ========================================
import * as i18n from "i18next";
import * as moment from "moment";
import { Logger, LogLevel, Site, storage, Util } from "sp-pnp-js";
import TaxonomyModule from "../../modules/TaxonomyModule";
import UtilityModule from "../../modules/UtilityModule";
import NavigationViewModel from "../NavigationViewModel";

class FooterLinksViewModel extends NavigationViewModel {

    public taxonomyModule: TaxonomyModule;
    public utilityModule: UtilityModule;
    public localStorageKey: string;
    public wait: KnockoutObservable<boolean>;

    constructor() {

        super();

        this.taxonomyModule = new TaxonomyModule();
        this.utilityModule = new UtilityModule();

        this.wait = ko.observable(true);
        const currentLanguage = i18n.t("languageLabel");
        this.localStorageKey = String.format("{0}_{1}", _spPageContextInfo.siteServerRelativeUrl, i18n.t("footerLinksLocalStorageKey"));

        this.utilityModule.getConfigurationListValuesForLanguage(currentLanguage).then((item) => {

            if (item) {

                // Get the boolean value
                const noCache: boolean = item.ForceCacheRefresh;

                // Get the term set id
                const termSetId = item.FooterLinksTermSetId;

                if (noCache) {

                        // Clear the local storage value
                        storage.local.delete(this.localStorageKey);

                        // Get navigation nodes
                        this.getNavigationNodes(termSetId);

                } else {

                    const navigationTree = this.utilityModule.isCacheValueValid(this.localStorageKey);

                    // Check if the local storage value is still valid
                    if (navigationTree) {

                        this.initialize(navigationTree);
                        this.wait(false);

                    } else {

                        this.getNavigationNodes(termSetId);
                    }
                }
            }
        }).catch((errorMesssage) => {

            Logger.write("[FooterLinks.readConfigItem]: " + errorMesssage, LogLevel.Error);
        });
    }

    private getNavigationNodes(termSetId: string): void {

        if (!termSetId) {

            Logger.write("[FooterLinks.getNavigationNodes]: The term set id for the footer links is null. Please specify a valid term set id in the configuration list", LogLevel.Error);

        } else {

            // Ensure all SP dependencies are loaded before retrieving navigation nodes
            this.taxonomyModule.init().then(() => {

            // Initialize the main menu with taxonomy terms
            this.taxonomyModule.getNavigationTaxonomyNodes(new SP.Guid(termSetId)).then((navigationTree) => {

                        // Initialize the mainMenu view model
                        this.initialize(navigationTree);
                        this.wait(false);

                        const now: Date = new Date();

                        // Set the navigation tree in the local storage of the browser
                        storage.local.put(this.localStorageKey, this.utilityModule.stringifyTreeObject(navigationTree), new Date(now.setDate(now.getDate() + 7)));

                }).catch((errorMesssage) => {

                    Logger.write("[FooterLinks.getNavigationNodes]: " + errorMesssage, LogLevel.Error);
                });

            }).catch((errorMesssage) => {

                Logger.write("[FooterLinks.getNavigationNodes]: " + errorMesssage, LogLevel.Error);
            });
        }
    }
}

export default FooterLinksViewModel;
