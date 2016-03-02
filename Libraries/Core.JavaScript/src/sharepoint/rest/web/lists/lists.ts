"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

/**
 * TODO
 */
export class Lists extends Queryable {
    private _url: string;
    private _id: string;
    private _title: string;
    private _select: string;
    private _filter: string;
    private _itemId: string;
    private _items: boolean;
    
    constructor(id?: string, items = false) {
        this._url = "/_api/web/Lists";
        this._id = id;
        this._items = items;
    }
    
    public getByTitle(title: string) {
        this._title = title;
        return this;
    }
    
    public select(select: string) {
        this._select = select;
        return this;
    } 
    
    public filter(filter: string) {
        this._filter = filter;
        return this;
    }
    
    public items(itemId: string) {
        this._items = true;
        this._itemId = itemId;
        return this;
    }    

    public get() {
        var urlBuilder = [this._url];
        this._id             && urlBuilder.push(`('${this._id}')`);
        this._title          && urlBuilder.push(`/getByTitle('${this._title}')`);
        this._items          && urlBuilder.push(`/Items`);
        this._itemId         && urlBuilder.push(`(${this._itemId})`);
        (this._select || 
        this._filter)        && urlBuilder.push('?');
        var queries        = [];
        this._select         && queries.push(`$select='${this._select}'`);
        this._filter         && queries.push(`$filter='${this._filter}'`);
        urlBuilder.push(queries.join("&"));
        return urlBuilder.join("");
    }
}