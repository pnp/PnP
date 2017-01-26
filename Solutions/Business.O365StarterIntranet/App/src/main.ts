// ===============================
// Application main entry point
// ===============================

/// <reference path="../typings/globals/knockout/index.d.ts" />
/// <reference path="../typings/globals/jquery/index.d.ts" />

// We must declare this function to get static files like html or CSS via the ts-loader
// More info here https://github.com/TypeStrong/ts-loader
declare var require: {
    <T>(path: string): T;
    (paths: string[], callback: (...modules: any[]) => void): void;
    ensure: (paths: string[], callback: (require: <T>(path: string) => T) => void) => void;
};

// View models for components
import { BreadcrumbViewModel } from "./viewmodels/breadcrumb.viewmodel";
import { ContextualMenuViewModel } from "./viewmodels/contextualmenu.viewmodel";
import { CarouselViewModel } from "./viewmodels/carousel.viewmodel";
import { DefaultDisplayTemplateItemViewModel } from "./viewmodels/defaultdisplaytemplateitem.viewmodel";
import { DefaultFilterViewModel } from "./viewmodels/defaultfilter-mui.viewmodel";
import { DocumentDisplayTemplateItemViewModel } from "./viewmodels/documentitem.viewmodel";
import { HeaderLinksViewModel } from "./viewmodels/headerlinks.viewmodel";
import { KnockoutComponent } from "./viewmodels/knockoutcomponent";
import { LanguageSwitcherViewModel } from "./viewmodels/languageswitcher.viewmodel";
import { NewsDisplayTemplateItemViewModel } from "./viewmodels/newsitem.viewmodel";
import { PageInfoViewModel } from "./viewmodels/pageinfo.viewmodel";
import { SearchBoxViewModel } from "./viewmodels/searchbox.viewmodel";
import { SearchBoxMobileViewModel } from "./viewmodels/searchboxmobile.viewmodel";
import { TopNavViewModel } from "./viewmodels/topnav.viewmodel";
import { TranslationControlViewModel } from "./viewmodels/translationcontrol.viewmodel";

// Third party libraries
import i18n = require("i18next");
import * as moment from "moment";
import * as pnp from "sp-pnp-js";

// Main style sheet for the application
require("./styles/css/global.scss");
require("./styles/css/layouts.scss");
require("./styles/css/layouts-edit.scss");
require("./styles/css/displaytemplates.scss");

// Reusable contents CSS
require("./styles/css/suggestionsbox.scss");

// Images
require("./styles/css/images/logo_intranet.png");
require("./styles/css/images/spinner.gif");
require("./styles/css/images/default_image.png");
require("./styles/css/images/favicon_intranet.ico");

// Bootstrap CSS isolation
require("./styles/css/bootstrap/bootstrap-prefix.less");

// Resources
require("moment/locale/fr");
let enUSResources = require("./resources/en-US.json");
let frFRResources = require("./resources/fr-FR.json");

export class Main {

    // Static methods are mainly used for SharePoint display templates (it is just a public wrapper)
    // We can't use Knockout components here because bindings are not triggered when the display template logic adds the component programmatically
    // We have to apply bindings manually after rendering
    public static initNewsDisplayTemplateItemViewModel = (currentItem: any, domElement: string) => {

        let viewModel = new NewsDisplayTemplateItemViewModel(currentItem);
        ko.applyBindings(viewModel, domElement);
    }

    public static initDocumentDisplayTemplateItemViewModel = (currentItem: any, domElement: string) => {

        let viewModel = new DocumentDisplayTemplateItemViewModel(currentItem);
        ko.applyBindings(viewModel, domElement);
    }

    public static initDefaultDisplayTemplateItemViewModel = (currentItem: any, domElement: string) => {

        let viewModel = new DefaultDisplayTemplateItemViewModel(currentItem);
        ko.applyBindings(viewModel, domElement);
    }

    public static initDefaultMuiFilterViewModel = (domElement: string) => {

        let viewModel = new DefaultFilterViewModel();
        ko.applyBindings(viewModel, domElement);
    }

    public static getResource = (resourceKey: string) => {

        return i18n.t(resourceKey);
    }

