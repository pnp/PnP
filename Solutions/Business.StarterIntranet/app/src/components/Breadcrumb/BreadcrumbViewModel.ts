// ========================================
// Breadcrumb Component View Model
// ========================================

import * as i18n from "i18next";
import "pubsub-js";
import { Logger, LogLevel, Web } from "sp-pnp-js";
import TaxonomyNavigationNode from "../../models/TaxonomyNavigationNode";
import UtilityModule from "../../modules/UtilityModule";
import NavigationViewModel from "../NavigationViewModel";

class BreadcrumbViewModel extends NavigationViewModel {

    public siteMapFieldName: string;
    public utilityModule: UtilityModule;
    public isEmptyNodes: KnockoutObservable<boolean>;
    public webServerRelativeUrl: string;
    public errorMessage: KnockoutObservable<string>;
    public wait: KnockoutObservable<boolean>;

    constructor(params: any) {

        super();

        this.errorMessage = ko.observable(i18n.t("breadcrumbErrorMessage"));

        this.utilityModule = new UtilityModule();
        this.isEmptyNodes = ko.observable(false);
        this.wait = ko.observable(true);

        // The internal name for the site map taxonomy field
        this.siteMapFieldName = params.siteMapFieldName;

        this.webServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl;

        // Subscribe to the main menu nodes
        PubSub.subscribe("navigationNodes", (msg, data) => {

            let breadcrumbNodes = [];

            const web = new Web(_spPageContextInfo.webAbsoluteUrl);

            // There are two ways to determine the position of the current page in the navigation site map
            // 1) By checking the explicit value of the property used for content classification (and mapped to the site map term set).
            // 2) By checking the current url and try to find it in the navigation nodes data to get the corresponding term.
            web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(_spPageContextInfo.pageItemId).select(this.siteMapFieldName).get().then((item) => {

                    const siteMapTermGuid = item[this.siteMapFieldName];
                    let currentNode: TaxonomyNavigationNode;

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

                        let currentNodeCopy = currentNode;

                        // If there is no 'ParentId', this is a root term
                        while (currentNodeCopy.ParentId !== null) {

                            const parentNode = this.utilityModule.getNodeByTermId(data.nodes, currentNodeCopy.ParentId);

                            breadcrumbNodes.push(parentNode);
                            currentNodeCopy = parentNode;
                        }

                        breadcrumbNodes = breadcrumbNodes.reverse();

                        this.initialize(breadcrumbNodes);
                        this.wait(false);

                        this.setCurrentNode(currentNode.Id);

                    } else {
                        this.wait(false);
                        this.isEmptyNodes(true);
                    }

            }).catch((errorMesssage) => {

                Logger.write("[Breadcrumb.subscribe]: " + errorMesssage, LogLevel.Error);
            });
        });
    }
}

export default BreadcrumbViewModel;
