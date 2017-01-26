// ====================
// Translation Control Component
// ====================

/* ****************TEST CASES********************
 *
 *  - Scenario: "New page creation - Component UI"
 *    - Init: Create a new blank page in the 'Pages' library
 *    - Expected behavior: 
 *        - The component must show only the languages available for target. 
 *        - Ex: if "EN" and "FR" was specified in the component parameters and the current page language is "EN", so only "FR" is displayed.
 *        - The component must allow the user to create a new translation for the selected language (button + file name control displayed).
 *        - The target page name can't contain special characters or have the same name as the source page.
 *
 *  - Scenario: "Add a new translation when no translation exist"
 *    - Init: Create a new blank page "EN", and add a translation for "FR"
 *    - Expected behavior:
 *       - A new page is created with a different physical page name (.aspx) at the same level in the 'Pages' library. Ex: Source = "Home.aspx", Target = "Accueil.aspx"
 *      - The language of the new page must be the one selected on the component (the "FR" language)
 *       - A new association key is created in the source item before the copy, doing the link between the translations
 *       - All metadata from the original page are copied to the target (like variations do)
 *       - The created file is checkout after the operation
 *       - The component must display a link to the new created page with a succes message
 *
 *  - Scenario: "Add a new translations when one or more translations already exist for other available languages."
 *    - Init: Add the "ES" translation on the "EN" page where there is already a translation for "FR"
 *    - Expected behavior:
 *       - The new page used the same association key as the other pages
 *       - The language of the new page must be the one selected on the component ("ES" here)
 *
 *  - Scenario: "Remove an existing translations other than the original"
 *    - Init: Remove an existing translation "EN" for the orignal "FR" page
 *    - Expected behavior:
 *       - The original page keeps its association key in its metadata
 *
 *  - Scenario: "Recreate a translation after removal"
 *    - Init: recreate the "EN" translation after removing it previously
 *    - Expected behavior
 *       - The association key is kept. The new created translation used the same association key as the orignal page  
 * 
 * **********************************************/

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/sprintf-js/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />
/// <reference path="../../typings/globals/es6-promise/index.d.ts" />

import { UtilityModule } from "../core/utility";
import * as pnp from "sp-pnp-js";
import i18n = require("i18next");
import sprintf = require("sprintf-js");

export class TranslationControlViewModel {

    // Inner properties
    private utilityModule: UtilityModule;

    // Static labels
    private selectLanguageMessage: string;
    private selectPageNameMessage: string;
    private translationComponentTitle: string;

    // Page context
    private languageFieldName: string;
    private associationKeyFieldName: string;
    private currentPageId: number;
    private currentPageUrl: string;

    // Observables
    private invalidFilenameMessage: KnockoutObservable<string>;
    private infoMessage: KnockoutObservable<string>;
    private wait: KnockoutObservable<boolean>;
    private messageStatusClass: KnockoutObservable<string>;
    private messageStatusIcon: KnockoutObservable<string>;
    private isFileNameValid: KnockoutComputed<boolean>;
    private isTranslationExist: KnockoutComputed<boolean>;
    private isError: KnockoutObservable<boolean>;
    private existingTranslations: KnockoutObservableArray<any>;
    private buttonLabel: KnockoutComputed<string>;
    private inputDestinationFileName: KnockoutObservable<string>;
    private selectedLanguage: KnockoutObservable<string>;
    private isNewCreation: KnockoutObservable<boolean>;
    private availableLanguages: KnockoutObservableArray<string>;

