// create namespaces for our Display Templates
var jslinkTemplates = window.jslinkTemplates || {};

jslinkTemplates.Lookups = {};
jslinkTemplates.Lookups.Generic = {};
jslinkTemplates.Lookups.Generic.SingleItem = {};
jslinkTemplates.Lookups.CheckBoxes = {};
jslinkTemplates.Lookups.Filtered = {};

/* UTILITY METHODS
   ===============
   Used by all of the types of lookup below */

jslinkTemplates.Lookups.clearAll = function (fieldName) {
    // clears all checkboxes for the current fieldname
    $("#" + fieldName + "LookupValues input[type=checkbox]").each(function () {
        this.checked = false;
    });
};
jslinkTemplates.Lookups.checkAll = function (fieldName) {
    // checks all checkboxes for the current fieldname
    $("#" + fieldName + "LookupValues input[type=checkbox]").each(function () {
        this.checked = true;
    });
};

jslinkTemplates.Lookups.registerCallBack = function (ctx) {
    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // registers the callback for the specified field
    if (ctx.CurrentFieldSchema.Type == "Lookup" &&
        ctx.CurrentFieldSchema.FieldType == "Lookup") {
        formCtx.registerGetValueCallback(formCtx.fieldName, jslinkTemplates.Lookups.getSingleValue.bind(null, formCtx.fieldName));
    }
    else if (ctx.CurrentFieldSchema.Type == "Lookup" &&
        ctx.CurrentFieldSchema.FieldType == "LookupMulti") {
        formCtx.registerGetValueCallback(formCtx.fieldName, jslinkTemplates.Lookups.getMultiValue.bind(null, formCtx.fieldName));
    }
};
jslinkTemplates.Lookups.getMultiValue = function (fieldName) {
    // returns the value of our multi-lookup field

    var returnValue = '';

    // get the values we want
    var checkboxes = $("#" + fieldName + "LookupValues input[type=checkbox]:checked");
    //var checkboxes = $("input[type=checkbox]:checked");

    for (var i = 0; i < checkboxes.length; i++) {
        if (returnValue != '') {
            returnValue += ';#'
        }

        returnValue += checkboxes[i].value;
    }

    return returnValue;
};
jslinkTemplates.Lookups.getSingleValue = function (fieldName, textOnly) {
    // returns the value of our single-item lookup field
    // option to just return the text value

    var selectedValue = $("#" + fieldName + "LookupValues option:selected");

    if (selectedValue[0]) {

        if (textOnly) {
            return selectedValue[0].innerText;
        }
        else {
            return selectedValue[0].value;
        }
    }
    else {
        return null;
    }
};

