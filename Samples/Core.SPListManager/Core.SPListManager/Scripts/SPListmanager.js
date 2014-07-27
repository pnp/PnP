/* jshint jquery: true */
/* global SP */

'use strict';

var HTMLH3OpenElement = "<h3 class='ms-dlgTitleText ms-accentText ms-dlg-heading'>";
var HTMLH3CloseElement = "</h3>";
var HTMLTableOpenElement = "<table class='detailstable' cellpadding=0 cellspacing=0";
var HTMLTROpenElement = "<tr>";
var HTMLTDOpenElement = "<td style='width:50%;'>";
var HTMLTDCloseElement = "</td>";
var HTMLTRCloseElement = "</tr>";
var HTMLTableCloseElment = "</table>";

var SPPRINCIPALTYPE = "PrincipalType";
var PrincipalTypes = new Array();
PrincipalTypes[0] = "None";
PrincipalTypes[1] = "User";
PrincipalTypes[2] = "DistributionList";
PrincipalTypes[4] = "SecurityGroup";
PrincipalTypes[8] = "SharePointGroup";
PrincipalTypes[15] = "All";

var SPListmanager = {
    context: "",
    user: "",
    hostweburl: "",
    appweburl: "",
    scriptbase: "",
    GetQueryStringParameter: function (param) {
        var params = document.URL.split("?")[1].split("&");
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == param) {
                return singleParam[1];
            }
        }
    },
    logToSharePoint : function (message) {
        console.info('logToSharePoint called with message '+message);
        SP.Utilities.Utility.logCustomAppError(SPListmanager.context, message);
        SPListmanager.context.executeQueryAsync();
    }
};

SPListmanager.Default = {
    lists: "",
    Translate: function () {
        $("#SPListmanagerDefaultTitle").text(SPListmanager.Default.Resources.Title);
        $("#SPListmanagerDefaultDiagnostics").text(SPListmanager.Default.Resources.Diagnostics);
        $("#SPListmanagerDefaultCreateNewList").attr("Title", SPListmanager.Default.Resources.CreateNewList);
        $("#appadd").attr("Title", SPListmanager.Default.Resources.CreateNewList);
        $("#SPListmanagerDefaultCreateNewList").text(SPListmanager.Default.Resources.CreateNewList);
    },
    init: function () {
        var executor = new SP.RequestExecutor(SPListmanager.appweburl);
        executor.executeAsync(
            {
                url: SPListmanager.appweburl + "/_api/SP.AppContextSite(@target)/web/lists?@target='" + SPListmanager.hostweburl + "'",
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: SPListmanager.Default.initSuccessHandler,
                error: function (data) {
                    alert(jQuery.parseJSON(data.body).error.message.value); 
                }
            }
        );
    },
    initSuccessHandler: function ($result) {
        var data = jQuery.parseJSON($result.body);
        SPListmanager.Default.lists = data.d.results;

        var results = new Array();

        for (var x = 0; x < data.d.results.length; x++) {
            var rawlist = SPListmanager.Default.lists[x];
            var list = new Object();
            list.Title = rawlist.Title;
            list.ImageUrl = rawlist.ImageUrl;
            if (list.ImageUrl.indexOf("users") == -1) {
                list.ImageUrl = list.ImageUrl.replace("/images/it", "/images/lt").replace(".gif", ".png"); 
            }
            list.InstanceID = x;
            list.ItemCount = rawlist.ItemCount;
            list.LastItemModifiedDate = rawlist.LastItemModifiedDate;
            results.push(list);
        }

        var template = $.templates("#tmpList");
        template.link("#results", results);
    },
    ShowListDetails: function (id) {
        var list = SPListmanager.Default.lists[id];

        SP.SOD.executeFunc("sp.ui.dialog.js", "SP.UI.ModalDialog.showModalDialog", function () {
            var options = SP.UI.$create_DialogOptions();
            options.title = list.Title;
            options.width = 1200;
            options.height = 1000;
            options.url = SPListmanager.appweburl + "/Pages/ListDetails.aspx?SPHostUrl=" + SPListmanager.hostweburl + "&SPAppWebUrl=" + SPListmanager.appweburl + "&List=" + list.Id;
            SP.UI.ModalDialog.showModalDialog(options);
        });
    }
};

