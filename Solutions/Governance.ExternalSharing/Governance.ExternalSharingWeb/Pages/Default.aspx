<%@ Page Title="External Sharing" Language="C#" MasterPageFile="~/contoso.office365.template/contoso.o365.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Governance.ExternalSharingWeb.Pages.Default" %>

<%@ MasterType VirtualPath="~/contoso.office365.template/contoso.o365.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <link rel="stylesheet" type="text/css" href="../Styles/app.contoso.css" />      
    <script type="text/javascript"src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" ></script>    
    <script type="text/javascript" src="../Scripts/app.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" AsyncPostBackTimeout="0"/>

    <asp:HiddenField ID="HiddenField_Init_ExternalSharing_Enabled" runat="server" />

    <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
        <ProgressTemplate>
            <div id="divWaitingPanel" style="z-index: 999; background: rgb(255, 255, 255); position: fixed; width: 100%; left:0px; bottom: 0px; top: 0px;">
                <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                    <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                    <span class="ms-accentText" style="font-size: 36px;">&nbsp; Working on it...</span>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    

    <div style="width: 100%; position: relative;">
        <div class="ms-textLarge" style="padding-bottom: 10px; max-width:100%">
            Control how users invite people outside your organization to access content.
            <a target="_blank" class="ms-hide">Learn more about external sharing</a> <br> <br>
        </div>

        <table border="0" cellspacing="0" cellpadding="0" width="100%">
            <!--separation line-->
            <tr>
                <td class="ms-sectionline" height="1" colspan="2">
                    <img src="../Styles/blank.gif?rev=41" width="1" height="1" alt="" data-accessibility-nocheck="true"/>
                </td>
            </tr>
            <!--site collection line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Site Collection
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="ms-descriptiontext ms-inputformdescription">
                                <!--Section description-->
                            </td>
                            <td><img src="../Styles/blank.gif?rev=41" width="8" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="19" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                    </table>
                </td>
                <td class="ms-authoringcontrols ms-inputformcontrols" valign="top" align="left">
                    <table border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                        <tr>
                            <td width="9px"><img src="../Styles/blank.gif?rev=41" width="9" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td width="10px"><img src="../Styles/blank.gif?rev=41" width="10" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="ms-authoringcontrols">
                                <table class="ms-authoringcontrols" border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                                    <tr id="">
                                        <td class="ms-authoringcontrols" colspan="2">
                                            <span></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="3" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                    <!-- End Right_Text -->
                                    <tr>
                                        <td width="11px"><img src="../Styles/blank.gif?rev=41" width="11" height="1" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                        <td class="ms-authoringcontrols" width="">
                                            <asp:Label runat="server" ID="lblSiteURL" CssClass="contoso-app-branding"></asp:Label>
                                            <br/>
                                            <span class="ms-formvalidation"></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="6" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                </table>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="13" alt="" data-accessibility-nocheck="true"/></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!--separation line-->
            <tr>
                <td class="ms-sectionline" height="1" colspan="2">
                    <img src="../Styles/blank.gif?rev=41" width="1" height="1" alt="" data-accessibility-nocheck="true"/>
                </td>
            </tr>
            <!--Change sharing option line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Change Sharing Option
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="ms-descriptiontext ms-inputformdescription">
                                <!--Section description-->
                            </td>
                            <td><img src="../Styles/blank.gif?rev=41" width="8" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="19" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                    </table>
                </td>
                <td class="ms-authoringcontrols ms-inputformcontrols" valign="top" align="left">
                    <table border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                        <tr>
                            <td width="9px"><img src="../Styles/blank.gif?rev=41" width="9" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td width="10px"><img src="../Styles/blank.gif?rev=41" width="10" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="ms-authoringcontrols">
                                <table class="ms-authoringcontrols" border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                                    <tr id="">
                                        <td class="ms-authoringcontrols" colspan="2">
                                            <span></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="3" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                    <!-- End Right_Text -->
                                    <tr>
                                        <td width="11px"><img src="../Styles/blank.gif?rev=41" width="11" height="1" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                        <td class="ms-authoringcontrols" width="">
                                            <asp:RadioButtonList runat="server" ID="rdbList" ClientIDMode="Static" CssClass="contoso-app-branding" onclick="GetRadioButtonListSelectedValue(this);">
                                                <asp:ListItem Text="Don't allow sharing outside your organization" Value="notallowed"></asp:ListItem>
                                                <asp:ListItem Text="Allow external users who accept sharing invitations and sign in as authenticated users" Value="allowed"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <br />
                                            <span id="disable_external_sharing_warning" class="ms-warning ms-hide">
                                                When you disable external sharing, existing external users will no longer be able to access content inside the site collection, and all external user permissions within the site collection will be permanently deleted. If you choose to re-enable it later, you may have to re-invite external users.
                                            </span>
                                            <br/>
                                            <span class="ms-formvalidation"></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="6" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                </table>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="13" alt="" data-accessibility-nocheck="true"/></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!--separation line-->
            <tr>
                <td class="ms-sectionline" height="1" colspan="2">
                    <img src="../Styles/blank.gif?rev=41" width="1" height="1" alt="" data-accessibility-nocheck="true"/>
                </td>
            </tr>
            <!--note line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Please Note
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="ms-descriptiontext ms-inputformdescription">
                                <!--Section description-->
                                	Site owners are responsible for reviewing site permissions for external sharing of content periodically, and revoking permissions in a timely manner 
                            </td>
                            <td><img src="../Styles/blank.gif?rev=41" width="8" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="19" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                    </table>
                </td>
                <td class="ms-authoringcontrols ms-inputformcontrols" valign="top" align="left">
                    <table border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                        <tr>
                            <td width="9px"><img src="../Styles/blank.gif?rev=41" width="9" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="7" alt="" data-accessibility-nocheck="true"/></td>
                            <td width="10px"><img src="../Styles/blank.gif?rev=41" width="10" height="1" alt="" data-accessibility-nocheck="true"/></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="ms-authoringcontrols">
                                <table class="ms-authoringcontrols" border="0" width="100%" cellspacing="0" cellpadding="0" summary="" role="presentation">
                                    <tr id="">
                                        <td class="ms-authoringcontrols" colspan="2">
                                            <span></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="3" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                    <!-- End Right_Text -->
                                    <tr>
                                        <td width="11px"><img src="../Styles/blank.gif?rev=41" width="11" height="1" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                        <td class="ms-authoringcontrols" width="">
                                            
                                            <br/>
                                            <span class="ms-formvalidation"></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><img src="../Styles/blank.gif?rev=41" width="1" height="6" style="display: block" alt="" data-accessibility-nocheck="true"/></td>
                                    </tr>
                                </table>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><img src="../Styles/blank.gif?rev=41" width="150" height="13" alt="" data-accessibility-nocheck="true"/></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td height="2px" class="ms-sectionline" colspan="2"><img src="../Styles/blank.gif?rev=41" width="1" height="1" alt="" data-accessibility-nocheck="true"/></td></tr>    
            <tr>
                <td colspan="2">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <colgroup>
                            <col width="99%"/>
                            <col width="1%"/>
                        </colgroup>
                        <tr>
                            <td>&nbsp;</td>
                            <td nowrap="nowrap">
                                <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <asp:Button ID="btnSave" runat="server" ClientIDMode="Static" Text="OK" CssClass="ms-ButtonHeightWidth" OnClick="btnSave_Click" UseSubmitBehavior="false"/>                 
                                        <asp:Button ID="btnCancel" runat="server" ClientIDMode="Static" Text="Cancel" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" UseSubmitBehavior="false"/>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td class="ms-descriptiontext" colspan="2"><span id="spanErrorMsg" style="display: none;" class="confirm-error-message">Sorry something went wrong, please try again.  If this problem persists, please contact <a href='https://support.microsoft.com' target='_blank'> IT Support</a></span></td></tr>
            <tr><td height="10" class="ms-descriptiontext" colspan="2"><img src="../Styles/blank.gif?rev=41" width="1" height="10" alt="" data-accessibility-nocheck="true"/></td></tr>
            <tr><td height="40" class="ms-descriptiontext s4-notdlg" colspan="2"><img src="../Styles/blank.gif?rev=41" width="1" height="40" alt="" data-accessibility-nocheck="true"/></td></tr>
        </table>

        <br />

        <div class="contoso-app-version" style="margin-top: 15px !important;">
            <span >Version 1.0 <span style="color:white;">Copyright © Contoso 2016</span> </span>
        </div>

    </div>

</asp:Content>

