// ========================================
// Document Item View Model (Display Template)
// ========================================

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/trunk8/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import "../shared/bindinghandlers";
import { DefaultDisplayTemplateItemViewModel } from "./defaultdisplaytemplateitem.viewmodel.ts";
import i18n = require("i18next");
import sprintf = require("sprintf-js");

export class DocumentDisplayTemplateItemViewModel extends DefaultDisplayTemplateItemViewModel {

    public allDocumentsLabel: KnockoutObservable<string>;
    public searchPageUrl: KnockoutObservable<string>;

    constructor(currentItem: any) {

        super(currentItem);

        this.searchPageUrl = ko.observable("");
        this.allDocumentsLabel = ko.observable(sprintf.sprintf(i18n.t("allDocumentsLabel"), _spPageContextInfo.webTitle.toLowerCase()));

        ko.bindingHandlers.getDocumentsSearchUrl = {

            init: () => {

                this.searchPageUrl(_spPageContextInfo.siteAbsoluteUrl + "/Pages/" + i18n.t("documentsSearchPageUrl"));
            },
        };
    }
}
