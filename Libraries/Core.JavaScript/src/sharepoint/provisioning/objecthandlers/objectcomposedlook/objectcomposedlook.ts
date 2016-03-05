"use strict";

/// <reference path="..\schema\icomposedlook.d.ts" />
import { replaceUrlTokens, getRelativeUrl } from "../../../Util";
import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";

export class ObjectComposedLook extends ObjectHandlerBase {
    constructor() {
        super("ComposedLook");
    }
    public ProvisionObjects(object: IComposedLook) {
        super.scope_started();
        return new Promise((resolve, reject) => {
            let clientContext = SP.ClientContext.get_current();
            let web = clientContext.get_web();
            let colorPaletteUrl = object.ColorPaletteUrl ? replaceUrlTokens(object.ColorPaletteUrl) : "";
            let fontSchemeUrl = object.FontSchemeUrl ? replaceUrlTokens(object.FontSchemeUrl) : "";
            let backgroundImageUrl = object.BackgroundImageUrl ? replaceUrlTokens(object.BackgroundImageUrl) : null;
            web.applyTheme(getRelativeUrl(colorPaletteUrl), getRelativeUrl(fontSchemeUrl), backgroundImageUrl, true);
            web.update();
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

