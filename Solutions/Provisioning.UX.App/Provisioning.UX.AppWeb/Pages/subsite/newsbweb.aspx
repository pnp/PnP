<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newsbweb.aspx.cs" Inherits="Provisioning.UX.AppWeb.Pages.SubSite.newsbweb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../../Styles/site.css" rel="stylesheet" type="text/css" />
     <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>      
     <script src="../../Scripts/chromeloader.js?rev=1" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>   
        <div class="page">
            <script type="text/javascript">
                $(function () {
                    $('#cancel_button').click(function () {
                        window.location = $('#Url').val();
                    });
                });
            </script>
        <asp:ScriptManager ID="scriptManager" runat="server" EnableCdn="True" />
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="mainPanel" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Working on it...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress> 
        <asp:UpdatePanel ID="mainPanel" runat="server">
            <ContentTemplate>
                    <fieldset>
                    <legend>New Site</legend>
                    <table id="SiteInfoTable" width="100%">
                        <tbody>
                            <tr>
                                <!-- Title and Description -->
                                <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Title and Description</p>
                                    </div>
                                </div>
                                </td>
                                <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Title:</p>
                                    <input id="siteTitle" type="text" name="txtTitle" runat="server" required/>
                                    <br />
                                    <p>Description:</p>
                                    <textarea id="siteDescription" rows="5" cols="35" title="Description" name="Description" runat="server"></textarea>
                                </div>
                                </td>
                                </tr>
                            <tr>
                                <!-- Web Site Address -->
                                <td valign="top">
                                    <div class="editor-label">
                                        <div class="O15_editor_label_head">
                                            <p>
                                                Web Site Address</p>
                                        </div>
                                    </div>
                                </td>
                                <td class="right-column" valign="top">
                                    <div class="editor-field O15_editor_field_head">
                                        <p>URL name:</p>
                                        <div>
                                            <table>
                                                <tr>
                                                    <td><label id="labelHostURL" runat="server"></label></td>
                                                    <td><input id="txtSiteUrl" type="text" name="txtTitle" runat="server" required/></td>
                                                </tr>
                                            </table>
                                        </div>
                                        <br />
                                    </div>
                                </td>
                            <tr>
                                <!-- Template -->
                                <td valign="top">
                                    <div class="editor-label">
                                        <div class="O15_editor_label_head">
                                            <p>Template Selection</p>
                                        </div>
                                    </div>
                                </td>
                                   <td class="right-column" valign="top">
                                    <div class="editor-field O15_editor_field_head">
                                        <p>Site Template:</p>
                                        <select id="selectSiteTeamplate" runat="server" name="SiteTemplateType" title="Site Template">
                                        </select>
                                    </div>
                                </td>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <input id="Url" name="Url" type="hidden" value="" runat="server"/>
                    <p style="float: right">
                        <asp:Button runat="server" ID="create_button" OnClick="Submit_Click" Text="Create" />
                        <input type="button" id="cancel_button" value="Cancel" />
                    </p>
                    <div class="clear"></div>
                    </fieldset>
                </ContentTemplate>
        </asp:UpdatePanel>
        </div>
    </form>
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
