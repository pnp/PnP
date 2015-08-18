<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="accessdenied.aspx.cs" Inherits="ECM.DocumentLibrariesWeb.Pages.AccessDenied" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link href="/Content/css/Site.css" rel="stylesheet" type="text/css" />
    <title>New Library</title>
    <script src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>     
    <script src ="//ajax.aspnetcdn.com/ajax/jquery.validate/1.13.0/jquery.validate.js" type="text/javascript" ></script>
    <script src ="//ajax.aspnetcdn.com/ajax/jquery.validate/1.13.0/jquery.validate.min.js" type="text/javascript" ></script>
    <script src ="//ajax.aspnetcdn.com/ajax/jquery.validate/1.13.0/additional-methods.js" type="text/javascript" ></script>
    <script src ="//ajax.aspnetcdn.com/ajax/jquery.validate/1.13.0/additional-methods.min.js" type="text/javascript" ></script>
    <script src="ChromeLoader.js"type="text/javascript" ></script>
    <script src="/Scripts/common.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.tipsy.js" type="text/javascript"></script>
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
                "appIconUrl": imageBase + "siteicon.png",
                "appTitle": "New Document Library",
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
                                    "chrome_ctrl_placeholder",
                                    options
                                );
            nav.setVisible(true);
        }

        // Function to retrieve a query string value.
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

<!-- The body is initally hidden. 
     The onCssLoaded callback allows you to 
     display the content after the required
     resources for the chrome control have
     been loaded.  -->
<body style="display: none">
    <form id="form" runat="server" method="post">
        <!-- Chrome control placeholder -->
        <div id="chrome_ctrl_placeholder"></div>
        <div class="page">
            <p class="result_oops_line">
                Oops, You're unable to work on it. You dont have the necessary rights. Only Site Administrators have access to this page. Please contact your site owners.
            </p>
        </div>  
    </form>
    <div id="MicrosoftOnlineRequired">
        <div style="float:left">
            <img style="position:relative;top:4px;"  src="/Content/img/MicrosoftLogo.png" alt="©2014 Microsoft Corporation"/>
            <span id="copyright">©2014 Contoso Corporation</span>&nbsp;&nbsp;&nbsp;
            <a id="legalUrl" href="https://officeams.codeplex.com/license" target="_blank">Legal</a> |
            <a id="privacyUrl" href="https://www.codeplex.com/site/legal/privacy" target="_blank">Privacy</a>
        </div>
        <div style="float:right">
            <a id="supportUrl" href="https://officeams.codeplex.com/" target="_blank">Community</a> |
            <a id="feedbackUrl" href="https://officeams.codeplex.com/discussions" target="_blank">Feedback</a>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>