// ========================================
// SharePoint Search Display Template View Model
// ========================================
import * as i18n from "i18next";
import * as moment from "moment";
import { ConsoleListener, Logger, LogLevel } from "sp-pnp-js";
import "trunk8"; // Trunk8 typings are exposed through an interface, so we have just to import it globally
import LocalizationModule from "../../modules/LocalizationModule";
import TaxonomyModule from "../../modules/TaxonomyModule";
import UtilityModule from "../../modules/UtilityModule";
import "../IKnockoutBindingHandlers";
import NavigationViewModel from "../NavigationViewModel";

declare var Srch; // Allow using SharePoint display template functions direclty in the viewmodel
declare var HP;
declare function unescape(s: string): string;

class DisplayTemplateViewModel {

    public item: KnockoutObservable<any>;
    public localization: LocalizationModule;
    public utilityModule: UtilityModule;
    public taxonomyModule: TaxonomyModule;
    public searchPageUrl: KnockoutObservable<string>;
    public currentLanguage: KnockoutObservable<string>;

    constructor(currentItem?: any) {

        // If an item has been specified, we create an observable so we can access item properties directly in the display template HTML
        if (currentItem) {
            this.item = ko.observable(currentItem);
        }

        this.searchPageUrl = ko.observable("");
        this.currentLanguage = ko.observable("");

        this.localization = new LocalizationModule();
        this.taxonomyModule = new TaxonomyModule();
        this.utilityModule = new UtilityModule();

        this.localization.ensureResourcesLoaded(() => {
            this.currentLanguage(i18n.t("languageLabel"));
        });

        /* Utility binding handlers for display templates */
        ko.bindingHandlers.summarize = {

            init: (element, valueAccessor, allBindings) => {

                // Get the current value of the current property we're bound to
                const value = ko.unwrap(valueAccessor());
                const linesCount = allBindings.get("linesCount") || 1;
                const toolTip = allBindings.get("tooltip") || false;

                const trunk8Options: Trunk8Options = {
                    lines: linesCount,
                    tooltip: toolTip,
                };

                // 1) Output the HTML string without modifications
                if (value.html) {

                    $(element).html(value.html);
                }

                // 2) Output the text only from an HTML string (For example to trim complex HTML elements likes tables or images)
                if (value.text) {

                    const decodedHtmlString = $("<textarea/>").html(value.text).text();
                    $(element).text($(decodedHtmlString).text());
                }

                // 3) Output the hit Highlighted summary with matched terms in bold
                if (value.hitHighlightedSummary) {

                    // Call the specific SharePoint function for this case
                    $(element).html(Srch.U.processHHXML(value.hitHighlightedSummary));
                }

                // Truncate the news summary
                $(element).trunk8(trunk8Options);

                // Adjust automatically news summary on resize
                $(window).resize((event) => {
                    $(element).trunk8(trunk8Options);
                });

                // Listen to a custom event to be able to truncate elements on demand
                // We need this little trick because of trunk8 doesn't work for hidden elements (see the "click" bindings in tabs HTML on the home page layout)
                $(element).on("truncate", (event) => {

                    $(document).ready(() => {
                        $(element).trunk8(trunk8Options);
                    });
                });
            },
        };

        ko.bindingHandlers.getResource = {

            init: (element, valueAccessor) => {

                this.localization.ensureResourcesLoaded(() => {
                    const value = ko.unwrap(valueAccessor());

                    if ($(element).is("optGroup")) {
                        $(element).attr("label", i18n.t(value));
                    } else {
                        $(element).text(i18n.t(value));
                    }
                });
            },
        };

        ko.bindingHandlers.formatDateField = {

            init: (element, valueAccessor, allBindings) => {

                // Needed to get the correct locale for the date
                this.localization.ensureResourcesLoaded(() => {

                    const value = ko.unwrap(valueAccessor());
                    const dateFormat = allBindings.get("dateFormat") || "LL";
                    const date = moment(value).format(dateFormat);
                    $(element).text(date);
                });
            },
        };

        // This handler is very specific to the search control display template. It allows to handle the localized labels when the webpart rendering mode is done server side.
        ko.bindingHandlers.getFormattedResultCount = {

             init: (element, valueAccessor, allBindings) => {

                 this.localization.ensureResourcesLoaded(() => {

                    const start: string = allBindings.get("start") || null;
                    const resultsPerPage: string = allBindings.get("resultsPerPage") || null;
                    const totalRows: string = allBindings.get("totalRows") || null;
                    const submittedKeywords: string = allBindings.get("submittedKeywords") || null;

                    if (start && resultsPerPage && totalRows) {

                        let resultCountString: string = "";
                        let countDisplayString = i18n.t("rs_countDisplayString");
                        if (!countDisplayString) { countDisplayString = $htmlEncode(Srch.Res.rs_ApproximateResultCount); }

                        if (parseInt(start, 10) + parseInt(resultsPerPage, 10) > parseInt(totalRows, 10)) { countDisplayString = (parseInt(totalRows, 10) === 1) ? i18n.t("rs_SingleResultCount") : i18n.t("rs_ResultCount"); }

                        resultCountString = STSHtmlDecode(String.format(countDisplayString, $htmlEncode(parseInt(totalRows, 10).localeFormat("N0"))));

                        if (submittedKeywords) {
                            resultCountString += " " + STSHtmlDecode(String.format(i18n.t("rs_submittedKeywords"), unescape(submittedKeywords)));
                        }

                        $(element).html(resultCountString);
                    }
                });
            },
        };

        ko.bindingHandlers.getWebPartTitle = {

            init: (element) => {
                SP.SOD.executeFunc("SP.js", "SP.ClientContext", () => {

                    // Get the webpart title
                    const ctx = new SP.ClientContext();
                    const pageFile = ctx.get_web().getFileByServerRelativeUrl(_spPageContextInfo.serverRequestPath);
                    const webpartId = $(element).closest("div[webpartid]").attr("webpartid");

                    const webPartManager = pageFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);
                    const webPartDef = webPartManager.get_webParts().getById(new SP.Guid(webpartId));
                    const webPart = webPartDef.get_webPart();

                    ctx.load(webPart, "Properties");
                    ctx.executeQueryAsync(
                        () => {
                            const properties = webPart.get_properties();
                            $(element).text(properties.get_fieldValues().Title);
                        },
                        (sender, args) => {
                            Logger.write(args.get_message(), LogLevel.Error);
                        },
                    );
                });
            },
        };

        // This binding handlers is used to avoid applying bindings twice (from the main script for components)
        // More info here http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
        ko.bindingHandlers.stopBinding = {
            init: () => {
                return { controlsDescendantBindings: true };
            },
        };
    }
}

export default DisplayTemplateViewModel;
