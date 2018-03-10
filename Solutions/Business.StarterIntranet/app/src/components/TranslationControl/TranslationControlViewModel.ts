// ========================================
// Translation Control Component
// ========================================

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
 *       - The language of the new page must be the one selected on the component (the "FR" language)
 *       - A new association key is created in the source item before the copy, doing the link between the translations
 *       - All metadata from the original page are copied to the target (like variations do)
 *       - The created file is checkout after the operation
 *       - The component must display a link to the new created page with a success message
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
import * as i18n from "i18next";
import { FetchOptions, Logger, LogLevel, ODataDefaultParser, Site, Web } from "sp-pnp-js";
import * as sprintf from "sprintf-js";
import LocalizationModule from "../../modules/LocalizationModule";
import UtilityModule from "../../modules/UtilityModule";

class TranslationControlViewModel {

    public shouldRender: KnockoutObservable<boolean>;

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

            const destinationName = this.inputDestinationFileName();
            const currentFileName = this.currentPageUrl.match(/([^\/]+)(?=\.\w+$)/)[0];

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

        this.shouldRender = ko.observable(true);

        const localization = new LocalizationModule();

        localization.getAvailableLanguages().then((languages) => {

            if (languages.length === 1) {

                this.shouldRender(false);

            } else {

                this.initAvailableLanguages(languages);
                this.checkForExistingTranslations();
            }

        }).catch((errorMesssage) => {
            Logger.write(errorMesssage, LogLevel.Error);
        });
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

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);
        const site = new Site(_spPageContextInfo.siteAbsoluteUrl);

        // Get the info for the current page
        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.currentPageId).select(this.associationKeyFieldName, "ID", this.languageFieldName).get().then((item) => {

            const targetLanguage: string = this.selectedLanguage();

            if (targetLanguage) {

                // Does a page in the 'Pages' library within the peer web exist with the same GUID as me for the selected target language ?
                const peerWeb = new Web(_spPageContextInfo.siteAbsoluteUrl + "/" + targetLanguage.toLowerCase());

                const filterQuery: string = this.associationKeyFieldName + " eq '" + item[this.associationKeyFieldName] + "' and " + this.languageFieldName + " eq '" + targetLanguage  + "'";

                peerWeb.select("AllProperties").expand("AllProperties").get().then((properties) => {

                    // tslint:disable-next-line:no-shadowed-variable
                    peerWeb.lists.getById(properties.AllProperties.OData__x005f__x005f_PagesListId).items.filter(filterQuery).select("FileRef, FileLeafRef").get().then((item) => {

                        let msg: string;

                        if (item.length > 0) {

                            if (this.isNewCreation()) {

                                msg = sprintf.sprintf(i18n.t("successTranslationCreation"), this.selectedLanguage());
                                this.messageStatusClass("bg-success");
                                this.messageStatusIcon("fa-check text-success");

                            } else {

                                msg = sprintf.sprintf(i18n.t("existingTranslations"), this.selectedLanguage());
                                this.messageStatusClass("bg-info");
                                this.messageStatusIcon("fa-info-circle text-info");
                            }

                            this.infoMessage(msg);
                            this.isNewCreation(false);
                        }

                        this.existingTranslations(item);
                        this.wait(false);

                    }).catch((data) => {

                        this.showErrorMessage(data);
                    });
                });
            }

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

        const targetLanguage: string = this.selectedLanguage();

        const peerWebAbsoluteUrl = _spPageContextInfo.siteAbsoluteUrl + "/" +  targetLanguage.toLowerCase();
        const peerWebServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl + "/" +  targetLanguage.toLowerCase();

        const peerWeb = new Web(peerWebAbsoluteUrl);
        const currentWeb = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Build the destination file URL

        // Get the Pages library name in the target language
        const i18nTargetLanguage = i18n.getFixedT(targetLanguage.toLowerCase());
        const pagesLibraryName = i18nTargetLanguage("pagesLibraryName");

        const regex = "\/" + pagesLibraryName + "\/(.*)\/.*\.aspx";
        const regexInstance = new RegExp(regex, "g");
        const currentPageFolder = regexInstance.exec(this.currentPageUrl);
        const folderName = currentPageFolder ? ("/" + currentPageFolder[1] + "/") : "/";

        const fileName = this.inputDestinationFileName() + ".aspx";
        const destinationFilePath = peerWebServerRelativeUrl + "/" + pagesLibraryName + folderName + fileName;

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        this.ensurePageGuid().then(() => {

            // Copy the page in the Pages library of the peer web with the new language
            // Note: during the copy operation, all original metadata are retained by default
            currentWeb.getFileByServerRelativeUrl(this.currentPageUrl).getBlob().then((blob: Blob) => {

                // Add the file in the peer web in the peer folder
                peerWeb.getFolderByServerRelativeUrl(peerWebServerRelativeUrl + "/" + pagesLibraryName + folderName).files.add(fileName, blob, true).then(() => {

                    // Checkout the file before making any changes
                    peerWeb.getFileByServerRelativeUrl(destinationFilePath).checkout().then(() => {

                        // Get the ID of the copied file and update the language (update does not work with a single operation)
                        peerWeb.getFileByServerRelativeUrl(destinationFilePath).listItemAllFields.select("ID").get().then((item) => {

                                peerWeb.select("AllProperties").expand("AllProperties").get().then((properties) => {

                                    // Set the peer language on the destination file
                                    // tslint:disable-next-line:no-shadowed-variable
                                    peerWeb.lists.getById(properties.AllProperties.OData__x005f__x005f_PagesListId).items.getById(item.ID).update({[this.languageFieldName]: targetLanguage}).then((item) => {

                                        this.isNewCreation(true);
                                        this.checkForExistingTranslations();

                                    }).catch((errorMesssage) => {

                                        this.showErrorMessage(errorMesssage);
                                        Logger.write("[TranslationControl.createPageTranslation]: " + errorMesssage, LogLevel.Error);
                                    });
                                });

                            }).catch((errorMesssage) => {

                                this.showErrorMessage(errorMesssage);
                                Logger.write("[TranslationControl.createPageTranslation]: " + errorMesssage, LogLevel.Error);
                            });

                        }).catch((errorMesssage) => {

                            this.showErrorMessage(errorMesssage);
                            Logger.write("[TranslationControl.createPageTranslation]: " + errorMesssage, LogLevel.Error);
                        });

                }).catch((errorMesssage) => {

                    this.showErrorMessage(errorMesssage + ". Make sure the folder structure is identical in the peer web ('" + destinationFilePath + "').");
                    Logger.write("[TranslationControl.createPageTranslation]: " + errorMesssage, LogLevel.Error);
                });

            });

        }).catch((errorMesssage) => {

            this.showErrorMessage(errorMesssage);
            Logger.write("[TranslationControl.createPageTranslation]: " + errorMesssage, LogLevel.Error);
        });
    }

    /**
     * Init the available languages for the user
     *
     * @param languages: the arbitrary languages set up for the component
     */
    private initAvailableLanguages(languages: string[]): void {

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Get the current page language
        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.currentPageId).select(this.languageFieldName, this.associationKeyFieldName).get().then((item) => {

            // Remove the current page language from the available languages
            const index = languages.indexOf(item[this.languageFieldName]);
            if (index > -1) {
                languages.splice(index, 1);
            }

            // Select by default the first item for left languages
            if (languages.length > 0) {
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

        const p = new Promise<void>((resolve) => {

            const web = new Web(_spPageContextInfo.webAbsoluteUrl);

            // Get the association key for the current item
            web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.currentPageId).select(this.associationKeyFieldName).get().then((item) => {

                const currentContentAssociationKey = item[this.associationKeyFieldName];

                if (currentContentAssociationKey) {

                        // Keep the existing guid
                        resolve();

                } else {

                    const guid = this.utilityModule.getNewGuid();

                    // Set a new unique identifier for this page
                    // tslint:disable-next-line:no-shadowed-variable
                    web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.currentPageId).update({[this.associationKeyFieldName] : guid}).then((item) => {

                        resolve();
                    });
                }

            }).catch((errorMesssage) => {

                this.showErrorMessage(errorMesssage);
                Logger.write("[TranslationControl.ensurePageGuid]: " + errorMesssage, LogLevel.Error);
            });

        }).catch((errorMesssage) => {

            this.showErrorMessage(errorMesssage);
            Logger.write("[TranslationControl.ensurePageGuid]: " + errorMesssage, LogLevel.Error);
        });

        return p;
    }

    private showErrorMessage(error) {

        this.isError(true);
        this.wait(false);
        this.messageStatusClass("bg-danger");
        this.messageStatusIcon("fa fa-exclamation text-danger");
        this.infoMessage(error);
    }
}

export default TranslationControlViewModel;
