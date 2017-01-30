// ========================================
// Breadcrumb Component View Model
// ========================================
import { UtilityModule } from "../core/utility";
import { NavigationViewModel } from "../shared/navigation.viewmodel";
import { NavigationNode } from "../shared/navigationnode";
import * as i18n from "i18next";
import "pubsub-js";
import * as pnp from "sp-pnp-js";

export class BreadcrumbViewModel extends NavigationViewModel {

    public siteMapFieldName: string;
    public utilityModule: UtilityModule;
    public isEmptyNodes: KnockoutObservable<boolean>;
    public siteServerRelativeUrl: string;
    public errorMessage: KnockoutObservable<string>;

    constructor(params: any) {

        super();

        this.errorMessage = ko.observable(i18n.t("breadcrumbErrorMessage"));

        this.utilityModule = new UtilityModule();
        this.isEmptyNodes = ko.observable(false);

        // The internal name for the site map taxonomy field
        this.siteMapFieldName = params.siteMapFieldName;

        this.siteServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl;

        // Subscribe to the main menu nodes
        PubSub.subscribe("navigationNodes", (msg, data) => {

            let breadcrumbNodes = [];

            // There are two ways to determine the position of the current page in the navigation site map
            // 1) By checking the explicit value of the property used for content classification (and mapped to the site map term set).
            // 2) By checking the current url and try to find it in the navigation nodes data to get the corresponding term.
            pnp.sp.web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select(this.siteMapFieldName).get().then((item) => {

                    let siteMapTermGuid = item[this.siteMapFieldName];
                    let currentNode: NavigationNode = undefined;

                    if (siteMapTermGuid) {
                        // 1: Search for this guid in the site map
                        currentNode = this.utilityModule.getNodeByTermId(data.nodes, siteMapTermGuid.TermGuid);
                    }

                    if (currentNode === undefined) {

                        // 2: Get the navigation node according to the current URL   
                        currentNode = this.utilityModule.getNodeByUrl(data.nodes, window.location.pathname);
                    }

                    if (currentNode !== undefined) {

                        breadcrumbNodes.push(currentNode);

                        // If there is no 'ParentId', this is a root term
                        while (currentNode.ParentId !== null) {

                            let parentNode = this.utilityModule.getNodeByTermId(data.nodes, new SP.Guid(currentNode.ParentId));

                            breadcrumbNodes.push(parentNode);
                            currentNode = parentNode;
                        }

                        breadcrumbNodes = breadcrumbNodes.reverse();

                        this.initialize(breadcrumbNodes);

                        this.setCurrentNode(new SP.Guid(currentNode.Id));

                    } else {

                        this.isEmptyNodes(true);
                    }

            }).catch((errorMesssage) => {

                pnp.log.write(errorMesssage, pnp.LogLevel.Error);
            });
        });
    }
}
