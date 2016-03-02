/// <reference path="..\..\typings\main.d.ts" />

export function get(url: string): any {
    return jQuery.ajax({
        url: url,
        type: "get",
        headers: { "accept": "application/json;odata=verbose" },
    });
}
