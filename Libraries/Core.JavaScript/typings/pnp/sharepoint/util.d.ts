/**
 * Retrieves the list ID of the current page from _spPageContextInfo
 */
export declare function getListId(): string;
/**
 * Make URL relative to host
 *
 * @param url The URL to make relative
 */
export declare function getRelativeUrl(url: string): string;
/**
 * Retrieves the node with the given title from a collection of SP.NavigationNode
 */
export declare function getNodeFromCollectionByTitle(nodeCollection: Array<SP.NavigationNode>, title: string): SP.NavigationNode;
/**
 * Replaces URL tokens in a string
 */
export declare function replaceUrlTokens(url: string): string;
export declare function encodePropertyKey(propKey: any): string;
