var SPHostUrl;
var SPAppWebUrl;
var ready = false;

// this function is executed when the page has finished loading. It performs two tasks:
//    1. It extracts the parameters from the url
//    2. It loads the request executor script from the host web
$(document).ready(function () {
    var params = document.URL.split("?")[1].split("&");
    for (var i = 0; i < params.length; i = i + 1) {
        var param = params[i].split("=");
        switch (param[0]) {
            case "SPAppWebUrl":
                SPAppWebUrl = decodeURIComponent(param[1]);
                break;
            case "SPHostUrl":
                SPHostUrl = decodeURIComponent(param[1]);
                break;
        }
    }

    // load the executor script, once completed set the ready variable to true so that
    // we can easily identify if the script has been loaded
    $.getScript(SPHostUrl + "/_Layouts/15/SP.RequestExecutor.js", function (data) {
        ready = true;
        // Create a SharePoint list with the name that the user specifies.
        createList("Events", "events");
       
    });


});

function createList(listToCreate, typeOfList) {
    // Create a SharePoint list with the name that the user specifies.
    var currentcontext = new SP.ClientContext.get_current();
    var hostUrl = decodeURIComponent(SPHostUrl);
    var hostContext = new SP.AppContextSite(currentcontext, hostUrl);
    var hostweb = hostContext.get_web();
    var listCreationInfo = new SP.ListCreationInformation();

    listCreationInfo.set_title(listToCreate);

    if (typeOfList === "events") {
        listCreationInfo.set_templateType(SP.ListTemplateType.events);
    }
    else if (typeOfList === "contacts") {
        listCreationInfo.set_templateType(SP.ListTemplateType.contacts);
    }
    var lists = hostweb.get_lists();
    var newList = lists.add(listCreationInfo);
    currentcontext.load(newList);
    currentcontext.executeQueryAsync(onListCreationSuccess, onListCreationFail);
}


function onListCreationSuccess() {
    getItems();
}

function onListCreationFail(sender, args) {
    console.log("List already exists");
    getItems();
}


function getListItemFormUrl(listName, listItemId, formTypeId, complete, failure) {
    var url = SPAppWebUrl + "/_api/SP.AppContextSite(@target)" +
           "/web/lists/getbytitle('" + listName + "')/Forms?$select=ServerRelativeUrl&$filter=FormType eq " + formTypeId +
           "&@target='" + SPHostUrl + "'"

    // create  new executor passing it the url created previously
    var executor = new SP.RequestExecutor(SPAppWebUrl);

    // execute the request, this is similar although not the same as a standard AJAX request
    executor.executeAsync(
        {
            url: url,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                var results = JSON.parse(data.body);
                var urlresult = results.d.results[0].ServerRelativeUrl + '?ID=' + listItemId;
                complete(urlresult);
            },
            error: function (data) {
                failure(data);
            }
        });
}

// this function retrieves the items within a list which is contained within the parent web
function getItems() {

    // only execute this function if the script has been loaded
    if (ready) {

        // the name of the list to interact with
        var listName = "Events";

        // the url to use for the REST call.
        var url = SPAppWebUrl + "/_api/SP.AppContextSite(@target)" +

            // this is the location of the item in the parent web. This is the line
            // you would need to change to add filters, query the site etc
          //  "/web/lists/getbytitle('" + listName + "')/items?" +
            "/web/lists/getbytitle('" + listName + "')/items?$select=Title,Category,EventDate,Description,EncodedAbsUrl,ID" +
            "&@target='" + SPHostUrl + "'";

        // create  new executor passing it the url created previously
        var executor = new SP.RequestExecutor(SPAppWebUrl);

        // execute the request, this is similar although not the same as a standard AJAX request
        executor.executeAsync(
            {
                url: url,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: function (data) {

                    // parse the results into an object that you can use within javascript
                    var results = JSON.parse(data.body);
                    var events = [];
                    $.each(results.d.results, function (i, obj) {

                        //Usage
                        getListItemFormUrl('Events', obj.ID, 4,
                            function (url) {
                                var event = {
                                    date: Date.parse(obj.EventDate).toString(),
                                    type: obj.Category,
                                    title: obj.Title,
                                    description: obj.Description,
                                    url: SPHostUrl + url
                                }
                                events.push(event);
                            },
                            function (error) {
                                console.log(JSON.stringify(error));
                            })

                        //use obj.id and obj.name here, for example:


                    });
                    var myJsonString = JSON.stringify(events);

                    $("#eventCalendarInline").eventCalendar({
                        jsonData: events,
                        openEventInNewWindow: true,
                        showDescription: true,
                        txt_GoToEventUrl: "Go to event"
                    });

                    Communica.Part.init();

                },
                error: function (data) {

                    // an error occured, the details can be found in the data object.
                    alert("Ooops an error occured");
                }
            });
    }
}

window.Communica = window.Communica || {};

Communica.Part = {
    senderId: '',

    init: function () {
        var params = document.URL.split("?")[1].split("&");
        for (var i = 0; i < params.length; i = i + 1) {
            var param = params[i].split("=");
            if (param[0].toLowerCase() == "senderid")
                this.senderId = decodeURIComponent(param[1]);
        }


        this.adjustSize();
    },

    adjustSize: function () {
        var step = 30,
            newHeight,
            contentHeight = $('#eventCalendarInline').height(),
            resizeMessage = '<message senderId={Sender_ID}>resize({Width}, {Height})</message>';

        newHeight = (step - (contentHeight % step)) + contentHeight;

        resizeMessage = resizeMessage.replace("{Sender_ID}", this.senderId);
        resizeMessage = resizeMessage.replace("{Height}", newHeight);
        resizeMessage = resizeMessage.replace("{Width}", "100%");

        window.parent.postMessage(resizeMessage, "*");
    }
};