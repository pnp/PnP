// ====================
// Page (and children) Item View Model (Display Template)
// ====================
import "../shared/bindinghandlers";
import { DefaultDisplayTemplateItemViewModel } from "./defaultdisplaytemplateitem.viewmodel";
import i18n = require("i18next");
import sprintf = require("sprintf-js");
import { Localization } from "../core/localization";
import { UtilityModule } from "../core/utility";

declare function unescape(s:string): string;

export class PageDisplayTemplateItemViewModel extends DefaultDisplayTemplateItemViewModel {

    public allLabel: KnockoutObservable<string>;
    public searchPageUrl: KnockoutObservable<string>;

    constructor(currentItem: any, filterProperty:string , filterValue: string, allLabel: string) {

        super(currentItem);

        this.searchPageUrl = ko.observable("");
        this.allLabel = ko.observable("");
        
        ko.bindingHandlers.getSearchUrl = {

            init: () => {

                let localization = new Localization();

                // We need to ensure the i18n global object is set up before executing the control
                // In some cases, the object was not initialized during the viewmodel execution resulting of empty strings for translations.
                localization.initLanguageEnv().then(() => {
                    
                    let utilityModule = new UtilityModule();
                    this.allLabel(sprintf.sprintf(i18n.t(allLabel)));

                    if (filterValue && filterProperty) {

                        // Get only the L0 refiner value from the taxonomy field
                        let itemContentType = filterValue.match(/L0\|#0[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}\|.*?;/);

                        // Encode diacritics
                        let ctValue = unescape(encodeURIComponent(itemContentType[0]));

                        if (itemContentType) {
                            let refinerValue = "\\\"ǂǂ" + utilityModule.stringToHex(ctValue.slice(0, -1)) + "\\\""; 

                            // We can't filter by ContentTypeId because the refinement does an "equal"" instead of a "contain" (so 0x0..* does not work). We use the taxonomy field ContentType instead (RefinableString02)
                            let refinementString = '{"k":"*","r":[{"n":"'+ filterProperty +'","t":["' + refinerValue + '"],"o":"and","k":true,"m":null}]}';
                            this.searchPageUrl(_spPageContextInfo.siteAbsoluteUrl + "/Pages/" + i18n.t("intranetSearchPageUrl") + "#Default=" + encodeURIComponent(refinementString));
                        }
                    }                    
                });
            },
        };
    }
}
