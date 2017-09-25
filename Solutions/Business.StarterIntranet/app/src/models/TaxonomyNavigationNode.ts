// ========================================
// Represents a taxonomy navigation node
// ========================================
import ITaxonomyNavigationNode from "./ITaxonomyNavigationNode";

class TaxonomyNavigationNode implements ITaxonomyNavigationNode {

    public Id: string; // Be careful, Ids must be strings instead of objects (i.e SP.Guid) because of serialization.
    public Title: string;
    public Url: string;
    public TaxonomyTerm: any;
    public ChildNodes: TaxonomyNavigationNode[];
    public ParentUrl: string;
    public ParentId: string;
    public FriendlyUrlSegment: string;
    public Properties: { [key: string]: string };
    public ExcludeFromGlobalNavigation: boolean;
    public ExcludeFromCurrentNavigation: boolean;

    constructor() {
        this.ChildNodes = [];
        this.ParentId = null;
        this.ParentUrl = null;
    }
}

export default TaxonomyNavigationNode;
