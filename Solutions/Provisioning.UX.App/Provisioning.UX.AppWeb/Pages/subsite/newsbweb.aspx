<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newsbweb.aspx.cs" Inherits="Provisioning.UX.AppWeb.Pages.SubSite.newsbweb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sub Site Creation</title>
     <link href="../../Styles/site.css" rel="stylesheet" type="text/css" />
     <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>      
     <script src="../../Scripts/chromeloader.js?rev=1" type="text/javascript"></script>
</head>
<body style="display: none; overflow: auto;">
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
        <asp:UpdatePanel ID="mainPanel" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                    <fieldset>
                    <legend>New SharePoint Site</legend>
                    <br />
                    <br />
                    <table id="SiteInfoTable" width="15%">
                        <tbody>                           
                            <tr>
                                <!-- Title and Description -->
                                <td class="ms-sectionheader" style="padding-top: 4px;" height="22"  valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">
                                        Title and Description
                                    </h3>                                
                                </td>
                           </tr>
                            <tr>                                
                                <td valign="top" style="padding-left: 150px;">
                                
                                    <h3 class="ms-standardheader ms-inputformheader">Title:</h3>
                                    <div class="ms-input">
                                        <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-fullWidth" onkeyup="javascript:txtTitleChanged();"></asp:TextBox>
                                    </div>
                                    <br />
                                    <h3 class="ms-standardheader ms-inputformheader">Description:</h3>
                                    <div class="ms-input">
                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="ms-fullWidth" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                <br />
                                <br />
                                </td>
                             </tr>
                            <tr>
                                <!-- Web Site Address -->
                                <td class="ms-sectionheader" style="padding-top: 4px;" height="22"  valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">
                                        Web Site Address
                                    </h3>                                
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 150px;" valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">URL name:</h3>
                                        <div style="float: left; white-space: nowrap; padding-bottom: 10px; padding-left: 15px; width: 450px;">
                                            <div style="width: 320px; font-size: 13px; float: left; padding-top: 2px;" id="divBasePath">
                                               <asp:Label ID="lblBasePath" runat="server"></asp:Label>
                                            </div>
                                            <div class="ms-input" style="width: 130px; float: left;">
                                                <asp:TextBox ID="txtUrl" runat="server" CssClass="ms-fullWidth"></asp:TextBox>
                                            </div>
                                       </div>
                                        <br />
                                    </div>
                                <br />
                                <br />
                                </td>
                            </tr>
                            <tr>
                                <!-- Template -->
                                <td class="ms-sectionheader" style="padding-top: 4px;" height="22"  valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">
                                        Template Selection
                                    </h3>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 150px;" valign="top">
                                    <h3 class="ms-standardheader ms-inputformheader">Select Template:</h3>
                                    <div class="ms-input" style="padding-left: 15px;">
                                        <asp:ListBox ID="listSites" runat="server" CssClass="ms-fullWidth"></asp:ListBox>                                 
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="Url" name="Url" type="hidden" value="" runat="server"/>
                        <br />
                        <br />
                    <div id="divButtons"  style="float: right;">
                        <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" />
                    </div>
                    <div class="clear"></div>
                                </td>
                            </tr>
                    </tbody>                  

                    </table>
                   
                    </fieldset>
                </ContentTemplate>
        </asp:UpdatePanel>
        </div>
    </form>
   <div id="MicrosoftOnlineRequired">
        <div style="float:left">
            <img style="position:relative;top:4px;"  src="../../images/MicrosoftLogo.png" alt="©2015 Microsoft Corporation"/>
            <span id="copyright">©2015 Contoso Corporation</span>&nbsp;&nbsp;&nbsp;
            <a id="legalUrl" href="https://yoururl/license" target="_blank">Legal</a> |
            <a id="privacyUrl" href="https://yoururl/site/legal/privacy" target="_blank">Privacy</a>
        </div>
        <div style="float:right">
            <a id="supportUrl" href="https://yoururl/" target="_blank">Community</a> |
            <a id="feedbackUrl" href="https://yoururl" target="_blank">Feedback</a>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>
