"use strict";

/// <reference path="..\schema\iwebsettings.d.ts" />
// import { Promise } from "es6-promise";
import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";

export class ObjectWebSettings extends ObjectHandlerBase {
    constructor() {
        super("WebSettings");
    }
    public ProvisionObjects(object: IWebSettings) {
        super.scope_started();
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            let web = clientContext.get_web();

            if (object.WelcomePage) {
                web.get_rootFolder().set_welcomePage(object.WelcomePage);
                web.get_rootFolder().update();
            }
            if (object.MasterUrl) {
                web.set_masterUrl(object.MasterUrl);
            }
            if (object.CustomMasterUrl) {
                web.set_customMasterUrl(object.CustomMasterUrl);
            }
            if (object.SaveSiteAsTemplateEnabled !== undefined) {
                web.set_saveSiteAsTemplateEnabled(object.SaveSiteAsTemplateEnabled);
            }
            if (object.QuickLaunchEnabled !== undefined) {
                web.set_saveSiteAsTemplateEnabled(object.QuickLaunchEnabled);
            }
            if (object.TreeViewEnabled !== undefined) {
                web.set_treeViewEnabled(object.TreeViewEnabled);
            }

            web.update();
            clientContext.load(web);
            clientContext.executeQueryAsync(
                () => {
                    super.scope_ended();
                    resolve();
                }, () => {
                    super.scope_ended();
                    resolve();
                });
        });
    }
}
