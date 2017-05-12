// ========================================
// Main Menu Component View Model
// ========================================
import * as i18n from "i18next";
import "pubsub-js";
import { Site, Logger, LogLevel, storage } from "sp-pnp-js";

import { TaxonomyModule } from "../core/taxonomy";
import { UtilityModule } from "../core/utility";
import { NavigationViewModel } from "../shared/navigation.viewmodel";

export class TopNavViewModel extends NavigationViewModel {

    public taxonomyModule: TaxonomyModule;
    public utilityModule: UtilityModule;
    public errorMessage: KnockoutObservable<string>;
    public localStorageKey: string;
    public wait: KnockoutObservable<boolean>;

    constructor() {

        super();

        this.taxonomyModule = new TaxonomyModule();
        this.utilityModule = new UtilityModule();
        this.errorMessage = ko.observable("");
        this.wait = ko.observable(true);

        this.localStorageKey = i18n.t("siteMapLocalStorageKey");

        // The current language is determined at the entry point of the application
        // Instead of making a second call to get the current langauge, we get the corresponding resource value according to the current context (like we already do for LCID)
        let currentLanguage = i18n.t("LanguageLabel");
        let configListName = "Configuration";

        // Yamm3! MegaMenu
        $(document).on("click", ".yamm .dropdown-menu", (e) => {
            e.stopPropagation();
        });

        let filterQuery: string = "IntranetContentLanguage eq '" + currentLanguage + "'";
        let site = new Site(_spPageContextInfo.siteAbsoluteUrl);

        // Read the configuration value from the configuration list and for the current langauge. We use a list item instead of a term set property to improve performances (SOD loading is slow compared to a simple REST call).
        site.rootWeb.lists.getByTitle(configListName).items.filter(filterQuery).top(1).get().then((item) => {

            if (item.length > 0) {

                // Get the boolean value
                let noCache: boolean = item[0].ForceCacheRefresh;

                // Get the term set id
                let termSetId = item[0].SiteMapTermSetId;

                if (noCache) {

                        // Clear the local storage value
                        storage.local.delete(this.localStorageKey);

                        // Get navigation nodes
                        this.getNavigationNodes(termSetId);

                } else {

                    let navigationTree = this.utilityModule.isCacheValueValid(this.localStorageKey);

                    // Check if the local storage value is still valid (i.e not null)
                    if (navigationTree) {

                        this.initialize(navigationTree);
                        this.wait(false);

                        // Publish the data to all subscribers (contextual menu and breadcrumb) 
                        PubSub.publish("navigationNodes", { nodes: navigationTree } );

                    } else {

                        this.getNavigationNodes(termSetId);
                    }
                }

            } else {

                Logger.write("There is no configuration item for the site map for the language '" + currentLanguage + "'", LogLevel.Error);
            }

        }).catch(errorMesssage => {

            this.errorMessage(errorMesssage);

            Logger.write(errorMesssage, LogLevel.Error);
        });
    }

    private getNavigationNodes(termSetId: string): void {

        if (!termSetId) {

            let errorMesssage = "The term set id for the site map is null. Please specify a valid term set id in the configuration list";
            Logger.write(errorMesssage, LogLevel.Error);

            this.errorMessage(errorMesssage);

        } else {

            // Ensure all SP dependencies are loaded before retrieving navigation nodes
            this.taxonomyModule.init().then(() => {

            // Initialize the main menu with taxonomy terms            
            this.taxonomyModule.getNavigationTaxonomyNodes(new SP.Guid(termSetId)).then(navigationTree => {

                        // Initialize the mainMenu view model
                        this.initialize(navigationTree);
                        this.wait(false);

                        // Publish the data to all subscribers (contextual menu and breadcrumb) 
                        PubSub.publish("navigationNodes", { nodes: navigationTree } );

                        let now: Date = new Date();

                        // Set the navigation tree in the local storage of the browser
                        storage.local.put(this.localStorageKey, this.utilityModule.stringifyTreeObject(navigationTree), new Date(now.setDate(now.getDate() + 7)));

                }).catch(errorMesssage => {

                    this.errorMessage(errorMesssage);
                    Logger.write(errorMesssage, LogLevel.Error);
                });

            }).catch(errorMesssage => {

                this.errorMessage(errorMesssage);
                Logger.write(errorMesssage, LogLevel.Error);
            });
        }
    }
}
