// ====================
// Language Switcher Component
// ====================

/* ****************TEST CASES********************
 *
 * - Scenario: "The current pas has no translation"
 *  - Init: The current page langauge is "FR" but it doesn't have an "EN" translation
 *  - Expected behavior: 
 *      - The current language of the page is selected in the switcher (non clickable). The "EN" label is disabled with a message displaying that there is no translation for this page.
 * 
 *  - Scenario: "The current has a translation"
 *      - Init: The current page langauge is "FR" and have and "EN" translation
 *      - Expected behavior:    
 *           - The current language of the page is selected in the switcher (non clickable) and the "EN" label redirect to the translated page.
 * 
 *  - Scenario: "The current has a no language"
 *      - Init: The current page doesn't have a language property
 *      - Expected behavior: 
 *          - Both "FR" and "EN" labels are disabled with a message displaying that there is no translation for these pages
 * 
 *  - Scenario: "The current page has more than one translations for a label"
 *      - Init: The current page langauge is "FR" but it has multiple "EN" translations
 *      - Expected behavior: 
 *          - Only the most recent translation is shown for the "EN" label, in the component.
 *
 * **********************************************/

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/sprintf-js/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />
/// <reference path="../../typings/globals/es6-promise/index.d.ts" />

import * as pnp from "sp-pnp-js";
import i18n = require("i18next");

export class LanguageSwitcherViewModel {

    private availableLanguages: KnockoutObservableArray<LanguageLinkViewModel>;
    private noTranslationMessage: KnockoutObservable<string>;

    // Page context
    private languageFieldName: string;
    private associationKeyFieldName: string;
    private currentPageId: number;

    constructor(params: any) {

        let languages: Array<string> = params.availableLanguages;
        this.languageFieldName = params.languageFieldName;
        this.associationKeyFieldName = params.associationKeyFieldName;

        // Get context informations for the current page
        this.currentPageId = _spPageContextInfo.pageItemId;

        this.availableLanguages = ko.observableArray([]);
        this.noTranslationMessage = ko.observable("");

        this.getPeerUrls(languages);
    }

    /**
     * Get all available translations for the current page
     * 
     * @param languages: the arbitrary languages set up for the component
     */
    private getPeerUrls(languages: Array<string>) {

        // Get the info for the current page
        pnp.sp.web.lists.getByTitle("Pages").items.getById(this.currentPageId).select(this.associationKeyFieldName, "ID", this.languageFieldName).get().then((item) => {

            let allLanguages: Array<LanguageLinkViewModel> = [];
            let currentPageLanguage = item[this.languageFieldName];

            // Does a page in the 'Pages' library exist with the same GUID as me and an other language?
            let filterQuery: string = this.associationKeyFieldName + " eq '" + item[this.associationKeyFieldName] + "' and ID ne '" + item.ID + "' and " + this.languageFieldName + " ne '" + currentPageLanguage  + "'";

            // Return only one element ordered descending by the Modified date
            // It can't have more than one translation for the current page
            pnp.sp.web.lists.getByTitle("Pages").items.filter(filterQuery).orderBy("Modified").top(1).select("FileRef, Title", this.languageFieldName).get().then((item: Array<any>) => {

                // Loop through each available languages and map the correct information according to the page context and its translations.
                // We want to notifiy the users if there is not translation for a target language so that's why we map an arbitrary array of languages with the results
                languages.map((element) => {

                    let languageLink = new LanguageLinkViewModel();

                    // The label is given by the component parameters
                    languageLink.label(element);

                    // This is the current language
                    if (element.localeCompare(currentPageLanguage) === 0) {

                        languageLink.isCurrentLanguage(true);

                    } else {

                        // If there is a translation, fill the appropriate information
                        if (item.length > 0) {

                            let itemLanguage: string = item[0][this.languageFieldName];
                            let itemUrl: string = item[0].FileRef;

                            if (element.localeCompare(itemLanguage) === 0 ) {

                                languageLink.url(itemUrl);
                                languageLink.isValidTranslation(true);

                            } else {

                                // This is a translation for an other language not listed in the available languages for the component...
                                languageLink.isValidTranslation(false);
                                this.noTranslationMessage(i18n.t("noTranslationMessage"));
                            }

                        } else {

                            // Not item = no translation at all
                            languageLink.isValidTranslation(false);
                            this.noTranslationMessage(i18n.t("noTranslationMessage"));
                        }
                    }

                    allLanguages.push(languageLink);
                });

                // Init available languages for the user
                this.availableLanguages(allLanguages);

            }).catch((errorMesssage) => {

                pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
            });

        }).catch((errorMesssage) => {

            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
        });
    }
}

class LanguageLinkViewModel {

    public label: KnockoutObservable<string>;
    public url: KnockoutObservable<string>;
    public isCurrentLanguage: KnockoutObservable<boolean>;
    public isValidTranslation: KnockoutObservable<boolean>;

    constructor() {

        this.label = ko.observable("");
        this.url = ko.observable("");
        this.isCurrentLanguage = ko.observable(false);
        this.isValidTranslation = ko.observable(false);
    }
}
