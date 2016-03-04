/// <reference path="..\..\typings\main.d.ts" />


/**
 * Combines an arbitrary set of paths ensuring that the slashes are normalized
 * 
 * @param paths 0 to n path parts to combine
 */
export function get(url: string): any {
    return jQuery.ajax({
        "url": url,
        "type": "get",
        "headers": { "accept": "application/json;odata=verbose" },
    });
}
