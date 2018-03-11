declare var require: {
    <T>(path: string): T;
    (paths: string[], callback: (...modules: any[]) => void): void;
    ensure: (paths: string[], callback: (require: <T>(path: string) => T) => void) => void;
};

import * as i18n from "i18next";
import { Site, Web } from "sp-pnp-js";

// Be careful, if you do a manual import, you can't add this module to the vendor chunk of webpack (an error will occur during execution otherwise)
import * as moment from "moment";

// Resources
// tslint:disable-next-line:no-var-requires
const enUSResources = require("../loc/en-US.json");
// tslint:disable-next-line:no-var-requires
const frFRResources = require("../loc/fr-FR.json");

class LocalizationModule {

    public languageFieldName: string;

    constructor() {
        this.languageFieldName = "IntranetContentLanguage";
    }

    /**
     * Get the current page language and setup global application variables like i18next and moment.js
     * @return {Promise<void>}       A promise allowing you to execute your code logic.
     */
    public initLanguageEnv(): Promise<void>  {

        const p = new Promise<void>((resolve) => {

            const web = new Web(_spPageContextInfo.webAbsoluteUrl);

            // Get the current page language. In this solution, the language context is given by the page itself instead of the web.
            // By this way, we don't have to create a synchronized symetric web structure (like SharePoint variations do). We keep a flat structure with only one site.
            // For a contributor, it is by far easier to use than variations.
            // The "IntranetContentLanguage" is a choice field so we don't need taxonomy field here. Values of this choice field have to be 'en' or 'fr' to fit with the format below.
            web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(_spPageContextInfo.pageItemId).select("IntranetContentLanguage").get().then((item) => {

                const itemLanguage: string = item.IntranetContentLanguage;

                // Default language for the intranet
                let workingLanguage: string = "en";

                if (itemLanguage) {
                    workingLanguage = itemLanguage.toLowerCase();
                }

                i18n.init({

                    // Init the working language and resource files for the entire application
                    fallbackLng: "en",
                    lng: workingLanguage,
                    resources: {

                        en: {
                            translation: enUSResources,

                        },
                        fr: {
                            translation: frFRResources,
                        },
                    },
                    }, (err, t) => {
                        resolve();
                    });
                });
            });

        return p;
    }

    /**
     * Ensure the i18n object is initialized before accesing resources
     * @param  {any} callback Anonymous function to execute after the i18n is initialized
     */
    public ensureResourcesLoaded(callback: any) {

        // We need to ensure the i18n global object is set up before getting resources
        // In some cases, the object was not initialized during the viewmodel execution resulting of empty strings for translations (i.e display templates loading are not managed by our code)
        // If translation returns 'undefined', the i18next object was not initialized, so we wait for init using the native i18next event
        if (!i18n.t("LCID")) {
            i18n.on("initialized", (options) => {

                this.initMomentLocale(options.lng);
                callback();
            });
        } else {
            this.initMomentLocale(i18n.t("languageLabel").toLowerCase());
            callback();
        }
    }

    /**
     * Get the available languages for the intranet
     * Languages are defined by the language choice field values ('EN', 'FR', etc.)
     */
    public getAvailableLanguages(): Promise<string[]> {

        const p = new Promise<string[]>((resolve, reject) => {

            // Get dynamically the available languages in the application
            const site = new Site(_spPageContextInfo.siteAbsoluteUrl);

            site.rootWeb.fields.getByInternalNameOrTitle(this.languageFieldName).select("Choices").get().then((languageField) => {

                resolve(languageField.Choices.results);

            }).catch((errorMesssage) => {

                reject(errorMesssage);
            });
        });

        return p;
    }

    private initMomentLocale(lng: string) {

         // Init the locale for the moment object (for date manipulations)
        moment.locale(lng);

        // 24h format
        moment.updateLocale(lng, {
            longDateFormat : i18n.t("longDateFormat", { returnObjects: true }),
        });
    }
}

export default LocalizationModule;
