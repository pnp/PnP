// ====================
// NodeViewModel
// ====================
import TaxonomyNavigationNode from "../models/TaxonomyNavigationNode";
import NavigationViewModel from "./NavigationViewModel";

class NodeViewModel {

    public title: KnockoutObservable<string>;
    public id: KnockoutObservable<string>;
    public url: KnockoutComputed<string>;
    public hasChildren: KnockoutObservable<boolean>;
    public hasParent: KnockoutObservable<boolean>;
    public dataToggle: KnockoutComputed<string>;
    public children: KnockoutObservableArray<NodeViewModel>;
    public friendlyUrlSegment: KnockoutObservable<string>;
    public isCurrentNode: KnockoutObservable<boolean>;
    public excludeFromGlobalNavigation: KnockoutObservable<boolean>;
    public excludeFromCurrentNavigation: KnockoutObservable<boolean>;
    public properties: KnockoutObservable<any>;
    public isSelected: KnockoutObservable<boolean>;

    constructor(node: TaxonomyNavigationNode) {

        this.title = ko.observable(node.Title);
        this.id = ko.observable(node.Id.toString());
        this.url = ko.pureComputed(() => {

            // Empty simple link URL or header for the term
            if (node.Url.localeCompare("") === 0) {
                return "#";
            } else {
                return node.Url;
            }
        });

        this.hasChildren = ko.observable(node.ChildNodes.length > 0);
        this.hasParent = ko.observable(node.ParentUrl !== null);
        this.dataToggle = ko.computed(() => {

            if (this.hasChildren()) {
                return "dropdown";
            } else {
                return "";
            }
        });

        this.children = ko.observableArray([]);
        this.friendlyUrlSegment = ko.observable(node.FriendlyUrlSegment);
        this.isCurrentNode = ko.observable(false);
        this.excludeFromGlobalNavigation = ko.observable(node.ExcludeFromGlobalNavigation);
        this.excludeFromCurrentNavigation = ko.observable(node.ExcludeFromCurrentNavigation);
        this.properties = ko.observable(node.Properties);
        this.isSelected = ko.observable(false);

        // Populate children recursively
        NavigationViewModel.populateObservableNodeArray(node.ChildNodes, this.children);
    }
}

export default NodeViewModel;
