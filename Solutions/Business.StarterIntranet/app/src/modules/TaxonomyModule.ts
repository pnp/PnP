// ====================
// Taxonomy module
// ====================
import * as i18n from "i18next";
import * as _ from "lodash";
import { Logger, LogLevel } from "sp-pnp-js";
import ITaxonomyNavigationNode from "../models/ITaxonomyNavigationNode";
import TaxonomyNavigationNode from "../models/TaxonomyNavigationNode";
import LocalizationModule from "./LocalizationModule";

class TaxonomyModule {

    private workingLanguage: number;

    constructor() {

        // Get the current working language from the global i18n object
        // We ensure a default language by the "fallbackLng" property (see main.ts for initialization)
        const localization = new LocalizationModule();

        // Ensure all resources are loaded before playng with the i18n object.
        localization.ensureResourcesLoaded(() => {
            this.workingLanguage = parseInt(i18n.t("LCID"), 10);
        });
    }

    /**
     * Ensure all script dependencies are loaded before using the taxonomy SharePoint CSOM functions
     * @return {Promise<void>}       A promise allowing you to execute your code logic.
     */
    public init(): Promise<void>  {

        // Initialize SharePoint script dependencies
        SP.SOD.registerSod("sp.runtime.js", "/_layouts/15/sp.runtime.js");
        SP.SOD.registerSod("sp.js", "/_layouts/15/sp.js");
        SP.SOD.registerSod("sp.taxonomy.js", "/_layouts/15/sp.taxonomy.js");
        SP.SOD.registerSod("sp.publishing.js", "/_layouts/15/sp.publishing.js");

        SP.SOD.registerSodDep("sp.js", "sp.runtime.js");
        SP.SOD.registerSodDep("sp.taxonomy.js", "sp.js");
        SP.SOD.registerSodDep("sp.publishing.js", "sp.js");

        const p = new Promise<void>((resolve) => {

            SP.SOD.loadMultiple(["sp.runtime.js", "sp.js", "sp.taxonomy.js", "sp.publishing.js"], () => {
                resolve();
            });
        });

        return p;
    }

    /**
     * Get a taxonomy term set custom property value
     * @param  {SP.Guid} termSetId The taxonomy term set Id
     * @param  {string} customPropertyName The name of the property to retrieve
     * @return {Promise<string>}       A promise containing the value of the property as string
     */
    public getTermSetCustomPropertyValue(termSetId: SP.Guid, customPropertyName: string): Promise<string> {

        const context: SP.ClientContext = SP.ClientContext.get_current();

        const taxSession: SP.Taxonomy.TaxonomySession  = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
        const termStore: SP.Taxonomy.TermStore = taxSession.getDefaultSiteCollectionTermStore();

        termStore.set_workingLanguage(this.workingLanguage);

        const termSet: SP.Taxonomy.TermSet = termStore.getTermSet(termSetId);

        context.load(termSet, "CustomProperties");

        const p = new Promise<string>((resolve, reject) => {

            context.executeQueryAsync(() => {

                const propertyValue: string = termSet.get_customProperties()[customPropertyName] !== undefined ? termSet.get_customProperties()[customPropertyName] : "";

                resolve(propertyValue);

            }, (sender, args) => {

                reject("Request failed. " + args.get_message() + "\n" + args.get_stackTrace());
            });
        });

        return p;
    }

