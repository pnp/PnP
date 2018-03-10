// ========================================
// Main Menu Component View Model
// ========================================
import * as i18n from "i18next";
import * as moment from "moment";
import "pubsub-js";
import { Logger, LogLevel, Site, storage, Util } from "sp-pnp-js";
import ITaxonomyNavigationNode from "../../models/ITaxonomyNavigationNode";
import TaxonomyModule from "../../modules/TaxonomyModule";
import UtilityModule from "../../modules/UtilityModule";
import NavigationViewModel from "../NavigationViewModel";

class TopNavViewModel extends NavigationViewModel {

    public taxonomyModule: TaxonomyModule;
    public utilityModule: UtilityModule;
    public errorMessage: KnockoutObservable<string>;
    public utilityNavigationLabel: KnockoutObservable<string>;
    public localStorageKey: string;
    public wait: KnockoutObservable<boolean>;
    public mainMenuCollapseElementId: KnockoutObservable<string>;
    public mainMenuCollapseElementIdSelector: KnockoutComputed<string>;
    public utilityMenuCollapseElementId: KnockoutObservable<string>;
    public utilityMenuCollapseElementIdSelector: KnockoutComputed<string>;

    constructor() {

        super();

        this.taxonomyModule = new TaxonomyModule();
        this.utilityModule = new UtilityModule();
        this.errorMessage = ko.observable("");
        this.utilityNavigationLabel = ko.observable(i18n.t("utilityNavigation"));
        this.wait = ko.observable(true);

        // Because we could use the top nav component twice in the DOM (in the welcome overlay control), the Bootstrap collapse id's can't be the same.
        // That's why we use a dynamic id to avoid conflicts
        this.mainMenuCollapseElementId = ko.observable("navbar-collapse-" + this.utilityModule.getNewGuid());
        this.mainMenuCollapseElementIdSelector = ko.computed(() => {
            return ("#" + this.mainMenuCollapseElementId());
        });

        this.utilityMenuCollapseElementId = ko.observable("utility-collapse-" + this.utilityModule.getNewGuid());
        this.utilityMenuCollapseElementIdSelector = ko.computed(() => {
            return ("#" + this.utilityMenuCollapseElementId());
        });

        // We set the current site collection URL as unique identifier prefix for local storage key
        // By this way, we are able to browse multiple versions of the PnP Starter Intranet solution within the same browser and without navigation conficts.
        this.localStorageKey = String.format("{0}_{1}", _spPageContextInfo.siteServerRelativeUrl, i18n.t("siteMapLocalStorageKey"));

        // The current language is determined at the entry point of the application
        // Instead of making a second call to get the current langauge, we get the corresponding resource value according to the current context (like we already do for LCID)
        const currentLanguage = i18n.t("languageLabel");

        // Yamm3! MegaMenu
        $(document).on("click", ".yamm .dropdown-menu", (e) => {
            e.stopPropagation();
        });

        $(document).ready(() => {

            $(this.mainMenuCollapseElementIdSelector()).on("collapse", () => {

                const elt = $("button.navbar-toggle i");

                if (elt.hasClass("fa-times")) {
                    elt.removeClass("fa-times");
                    elt.addClass("fa-bars");

                } else {

                    if (elt.hasClass("fa-bars")) {
                        elt.removeClass("fa-bars");
                        elt.addClass("fa-times");
                    }
                }
            });

            // Ovveride the collapse default behavior to get a slide transition (R to L)
            $('[data-target="' + this.mainMenuCollapseElementIdSelector()  + '"]').on("click", (elt) => {
                const navMenuCont = $($(elt.currentTarget).data("target"));
                navMenuCont.animate(
                    {
                        width: "toggle",
                    },
                    {
                        complete: () => {
                            navMenuCont.trigger("collapse");
                        },
                        duration: 200,
                        specialEasing: {
                            height: "easeOutBounce",
                            width: "linear",
                        },
                    });
            });
        });

        // jQuery event for the collapsible menu in the mobile view for header links and language switcher
        $("#navigation-panel").on("hide.bs.collapse", () => {
            $("#utility-collapse i").removeClass("fa-angle-up");
            $("#utility-collapse i").addClass("fa-angle-down");
        });

        $("#navigation-panel").on("show.bs.collapse", () => {
            $("#utility-collapse i").removeClass("fa-angle-down");
            $("#utility-collapse i").addClass("fa-angle-up");
        });

        // Read the configuration value from the configuration list and for the current language.
        // We use a list item instead of a term set property to improve performances (SOD loading is slow compared to a simple REST call).
        // We also use caching to improve performances
        this.utilityModule.getConfigurationListValuesForLanguage(currentLanguage).then((item) => {

            if (item) {

                // Get the boolean value
                const noCache: boolean = item.ForceCacheRefresh;

                // Get the term set id
                const termSetId = item.SiteMapTermSetId;

                const navigationTree = this.utilityModule.isCacheValueValid(this.localStorageKey);

                // Check if the local storage value is still valid (i.e not null)
                if (navigationTree) {

                    // We first initialize with the value from cache to avoid the waiting time.
                    // Reason: In some cases, the waiting time was too long for the user to get the new nodes.
                    // We do prefer display the menu first from cache and then make updates behind the scenes.
                    // It is less frustrating and nearly seamless for users especially if your intranet links change often (i.e in the early stages of deployment).
                    this.initialize(navigationTree);
                    this.wait(false);

                    // Publish the data to all subscribers (contextual menu and breadcrumb)
                    PubSub.publish("navigationNodes", { nodes: navigationTree });

                    if (noCache) {

                        // Get the new navigation nodes
                        this.getNavigationNodes(termSetId);
                    }

                } else {

                    this.getNavigationNodes(termSetId);
                }
            }
        }).catch((errorMesssage) => {

            this.errorMessage(errorMesssage);
            this.wait(false);
            Logger.write("[TopNav.readConfigItem]: " + errorMesssage, LogLevel.Error);
        });
    }