jslinkTemplates.Lookups.renderFilteredLookup = function (formCtx, conditionalFieldName, conditionalFieldValue) {
    $("document").ready(function () {
        // executes an AJAX call to retrieve the lookup values

        // Try to scrape the value from the form (assuming the field is using our custom field)
        var conditionalValue = jslinkTemplates.Lookups.getSingleValue(conditionalFieldName);

        if (conditionalValue != null &&
            conditionalValue != "") {
            // we got a hit on the local control so
            // add an event handler to capture when
            // the field changes again
            var selector = "#" + conditionalFieldName + "LookupValues select";
            $(selector).change(function () {
                jslinkTemplates.Lookups.renderFilteredLookup(formCtx, conditionalFieldName);
            });
        }
        else {
            // if it didn't work, just use the value passed in from the method
            conditionalValue = conditionalFieldValue;
        }

        if (jslinkTemplates.Lookups.getSingleValue(conditionalFieldName, true) == "(None)") {
            // make sure it fires again when the value changes
            var selector = "#" + conditionalFieldName + "LookupValues select";
            $(selector).change(function () {
                jslinkTemplates.Lookups.renderFilteredLookup(formCtx, conditionalFieldName);
            });

            var divId = '#' + formCtx.fieldSchema.Name + 'LookupValues';
            $(divId).html("(None)");
            return;
        }
        

        // strip out the "ID" from the field
        conditionalValue = conditionalValue.substring(0, conditionalValue.indexOf(";"));
        

        // get the lookup list ID from the current field
        var lookupListId = formCtx.fieldSchema.LookupListId;

        // construct the URL using the page context info to get the Web URL
        var requestUri = _spPageContextInfo.webAbsoluteUrl +
                      "/_api/web/lists/getbyid('" + lookupListId + "')/items";

        requestUri += "?$filter=" + conditionalFieldName + "/Id eq " + conditionalValue + "";

        // execute AJAX request
        $.ajax({
            url: requestUri,
            type: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            success: function (data) {

                // store the lookup items here
                var lookupItems = new Array();

                // loop through each of the returned items
                $.each(data.d.results, function (i, result) {
                    // for each one, create an object and specify the values
                    var lookupItem = { LookupId: result.Id, LookupValue: result.Title };
                    // push the object into our return array
                    lookupItems.push(lookupItem);
                });

                // pick appropriate render method
                if (formCtx.fieldSchema.FieldType == "LookupMulti") {
                    jslinkTemplates.Lookups.renderCheckboxes(formCtx.fieldValue, formCtx.fieldName, lookupItems);
                }
                else {
                    jslinkTemplates.Lookups.renderDropdown(formCtx.fieldValue, formCtx.fieldName, lookupItems, formCtx.fieldSchema.Required);
                }
            },
            error: function () {
                var divId = '#' + formCtx.fieldSchema.Name + 'LookupValues';

                $(divId).html('Error: Failed to get Lookup Items');
            }
        });
    });
}
jslinkTemplates.Lookups.renderCheckboxes = function (fieldValue, fieldName, items) {
    $(document).ready(function () {
            var divId = '#' + fieldName + 'LookupValues';

            try {
                $(divId).html("Loading ...");
            } catch (error) {
            }

            if (items.length == 0) {
                $(divId).html("No items for this filter");
                $("#" + fieldName + "checkboxControls").remove();
                return;
            }

            $(divId).html('');

            // loop through each of the returned items
            for (i = 0; i < items.length; i++) {

                // construct the choice text and "value" (as a valid <ID>;#<Text> lookup string)
                var choiceText = items[i].LookupValue;
                var choiceValue = items[i].LookupId + ';#' + items[i].LookupValue;

                var inputHtml = "<input type='checkbox' name='" + fieldName + "' value='" + choiceValue + "' style='float: left;'";

                // if the current field value contains the checkbox's value
                // then we need to make it checked
                if (fieldValue && fieldValue.indexOf(choiceValue) != -1) {
                    // if the item contains the current checkbox value then check it
                    inputHtml += " checked ";
                }

                inputHtml += "/><span style='display: block; padding-left: 25px; min-height: 20px;'>" + choiceText + "</span>";

                var label = $(document.createElement('label'))
                    .attr('style', 'display:block;');
                label.html(inputHtml);

                // add the checkbox to the div
                label.appendTo(divId);
            }

            if (!$("#" + fieldName + "checkboxControls")[0]) {
                // provide "check all" and "clear all" links
                // if you don't want these you can remove them
                var followingHtml = "<p id='" + fieldName + "checkboxControls'><a href='#' onclick='jslinkTemplates.Lookups.checkAll(\"" + fieldName + "\")'>Check All</a>";
                followingHtml += " | <a href='#' onclick='jslinkTemplates.Lookups.clearAll(\"" + fieldName + "\")'>Uncheck All</a></p>";
                $(divId).after(followingHtml);
            }
       
    });
}
jslinkTemplates.Lookups.renderDropdown = function (fieldValue, fieldName, items, required) {
    $(document).ready(function () {
            // check for querystring value
            var queryStringId = jslinkTemplates.Lookups.getQuerystring(fieldName);

            var divId = '#' + fieldName + 'LookupValues';

            try {
                $(divId).html("Loading ...");
            } catch (error) {
            }

            if (items.length == 0) {
                $(divId).html("No items for this filter");
                return;
            }

            var dropdownHtml = "<select";
            if (queryStringId != "") {
                dropdownHtml += " disabled='true' title='You cannot change this field'";
            }
            dropdownHtml += ">";

            if (!required) {
                dropdownHtml += "<option value=''>(None)</option>";
            }

            // loop through each of the returned items
            for (i = 0; i < items.length; i++) {

                // construct the choice text and "value" (as a valid <ID>;#<Text> lookup string)
                var choiceText = items[i].LookupValue;
                var choiceValue = items[i].LookupId + ';#' + items[i].LookupValue;

                dropdownHtml += "<option value='" + choiceValue + "'";

                if (items[i].LookupId == queryStringId) {
                    dropdownHtml += " selected='true'";
                }
                else if (fieldValue == choiceValue) {
                    // add "selected" attribute
                    dropdownHtml += " selected='true' ";
                }

                dropdownHtml += ">" + choiceText + "</option>";
            }

            dropdownHtml += "</select>";

            $(divId).html(dropdownHtml);
    });
};

/* GENERIC LOOKUPS 
   =============== */
