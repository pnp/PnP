// ====================
// News Item View Model (Display Template)
// ====================

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/trunk8/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import "../shared/bindinghandlers";
import { DefaultDisplayTemplateItemViewModel } from "./defaultdisplaytemplateitem.viewmodel.ts";
import i18n = require("i18next");
import sprintf = require("sprintf-js");

export class NewsDisplayTemplateItemViewModel extends DefaultDisplayTemplateItemViewModel {

    public allNewsLabel: KnockoutObservable<string>;
    public searchPageUrl: KnockoutObservable<string>;

    constructor(currentItem: any) {

        super(currentItem);

        this.searchPageUrl = ko.observable("");
        this.allNewsLabel = ko.observable(sprintf.sprintf(i18n.t("allNewsLabel"), _spPageContextInfo.webTitle.toLowerCase()));

        ko.bindingHandlers.getNewsSearchUrl = {

            init: () => {

                let newsContentTypeId = "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF39000650D0E024D0AE42B88AF5AF825F709C02";
                let refinementString = '{"k":"","r":[{"n":"ContentTypeId","t":["' + newsContentTypeId + '*"],"o":"and","k":false,"m":null}';

                this.searchPageUrl(_spPageContextInfo.siteAbsoluteUrl + "/Pages/" + i18n.t("intranetSearchPageUrl") + "#Default=" + encodeURIComponent(refinementString));
            },
        };
    }
}
