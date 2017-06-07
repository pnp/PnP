declare var require: {
    <T>(path: string): T;
    (paths: string[], callback: (...modules: any[]) => void): void;
    ensure: (paths: string[], callback: (require: <T>(path: string) => T) => void) => void;
};

import i18n = require("i18next");
import { Web } from "sp-pnp-js";

// Be careful, if you do a manual import, you can't add this module to the vendor chunk of webpack (an error will occur during execution otherwise)
import * as moment from "moment";

// Resources
//require("moment/locale/fr");
let enUSResources = require("../resources/en-US.json");
let frFRResources = require("../resources/fr-FR.json");

export class Localization {

    /**
     * Get the current page language and setup global application variables like i18next and moment.js 
     * @return {Promise<void>}       A promise allowing you to execute your code logic.
     */
    public initLanguageEnv() : Promise<void>  {

        let p = new Promise<void>((resolve) => {

            let web = new Web(_spPageContextInfo.webAbsoluteUrl);

            // Get the current page language. In this solution, the language context is given by the page itself instead of the web.
            // By this way, we don't have to create a synchronized symetric web structure (like SharePoint variations do). We keep a flat structure with only one site.
            // For a contributor, it is by far easier to use than variations.
            // The "IntranetContentLanguage" is a choice field so we don't need taxonomy field here. Values of this choice field have to be 'en' or 'fr' to fit with the format below.
            web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select("IntranetContentLanguage").get().then((item) => {

                let itemLanguage: string = item.IntranetContentLanguage;

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

                        // Init the locale for the moment object (for date manipulations)
                        moment.locale(workingLanguage); 
                        resolve();               
                    });
                });
            });

        return p;
    }
}


