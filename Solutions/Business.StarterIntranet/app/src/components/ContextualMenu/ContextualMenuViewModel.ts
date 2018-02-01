// ========================================
// Contextual Menu Component View Model
// ========================================
import "pubsub-js";
import { Logger, LogLevel, Web } from "sp-pnp-js";
import TaxonomyNavigationNode from "../../models/TaxonomyNavigationNode";
import UtilityModule from "../../modules/UtilityModule";
import NavigationViewModel from "../NavigationViewModel";

class ContextualMenuViewModel extends NavigationViewModel {

    public siteMapFieldName: string;
    public utilityModule: UtilityModule;
    public parentSection: KnockoutObservable<TaxonomyNavigationNode>;
    public wait: KnockoutObservable<boolean>;

    constructor(params: any) {

        super();

        this.utilityModule = new UtilityModule();

        // The internal name for the site map taxonomy field
        this.siteMapFieldName = params.siteMapFieldName;

        this.parentSection = ko.observable(null);
        this.wait = ko.observable(true);

        // Collapse events
        $("#contextualmenu").on("hide.bs.collapse", (event) => {

            const iconElt = $("[data-target='#" + event.target.id + "']").find("i");
            if (iconElt) {
                iconElt.removeClass("fa-angle-up");
                iconElt.addClass("fa-angle-down");
            }
        });

        $("#contextualmenu").on("show.bs.collapse", (event) => {

            event.stopPropagation();

            // Get the parent with the data-target attribute equals to my id.
            const iconElt = $("[data-target='#" + event.target.id + "']").find("i");
            if (iconElt) {
                iconElt.removeClass("fa-angle-down");
                iconElt.addClass("fa-angle-up");
            }
        });

        // Subscribe to the main menu nodes
        PubSub.subscribe("navigationNodes", (msg, data) => {

            const navigationTree: TaxonomyNavigationNode[] = data.nodes;
            const contextualMenuNodes: TaxonomyNavigationNode[] = [];
            const web = new Web(_spPageContextInfo.webAbsoluteUrl);

            web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(_spPageContextInfo.pageItemId).select(this.siteMapFieldName).get().then((item) => {

                    const siteMapTermGuid = item[this.siteMapFieldName];
                    let currentNode: TaxonomyNavigationNode;

                    if (siteMapTermGuid) {

                        // 1: Search for this guid in the site map
                        currentNode = this.utilityModule.getNodeByTermId(navigationTree, siteMapTermGuid.TermGuid);
                    }

                    if (currentNode === undefined) {

                        // 2: Get the navigation node according to the current URL
                        currentNode = this.utilityModule.getNodeByUrl(navigationTree, window.location.pathname);
                    }

                    if (currentNode !== undefined) {

                        // Set the current node in the contextual nodes
                        contextualMenuNodes.push(currentNode);

                        // If there is no 'ParentId', this is a root term
                        if (currentNode.ParentId !== null) {

                            const parentNode = this.utilityModule.getNodeByTermId(navigationTree, currentNode.ParentId);

                            // Set the parent section
                            this.parentSection(parentNode);

                            /*if (parentNode.ChildNodes.length > 0) {

                                // Display all siblings and child nodes from the current node (just like the CSOM results)
                                // Siblings = children of my own parent ;)
                                navigationTree = parentNode.ChildNodes;

                                // Set the current node as first item
                                navigationTree = this.utilityModule.moveItem(navigationTree, navigationTree.indexOf(currentNode), 0);
                            }*/
                        }

                    } else {
                        Logger.write("[ContextualMenu.subscribe]: Unable to determine the current position in the site map", LogLevel.Warning);
                    }

                    this.initialize(contextualMenuNodes);
                    this.wait(false);

                    if (currentNode !== undefined) {

                        this.setCurrentNode(currentNode.Id);
                    }

                    // Truncate links
                    $("#contextualmenu a").trunk8({
                        lines: 3,
                        tooltip: true,
                    });

                    // Collapse the contextual menu on mobile view
                    const isVisibleAnchor = $("#isVisibleAnchor");
                    if (isVisibleAnchor.is(":visible")) {
                        $("[id^='collapse']").collapse();
                    }

            }).catch((errorMesssage) => {
                Logger.write("[ContextualMenu.subscribe]: " + errorMesssage, LogLevel.Error);
            });
        });
    }
}

export default ContextualMenuViewModel;
