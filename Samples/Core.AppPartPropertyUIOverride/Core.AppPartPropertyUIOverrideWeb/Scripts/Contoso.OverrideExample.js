/*! Contoso.OverrideExample.js
 *  
 *  Example JavaScript code that changes the user interface of the example 
 *  App Part via JavaScript at runtime by using the example
 *  Contoso.AppPartPropertyUIOverride JavaScript library
 *  
 */

// NOTE: the following four lines of course ensure that everything's in it's 
// isolated module and loaded when SharePoint Minimal Download Strategy (MDS) 
// is activated or not activated
(function () {
    "use strict";
    window.startConstosoAppPartPropertyUIOverride = function () {
        (function ($, overrider) {
    
            // at this point of JavaScript code execution, the following 
            // three JavaScript libraries have been automatically loaded and 
            // are ready for use:
            // 
            //     jQuery
            //     sp.js (SharePoint 2013 JavaScript Client Side Object Model (CSOM))
            //     Contoso.AppPartPropertyUIOverride.js
            //
            // also, the static Contoso.AppPartPropertyUIOverride runtime module has 
            // been assigned to the "overrider" variable for ease of use

            overrider.moveCategoryToTop("Custom Category 2");
            overrider.moveCategoryToTop("Custom Category 1");
            overrider.expandCategory("Custom Category 1");
            overrider.hideProperty("HostWebListTitleHiddenTextBox", "Custom Category 1");

            // create new custom dropdown list in property UI at runtime
            var listsDropDownJQueryWrapper = $(
                    overrider.createNewContentAtTop({
                        category:"Custom Category 1",
                        optionalName:"Select List",
                        optionalToolTip:"Select a SharePoint list from this web."
                        })
                    .html("<select id=\"contosoSelectSPList\"></select>")[0]
                ).find("#contosoSelectSPList");

            // now use the SharePoint JavaScript Client Side Object (CSOM) model 
            // to get the titles of all lists in this host web
            var clientContext = new SP.ClientContext();
            var hostWeb = clientContext.get_web();
            var lists = hostWeb.get_lists();
            clientContext.load(lists, 'Include(Title)');
            clientContext.executeQueryAsync(
                function () {
                    // query is done
                    // loop through list titles and construct html to output
                    var listEnumerator = lists.getEnumerator();
                    var list = null;
                    var html = [];
                    while (listEnumerator.moveNext()) {
                        list = listEnumerator.get_current();

                        // build the html string for a select list (dropdown) value
                        html.push("<option>");
                        html.push(list.get_title());
                        html.push("</option>");
                    }

                    // inject the html to the drop down
                    listsDropDownJQueryWrapper.html(html.join(""));

                    // now set the current value of the dropdown 
                    // based on what is in the hidden property
                    listsDropDownJQueryWrapper.val(overrider.getValue("HostWebListTitleHiddenTextBox", "Custom Category 1"));

                    // wire up an event handler on the drop down
                    // so that when it's changed,
                    // we automatically write the value to the hidden text box
                    listsDropDownJQueryWrapper.change(function () {
                        // dropdown changed by end user
                        // write value to hidden text box
                        overrider.setValue("HostWebListTitleHiddenTextBox", listsDropDownJQueryWrapper.val(), "Custom Category 1");
                    });

                    // render the tool tips as instructions
                    overrider.renderToolTipsAsInstructions("Custom Category 1");

                    // tell the AppPartPropertyUIOverride framework that we are done
                    // overriding the App Part property UI and to show the property pane now
                    overrider.finished();
                });
        }(jQuery, Contoso.AppPartPropertyUIOverride))
    };
}())

// Register this JavaScript file for SharePoint 2013's SharePoint Minimal Download Strategy (MDS) if possible
RegisterModuleInit("Contoso.OverrideExample.js", startConstosoAppPartPropertyUIOverride); //MDS registration
startConstosoAppPartPropertyUIOverride(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("Contoso.OverrideExample.js");
}
