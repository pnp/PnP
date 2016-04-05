"use strict";

import { Queryable } from "../Queryable";
import { Lists } from "./Lists/Lists";
import { RoleAssignments } from "./RoleAssignments/RoleAssignments";
import { Navigation } from "./Navigation/Navigation";
import { SiteUsers } from "./SiteUsers/SiteUsers";

export class Web extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/web");
    }
    public roleassignments = new RoleAssignments(this._url);
    public navigation = new Navigation(this._url);
    public siteusers = new SiteUsers(this._url);
    public lists = new Lists(this._url);
}
