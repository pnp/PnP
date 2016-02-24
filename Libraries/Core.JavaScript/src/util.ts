"use strict";

export class Util {
    public static $: Util = new Util();

    // allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, ...)
    public getCtxCallback(context: any, method: Function, ...params: any[]): Function {
        return function() {
            method.apply(context, params);
        };
    }

    // returns the browser location value
    public getBrowserLocation(): Location {
        return location;
    }

    // tests if a url param exists
    public urlParamExists(name: string): boolean {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        let regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
        return regex.test(this.getBrowserLocation().search);
    }

    // gets a url param value by name
    public getUrlParamByName(name: string): string {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        let regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
        let results = regex.exec(this.getBrowserLocation().search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    // gets a url param by name and attempts to parse a bool value
    public getUrlParamBoolByName(name: string): boolean {
        let p = this.getUrlParamByName(name);
        let isFalse = (p === "" || /false|0/i.test(p));
        return !isFalse;
    }

    // inserts the string s into the string target as the index specified by index
    public stringInsert(target: string, index: number, s: string): string {
        if (index > 0) {
            return target.substring(0, index) + s + target.substring(index, target.length);
        }
        return s + target;
    }

    // adds a value to a date
    // http://stackoverflow.com/questions/1197928/how-to-add-30-minutes-to-a-javascript-date-object
    public dateAdd(date: Date, interval: string, units: number): Date {
        let ret = new Date(date.toLocaleString()); // don't change original date
        switch (interval.toLowerCase()) {
            case "year": ret.setFullYear(ret.getFullYear() + units); break;
            case "quarter": ret.setMonth(ret.getMonth() + 3 * units); break;
            case "month": ret.setMonth(ret.getMonth() + units); break;
            case "week": ret.setDate(ret.getDate() + 7 * units); break;
            case "day": ret.setDate(ret.getDate() + units); break;
            case "hour": ret.setTime(ret.getTime() + units * 3600000); break;
            case "minute": ret.setTime(ret.getTime() + units * 60000); break;
            case "second": ret.setTime(ret.getTime() + units * 1000); break;
            default: ret = undefined; break;
        }
        return ret;
    }

    // loads a stylesheet into the current page
    public loadStylesheet(path: string, avoidCache: boolean): void {
        if (avoidCache) {
            path += "?" + encodeURIComponent((new Date()).getTime().toString());
        }
        let head = document.getElementsByTagName("head");
        if (head.length > 1) {
            let e = document.createElement("link");
            head[0].appendChild(e);
            e.setAttribute("type", "text/css");
            e.setAttribute("rel", "stylesheet");
            e.setAttribute("href", path);
        }
    }

    // combines an arbitrary set of paths ensureing that the slashes are normalized
    public combinePaths(...paths: string[]): string {
        let parts = [];
        for (let i = 0; i < paths.length; i++) {
            parts.push(arguments[i].replace(/^[\\|\/]/, "").replace(/[\\|\/]$/, ""));
        }
        return parts.join("/").replace(/\\/, "/");
    }

    // gets a random string of chars length
    public getRandomString(chars: number): string {
        let text = "";
        let possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        for (let i = 0; i < chars; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return text;
    }

    // gets a random GUID value
    // http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
    public getGUID(): string {
        let d = new Date().getTime();
        let guid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(c) {
            let r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === "x" ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return guid;
    }

    public isFunction(candidateFunction: any) {
        return typeof candidateFunction === "function";
    }
}