    /**
     * Get the taxonomy navigation terms for a specific term set
     * @param  {SP.Guid} termSetId The taxonomy term set Id
     * @return {Promise<ITaxonomyNavigationNode[]>}       A promise containing the array of navigation nodes for the term set
     */
    public getNavigationTaxonomyNodes(termSetId: SP.Guid): Promise<ITaxonomyNavigationNode[]> {

        const context: SP.ClientContext = SP.ClientContext.get_current();
        const currentWeb: SP.Web = SP.ClientContext.get_current().get_web();

        const taxSession: SP.Taxonomy.TaxonomySession  = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
        const termStore: SP.Taxonomy.TermStore = taxSession.getDefaultSiteCollectionTermStore();

        termStore.set_workingLanguage(this.workingLanguage);

        const termSet: SP.Taxonomy.TermSet = termStore.getTermSet(termSetId);

        // The method 'getTermSetForWeb' gets the cached read only version of the term set
        // https://msdn.microsoft.com/EN-US/library/office/microsoft.sharepoint.publishing.navigation.taxonomynavigation.gettermsetforweb.aspx
        // Ex: var webNavigationTermSet = SP.Publishing.Navigation.TaxonomyNavigation.getTermSetForWeb(context, currentWeb, 'GlobalNavigationTaxonomyProvider', true);
        // In our case, we use 'getAsResolvedByWeb' method instead to retrieve a taxonomy term set as a navigation term set regardless if it is bound to the current web.
        // The downside of this approach is that the results are not retrieved from the navigation cache that can cause performance issues during the initial load
        let webNavigationTermSet = SP.Publishing.Navigation.NavigationTermSet.getAsResolvedByWeb(context, termSet, currentWeb, "GlobalNavigationTaxonomyProvider");

        // Get the existing view from the navigation term set
        const termSetView = webNavigationTermSet.get_view().getCopy();

        // Return global and current navigation terms (the subsequent filtering can be done in the Knockout html view)
        termSetView.set_excludeTermsByProvider(false);

        // Sets a value that indicates whether NavigationTerm objects are trimmed if the current user does not have permissions to view the target page (the aspx physical page) for the friendly URL
        // If you don't see anything in the menu, check the node type (term driven page or simple link). In the case of term driven page, the target page must be accessible for the current user
        termSetView.set_excludeTermsByPermissions(true);

        // Apply the new view filters
        webNavigationTermSet = webNavigationTermSet.getWithNewView(termSetView);

        const firstLevelNavigationTerms = webNavigationTermSet.get_terms();
        const allNavigationterms = webNavigationTermSet.getAllTerms();

        context.load(allNavigationterms, "Include(Id, Terms, Title, FriendlyUrlSegment, ExcludeFromCurrentNavigation, ExcludeFromGlobalNavigation)");
        context.load(firstLevelNavigationTerms, "Include(Id, Terms, Title, FriendlyUrlSegment, ExcludeFromCurrentNavigation, ExcludeFromGlobalNavigation)");

        const p = new Promise<ITaxonomyNavigationNode[]>((resolve, reject) => {

            context.executeQueryAsync(() => {

                this.getTermNodesAsFlat(context, allNavigationterms).then((nodes: ITaxonomyNavigationNode[]) =>  {

                    const navigationTree = this.getTermNodesAsTree(context, nodes, firstLevelNavigationTerms, null);

                    resolve(navigationTree);
                });

            },  (sender, args) => {
                reject("Request failed. " + args.get_message() + "\n" + args.get_stackTrace());
            });
        });

        return p;
    }

    /**
     * Get a single term by its Id using the current taxonomy context.
     * @param  {SP.Guid} termId The taxonomy term Id
     * @return {Promise<SP.Taxonomy.Term>}       A promise containing the term infos.
     */
    public getTermById(termId: SP.Guid): Promise<SP.Taxonomy.Term> {

        if (termId) {

            const context: SP.ClientContext = SP.ClientContext.get_current();

            const taxSession: SP.Taxonomy.TaxonomySession  = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
            const termStore: SP.Taxonomy.TermStore = taxSession.getDefaultSiteCollectionTermStore();

            termStore.set_workingLanguage(this.workingLanguage);

            const term: SP.Taxonomy.Term = termStore.getTerm(termId);

            context.load(term, "Name");

            const p = new Promise<SP.Taxonomy.Term>((resolve, reject) => {

                context.executeQueryAsync(() => {

                    resolve(term);

                },  (sender, args) => {

                    reject("Request failed. " + args.get_message() + "\n" + args.get_stackTrace());
                });
            });

            return p;

        } else {
            Logger.write("[TaxonomyModule.getTermById]: the provided term id is null!", LogLevel.Error);
        }
    }

