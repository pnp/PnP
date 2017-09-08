// ====================
// NavigationViewModel used as base for navigation components
// ====================
import TaxonomyNavigationNode from "../models/TaxonomyNavigationNode";
import NodeViewModel from "./NodeViewModel";

class NavigationViewModel {

    public static populateObservableNodeArray(nodes: TaxonomyNavigationNode[], observableArray: KnockoutObservableArray<NodeViewModel>): void {

        const navNodes: NodeViewModel[] = [];
        for (const node of nodes) {

            navNodes.push(new NodeViewModel(node));
        }

        observableArray(navNodes);
    }

    public nodes: KnockoutObservableArray<NodeViewModel>;

    constructor() {

       this.nodes = ko.observableArray([]);
    }

    public initialize(navigationNodes: TaxonomyNavigationNode[]): void {

        // Reset the observable array first
        this.nodes.removeAll();

        NavigationViewModel.populateObservableNodeArray(navigationNodes, this.nodes);
    }

    public setCurrentNode(nodeId: string): void {

        const match = ko.utils.arrayFirst(this.nodes(), (item) => {
                return nodeId === item.id();
        });

        if (match) {
            match.isCurrentNode(true);
        }
    }
}

export default NavigationViewModel;
