"use strict";

/// <reference path="..\..\typings\main.d.ts" />

/**
 * Retrieves the list ID of the current page from _spPageContextInfo
 */
export function getListId(): string {
    return _spPageContextInfo.hasOwnProperty("pageListId") ? _spPageContextInfo.pageListId.substring(1, 37) : "";
}

/**
 * Make URL relative to host
 * 
 * @param url The URL to make relative
 */
export function getRelativeUrl(url: string) {
    return url.replace(`${document.location.protocol}//${document.location.hostname}`, "");
}

/**
 * Retrieves the node with the given title from a collection of SP.NavigationNode
 */
export function getNodeFromCollectionByTitle(nodeCollection: Array<SP.NavigationNode>, title: string) {
    const f = jQuery.grep(nodeCollection, (val: SP.NavigationNode) => {
        return val.get_title() === title;
    });
    return f[0] || null;
};

/**
 * Replaces URL tokens in a string
 */
export function replaceUrlTokens(url: string) {
    return url.replace(/{site}/g, _spPageContextInfo.webAbsoluteUrl)
              .replace(/{sitecollection}/g, _spPageContextInfo.siteAbsoluteUrl);
};

export function encodePropertyKey(propKey) {
    let bytes = [];
    for (let i = 0; i < propKey.length; ++i) {
        bytes.push(propKey.charCodeAt(i));
        bytes.push(0);
    }
    const b64encoded = window.btoa(String.fromCharCode.apply(null, bytes));
    return b64encoded;
}
