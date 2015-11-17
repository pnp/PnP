<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ECM.DocumentLibrariesWeb.Default" %>

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
    <script type="text/javascript">
        function fn_init() {
            $("#form").validate({
                rules: {
                    '<%=LibraryName.UniqueID %>': {
                        required: true
                    }
                }, messages: {}
            });
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(onEachRequest);
        }

        function onEachRequest(sender, args) {
            if ($("#form").valid() == false) {
                args.set_cancel(true);
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
    <script type="text/javascript">
        $(function () {
            $('#cancel_button').click(function () {
                window.location = $('#Url').val();
            });
        });
    </script>
    <form id="form" runat="server" method="post">
        <!-- Chrome control placeholder -->
        <div id="chrome_ctrl_placeholder"></div>
        <div class="page">
            <asp:ScriptManager ID="scriptManager" runat="server" EnableCdn="True" />
            <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="mainPanel" DynamicLayout="true">
                <ProgressTemplate>
                    <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                        <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                            <img alt="Working on it" src="/Content/img/spinningwheel.gif" style="width: 32px; height: 32px;" />
                            <span class="ms-accentText" style="font-size: 18px;">Please wait while your changes are processed....</span>
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel ID="mainPanel" runat="server">
                <ContentTemplate>
                    <fieldset>
                    <legend>New Document Library</legend>
                    <table id="DocumentLibraryTable" width="100%">
                        <tbody>
                            <tr>
                                <!-- Name and Description -->
                                <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Name and Description</p>
                                    </div>
                                    <div class='O15_editor_label_body'>
                                        <p>Type a new name as you want it to appear in headings and links throughout the site. Type descriptive text that will help site visitors use this document library.</p>
                                    </div>
                                </div>
                                </td>
                                <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Name:</p>
                                    <input id="LibraryName" type="text" name="DocumentLibraryName" runat="server" required/>
                                    <br />
                                    <p>Description:</p>
                                    <textarea id="LibraryDescription" rows="5" cols="35" title="Description" name="Description" runat="server"></textarea>
                                </div>
                                </td>
                                </tr>
                            <tr>
                                <!-- Verision History -->
                                <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Document Version History</p>
                                    </div>
                                    <div class='O15_editor_label_body'>
                                        <p>Specify whether a version is created each time you edit a file in this document library.</p>
                                    </div>
                                </div>
                                </td>
                                <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Create a version each time you edit a file in this document library?</p>
                                    <input id="onetidVersioningEnabledYes" type="radio" value="TRUE" name="VersioningEnabled" runat="server" title="Create a version each time you edit a file in this document library: Yes"/>
                                    <label for="onetidVersioningEnabledYes">Yes</label>
                                    <input id="onetidVersioningEnabledNo" type="radio" value="FALSE" checked="checked" name="VersioningEnabled" title="Create a version each time you edit a file in this document library: No" />
                                    <label for="onetidVersioningEnabledNo">No</label>
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <!-- Document Library -->
                                <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Document Template</p>
                                    </div>
                                    <div class='O15_editor_label_body'>
                                        <p>Select a document template to determine the default for all new files created in this document library.	</p>
                                    </div>
                                </div>
                                </td>
                                <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Document Template:</p>
                                    <select name="DocumentTemplateType" id="DocumentTemplateType" title="Document Template" runat="server">
                                    </select>
                                </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <input id="Url" name="Url" type="hidden" value="" runat="server"/>
                    <p style="float: right">
                        <asp:Button runat="server" ID="create_button" OnClick="CreateLibrary_Click" Text="Create" />
                        <input type="button" id="cancel_button" value="Cancel" />
                    </p>
                    <div class="clear"></div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>  
    </form>
    <script type="text/javascript">
        $("#form").validate();
    </script>
    <script type="text/javascript">
        function pageLoad() {
            fn_init();
        }
    </script>
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