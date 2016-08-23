/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import { NavigationNode } from "./navigationnode";

export class NavigationViewModel {

    public static populateObservableNodeArray(nodes: Array<NavigationNode>, observableArray: KnockoutObservableArray<NodeViewModel>): void {

        for (let node of nodes) {

            observableArray.push(new NodeViewModel(node));
        }
    }

    public nodes: KnockoutObservableArray<NodeViewModel>;

    constructor() {

       this.nodes = ko.observableArray([]);
    }

    public initialize (navigationNodes: Array<NavigationNode>): void {

        NavigationViewModel.populateObservableNodeArray(navigationNodes, this.nodes);
    }

    public setCurrentNode (nodeId: SP.Guid): void {

        let match = ko.utils.arrayFirst(this.nodes(), (item) => {
                return nodeId.toString() === item.id();
        });

        if (match) {
            match.isCurrentNode(true);
        }
    };
}

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

    constructor(node: NavigationNode) {

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

        // Populate children recursively
        NavigationViewModel.populateObservableNodeArray(node.ChildNodes, this.children);
    }
}
