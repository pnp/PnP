"use strict";

/// <reference path="..\..\typings\main.d.ts" />

/**
 * Retrieves the list ID of the current page from _spPageContextInfo
 */
export function getListId(): string {
    return _spPageContextInfo.hasOwnProperty("pageListId") ? _spPageContextInfo.pageListId.substring(1,37) : "";
}