    // Get the navigation hierarchy as a flat list
    // This list will be used to easily find a node without dealing too much with asynchronous calls and recursion
    private getTermNodesAsFlat(context: SP.ClientContext, allTerms: SP.Publishing.Navigation.NavigationTermCollection): Promise<any> {

        const termNodes: Array<Promise<ITaxonomyNavigationNode>> = [];

        const termsEnumerator = allTerms.getEnumerator();

        while (termsEnumerator.moveNext()) {

            const p = new Promise<ITaxonomyNavigationNode>((resolve, reject) => {

                const currentTerm: SP.Publishing.Navigation.NavigationTerm =  termsEnumerator.get_current();
                const termNode = new TaxonomyNavigationNode();

                termNode.Id = currentTerm.get_id().toString();
                termNode.Title = currentTerm.get_title().get_value();
                termNode.TaxonomyTerm = currentTerm;
                termNode.FriendlyUrlSegment = currentTerm.get_friendlyUrlSegment().get_value();
                termNode.ExcludeFromCurrentNavigation = currentTerm.get_excludeFromCurrentNavigation();
                termNode.ExcludeFromGlobalNavigation = currentTerm.get_excludeFromGlobalNavigation();

                this.getNavigationTermUrlInfo(context, currentTerm).then((termUrlInfo) => {

                    termNode.Url = termUrlInfo;

                    this.getTermCustomPropertiesForTerm(context, currentTerm.getTaxonomyTerm()).then((properties) => {

                        termNode.Properties = properties;
                        resolve(termNode);
                        termsEnumerator.moveNext();
                    });
                });
            });

            termNodes.push(p);
        }

        return Promise.all(termNodes);
    }

    // Get the navigation nodes as tree
    private getTermNodesAsTree(context: SP.ClientContext, allTerms: ITaxonomyNavigationNode[], currentNodeTerms: SP.Publishing.Navigation.NavigationTermCollection, parentNode: ITaxonomyNavigationNode): ITaxonomyNavigationNode[] {

        // Special thanks to this blog post
        // https://social.msd#n.microsoft.com/Forums/office/en-US/ede1aa39-4c47-4308-9aef-3b036ec9b318/get-navigation-taxonomy-term-tree-in-sharepoint-app?forum=appsforsharepoint
        const termsEnumerator = currentNodeTerms.getEnumerator();
        const termNodes: ITaxonomyNavigationNode[] = [];

        while (termsEnumerator.moveNext()) {

            // Get the corresponding navigation node in the flat tree
            const termId = termsEnumerator.get_current().get_id();
            const currentNode = _.find(allTerms, (term) => term.Id.toString().localeCompare(termId.toString()) === 0);

            const subTerms = currentNode.TaxonomyTerm.get_terms();
            if (subTerms.get_count() > 0) {

                currentNode.ChildNodes = this.getTermNodesAsTree(context, allTerms, subTerms, currentNode);
            }

            // Clear TaxonomyTerm property to simplify JSON string (property not useful anymore after this step)
            currentNode.TaxonomyTerm = null;

            if (parentNode !== null) {

                // Set the parent infos for the current node (used by the contextual menu and the breadcrumb components)
                currentNode.ParentUrl = parentNode.Url;
                currentNode.ParentId = parentNode.Id;
            }

            termNodes.push(currentNode);
        }

        return termNodes;
    }

    // Get the term URL info (simple link or friendly URL)
    private getNavigationTermUrlInfo(context: SP.ClientContext, navigationTerm: SP.Publishing.Navigation.NavigationTerm): Promise<string> {

        // This method gets the resolved URL whatever if it is a simple link or a friendly URL
        const resolvedDisplayUrl = navigationTerm.getResolvedDisplayUrl("");

        context.load(navigationTerm);

        const p = new Promise<string>((resolve, reject) => {

            context.executeQueryAsync(() => {
                resolve(resolvedDisplayUrl.get_value());

            }, (sender, args) => {

                reject("Request failed. " + args.get_message() + "\n" + args.get_stackTrace());
            });
        });

        return p;
    }

    // Get all custom proeprties for the term
    private getTermCustomPropertiesForTerm(context: SP.ClientContext, taxonomyTerm: SP.Taxonomy.Term): Promise<{ [key: string]: string }> {

        context.load(taxonomyTerm, "CustomProperties");

        const p = new Promise<{ [key: string]: string }>((resolve, reject) => {

            context.executeQueryAsync(() => {

                const properties = taxonomyTerm.get_customProperties();

                resolve(properties);

            },  (sender, args) => {

                reject("Request failed. " + args.get_message() + "\n" + args.get_stackTrace());
            });
        });

        return p;
    }
}

export default TaxonomyModule;