SPListmanager.ListDetails = {
    listID: "",
    init: function () {
        console.info('initiate listdetails.init for ' + SPListmanager.appweburl);

        SPListmanager.ListDetails.displayListDetails();
        //SPListmanager.ListDetails.displayRoleAssignments();
        SPListmanager.ListDetails.displayContentTypes();
        SPListmanager.ListDetails.displayDefaultView();
        SPListmanager.ListDetails.displayEventReceivers();
        SPListmanager.ListDetails.displayFields();
        SPListmanager.ListDetails.displayForms();
        SPListmanager.ListDetails.displayInformationRightsManagementSettings();
        SPListmanager.ListDetails.displayItems();
        SPListmanager.ListDetails.displayParentWeb();
        SPListmanager.ListDetails.displayRootFolder();
        SPListmanager.ListDetails.displayUserCustomActions();
        SPListmanager.ListDetails.displayViews();
        SPListmanager.ListDetails.displayWorkflowAssociations();
    },
    displayListDetails: function () {
        console.info('initiate listdetails.DisplayListDetails for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-1";
        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        var serviceUrl = SPListmanager.appweburl + "/_api/SP.AppContextSite(@target)/web/lists(guid'" + SPListmanager.ListDetails.listID + "')?@target='" + SPListmanager.hostweburl + "'";
        console.info('execute async call to ' + serviceUrl);

        var executor = new SP.RequestExecutor(SPListmanager.appweburl);
        executor.executeAsync(
            {
                url: serviceUrl,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: function (data) {
                    console.info('success method called for ' + data.body);

                    var prop = jQuery.parseJSON(data.body).d;
                    var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);
                    var html = SPListmanager.ListDetails.buildPropertyTable(listproperties);

                    $(PLACEHOLDER).html(html);
                },
                error: function (data) {
                    SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
                }
            }
        );

    },
    displayRoleAssignments: function () {
        console.info('initiate listdetails.DisplayRoleAssignments for ' + SPListmanager.appweburl);
        var method = "RoleAssignments";
        var PLACEHOLDER = "#tabs-2";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            method,
            function (data) {

                var collection = new Array();
                var prop = jQuery.parseJSON(data.body).d;
                for (var result in prop.results) {

                    var member = prop.results[result].Member;
                    if (member !== undefined) {
                        //roleassignments
                        var deferreduri = prop.results[result].Member.__deferred.uri.split(method)[1];
                        collection.push(deferreduri);
                        for (var x in collection) {
                            var uri = collection[x];
                            console.info(method + "uri found: " + uri);

                            var methodserviceUrl = SPListmanager.appweburl + "/_api/SP.AppContextSite(@target)/web/lists(guid'" + SPListmanager.ListDetails + "')/" + method + "/" + uri + "?@target='" + SPListmanager.hostweburl + "'";
                            console.info('execute async call to retrieve Role Assignments on ' + methodserviceUrl);

                            var executor = new SP.RequestExecutor(SPListmanager.appweburl);
                            executor.executeAsync(
                            {
                                url: methodserviceUrl,
                                method: "GET",
                                headers: { "Accept": "application/json; odata=verbose" },
                                success: function (data) {

                                    console.info('success method called for ' + data.body);

                                    var html = "";
                                    var prop = jQuery.parseJSON(data.body).d;
                                    var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);

                                    html += HTMLH3OpenElement + prop.Title + HTMLH3CloseElement; //H3
                                    html += HTMLTableOpenElement;    //TABLE
                                    for (var x in listproperties) {
                                        var property = listproperties[x];
                                        if (property.title == SPPRINCIPALTYPE) {
                                            property.value = PrincipalTypes[property.value];
                                        }
                                        html += HTMLTROpenElement;      //TR
                                        html += HTMLTDOpenElement + property.title + HTMLTDCloseElement;
                                        html += HTMLTDOpenElement + property.value + HTMLTDCloseElement;
                                        html += HTMLTRCloseElement;    //TR
                                    }
                                    html += HTMLTableCloseElment;  //TABLE

                                    //append html
                                    $(PLACEHOLDER).append(html);
                                    $(PLACEHOLDER).append("<p>&nbsp;</p>");

                                },
                                error: function (data) {
                                    SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
                                }
                            });
                        }
                    }
                }
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayContentTypes: function () {
        console.info('initiate listdetails.DisplayContentTypes for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-3";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "ContentTypes",
            function (data) {
                console.info('success method called for ' + data.body);

                $(PLACEHOLDER).html(""); //clear

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Name");

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayDefaultView: function () {
        console.info('initiate listdetails.displayDefaultView for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-4";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "DefaultView",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d;
                var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);
                var html = SPListmanager.ListDetails.buildViewPropertyTable(listproperties);

                $(PLACEHOLDER).html(html);

            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayEventReceivers: function () {
        console.info('initiate listdetails.displayEventReceivers for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-5";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "EventReceivers",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "ReceiverName");

                $(PLACEHOLDER).html(html);

            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayFields: function () {
        console.info('initiate listdetails.displayFields for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-6";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "Fields",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Title");

                $(PLACEHOLDER).append(html);
                $(PLACEHOLDER).append("<p>&nbsp;</p>");
            },
        function (data) {
            SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
        }
        );
    },
    displayForms: function () {
        console.info('initiate listdetails.displayForms for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-7";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "Forms",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "ServerRelativeUrl");
                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayInformationRightsManagementSettings: function () {
        console.info('initiate listdetails.displayInformationRightsManagementSettings for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-8";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "InformationRightsManagementSettings",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d;
                var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);
                var html = SPListmanager.ListDetails.buildPropertyTable(listproperties);

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayItems: function () {
        console.info('initiate listdetails.displayItems for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-9";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "Items",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Title", "Id");

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayParentWeb: function () {
        console.info('initiate listdetails.displayParentWeb for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-10";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "ParentWeb",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d;
                var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);
                var html = SPListmanager.ListDetails.buildPropertyTable(listproperties);

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayRootFolder: function () {
        console.info('initiate listdetails.displayRootFolder for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-11";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "RootFolder",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d;
                var listproperties = SPListmanager.ListDetails.parseResultToArray(prop);
                var html = SPListmanager.ListDetails.buildPropertyTable(listproperties);

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayUserCustomActions: function () {
        console.info('initiate listdetails.displayUserCustomActions for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-12";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "UserCustomActions",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Title", "Id");

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayViews: function () {
        console.info('initiate listdetails.displayViews for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-13";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "Views",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Title", "Name");

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    displayWorkflowAssociations: function () {
        console.info('initiate listdetails.displayWorkflowAssociations for ' + SPListmanager.appweburl);

        var PLACEHOLDER = "#tabs-14";

        //clear
        $(PLACEHOLDER).html("");
        console.info(PLACEHOLDER + " html is cleared");

        // display information
        SPListmanager.ListDetails.initiateMethodDetails(
            SPListmanager.appweburl,
            "WorkflowAssociations",
            function (data) {
                console.info('success method called for ' + data.body);

                var prop = jQuery.parseJSON(data.body).d.results;
                var html = SPListmanager.ListDetails.buildPropertiesTable(prop, "Title", "Name");

                $(PLACEHOLDER).html(html);
            },
            function (data) {
                SPListmanager.ListDetails.exceptionHandler(data, PLACEHOLDER);
            }
        );
    },
    initiateMethodDetails: function (appweburl, method, succesFunction, errorFunction) {

        console.info('initiate ' + method + ' for ' + SPListmanager.appweburl);

        var serviceUrl = SPListmanager.appweburl + "/_api/SP.AppContextSite(@target)/web/lists(guid'" + SPListmanager.ListDetails.listID + "')/" + method + "?@target='" + SPListmanager.hostweburl + "'";
        console.info('execute async call to ' + serviceUrl);

        var executor = new SP.RequestExecutor(appweburl);
        executor.executeAsync(
            {
                url: serviceUrl,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: succesFunction,
                error: errorFunction
            }
        );
    },
    parseResultToArray: function (object) {
        var listproperties = new Array();
        for (var att in object) {
            if (object[att] === null) continue;
            if (object[att].__deferred === undefined && att != "__metadata") {
                var property = new Object();
                property.title = att;
                property.value = SPListmanager.ListDetails.visualize(object[att]);
                listproperties.push(property);
            }
        }
        return listproperties;
    },
    visualize: function (data) {
        if (typeof data === "boolean") return data; // BOOLEAN
        if (typeof data === "number") return data; // NUMBER
        if (data.toString().indexOf("/_layouts/15/images") === 0) {
            return "<img src='" + data + "' /> (" + data + ")"; // IMAGE FROM LAYOUTS
        }
        return data; // FALLBACK
    },
    buildPropertiesTable: function (prop, TitlePropertyName, alternativePropertyName) {
        var html = "";
        if (prop.length === 0) {
            html = "No information available";
        }
        for (var index in prop) {
            var field = prop[index];

            var title = field[TitlePropertyName];
            if (title === undefined) {
                title = field[alternativePropertyName];
            }

            html += HTMLH3OpenElement + title + HTMLH3CloseElement; //H3
            html += HTMLTableOpenElement;    //TABLE

            var dataObject = SPListmanager.ListDetails.parseResultToArray(field);
            for (var x in dataObject) {
                var property = dataObject[x];
                html += HTMLTROpenElement;      //TR
                html += HTMLTDOpenElement + property.title + HTMLTDCloseElement;
                html += HTMLTDOpenElement + SPListmanager.ListDetails.visualizeContentTypePropertyValue(property) + HTMLTDCloseElement;
                html += HTMLTRCloseElement;    //TR
            }

            html += HTMLTableCloseElment;  //TABLE
            html += "<p>&nbsp;</p>";  //TABLE

        }

        return html;
    },
    buildViewPropertyTable: function (listproperties) {
        var html = HTMLTableOpenElement;
        for (var x in listproperties) {
            var property = listproperties[x];

            html += HTMLTROpenElement;      //TR
            html += HTMLTDOpenElement + property.title + HTMLTDCloseElement;
            html += HTMLTDOpenElement + SPListmanager.ListDetails.visualizeContentTypePropertyValue(property) + HTMLTDCloseElement;
            html += HTMLTRCloseElement;    //TR
        }
        html += HTMLTableCloseElment;  //TABLE

        return html;
    },
    buildPropertyTable: function (listproperties) {
        var html = HTMLTableOpenElement;
        for (var x in listproperties) {
            var property = listproperties[x];

            html += HTMLTROpenElement;      //TR
            html += HTMLTDOpenElement + property.title + HTMLTDCloseElement;
            html += HTMLTDOpenElement + property.value + HTMLTDCloseElement;
            html += HTMLTRCloseElement;    //TR
        }
        html += HTMLTableCloseElment;  //TABLE

        return html;
    },
    exceptionHandler: function (data, placeholderID) {
        if (typeof (console) !== "undefined") {
            if (data.body.length > 0) {
                console.error(jQuery.parseJSON(data.body).error.message.value);
            }
            else {
                console.error("error occured, no further details in passed along")
            }
        }
        $(placeholderID).append("An unexpected error occured, please contact your administator.");
    },
    visualizeContentTypePropertyValue: function (prop) {
        if (prop.title == "SchemaXml" || prop.title == "HtmlSchemaXml") {
            prop.value = "<div>" + SPListmanager.ListDetails.htmlEncode(prop.value) + "</div>";
            return prop.value;

        }
        if (prop.title == "Id") {
            prop.value = prop.value.StringValue;
            return prop.value;

        }
        if (prop.title == "ImageUrl") {
            return prop.value;
        }

        if (prop.title == "ListViewXml") {
            prop.value = "<div>" + SPListmanager.ListDetails.htmlEncode(prop.value) + "</div>";
            return prop.value;
        }

        if (typeof prop.value === "boolean") return prop.value; // BOOLEAN
        if (typeof prop.value === "number") return prop.value; // NUMBER

        if (prop.value.toString().indexOf("/_layouts/15/images") >= 0) return "<img src='" + prop.value + "' /> (" + prop.value + ")"; // IMAGE FROM LAYOUTS

        return prop.value; // FALLBACK
    },
    htmlEncode: function (value) {
        //create a in-memory div, set it's inner text(which jQuery automatically encodes)
        //then grab the encoded contents back out.  The div never exists on the page.
        return $('<div/>').text(value).html();
    },
    htmlDecode: function (value) {
        return $('<div/>').html(value).text();
    }
};

SPListmanager.NewList = {
    init: function () {
        console.info('init started');

        //provision create list logic
        $("#btnCreateList").click(function () {

            $(".s4-bodypadding :input").attr("disabled", true);

            console.info('btnCreateList clicked');

            // Get the new name of the list from the textbox
            var newListName = $("#txtNewListTitle").val();
            var description = $("#txtNewListDescription").val();
            var listTemplateType = $("#cmbNewListTemplate").val();

            console.info('btnCreateList action with params ' + newListName + "|" + description + "|" + listTemplateType);

            // get the context from the hostweb where the app is installed
            var hostwebContext = new SP.AppContextSite(SPListmanager.context, SPListmanager.hostweburl);
            var web = hostwebContext.get_web();

            // create the listinstance of the new SPList
            var listCreationInfo = new SP.ListCreationInformation();
            listCreationInfo.set_title(newListName); // list name
            listCreationInfo.set_description(description); // list description
            listCreationInfo.set_templateType(listTemplateType); //list type

            // add the listinstance to the lists on the parentweb
            var list = web.get_lists().add(listCreationInfo);
            SPListmanager.context.load(list);

            // execute the action
            SPListmanager.context.executeQueryAsync(onQuerySucceeded, onQueryFailed);
        });

        function onQuerySucceeded(sender, args) {
            document.location.href = SPListmanager.hostweburl + "/lists/" + $("#txtNewListTitle").val();
        }

        function onQueryFailed(sender, args) {
            $("#message").css('color', 'red').text("An unexpected error occured, please try again.");
            console.error("Unexpected error while creating new list: " + args.$2D_2);
        }
    },
    Translate: function () {
        $("#SPListmanagerNewListTitleAndDescription").text(SPListmanager.NewList.Resources.TitleAndDescription);
        $("#SPListmanagerNewListTitle").text(SPListmanager.NewList.Resources.Title);
        $("#SPListmanagerNewListDescription").text(SPListmanager.NewList.Resources.Description);
        $("#SPListmanagerNewListTemplate").text(SPListmanager.NewList.Resources.Template);
        $("#btnCreateList").val(SPListmanager.NewList.Resources.Create);
        $("#onetidClose").val(SPListmanager.NewList.Resources.Cancel);

        $("#cmbNewListTemplate_100").text(SPListmanager.NewList.Resources.cmbNewListTemplate_100);
        $("#cmbNewListTemplate_102").text(SPListmanager.NewList.Resources.cmbNewListTemplate_102);
        $("#cmbNewListTemplate_104").text(SPListmanager.NewList.Resources.cmbNewListTemplate_104);
        $("#cmbNewListTemplate_105").text(SPListmanager.NewList.Resources.cmbNewListTemplate_105);
        $("#cmbNewListTemplate_106").text(SPListmanager.NewList.Resources.cmbNewListTemplate_106);
        $("#cmbNewListTemplate_107").text(SPListmanager.NewList.Resources.cmbNewListTemplate_107);
        $("#cmbNewListTemplate_108").text(SPListmanager.NewList.Resources.cmbNewListTemplate_108);
        $("#cmbNewListTemplate_109").text(SPListmanager.NewList.Resources.cmbNewListTemplate_109);
        $("#cmbNewListTemplate_171").text(SPListmanager.NewList.Resources.cmbNewListTemplate_171);
    }
};
