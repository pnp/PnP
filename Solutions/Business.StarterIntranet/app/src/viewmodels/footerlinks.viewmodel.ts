// ========================================
// Footer Links Component View Model
// ========================================
import { TaxonomyModule } from "../core/taxonomy";
import { UtilityModule } from "../core/utility";
import { NavigationViewModel } from "../shared/navigation.viewmodel";
import * as i18n from "i18next";
import { Site, storage, Logger, LogLevel } from "sp-pnp-js";

export class FooterLinksViewModel extends NavigationViewModel {

    public taxonomyModule: TaxonomyModule;
    public utilityModule: UtilityModule;
    public localStorageKey: string;
    public wait: KnockoutObservable<boolean>;

    constructor() {

        super();

        this.taxonomyModule = new TaxonomyModule();
        this.utilityModule = new UtilityModule();

        this.wait = ko.observable(true);

        let currentLanguage = i18n.t("LanguageLabel");
        let configListName = "Configuration";

        this.localStorageKey = i18n.t("footerLinksLocalStorageKey");

        let filterQuery: string = "IntranetContentLanguage eq '" + currentLanguage + "'";
        
        let site = new Site(_spPageContextInfo.siteAbsoluteUrl);

        // Read the configuration value from a configuration list instead from a term set property to improve performances
        // Get only the first item
        site.rootWeb.lists.getByTitle(configListName).items.filter(filterQuery).top(1).get().then((item) => {

            if (item.length > 0) {

                // Get the boolean value
                let noCache: boolean = item[0].ForceCacheRefresh;

                // Get the term set id
                let termSetId = item[0].FooterLinksTermSetId;

                if (noCache) {

                        // Clear the local storage value
                        storage.local.delete(this.localStorageKey);

                        // Get navigation nodes
                        this.getNavigationNodes(termSetId);

                } else {

                    let navigationTree = this.utilityModule.isCacheValueValid(this.localStorageKey);

                    // Check if the local storage value is still valid
                    if (navigationTree) {

                        this.initialize(navigationTree);
                        this.wait(false);

                    } else {

                        this.getNavigationNodes(termSetId);
                    }
                }

            } else {

                Logger.write("There is no configuration item for this site", LogLevel.Warning);
            }

        }).catch(errorMesssage => {

            Logger.write(errorMesssage, LogLevel.Error);
        });
    }

    private getNavigationNodes(termSetId: string): void {

        if (!termSetId) {

            Logger.write("The term set id for the footer links is null. Please specify a valid term set id in the configuration list", LogLevel.Error);

        } else {

            // Ensure all SP dependencies are loaded before retrieving navigation nodes
            this.taxonomyModule.init().then(() => {

            // Initialize the main menu with taxonomy terms            
            this.taxonomyModule.getNavigationTaxonomyNodes(new SP.Guid(termSetId)).then(navigationTree => {

                        // Initialize the mainMenu view model
                        this.initialize(navigationTree);
                        this.wait(false);

                        let now: Date = new Date();

                        // Set the navigation tree in the local storage of the browser
                        storage.local.put(this.localStorageKey, this.utilityModule.stringifyTreeObject(navigationTree), new Date(now.setDate(now.getDate() + 7)));

                }).catch(errorMesssage => {

                    Logger.write(errorMesssage, LogLevel.Error);
                });

            }).catch(errorMesssage => {

                Logger.write(errorMesssage, LogLevel.Error);
            });
        }
    }
}