    /**
     * Register all Knockout components for the entire application
     * @return {String}       The stringified tree object
     */
    public registerComponents() {

        // ===============================
        // Register Knockout components   
        // ===============================

        // Component: "MainMenu"
        let mainMenuTemplate = require("./templates/topnav.template.html");
        require("./styles/css/topnav.scss");
        let mainMenuComponent = new KnockoutComponent("component-topnav", TopNavViewModel, mainMenuTemplate);

        // Component: "ContextualMenu"
        let contextualMenuTemplate = require("./templates/contextualmenu.template.html");
        require("./styles/css/contextualmenu.scss");
        let contextualMenuComponent = new KnockoutComponent("component-contextualmenu", ContextualMenuViewModel, contextualMenuTemplate);

        // Component: "Breadcrumb"
        let breadcrumbTemplate = require("./templates/breadcrumb.template.html");
        require("./styles/css/breadcrumb.scss");
        let breadcrumbComponent = new KnockoutComponent("component-breadcrumb", BreadcrumbViewModel, breadcrumbTemplate);

        // Component: "Header" (template only)
        let headerTemplate = require("./templates/header.template.html");
        require("./styles/css/header.scss");
        let headerComponent = new KnockoutComponent("component-header", null, headerTemplate);

        // Component: "Page Info"
        let pageInfoTemplate = require("./templates/pageinfo.template.html");
        require("./styles/css/pageinfo.scss");
        let pageInfoComponent = new KnockoutComponent("component-pageinfo", PageInfoViewModel, pageInfoTemplate);

        // Component: "Translation Control"
        let translationControlTemplate = require("./templates/translationcontrol.template.html");
        require("./styles/css/translationcontrol.scss");
        let translationcontrolComponent = new KnockoutComponent("component-translationcontrol", TranslationControlViewModel, translationControlTemplate);

        // Component: "Language Switcher"
        let languageSwitcherTemplate = require("./templates/languageswitcher.template.html");
        require("./styles/css/languageswitcher.scss");
        let languageSwitcherComponent = new KnockoutComponent("component-languageswitcher", LanguageSwitcherViewModel, languageSwitcherTemplate);

        // Component: "Searchbox"
        let searchboxTemplate = require("./templates/searchbox.template.html");
        require("./styles/css/searchbox.scss");
        let searchboxComponent = new KnockoutComponent("component-searchbox", SearchBoxViewModel, searchboxTemplate);

        // Component: "Footer" (template only)
        let footerTemplate = require("./templates/footer.template.html");
        require("./styles/css/footer.scss");
        let footerComponent = new KnockoutComponent("component-footer", null, footerTemplate);

        // Component: "Header Links"
        let headerLinksTemplate = require("./templates/headerlinks.template.html");
        let headerLinksComponent = new KnockoutComponent("component-headerlinks", HeaderLinksViewModel, headerLinksTemplate);

        // Component: "Search Box (mobile)"
        let searchboxMobileTemplate = require("./templates/searchboxmobile.template.html");
        require("./styles/css/searchboxmobile.scss");
        let searchboxMobileComponent = new KnockoutComponent("component-searchboxmobile", SearchBoxMobileViewModel, searchboxMobileTemplate);

        // Component: "Carousel"
        let carouselTemplate = require("./templates/carousel.template.html");
        require("./styles/css/carousel.scss");
        require("flickity/dist/flickity.css")
        let carouselComponent = new KnockoutComponent("component-carousel", CarouselViewModel, carouselTemplate);
    }

    public init() {

        this.registerComponents();

        // Init the pnp logger
        let consoleLogger = new pnp.log.ConsoleListener();
        pnp.log.subscribe(consoleLogger);
        pnp.log.activeLogLevel = pnp.log.LogLevel.Verbose;

        // Be careful, we need to apply bindings after the document is ready
        $(document).ready(() => {

            // Get the current page language. In this solution, the language context is given by the page itself instead of the web.
            // By this way, we don't have to create a synchronized symetric web structure (like SharePoint variations do). We keep a flat structure with only one site.
            // For a contributor, it is by far easier to use than variations.
            // The "IntranetContentLanguage" is a choice field so we don't need taxonomy field here. Values of this choice field have to be 'en' or 'fr' to fit with the format below.
            pnp.sp.web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select("IntranetContentLanguage").get().then((item) => {

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

                        // Apply the Knockout JS magic!
                        ko.applyBindings();

                        // Add Bootstrap responsive behavior for news images
                        $("#page-image img").addClass("img-responsive");
                });
            });
        });
    }
}

// Start the engine
let main = new Main();
main.init();
