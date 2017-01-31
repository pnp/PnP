// ========================================
// Document Item View Model (Display Template)
// ========================================
import "../shared/bindinghandlers";
import { DefaultDisplayTemplateItemViewModel } from "./defaultdisplaytemplateitem.viewmodel";
import * as i18n from "i18next";
import * as sprintf from "sprintf-js";

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
