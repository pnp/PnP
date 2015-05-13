<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.UX.AppWeb.Default" Async="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" data-ng-app="app">
<head>
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title></title>

    <!-- CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.css"/>
    <link rel="stylesheet" href="../styles/app.css" /> 
    <link rel="stylesheet" href="../styles/peoplepickercontrol.css" />    
    
    <!-- AngularJS -->
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular-messages.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular-animate.min.js"></script>    

    <!-- Vendor JS -->
    <script src="../scripts/vendor/jquery/jquery-2.1.3.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>
    <script src="../scripts/vendor/angular-ui/ui-bootstrap-tpls-0.12.1.min.js"></script>  
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>

    <!-- People Picker from PnP -->
    <script src="../scripts/peoplepickercontrol.js?rev=2" type="text/javascript"></script>   
    
    <script type="text/javascript">
        "use strict";

        var hostweburl;

        //load the SharePoint resources
        $(document).ready(function () {
            //Get the URI decoded URL.
            hostweburl =
                decodeURIComponent(getQueryStringParameter("SPHostUrl")

            );
            // The SharePoint js files URL are in the form:
            // web_url/_layouts/15/resource
            var scriptbase = hostweburl + "/_layouts/15/";

            // Load the js file and continue to the 
            //   success handler
            $.getScript(scriptbase + "SP.UI.Controls.js", renderChrome)
        });

        // Callback for the onCssLoaded event defined
        //  in the options object of the chrome control
        function chromeLoaded() {
            // When the page has loaded the required
            //  resources for the chrome control,
            //  display the page body.
            $("body").show();
        }

        //Function to prepare the options and render the control
        function renderChrome() {
            // The Help, Account and Contact pages receive the 
            //   same query string parameters as the main page
            var imageBase = hostweburl + "/_layouts/15/images/";

            var options = {
                "appHelpPageUrl": "Help.html?"
                    + document.URL.split("?")[1],
                // The onCssLoaded event allows you to 
                //  specify a callback to execute when the
                //  chrome resources have been loaded.
                "onCssLoaded": "chromeLoaded()",
                "settingsLinks": [
                    {
                        "linkUrl": "Account.html?"
                            + document.URL.split("?")[1],
                        "displayName": "Account settings"
                    },
                    {
                        "linkUrl": "Contact.html?"
                            + document.URL.split("?")[1],
                        "displayName": "Contact us"
                    }
                ]
            };

            var nav = new SP.UI.Controls.Navigation(
                                    "divSPChrome",
                                    options
                                );
            nav.setVisible(true);
        }

        // Function to retrieve a query string value
        // For production purposes you may want to use
        //  a library to handle the query string.
        function getQueryStringParameter(paramToRetrieve) {
            var params =
                document.URL.split("?")[1].split("&");
            var strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == paramToRetrieve)
                    return singleParam[1];
            }
        }
    </script>

</head>
<body>    
    <div id="divSPChrome"></div>            
        
    <!-- Include the Wizard View -->
    <div data-ng-include="'wizard.html'"></div>
    <!-- App JS -->
    <script src="../scripts/app.module.js"></script>    
    <script src="../scripts/wizard/modules/wizard.module.js"></script>    
    <script src="../scripts/wizard/services/utilservice.js"></script>
    <script src="../scripts/wizard/services/peoplepickerfactory.js"></script>
    <script src="../scripts/wizard/controllers/wizard.controller.js"></script>
    <script src="../scripts/wizard/services/siteQueryService.js"></script>    
    <script src="../scripts/wizard/directives/restrict.js"></script>    
    <script src="../scripts/wizard/directives/formDirectives.js"></script>    
    <script src="../scripts/wizard/modal/wizard.modal.controller.js"></script>
    <script src="../scripts/wizard/modal/siteowners.peoplepicker.controller.js"></script>    
    <script src="../scripts/data/data.module.js"></script>
    <script src="../scripts/data/templates.factory.js"></script>
    <script src="../scripts/data/metadata.factory.js"></script>
    <script src="../scripts/data/appsettings.factory.js"></script>
    <script src="../scripts/wizard/services/provisioningServices.js"></script>
    <script src="../scripts/app.js"></script>    
</body>
</html>
