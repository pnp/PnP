"use strict";

import { Queryable } from "../../Queryable";
import { QuickLaunch } from "./QuickLaunch/QuickLaunch";
import { TopNavigationBar } from "./TopNavigationBar/TopNavigationBar";

export class Navigation extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/Navigation");
    }
    public quicklaunch = new QuickLaunch(this._url);
    public topnavigationbar = new TopNavigationBar(this._url);
}
