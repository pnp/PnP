// ========================================
// iCalendar Generator Component View Model
// ========================================
declare function require(name: string);
const icalToolkit = require("ical-toolkit");
const fileSaver = require("file-saver");
const sanitize = require("sanitize-filename");

import * as i18n from "i18next";
import { Logger, LogLevel, Web } from "sp-pnp-js";
import LocalizationModule from "../../modules/LocalizationModule";
import UtilityModule from "../../modules/UtilityModule";

class IcsCalendarGeneratorViewModel {

    public eventItemId: number;
    public icsButtonLabel: KnockoutObservable<string>;
    public displayMode: KnockoutObservable<number>;
    private wait: KnockoutObservable<boolean>;
    private utilityModule: UtilityModule;

    constructor(params: any) {

        this.eventItemId =  params.eventItemId;
        const displayMode = params.displayMode ? params.displayMode : 1;
        this.displayMode = ko.observable(displayMode);
        this.icsButtonLabel = ko.observable("");
        const localization = new LocalizationModule();
        this.utilityModule = new UtilityModule();
        this.wait = ko.observable(false);

        localization.ensureResourcesLoaded(() => {
            this.icsButtonLabel(i18n.t("icsButtonLabel"));
        });
    }

    public generatorIcs = () => {

        this.wait(true);

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Get the event details directly from the Pages library
        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(this.eventItemId).get().then((item) => {

            // Create a builder
            const builder = icalToolkit.createIcsFileBuilder();

            const itemEndDate: string = item.OData__EndDate;
            const itemStartDate: string = item.StartDate;
            let endDate: Date;
            let startDate: Date;

            // tslint:disable-next-line:prefer-conditional-expression
            if (!itemStartDate) {
                startDate = new Date(); // Today;
            } else {
                startDate = new Date(itemStartDate);
            }

            // tslint:disable-next-line:prefer-conditional-expression
            if (!itemEndDate) {
                endDate = startDate;
            } else {
                endDate = new Date(itemEndDate);
            }

            // Add events
            builder.events.push({

                // Optional description of event.
                description: $(item.PublishingPageContent).text(),

                // Event end time, Required: type Date()
                end: endDate,

                // Location of event, optional.
                location: item.Location,

                // Event start time, Required: type Date()
                start: startDate,

                // Status of event
                status: "CONFIRMED",

                // Event summary, Required: type String
                summary: item.Title,

                // Event identifier, Optional, default auto generated
                // Issue with IE11 due to the method getRandomBytes(), we need to generate an unique identifier
                uid: this.utilityModule.getNewGuid(),
            });

            // Try to build
            const icsFileContent = builder.toString();

            // Check if there was an error (Only required if yu configured to return error, else error will be thrown.)
            if (icsFileContent instanceof Error) {
                Logger.write("[iCalendar Generator] Error during the iCal generation", LogLevel.Error);
            }

            // Prompt for download (text file)
            const blob = new Blob([icsFileContent], {type: "text/plain;charset=utf-8"});

            this.wait(false);

            fileSaver.saveAs(blob, sanitize(item.Title.substr(0, 9) + ".ics"));
        });
    }
}

export default IcsCalendarGeneratorViewModel;