jslinkTemplates.Lookups.Generic.SingleItem.editForm = function (formContext) {

    // check for querystring value
    var queryStringId = jslinkTemplates.Lookups.getQuerystring(formContext.CurrentFieldSchema.Name);

    // we need this if we use the looking as a cascading drop-down anywhere
    jslinkTemplates.Lookups.registerCallBack(formContext);

    var returnHtml = "<div id='" + formContext.CurrentFieldSchema.Name + "LookupValues'>";
    
    returnHtml += "<select";
    if (queryStringId != "") {
        returnHtml += " disabled='true' title='You cannot change this field'";
    }
    returnHtml += ">";

    if (!formContext.CurrentFieldSchema.Required) {
        returnHtml += "<option value=''";
        
        if(formContext.CurrentFieldValue == "")
        {
            returnHtml += " selected='true'";
        }

        returnHtml += ">(None)</option>";
    }

    var choices = formContext.CurrentFieldSchema.Choices;

    for (var i = 0; i < choices.length; i++) {
        var lookupString = choices[i].LookupId + ";#" + choices[i].LookupValue;
        returnHtml += "<option value='" + lookupString + "'";

        if (choices[i].LookupId == queryStringId) {
            returnHtml += " selected='true'";
        }
        else if (formContext.CurrentFieldValue == lookupString) {
            returnHtml += " selected='true'";
        }

        returnHtml += ">" + choices[i].LookupValue + "</option>";
    }

    returnHtml += "</select></div>";

    return returnHtml;
};
jslinkTemplates.Lookups.Generic.displayForm = function (formContext) {
    if (formContext.CurrentFieldValue != "") {
        var returnHtml = "<ul style='margin-top: 0px; margin-bottom: 0px;'>";

        var values = formContext.CurrentItem[formContext.CurrentFieldSchema.Name].toString();
        var choices = formContext.CurrentFieldSchema.Choices;

        for (var i = 0; i < choices.length; i++) {
            if (values.indexOf(choices[i].LookupValue) != -1) {
                returnHtml += "<li>" + choices[i].LookupValue + "</li>";
            }
        }

        returnHtml += "</ul>";

        return returnHtml;
    }
    else {
        return "";
    }
};
jslinkTemplates.Lookups.Generic.view = function (viewContext) {
    // returns all items as an unordered list

    // get the values (it will be in the format of an array)
    var values = viewContext.CurrentItem[viewContext.CurrentFieldSchema.Name];

    if (values.length > 1) {

        var returnHtml = "<ul style='margin-top: 0px; margin-bottom: 0px;'>";

        for (var i = 0; i < values.length; i++) {
            // add a new LI element for each value
            returnHtml += "<li>" + values[i].lookupValue + "</li>";
        }

        returnHtml += "</ul>";

        return returnHtml;
    }
    else if (values.length == 1) {
        return "<span>" + values[0].lookupValue + "</span>";
    }
    else {
        return "";
    }
};

/*  GENERIC CHECKBOX LOOKUP
    ======================= */
// scrollbar-contained box of checkboxes
jslinkTemplates.Lookups.CheckBoxes.editForm = function (formContext) {
    // create a FormContext object for easy extraction of field information    
    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(formContext);

    // register callback functions with SharePoint
    jslinkTemplates.Lookups.registerCallBack(formContext);

    // start constructing the HTML
    var returnHtml = "";

    // load in the available values from the Lookup field
    // note - another way would be to use the CSOM and
    // load them from a query .. but this approach seems to work well 
    // for small lists
    var choices = formContext.CurrentFieldSchema.Choices;

    jslinkTemplates.Lookups.renderCheckboxes(formContext.CurrentFieldValue, formCtx.fieldName, choices);

    // opening div, with max-height and scrollbars
    // we use the fieldName in the ID so we can refer to these
    // checkboxes explicitly in the DOM
    return "<div id='" + formCtx.fieldName + "LookupValues' style='max-height: 150px; overflow: auto;'></div>";
};

/* FILTERED LOOKUP 
   =============== 
   either renders a drop-down (single-select) or
   checkboxes (multi-select) which is automatically 
*/
jslinkTemplates.Lookups.Filtered.editForm = function (conditionalFieldName, formContext) {
    
    jslinkTemplates.Lookups.registerCallBack(formContext);

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(formContext);

    var returnHtml = "";

    // load the items from a REST call
    // this is THEME filtered so we use the SCTheme method
    jslinkTemplates.Lookups.renderFilteredLookup(formCtx, conditionalFieldName, formContext.CurrentItem[conditionalFieldName]);

    // opening div, with max-height and scrollbars
    // we use the fieldName in the ID so we can refer to these
    // checkboxes explicitly in the DOM
    return "<div id='" + formCtx.fieldName + "LookupValues' style='max-height: 150px; overflow: auto;'></div>";
};

jslinkTemplates.Lookups.getQuerystring = function (key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
};