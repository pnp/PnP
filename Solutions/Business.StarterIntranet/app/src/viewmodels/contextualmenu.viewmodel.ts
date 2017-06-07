// ========================================
// Contextual Menu Component View Model
// ========================================
import { UtilityModule } from "../core/utility";
import { NavigationViewModel } from "../shared/navigation.viewmodel";
import { NavigationNode } from "../shared/navigationnode";
import "pubsub-js";
import { Web, Logger, LogLevel } from "sp-pnp-js";

export class ContextualMenuViewModel extends NavigationViewModel {

    public siteMapFieldName: string;
    public utilityModule: UtilityModule;
    public parentSection: KnockoutObservable<NavigationNode>;
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

            let iconElt = $("[data-target='#" + event.target.id + "']").find("i");
            if (iconElt) {
                iconElt.removeClass("fa-angle-up");
                iconElt.addClass("fa-angle-down");
            }
        });

        $("#contextualmenu").on("show.bs.collapse", (event) => {

            event.stopPropagation();

            // Get the parent with the data-target attribute equals to my id.
            let iconElt = $("[data-target='#" + event.target.id + "']").find("i");
            if (iconElt) {
                iconElt.removeClass("fa-angle-down");
                iconElt.addClass("fa-angle-up");
            }
        });

        // Subscribe to the main menu nodes
        PubSub.subscribe("navigationNodes", (msg, data) => {

            let navigationTree: Array<NavigationNode> = data.nodes;
            let web = new Web(_spPageContextInfo.webAbsoluteUrl);  

            web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select(this.siteMapFieldName).get().then((item) => {

                    let siteMapTermGuid = item[this.siteMapFieldName];
                    let currentNode: NavigationNode = undefined;

                    if (siteMapTermGuid) {

                        // 1: Search for this guid in the site map
                        currentNode = this.utilityModule.getNodeByTermId(navigationTree, siteMapTermGuid.TermGuid);
                    }

                    if (currentNode === undefined) {

                        // 2: Get the navigation node according to the current URL   
                        currentNode = this.utilityModule.getNodeByUrl(navigationTree, window.location.pathname);
                    }

                    if (currentNode !== undefined) {

                        // If there is no 'ParentId', this is a root term
                        if (currentNode.ParentId !== null) {


                            let parentNode = this.utilityModule.getNodeByTermId(navigationTree, currentNode.ParentId);

                            // Set the parent section
                            this.parentSection(parentNode);

                            if (parentNode.ChildNodes.length > 0) {

                                // Display all siblings and child nodes from the current node (just like the CSOM results)
                                // Siblings = children of my own parent ;)
                                navigationTree = parentNode.ChildNodes;

                                // Set the current node as first item
                                navigationTree = this.utilityModule.moveItem(navigationTree, navigationTree.indexOf(currentNode), 0);
                            }
                        }

                    } else {

                        Logger.write("[Contextual Menu] Unable to determine the current position in the site map", LogLevel.Warning);
                    }

                    this.initialize(navigationTree);
                    this.wait(false);

                    if (currentNode !== undefined) {

                        this.setCurrentNode(currentNode.Id);
                    }

            }).catch((errorMesssage) => {

                Logger.write(errorMesssage, LogLevel.Error);
            });
        });
    }
}
