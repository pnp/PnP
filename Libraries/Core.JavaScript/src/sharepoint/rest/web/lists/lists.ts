"use strict";

/// <reference path="..\..\typings\main.d.ts" />

/**
 * TODO
 */
export class Lists {
    constructor() { }

    public query() {
        var xmlhttp = new XMLHttpRequest();
        var url = `${_spPageContextInfo.webAbsoluteUrl}/_api/web/lists`;
        xmlhttp.setRequestHeader("Accept", "application/json;odata=verbose");
        xmlhttp.onreadystatechange = function() {
            if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                console.log(JSON.parse(xmlhttp.responseText));
            }
        };
        xmlhttp.open("GET", url, true);
        xmlhttp.send();
    }
}