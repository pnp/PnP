// ========================================
// Language Switcher Component
// ========================================

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
import * as i18n from "i18next";
import * as _ from "lodash";
import { Logger, LogLevel, Site, storage, Web } from "sp-pnp-js";
import LocalizationModule from "../../modules/LocalizationModule";
import LanguageLinkViewModel from "./LanguageLinkViewModel";

class LanguageSwitcherViewModel {

    public shouldRender: KnockoutObservable<boolean>;

    private availableLanguages: KnockoutObservableArray<LanguageLinkViewModel>;
    private noTranslationMessage: KnockoutObservable<string>;

    // Page context
    private languageFieldName: string;
    private associationKeyFieldName: string;
    private currentPageId: number;

    constructor(params: any) {

        this.languageFieldName = params.languageFieldName;
        this.associationKeyFieldName = params.associationKeyFieldName;

        // Get context informations for the current page
        this.currentPageId = _spPageContextInfo.pageItemId;

        this.availableLanguages = ko.observableArray([]);
        this.noTranslationMessage = ko.observable("");

        this.shouldRender = ko.observable(true);

        const localization = new LocalizationModule();

        localization.getAvailableLanguages().then((languages) => {

            if (languages.length === 1) {

                this.shouldRender(false);

            } else {

                this.getPeerUrls(languages);
            }

        }).catch((errorMesssage) => {
            Logger.write(errorMesssage, LogLevel.Error);
        });
    }

    /**
     * Set the preferred language in the local storage of the browser
     *
     * @param data: the current element data coming from the binding handler
     */
    public setPreferredLanguage = (data) => {

        // Set the preferred language i nthe local storage (used for the redirection then)
        storage.local.put("preferredLanguage", data.languageLabel().toLowerCase());

        // Allow the href
        return true;
    }

    /**
     * Get all available translations for the current page
     *
     * @param languages: the arbitrary languages set up for the component
     */
    private getPeerUrls(languages: string[]) {

        // Get the info for the current page
        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.currentPageId).select(this.associationKeyFieldName, "ID", this.languageFieldName).get().then((item) => {

            const allLanguages: LanguageLinkViewModel[] = [];
            const currentPageLanguage = item[this.languageFieldName];

            // Does a page in the 'Pages' library whithin a peer web exist with the same GUID as me and an other language?
            const filterQuery: string = this.associationKeyFieldName + " eq '" + item[this.associationKeyFieldName] + "' and " + this.languageFieldName + " ne '" + currentPageLanguage  + "'";
            const allLanguagesPromises: Array<Promise<LanguageLinkViewModel>> = [];

            // Loop through each available languages and map the correct information according to the page context and its translations.
            // We want to notifiy the users if there is not translation for a target language so that's why we map an arbitrary array of languages with the results
            languages.map((element) => {

                const p = new Promise<LanguageLinkViewModel>((resolve, reject) => {

                    const peerWeb = new Web(_spPageContextInfo.siteAbsoluteUrl + "/" + element.toLowerCase());

                    const languageLink = new LanguageLinkViewModel();

                    languageLink.displayName(i18n.t(element));
                    languageLink.languageLabel(element.toLowerCase());

                    // Set the corresponding flag icon CSS class
                    languageLink.flagCssClass(element.toLowerCase());

                    // This is the current language
                    if (element.localeCompare(currentPageLanguage) === 0) {

                        languageLink.isCurrentLanguage(true);
                        resolve(languageLink);

                    } else {

                        peerWeb.select("AllProperties").expand("AllProperties").get().then((properties) => {

                            // Return only one element ordered descending by the Modified date
                            // It can't have more than one translation per language for the current page
                            // tslint:disable-next-line:no-shadowed-variable
                            peerWeb.lists.getById(properties.AllProperties.OData__x005f__x005f_PagesListId).items.filter(filterQuery).orderBy("Modified").select("FileRef, Title", this.languageFieldName).top(1).get().then((item) => {

                                // If there is one or more translations, fill the appropriate information
                                if (item.length > 0) {

                                    const itemLanguage: string = item[0][this.languageFieldName];
                                    const itemUrl: string = item[0].FileRef;

                                    if (element.localeCompare(itemLanguage) === 0) {

                                        languageLink.url(itemUrl);
                                        languageLink.isValidTranslation(true);

                                    } else {

                                        // This is a translation for an other language not listed in the available languages for the component...
                                        languageLink.isValidTranslation(false);
                                        this.noTranslationMessage(i18n.t("noTranslationMessage"));
                                    }

                                } else {

                                    // No item = no translation at all
                                    languageLink.isValidTranslation(false);
                                    this.noTranslationMessage(i18n.t("noTranslationMessage"));
                                }

                                resolve(languageLink);

                            }).catch((errorMesssage) => {

                                reject(errorMesssage);
                            });
                        });
                    }
                });

                allLanguagesPromises.push(p);
            });

            // Resolve all calls at once
            Promise.all(allLanguagesPromises).then((languageLinks) => {

                _.each(languageLinks, (languageLink) => {
                    allLanguages.push(languageLink);
                });

                // Init available languages for the user
                this.availableLanguages(allLanguages);

            }).catch((errorMesssage) => {
                Logger.write(errorMesssage, LogLevel.Error);
            });

        }).catch((errorMesssage) => {

            Logger.write(errorMesssage, LogLevel.Error);
        });
    }
}

export default LanguageSwitcherViewModel;
