// ========================================
// iCalendar Generator Component View Model
// ========================================
declare function require(name: string);
let icalToolkit = require('ical-toolkit');
let fileSaver = require('file-saver');
let sanitize = require("sanitize-filename");

import { Localization } from "../core/localization";
import * as pnp from "sp-pnp-js";
import i18n = require("i18next");

export class ICSCalendarGeneratorViewModel {

    public eventItemId: number;
    public icsButtonLabel: KnockoutObservable<string>;
    public displayMode: KnockoutObservable<number>;
    private wait: KnockoutObservable<boolean>;

    constructor (params: any) {

        this.eventItemId =  params.eventItemId;
        let displayMode = params.displayMode ? params.displayMode : 1;
        this.displayMode = ko.observable(displayMode); 
        this.icsButtonLabel = ko.observable("");
        let localization = new Localization();
        this.wait = ko.observable(false);

        localization.initLanguageEnv().then(() => {
            this.icsButtonLabel(i18n.t("icsButtonLabel"));
        });
    }

    public generatorIcs = () => {

        this.wait(true);

        // Get the event details directly from the Pages library
        pnp.sp.web.lists.getByTitle("Pages").items.getById(this.eventItemId).get().then(item => {
            
            // Create a builder
            var builder = icalToolkit.createIcsFileBuilder();

            let itemEndDate: string = item.OData__EndDate;
            let itemStartDate: string = item.StartDate
            let endDate: Date;
            let startDate: Date;

            if (!itemStartDate) {
                startDate = new Date() // Today;
            } else {
                startDate = new Date(itemStartDate);
            }

            if (!itemEndDate) {
                endDate = startDate;
            } else {
                endDate = new Date(itemEndDate);
            }

            // Add events
            builder.events.push({

                // Event start time, Required: type Date()
                start: startDate,
                
                // Event end time, Required: type Date()
                end: endDate,
                
                // Event summary, Required: type String
                summary: item.Title,
                
                // Event identifier, Optional, default auto generated
                uid: null, 
                                                
                // Location of event, optional.
                location: item.Location,
                
                //Optional description of event.
                description: $(item.PublishingPageContent).text(),
                                
                //Status of event
                status: 'CONFIRMED',
                
            });
        
            //Try to build
            var icsFileContent = builder.toString();

            //Check if there was an error (Only required if yu configured to return error, else error will be thrown.)
            if (icsFileContent instanceof Error) {
                pnp.log.write("[iCalendar Generator] Error during the iCal generation", pnp.LogLevel.Error);
            }

            // Prompt for download (text file)
            var blob = new Blob([icsFileContent], {type: "text/plain;charset=utf-8"});

            this.wait(false);

            fileSaver.saveAs(blob, sanitize(item.Title.substr(0, 9) + ".ics"));
        });          
    }
}
