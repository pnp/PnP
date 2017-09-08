// ========================================
// Represents a taxonomy navigation node
// ========================================
interface ITaxonomyNavigationNode {
    Id: string; // Be careful, Ids must be strings instead of objects (i.e SP.Guid) because of serialization.
    Title: string;
    Url: string;
    TaxonomyTerm: any;
    ChildNodes: ITaxonomyNavigationNode[];
    ParentUrl: string;
    ParentId: string;
    FriendlyUrlSegment: string;
    Properties: { [key: string]: string };
    ExcludeFromGlobalNavigation: boolean;
    ExcludeFromCurrentNavigation: boolean;
}

export default ITaxonomyNavigationNode;
