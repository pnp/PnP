<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="siteedit.aspx.cs" Inherits="Provisioning.UX.AppWeb.Pages.SiteClassification.SiteEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Edit Site Information</title>
    <link href="../../Styles/site.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>     
    <script src="../../Scripts/commonapp.js?rev=1" type="text/javascript"></script> 
    <script src="../../Scripts/vendor/jquery/jquery.tipsy.js" type="text/javascript"></script>
    <script src="../../Scripts/chromeloader.js?rev=1" type="text/javascript"></script>
</head>
<body>
   <div id="divSPChrome"></div>    
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
                    <img src="../../images/spinningwheel.gif" width='20' height='20' alt="" style="position:relative; top:5px; right:5px; " />
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
                   
                    <table id="SitePropertiesTable" width="100%">
                        <tbody>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Site Properties</p>
                                    </div>
                                    <div class="O15_editor_label_body">
                                        <p>Learn more about  <a href="http://INSERTYOURPOLICY" target="_blank">site classification</a>.</p>
                                        <p>All Sites must have the following Site metadata applied.</p>
                                    </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Current Site Division:</p>
                                    <b>&nbsp&nbsp&nbsp<asp:Label ID="lblDivision" runat="server" Visible="true"></asp:Label></b>
                                    <p>
                                        <span>Division</span>
                                    </p>
                                    <script type="text/javascript" charset="utf-8">
                                        $(document).ready(function () {
                                            $.ajax({
                                                url: "../../Scripts/data/json/divisions.json",
                                                data: {},
                                                dataType: "json",
                                                contentType: "application/json; charset=utf-8",
                                                success: function (data) {
                                                    var jsdata = data.divisions;
                                                    $.each(jsdata, function (key, value) {
                                                        $('#<%=selectDivision.ClientID%>').append($("<option></option>").val(value.key).html(value.value));
                                                    });
                                                },
                                                error: function (data) {
                                                    alert("../../Scripts/data/json/divisions.json not found!");
                                                }
                                            });
                                        });
                                    </script>
                                    <select id="selectDivision" runat="server" style="width: 230px; margin-left: 13px;"></select>
                                </div>
                               </td>
                            </tr>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>   </div>
                                    <div class="O15_editor_label_body">   </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Current Site Function:</p>
                                    <b>&nbsp&nbsp&nbsp<asp:Label ID="lblFunction" runat="server" Visible="true"></asp:Label></b>
                                    <p>
                                        <span>Function</span>
                                    </p>
                                    <script type="text/javascript" charset="utf-8">
                                        $(document).ready(function () {
                                            $.ajax({
                                                url: "../../Scripts/data/json/functions.json",
                                                data: {},
                                                dataType: "json",
                                                contentType: "application/json; charset=utf-8",
                                                success: function (data) {
                                                    var jsdata = data.functions;
                                                    $.each(jsdata, function (key, value) {
                                                        $('#<%=selectFunction.ClientID%>').append($("<option></option>").val(value.key).html(value.value));
                                                    });
                                                },
                                                error: function (data) {
                                                    alert("../../Scripts/data/json/functions.json not found!");
                                                }
                                            });
                                        });
                                    </script>
                                    <select id="selectFunction" runat="server" style="width: 230px; margin-left: 13px;"></select>
                                </div>
                               </td>
                            </tr>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>   </div>
                                    <div class="O15_editor_label_body">   </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Current Site Region:</p>
                                    <b>&nbsp&nbsp&nbsp<asp:Label ID="lblRegion" runat="server" Visible="true"></asp:Label></b>
                                    <p>
                                        <span>Region</span>
                                    </p>
                                    <script type="text/javascript" charset="utf-8">
                                        $(document).ready(function () {
                                            $.ajax({
                                                url: "../../Scripts/data/json/regions.json",
                                                data: {},
                                                dataType: "json",
                                                contentType: "application/json; charset=utf-8",
                                                success: function (data) {
                                                    var jsdata = data.regions;
                                                    $.each(jsdata, function (key, value) {
                                                        $('#<%=selectRegions.ClientID%>').append($("<option></option>").val(value.key).html(value.value));
                                                    });
                                                },
                                                error: function (data) {
                                                    alert("../../Scripts/data/json/regions.json not found!");
                                                }
                                            });
                                        });
                                    </script>
                                    <select id="selectRegions" runat="server" style="width: 230px; margin-left: 13px;"></select>
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
            <img style="position:relative;top:4px;"  src="../../images/MicrosoftLogo.png" alt="©2015 Microsoft Corporation"/>
            <span id="copyright">©2015 Contoso Corporation</span>&nbsp;&nbsp;&nbsp;
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