    public selectNode = (data, event) => {

        const nodes = this.nodes().map((node) => {
           if (node.id === data.id) {
                // Select current node
                node.isSelected(!node.isSelected());
                return node;
           } else {
                // Unselect all other nodes
                node.isSelected(false);
                return node;
           }
        });

        this.nodes(nodes);
    }

    public unselectNode = (elt) => {
        if ($(elt).parent("li").hasClass("open")) {
            const nodes = this.nodes().map((node) => {
                     // Unselect all other nodes
                     node.isSelected(false);
                     return node;
            });

            this.nodes(nodes);
        }
    }

    private getNavigationNodes(termSetId: string): void {

        if (!termSetId) {

            const errorMesssage = "The term set id for the site map is null. Please specify a valid term set id in the configuration list";
            Logger.write("[TopNav.getNavigationNodes]: " + errorMesssage, LogLevel.Error);

            this.errorMessage(errorMesssage);
            this.wait(false);

        } else {

            // Ensure all SP dependencies are loaded before retrieving navigation nodes
            this.taxonomyModule.init().then(() => {

            // Initialize the main menu with taxonomy terms
            this.taxonomyModule.getNavigationTaxonomyNodes(new SP.Guid(termSetId)).then((navigationTree: ITaxonomyNavigationNode[]) => {

                        // Initialize the mainMenu view model
                        this.initialize(navigationTree);
                        this.wait(false);

                        // Publish the data to all subscribers (contextual menu and breadcrumb)
                        PubSub.publish("navigationNodes", { nodes: navigationTree });

                        const now: Date = new Date();

                        // Clear the local storage value
                        storage.local.delete(this.localStorageKey);

                        // Set the navigation tree in the local storage of the browser
                        storage.local.put(this.localStorageKey, this.utilityModule.stringifyTreeObject(navigationTree), new Date(now.setDate(now.getDate() + 7)));

                }).catch((errorMesssage) => {

                    this.errorMessage(errorMesssage + ". Empty the localStorage values in the browser for the configuration list and try again.");

                    // Clear the local storage value
                    storage.local.delete(this.localStorageKey);

                    this.wait(false);
                    this.initialize([]);
                    Logger.write("[TopNav.getNavigationNodes]: " + errorMesssage, LogLevel.Error);
                });

            }).catch((errorMesssage) => {

                this.errorMessage(errorMesssage + ". Empty the localStorage values in the browser for the configuration list and try again.");

                // Clear the local storage value
                storage.local.delete(this.localStorageKey);

                this.wait(false);
                this.initialize([]);
                Logger.write("[TopNav.getNavigationNodes]: " + errorMesssage, LogLevel.Error);
            });
        }
    }
}

export default TopNavViewModel;
