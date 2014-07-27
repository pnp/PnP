$(document).ready(function () {
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    //Get the URI decoded SharePoint app web url from the SPAppWebUrl parameter.
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    //Get the isDialog from url parameters
    var isDialog = decodeURIComponent(getQueryStringParameter('IsDlg'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Create a Link element for the defaultcss.ashx resource
                    var linkElement = document.createElement('link');
                    linkElement.setAttribute('rel', 'stylesheet');
                    linkElement.setAttribute('href', layoutsRoot + 'defaultcss.ashx');

                    ////Add the linkElement as a child to the head section of the html
                    var headElement = document.getElementsByTagName('head');
                    headElement[0].appendChild(linkElement);

                    //Load the SP.UI.Controls.js file to render the App Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //Widgets require the cross domain library for communicating back to SharePoint
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        //Initialize Controls Runtime 
                        Office.Controls.Runtime.initialize({ sharePointHostUrl: spHostUrl, appWebUrl: appWebUrl });

                        //Create the people picker
                        var siteOwnerPeoplePicker = new Office.Controls.PeoplePicker(document.getElementById("peoplePickerSiteOwner"),
                            {
                                allowMultipleSelections: false,
                                placeholder: "Please choose an site owner",
                                onChange: handleSiteOwnerChange
                            });

                        //Create the listview
                        var listViewAppWeb = new Office.Controls.ListView(document.getElementById("listViewAppWeb"),
                            {
                                listUrl: appWebUrl + "/_api/web/lists/getbytitle('Announcements')"
                            });

                        var listViewHostWeb = new Office.Controls.ListView(document.getElementById("listViewHostWeb"),
                            {
                                listUrl: spHostUrl + "/_api/web/lists/getbytitle('Site Pages')"
                            });
                        
                        //Render the control
                        Office.Controls.Runtime.renderAll();

                        // a value has been preset or was persisted via a postback
                        if ($('#txtSiteOwner').val().length > 0) {
                            var siteOwnersToAdd = JSON.parse($('#txtSiteOwner').val());
                            for (var i = 0; i < siteOwnersToAdd.length; i++) {
                                siteOwnerPeoplePicker.add(siteOwnersToAdd[i]);
                            }
                        }

                        if ($('#txtBackupSiteOwners').val().length > 0) {
                            var backupOwnersToAdd = JSON.parse($('#txtBackupSiteOwners').val());
                            for (var i = 0; i < backupOwnersToAdd.length; i++) {
                                //Shows how to fetch a reference to a declaratively defined control
                                document.getElementById("peoplePickerBackupSiteOwners")._officeControl.add(backupOwnersToAdd[i]);
                            }
                        }
                    });
                });
        });
});

function handleSiteOwnerChange(args) {
    if (args.selectedItems.length > 0) {
        $('#txtSiteOwner').val(JSON.stringify(args.selectedItems));
    }
    else {
        $('#txtSiteOwner').val('');
    }
}

function handleSiteOwnerBackupChange(args) {
    if (args.selectedItems.length > 0) {
        $('#txtBackupSiteOwners').val(JSON.stringify(args.selectedItems));
    }
    else {
        $('#txtBackupSiteOwners').val('');
    }
}

// Callback for the onCssLoaded event defined 
//  in the options object of the chrome control 
function chromeLoaded() {
    // When the page has loaded the required 
    //  resources for the chrome control, 
    //  display the page body. 
    $("body").show();
}

//Function to prepare the options and render the control 
function renderSPChrome() {
    // The Help, Account and Contact pages receive the  
    //   same query string parameters as the main page 
    var options = {
        "appTitle": document.title,
        // The onCssLoaded event allows you to  
        //  specify a callback to execute when the 
        //  chrome resources have been loaded. 
        "onCssLoaded": "chromeLoaded()"
    };

    var nav = new SP.UI.Controls.Navigation("divSPChrome", options);
    nav.setVisible(true);
}

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}

