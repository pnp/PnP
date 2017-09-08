// ========================================
// Represents a search navigation node
// ========================================
interface ISearchNavigationNode {
    Title: string;
    Url: string;
    Icon: string; // The icon will be retrieved via the query string of the URL.
}

export default ISearchNavigationNode;
