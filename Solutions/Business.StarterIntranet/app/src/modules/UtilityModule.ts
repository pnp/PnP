// ====================
// Utility module
// ====================
import * as _ from "lodash";
import * as moment from "moment";
import { Logger, LogLevel, Site, spODataEntityArray } from "sp-pnp-js";
import ConfigurationItem from "../models/ConfigurationItem";
import IConfigurationItem from "../models/IConfigurationItem";
import TaxonomyNavigationNode from "../models/TaxonomyNavigationNode";

class UtilityModule {

    /**
     * Stringify a tree object with circular dependencies
     * @return {String}       The stringified tree object
     */
    public stringifyTreeObject(object: object): string {

            let cache = [];
            const stringified = JSON.stringify(object, (key, value) => {
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
    public getNewGuid(): string {

        const guid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, (c) => {
            // tslint:disable-next-line:no-bitwise
            const r = Math.random() * 16 | 0;
            // tslint:disable-next-line:no-bitwise
            const v = c === "x" ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
        return guid;
    }

    /**
     * Get the navigation node in the specified array by its resolved display URL
     * @param  {TaxonomyNavigationNode[]} nodes The navigation nodes array to search in
     * @param  {string} pageUrl The page URL. Can be the current window.location
     * @return {TaxonomyNavigationNode}       The corresponding node, null otherwise
     */
    public getNodeByUrl(nodes: TaxonomyNavigationNode[], pageUrl: string): TaxonomyNavigationNode {

        if (nodes) {

            for (const node of nodes) {
                // Does a node in the whole site map have this current page url as resolved display URL (friendly or simple link)
                if (node.Url.replace(/(\/*|#*|\?*)$/g, "").toUpperCase().localeCompare(decodeURI(pageUrl).replace(/(\/*|#*|\?*)$/g, "").toUpperCase()) === 0) {

                    // If there are multiple nodes with the same simple link url, only the first match is returned (and you probably have some problems with your navigation consistency...)
                    return node;
                }

                const found = this.getNodeByUrl(node.ChildNodes, pageUrl);
                if (found) {
                    return found;
                }
            }
        }
    }

    /**
     * Get the navigation node in the specified array by its id
     * @param  {TaxonomyNavigationNode[]} nodes The navigation nodes array to search in
     * @param  {string} termId The navigation node id
     * @return {TaxonomyNavigationNode}       The corresponding node, null otherwise
     */
    public getNodeByTermId(nodes: TaxonomyNavigationNode[], termId: string): TaxonomyNavigationNode {

        if (nodes) {

            for (const node of nodes) {
                // Does a node in the whole site map have this current page url as resolved display URL (friendly or simple link)
                if (node.Id.toString().toUpperCase().localeCompare(termId.toString().toUpperCase()) === 0) {
                    return node;
                }

                const found = this.getNodeByTermId(node.ChildNodes, termId);
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
    public getQueryStringParam(field: string , url: string) {
        const href = url ? url : window.location.href;
        const reg = new RegExp("[?&#]" + field + "=([^&#]*)", "i");
        const qs = reg.exec(href);
        return qs ? qs[1] : null;
    }

    /**
     * @param {String} field The field name of the query string to remove
     * @param {String} sourceURL The source URL
     * @return {String}       The updated URL
     */
    public removeQueryStringParam(field: string , sourceURL: string) {
        let rtn = sourceURL.split("?")[0];
        let param = null;
        let paramsArr = [];
        const queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";

        if (queryString !== "") {
            paramsArr = queryString.split("&");
            for (let i = paramsArr.length - 1; i >= 0; i -= 1) {
                param = paramsArr[i].split("=")[0];
                if (param === field) {
                    paramsArr.splice(i, 1);
                }
            }

            if (paramsArr.length > 0) {
                rtn = rtn + "?" + paramsArr.join("&");
            }
        }
        return rtn;
    }

    /**
     * Replace a query string parameter
     * @param url The current URL
     * @param param The query string parameter to replace
     * @param value The new value
     */
    public replaceQueryStringParam(url: string, param: string, value: string) {
        const re = new RegExp("[\\?&]" + param + "=([^&#]*)");
        const match = re.exec(url);
        let delimiter;
        let newString;

        if (match === null) {
            // Append new param
            const hasQuestionMark = /\?/.test(url);
            delimiter = hasQuestionMark ? "&" : "?";
            newString = url + delimiter + param + "=" + value;
        } else {
            delimiter = match[0].charAt(0);
            newString = url.replace(re, delimiter + param + "=" + value);
        }

        return newString;
    }

    /**
     * Check if the cache value from the local storage is still valid
     * A valid cache value is when:
     *  - Not null or empty string
     *  - Not an empty array
     *  - Not expired
     * @param  {String} localStorageKey The key in the browser local storage
     * @return {String}       The cache value if valid, null otherwise
     */
    public isCacheValueValid(localStorageKey: string): any {

        let value = null;

        // Get the current value in local storage
        const cachedValue: string = localStorage.getItem(localStorageKey);

        if (cachedValue !== null && cachedValue !== undefined) {

            // Get the cached value
            let localStorageValue = JSON.parse(cachedValue).value;

            // If the value is a JSON object
            if (!_.isError(_.attempt(() => JSON.parse(localStorageValue)))) {
                localStorageValue = JSON.parse(localStorageValue);
            }

            // Make sure there is a valid value in the cache (not [])
            if (localStorageValue.length > 0) {

                // Check if the cache value is expired
                const expirationValue = JSON.parse(cachedValue).expiration;

                if (expirationValue) {
                    const expirationDate: Date = new Date(JSON.parse(cachedValue).expiration);

                    const now: Date = new Date();

                    if (now < expirationDate) {

                        value = localStorageValue;
                    }

                } else {
                    value = localStorageValue;
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
    public getLocation(url): any {

        const l = document.createElement("a");
        l.href = url;
        return l;
    }

    /**
     * Move an item inside an array by changing its index
     * @param  {number} oldIndex The index of the item to move
     * @param  {number} newIndex The new desired index in the array
     * @return {any[]}       The modified array
     */
    public moveItem(array: any[], oldIndex: number, newIndex: number): any[] {

        if (newIndex >= array.length) {
            let k = newIndex - array.length;
            while ((k--) + 1) {
                array.push(undefined);
            }
        }

        array.splice(newIndex, 0, array.splice(oldIndex, 1)[0]);

        return array;
    }

    /**
     * Convert a string to an hexadecimal value
     * @param  {TaxonomyNavigationNode[]} nodes The navigation nodes array to search in
     * @param  {SP.Guid} tmp The original string
     * @return {string}       The converted string
     */
    public stringToHex(tmp: string): string {
        const d2h = (d) => {
            return d.toString(16);
        };

        let str = "";
        let i = 0;
        const tmpLen = tmp.length;
        let c;

        for (; i < tmpLen; i += 1) {
            c = tmp.charCodeAt(i);
            str += d2h(c);
        }

        return str;
    }

    /**
     * Get the current build number of SharePoint
     * @return {string}       The build number
     */
    public getSharePointBuildNumber(): Promise<string> {

        const p = new Promise<string>((resolve, reject) => {
            SP.SOD.executeFunc("sp.js", "SP.ClientContext", () => {
                const clientContext = SP.ClientContext.get_current();

                clientContext.executeQueryAsync(() =>  {
                    resolve(clientContext.get_serverVersion());
                }, (errorMessage) => {
                    reject(errorMessage);
                });
            });
        });

        return p;
    }

    /**
     * Get the configuration list item for a specific language in the configuration list
     * @param language the language of item of get
     */
    public getConfigurationListValuesForLanguage(language: string): Promise<IConfigurationItem> {

        const configListName = "Configuration";
        const site = new Site(_spPageContextInfo.siteAbsoluteUrl);

        const p = new Promise<IConfigurationItem>((resolve, reject) => {

            site.rootWeb.lists.getByTitle(configListName).items.usingCaching({
                    expiration: moment().add(1, "h").toDate(),
                    key: String.format("{0}_{1}", _spPageContextInfo.siteServerRelativeUrl, "configurationListValues"),
                    storeName: "local",
                }).select(ConfigurationItem.SelectFields.toString()).getAs(spODataEntityArray(ConfigurationItem)).then((items: ConfigurationItem[]) => {

                if (items.length > 0) {

                    // Get item corresponding to the current language
                    const item: any = _.find(items, (e: ConfigurationItem) => e.IntranetContentLanguage === language);

                    if (item) {
                        resolve(item);
                    }
                } else {
                    Logger.write("[UtilityModule.getConfigurationListValuesForLanguage]: There is no configuration item for the language '" + language + "'", LogLevel.Error);
                }
            }).catch((errorMesssage) => {

                Logger.write("[UtilityModule.getConfigurationListValuesForLanguage]: " + errorMesssage, LogLevel.Error);
                reject(errorMesssage);
            });
        });

        return p;
    }

    /**
     * Clean JS string
     * @param s String to clean out
     */
    public stripScripts(s) {
        const div = document.createElement("div");
        div.innerHTML = s;
        const scripts = div.getElementsByTagName("script");
        let i = scripts.length;
        while (i--) {
            scripts[i].parentNode.removeChild(scripts[i]);
        }
        return div.innerHTML;
    }
}

export default UtilityModule;
