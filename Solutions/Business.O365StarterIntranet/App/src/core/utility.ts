// ====================
// Utility module
// ====================

/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import { NavigationNode } from "../shared/navigationnode.ts";

export class UtilityModule {

    /**
     * Stringify a tree object with circular dependencies
     * @return {String}       The stringified tree object
     */
    public stringifyTreeObject (object: Object): string {

            let cache = [];
            let stringified = JSON.stringify(object, (key, value) => {
                if (typeof value === "object" && value !== null) {
                    if (cache.indexOf(value) !== -1) {
                        // Circular reference found, discard key
                        return;
                    }
                    // Store value in our collection
                    cache.push(value);
                }
                return value;
            });
            cache = null;

            return stringified;
    }

    /**
     * Create a new Guid
     * @return {String}       A new guid as tring
     */
    public getNewGuid (): string {

        let guid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, (c) => {
            let r = Math.random()*16|0, v = c === "x" ? r : (r&0x3|0x8);
            return v.toString(16);

        });
        return guid;
    }

    /**
     * Get the navigation node in the specified array by its resolved display URL
     * @param  {Array<NavigationNode>} nodes The navigation nodes array to search in
     * @param  {string} pageUrl The page URL. Can be the current window.location
     * @return {NavigationNode}       The corresponding node, null otherwise
     */
    public getNodeByUrl (nodes: Array<NavigationNode>, pageUrl: string): NavigationNode {

        if (nodes) {

            for (let node of nodes) {
                // Does a node in the whole site map have this current page url as resolved display URL (friendly or simple link)
                if (node.Url.replace(/(\/*|#*|\?*)$/g, "").toUpperCase().localeCompare(decodeURI(pageUrl).replace(/(\/*|#*|\?*)$/g, "").toUpperCase()) === 0) {

                    // If there are multiple nodes with the same simple link url, only the first match is returned (and you probably have some problems with your navigation consistency...)
                    return node;
                }

                let found = this.getNodeByUrl(node.ChildNodes, pageUrl);
                if (found) {
                    return found;
                }
            }
        }
    }

    /**
     * Get the navigation node in the specified array by its id
     * @param  {Array<NavigationNode>} nodes The navigation nodes array to search in
     * @param  {SP.Guid} termId The navigation node id
     * @return {NavigationNode}       The corresponding node, null otherwise
     */
    public getNodeByTermId (nodes: Array<NavigationNode>, termId: SP.Guid): NavigationNode {

        if (nodes) {

            for (let node of nodes) {
                // Does a node in the whole site map have this current page url as resolved display URL (friendly or simple link)
                if (node.Id.toString().toUpperCase().localeCompare(termId.toString().toUpperCase()) === 0) {
                    return node;
                }

                let found = this.getNodeByTermId(node.ChildNodes, termId);
                if (found) {
                    return found;
                }
            }
        }
    }

    /**
     * Get the value of a querystring
     * @param  {String} field The field to get the value of
     * @param  {String} url   The URL to get the value from (optional)
     * @return {String}       The field value
     */
    public getQueryString (field: string , url: string ) {
        let href = url ? url : window.location.href;
        let reg = new RegExp( "[?&]" + field + "=([^&#]*)", "i" );
        let qs = reg.exec(href);
        return qs ? qs[1] : null;
    }

    /**
     * Check if the cache value from the local storage is still valid
     * A valid cache value is when:
     *  - Not null or empty string
     *  - Not an empty array
     *  - Not expired
     * @param  {String} localStorageKey The key in the browser local storage
     * @return {String}       The cache value is valid, null otherwise
     */
    public isCacheValueValid (localStorageKey: string): any {

        let value = null;

        // Get the current value in local storage
        let cachedValue: string = localStorage.getItem(localStorageKey);

        if (cachedValue !== null && cachedValue !== undefined) {

            // Get the cached value
            let navigationTree = JSON.parse(JSON.parse(cachedValue).value);

            // Make sure there is a valid value in the cache (not [])
            if (navigationTree.length > 0) {

                // Check if the cache value is expired
                let expiration: Date = new Date(JSON.parse(cachedValue).expiration);
                let now: Date = new Date();

                if (now < expiration) {

                    value = navigationTree;
                }
            }
        }

        return value;
    }

    /**
     * Transform an URL to a DOM link element to be able to parse it more easily
     * @param  {String} url The url to convert
     * @return {String}       The link DOM element
     */
    public getLocation (url): any {

        let l = document.createElement("a");
        l.href = url;
        return l;
    };

    /**
     * Move an item inside an array by changing its index
     * @param  {number} oldIndex The index of the item to move
     * @param  {number} newIndex The new desired index in the array
     * @return {Array<any>}       The modified array
     */
    public moveItem (array: Array<any>, oldIndex: number, newIndex: number): Array<any> {

        if (newIndex >= array.length) {
            let k = newIndex - array.length;
            while ((k--) + 1) {
                array.push(undefined);
            }
        }

        array.splice(newIndex, 0, array.splice(oldIndex, 1)[0]);

        return array;
    };
}
