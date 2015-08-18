<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Core.SiteClassificationWeb.Pages.Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
   <link href="/Content/css/site.css" rel="stylesheet" type="text/css" />
    <title>Edit Site Information</title>
    <script src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>      
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
            "appTitle": "Edit Site Information",
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
    <!-- Chrome control placeholder -->
    <div id="chrome_ctrl_placeholder"></div>
    <div class="page">
        <section id ="main">
            <script type="text/javascript">
                $(function () {
                    $('#edit_button').click(function () {
                        $.ajax({
                            success: function () {
                            }
                        });
                    });
                    $('#cancel_button').click(function () {
                        window.location = $('#Url').val();
                    });
                });
            </script>
            <div id="loading_dialog" title="Saving..." style="display:none;">
                <p>
                    <img src="/Content/img/spinningwheel.gif" width='20' height='20' alt="" style="position:relative; top:5px; right:5px; " />
                    <span style="color: #696969;">Please wait while your changes are processed. </span>
                </p>
             </div>
            <form id="form" runat="server">
                <fieldset>
                    <legend>Site Information</legend>
                    <script type="text/javascript">
                        $(function () {
                            $('input[name=AudienceScope]').click(function () {
                                var selected = $(this).val();
                                Hide('.AS_group');
                                Show('#AS_' + selected);
                            });
                            $('#TargetedAudienceTable').addClass('TargetedAudienceTable');
                        });

                        function RevertAudienceScope() {
                            Check('#AudienceScope_' + 'Team');
                            $('#AudienceScope_' + 'Team').click();
                        }
                    </script>
                   
                    <table id="AudienceScopeTable" width="100%">
                       <tbody>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Audience Scope</p>
                                    </div>
                                    <div class='O15_editor_label_body'><p>Which best describes the scope of your site’s reach?
                                        <div id='AS_Parent' class='sitetype_container_style'>
                                        <br/>
				                            <div id='AS_Enterprise' style='display:none' class='AS_group'>
				                            <b>Enterprise</b> - Target audience 40%+ of company.
				                            </div>
				                            <div id='AS_Organization' style='display:none' class='AS_group'>
				                                <b>Organization</b> - Target audience is as large as a division, but not as small as a team.
				                            </div>
				                            <div id='AS_Team' style='display:block' class='AS_group'>
				                                <b>Team</b> - Target audience is your workgroup or virtual team.
				                            </div>
				                            <div id='AS_Project' style='display:none' class='AS_group'>
				                                <b>Project</b> - Target Audience is project stakeholders with an end date.
				                            </div>
				                        </div>
                                       </p>
                                    </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Current Audience Reach:</p>
                                    <b>&nbsp&nbsp&nbsp<asp:Label ID="lblAudienceReach" runat="server"></asp:Label></b>
                                    <p>Change Audience Reach:</p>
                                     <table class="select-main">
                                         <tr>
                                             <td>
                                                 <label id="Enterprise_label" title="Target audience 40%+ of company.">
                                                    <input id="AudienceScope_Enterprise" name="AudienceScope" type="radio" value="Enterprise" runat="server"/>Enterprise
                                                 </label>
                                             </td>
                                         </tr>
                                         <tr>
                                             <td>
                                                 <label id="Organization_label" title="Target audience is as large as a division, but not as small as a team.">
                                                    <input id="AudienceScope_Organization" name="AudienceScope" type="radio" value="Organization" runat="server"/>Organization
                                                 </label>
                                             </td>
                                         </tr>
                                         <tr>
                                             <td>
                                                <label id="Team_label" title="Target audience is your workgroup or virtual team.">
                                                    <input id="AudienceScope_Team" name="AudienceScope" type="radio" value="Team" runat="server"/>Team
                                                </label>
                                             </td>
                                         </tr>
                                     </table>
                                     <span class="field-validation-valid" data-valmsg-for="AudienceScope" data-valmsg-replace="false">Please specify a value for Audience Reach!</span>       
                                </div>
                               </td>
                             </tr>
                       </tbody>
                    </table>

                    <table id="RiskClassificationTable" width="100%">
                        <tbody>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Security Classification</p>
                                    </div>
                                    <div class="O15_editor_label_body">
                                        <p>Learn more about  <a href="http://INSERTYOURPOLICY" target="_blank">protecting your documents</a>.</p><p>
                                            <div class="ms-status-yellow" id="pageStatusBar" aria-live="polite" aria-relevant="all" style="display: block;">
                                                All sites must have a security classification
                                             </div>
                                    </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Current Site Classification:</p>
                                    <b>&nbsp&nbsp&nbsp<asp:Label ID="lblSitePolicy" runat="server" Visible="true"></asp:Label></b>
                                    <p>
                                        <span>How sensitive is your site?</span>
                                    </p>
                                    <select name="BusinessImpact" id="BusinessImpact" runat="server" style="width: 230px; margin-left: 13px;" data-val-required="The BusinessImpact field is required." data-val-length-max="128" data-val-length="Security Classification must be a string with a maximum length of 128." data-val="true">
                                    </select>
                                </div>
                               </td>
                             </tr>
                       </tbody>
                    </table>
                    <table id="ExpirationDateTable" width="100%">
                        <tbody>
                        <tr>
                            <td valign="Top">
                                <div class="editor-label">
                                    <div class="O15_editor_label_head"><p>Expiration Date</p></div>
                                    <div class="O15_editor_label_body"><p>If your site reaches the expiration date in one month or less than one month, you may extend your site deletion.</p></div>
                                </div>
                            </td>
                            <td class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                     <p>Current Expiration Date:</p>
                                        <div class="ExpirationDateFilterClass" id="expirationDateOneYear" style="display: block;">
                                         &nbsp;&nbsp; <b><asp:Label ID="lblExpirationDate" runat="server" Visible="true"></asp:Label></b>
                                        </div>
                                 </div>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                    <input id="Url" name="Url" type="hidden" value="" runat="server"/>
                    <p style="float: right">
                        <asp:Button runat="server" ID="submit_button" Text="OK" OnClick="Submit_Click" />
                        <input type="button" id="cancel_button" value="Cancel" />
                    </p>
                    <div class="clear"></div>
                </fieldset>
            </form>
        </section>
    </div>  
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