    constructor(params: any) {

        this.utilityModule = new UtilityModule();

        // Get the available languages from the the component parameters (arbitrary)
        // These languages must correspond to the column values used for the language column (choice field in this case)
        let languages: Array<string> = params.availableLanguages;
        this.languageFieldName = params.languageFieldName;
        this.associationKeyFieldName = params.associationKeyFieldName;

        // Get context informations for the current page
        this.currentPageId = _spPageContextInfo.pageItemId;
        this.currentPageUrl = _spPageContextInfo.serverRequestPath; // Note: _spPageContextInfo.serverRequestPath works with friendly URLs as well

        // Init observables
        this.wait = ko.observable(true);
        this.isNewCreation = ko.observable(false);
        this.availableLanguages = ko.observableArray([]);
        this.selectedLanguage = ko.observable("");
        this.selectedLanguage.subscribe(this.checkForExistingTranslations, this); // Avoid the "click" binding on the radio button. More info here: http://jsfiddle.net/rniemeyer/cnkVA/2/ 
        this.invalidFilenameMessage = ko.observable("");
        this.inputDestinationFileName = ko.observable("");

        this.isFileNameValid = ko.pureComputed(() => {

            let destinationName = this.inputDestinationFileName();
            let currentFileName = this.currentPageUrl.match(/([^\/]+)(?=\.\w+$)/)[0];

            if (destinationName.length === 0) {

                    this.invalidFilenameMessage(i18n.t("emptyFilenameMessage"));
                    return false;

            } else {
                if (/[#%\*\[\]\\/|\\":<>\?]/.test(destinationName) || destinationName.localeCompare(currentFileName) === 0) {

                    this.invalidFilenameMessage(i18n.t("invalidFilenameMessage"));
                    return false;

                } else {

                    this.invalidFilenameMessage("");
                    return true;
                }
            }
        });

        this.buttonLabel = ko.computed(() => {

            return sprintf.sprintf(i18n.t("translateButtonLabel"), this.selectedLanguage());
        });

        this.selectLanguageMessage = i18n.t("selectLanguageMessage");
        this.selectPageNameMessage = i18n.t("selectPageNameMessage");
        this.translationComponentTitle = i18n.t("translationComponentTitle");
        this.messageStatusClass = ko.observable("");
        this.messageStatusIcon = ko.observable("");
        this.existingTranslations = ko.observableArray([]);

        this.isTranslationExist = ko.computed(() => {

            if (this.existingTranslations().length > 0) {

                return true;

            } else {

                return false;
            }
        });

        this.isError = ko.observable(false);
        this.infoMessage = ko.observable("");

        // Pre-flight check
        this.initAvailableLanguages(languages);
        this.checkForExistingTranslations();
    }

    // ------------------
    // Callback functions
    // Note for callback syntax with TypeScript: https://blogs.msdn.microsoft.com/typescript/2013/08/06/announcing-0-9-1/
    // ------------------

    /**
     * Check existing translations for the current page.
     * 
     */
    public checkForExistingTranslations = () => {

        // Reset internal states
        this.wait(true);
        this.isError(false);

        // Get the info for the current page
        pnp.sp.web.lists.getByTitle("Pages").items.getById(this.currentPageId).select(this.associationKeyFieldName, "ID", this.languageFieldName).get().then((item) => {

            let targetLanguage: string = this.selectedLanguage();

            // Does a page in the 'Pages' library exist with the same GUID as me for the selected target language ?
            let filterQuery: string = this.associationKeyFieldName + " eq '" + item[this.associationKeyFieldName] + "' and ID ne '" + item.ID + "' and " + this.languageFieldName + " eq '" + targetLanguage  + "'";

            pnp.sp.web.lists.getByTitle("Pages").items.filter(filterQuery).select("FileRef, FileLeafRef").get().then((item) => {

                let msg: string;

                if (item.length > 0) {

                    if (this.isNewCreation()) {

                        msg = sprintf.sprintf(i18n.t("successTranslationCreation"), this.selectedLanguage());
                        this.messageStatusClass("ms-MessageBar--success");
                        this.messageStatusIcon("ms-Icon--checkboxCheck");

                    } else {

                        msg = sprintf.sprintf(i18n.t("existingTranslations"), this.selectedLanguage());
                        this.messageStatusClass("warning-msg");
                        this.messageStatusIcon("ms-Icon--infoCircle");
                    }

                    this.infoMessage(msg);
                    this.isNewCreation(false);
                }

                this.existingTranslations(item);
                this.wait(false);

            }).catch((data) => {

                this.showErrorMessage(data);
            });

        }).catch((data) => {

            this.showErrorMessage(data);
        });
    }

    /**
     * Call back function that creates a new translation for the current page according the selected target language
     */
    public createPageTranslation = () => {

        // Reset internal states
        this.wait(true);
        this.isError(false);

        // Build the destination file URL
        let destinationFile = this.currentPageUrl.replace(/(.*)\/.*(\.aspx$)/i, "$1/" + this.inputDestinationFileName() + "$2");

        this.ensurePageGuid().then(() => {

            // Copy the page in the Pages library with the new language
            // Note: during the copy operation, all original metadata are retained by default 
            pnp.sp.web.getFileByServerRelativeUrl(this.currentPageUrl).copyTo(destinationFile, true).then(() => {

                // Checkout the file before making any changes
                pnp.sp.web.getFileByServerRelativeUrl(destinationFile).checkout().then(() => {

                    // Get the ID the copied file and update the language (update does not work with a single operation)
                    pnp.sp.web.getFileByServerRelativeUrl(destinationFile).listItemAllFields.select("ID").get().then((item) => {

                        // Set the peer language on the destination file
                        pnp.sp.web.lists.getByTitle("Pages").items.getById(item.ID).update({[this.languageFieldName]: this.selectedLanguage()}).then((item) => {

                            this.isNewCreation(true);
                            this.checkForExistingTranslations();

                        }).catch((errorMesssage) => {

                            this.showErrorMessage(errorMesssage);
                            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
                        });

                    }).catch((errorMesssage) => {

                        this.showErrorMessage(errorMesssage);
                        pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
                    });

                }).catch((errorMesssage) => {

                    this.showErrorMessage(errorMesssage);
                    pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
                });

            }).catch((errorMesssage) => {

                this.showErrorMessage(errorMesssage);
                pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
            });

        }).catch((errorMesssage) => {

            this.showErrorMessage(errorMesssage);
            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
        });
    }

    /**
     * Init the available languages for the user
     *
     * @param languages: the arbitrary languages set up for the component
     */
    private initAvailableLanguages(languages: Array<string>): void {

        // Get the current page language
        pnp.sp.web.lists.getByTitle("Pages").items.getById(this.currentPageId).select(this.languageFieldName, this.associationKeyFieldName).get().then((item) => {

            // Remove the current page language from the available languages
            let index = languages.indexOf(item[this.languageFieldName]);
            if (index > -1) {
                languages.splice(index, 1);
            }

            // Select by default the first item for left languages
            if ( languages.length > 0) {
                this.selectedLanguage(languages[0]);
            }

            // Init available languages for the user
            this.availableLanguages(languages);
        });
    }

    /**
     * Ensure that the page has an unique identifier for translations linking. If it doesn't a new one is created in the appropriate field.
     * This field is configurable via the "associationKeyFieldName" parameter for the component
     *
     * @param languages: the arbitrary languages set up for the component
     */
    private ensurePageGuid(): Promise<void> {

        let p = new Promise<void>((resolve) => {

            // Get the association key for the current item
            pnp.sp.web.lists.getByTitle("Pages").items.getById(this.currentPageId).select(this.associationKeyFieldName).get().then((item) => {

                let currentContentAssociationKey = item[this.associationKeyFieldName];

                if (currentContentAssociationKey) {

                        // Keep the existing guid
                        resolve();

                } else {

                    let guid = this.utilityModule.getNewGuid();

                    // Set a new unique identifier for this page
                    pnp.sp.web.lists.getByTitle("Pages").items.getById(this.currentPageId).update({[this.associationKeyFieldName] : guid}).then((item) => {

                        resolve();
                    });
                }

            }).catch((errorMesssage) => {

                this.showErrorMessage(errorMesssage);
                pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
            });

        }).catch((errorMesssage) => {

            this.showErrorMessage(errorMesssage);
            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
        });

        return p;
    }

    private showErrorMessage(error) {

        this.isError(true);
        this.wait(false);
        this.messageStatusClass("ms-MessageBar--error");
        this.messageStatusIcon("ms-Icon ms-Icon--xCircle");
        this.infoMessage(error);
    }
}
