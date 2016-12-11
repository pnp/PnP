AccordionContent.Main = function () {
    var AccordionContentEventHandlers = function () {
        var AccordionContentUpdate = function () {
            RefreshData(getaccordionContentRestURL());
        };

        var SetAccordionContentResponse = function (contentKey, accordionContentJSON) {
            if (typeof contentKey != "undefined" && contentKey != null) {
                AccordionContent.CacheFunctions.Init();
                var dataToStore = JSON.stringify(accordionContentJSON);
                AccordionContent.CacheFunctions.SetCachedData(contentKey, dataToStore);
            }
        };

        var GetAccordionContentResponse = function (contentKey) {
            var returnResponse = null;
            if (typeof contentKey != "undefined" && contentKey != null) {
                AccordionContent.CacheFunctions.Init();
                var getJSONdata = AccordionContent.CacheFunctions.GetCachedData(contentKey);
            
                if (typeof getJSONdata != "undefined" && getJSONdata != null && getJSONdata != "") {
                    returnResponse = JSON.parse(getJSONdata);
                } 
            }
        
            return returnResponse;
        };

        return {
            StoreAccordionContentResponse: SetAccordionContentResponse,
            RetrieveAccordionContentResponse: GetAccordionContentResponse,
            AccordionContentUpdate: AccordionContentUpdate
        }
    }();

    var AccordionContentCommon = function () {
        var CommonVariables = {
            contentKeyValue: "AccordionListKey",
            listSource: "AccordionList"
        };

        return {
            CommonVariables: CommonVariables 
        }
    }();

    var AccordionContentDataModel = function () {
         // Data    
         var self = this;
         self.bindedAllSlideItems = ko.observableArray([]);
         self.allSlideItems = ko.observableArray([]);
         self.loadingComplete = ko.observable();
         self.resultsFound = ko.observable();
         self.useFormat = ko.observable(); 

         getaccordionContentRestURL = function () {
             return _spPageContextInfo.webServerRelativeUrl + "/_api/lists/" + AccordionContentCommon.CommonVariables.listSource + "/items?$select=SlideTitle,SlideDescription,SlideWho"
         };

         processJSONResponse = function (returnedData) {
             // Returning the results
             var mappedaccordionContentItems = null;
             var resultsFound = false;
             var accordionContentCounter = 0;
             self.loadingComplete(false);

             if (returnedData.length > 0) {
                 resultsFound = true;

                 mappedaccordionContentItems = $.map(returnedData, function (item, i) {
                     if (item.SlideTitle != null) {
                         resultsFound = true;

                         return new AccordionContent.SlideModel.AccordionContentSlide.SlideItem(item.SlideTitle, item.SlideDescription, item.SlideWho)
                     }
                 });
             }

             self.loadingComplete(true);

             if (resultsFound) {
                 self.resultsFound(true);
             }
             else {
                 self.resultsFound(false);
                 $('#accordionContentNoDataFound').show(); 
             }

             self.allSlideItems = mappedaccordionContentItems;
             self.bindedAllSlideItems(mappedaccordionContentItems);

             $('#accordion').accordion({ header: 'h3' });
         }     
   
         RefreshData = function (urlData) {
             $('#accordionContentCurrentLoadingStatus').text('Loading Data, Please wait...');
             self.loadingComplete(false);  // Show loading while retrieve data that isn't cached or new data
             var cachedData = AccordionContentEventHandlers.RetrieveAccordionContentResponse(AccordionContentCommon.CommonVariables.contentKeyValue); 

             if (typeof cachedData == "undefined" || cachedData == null || cachedData == "") {
                 // Data is not cached, so retrieve
                 console.log('Accordion Content: Retrieving Data: Not using cache');

                 $.ajax({
                     url: urlData,
                     method: "GET",
                     headers: { "Accept": "application/json; odata=verbose" },
                     success: function (allData) {
                         if (allData != null) {
                             var returnedData = null;
                             if (allData.d.length > -1) {
                                 returnedData = allData.d;
                             }
                             else {
                                 returnedData = allData.d.results;
                             }

                             // Cache results and process response for display
                             AccordionContentEventHandlers.StoreAccordionContentResponse(AccordionContentCommon.CommonVariables.contentKeyValue, returnedData);
                             processJSONResponse(returnedData);
                         }
                     },
                     error: function (allData, err) {
                         console.log(err);
                         $("#accordionContentCurrentLoadingStatus").removeClass("progress-bar");
                         $("#accordionContentCurrentLoadingStatus").parent().removeClass("progress progress-striped active");
                         $('#accordionContentCurrentLoadingStatus').text('There was an error loading the important content data.  Please try again later.  If this problem continues please contact the help desk.');
                     }
                 });
             } else {
                 // Process cached data
                 console.log('Accordion Content: Retrieving Data: Using cache');
                 processJSONResponse(cachedData);
             }
         };

         GetaccordionContentData = function () {
             AccordionContentEventHandlers.AccordionContentUpdate();
        };

        return {
            GetaccordionContentData: GetaccordionContentData,
            bindedAllSlideItems: bindedAllSlideItems
        }
     }();

     var StartAccordionContentScript = function () {
         ko.applyBindings(AccordionContentDataModel);
         AccordionContentDataModel.GetaccordionContentData();
     }

     return {
         StartAccordionContentScript: StartAccordionContentScript
     }
}();

$(function () {
    // Register script for MDS if possible
    RegisterModuleInit("main.js", AccordionContent.Main.StartAccordionContentScript);
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', AccordionContent.Main.StartAccordionContentScript);

    if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
        Sys.Application.notifyScriptLoaded();
    }

    if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
        NotifyScriptLoadedAndExecuteWaitingJobs("main.js");
    }
});