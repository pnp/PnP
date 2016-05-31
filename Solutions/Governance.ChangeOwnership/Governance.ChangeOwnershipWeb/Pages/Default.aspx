<%@ Page Title="Change Site Collection Ownership" Language="C#" MasterPageFile="~/contoso.office365.template/contoso.o365.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Governance.ChangeOwnershipWeb.Pages.Default" %>

<%@ MasterType VirtualPath="~/contoso.office365.template/contoso.o365.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="Stylesheet" type="text/css" href="../Styles/peoplepickercontrol.css" />
    <link rel="stylesheet" type="text/css" href="../Styles/app.contoso.css" />
    <script type="text/javascript" src="../Scripts/app.js?rev=2" ></script>
    <script type="text/javascript" src="../Scripts/peoplepickercontrol.js?rev=3" ></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" AsyncPostBackTimeout="0" />
        
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
                                            <asp:Label ID="lblsitename" runat="server" class="contoso-app-branding"></asp:Label>
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
            <!--current owner line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Current Site Owner
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
                                            <asp:HyperLink ID="hyperlinkCurrentOwner" runat="server"></asp:HyperLink>
                                            <asp:Label ID="lblSiteOwner" runat="server" class="contoso-app-branding"></asp:Label>
                                              <br/>
                                            <asp:HyperLink ID="hyperlinkSiteOwnerEmail" runat="server" Font-Size="12px"></asp:HyperLink>
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
            <!--new site owner line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Choose New Site Owner
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="ms-descriptiontext ms-inputformdescription">
                                <!--Section description-->
                                The Site Owner has full adminstrative rights to this site and all sites under it.  The Site Owner is the main contact for processes such as quota management, migration, and policy compliance.
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
                                            <asp:UpdatePanel ID="userselectionUpdatePnl" runat="server" UpdateMode="Always">
                                                <ContentTemplate>
                                                    <asp:RadioButtonList ID="rdbList" runat="server" AutoPostBack="true" CssClass="contoso-app-branding" OnSelectedIndexChanged="rdbList_SelectedIndexChanged" onclick="GetRadioButtonListSelectedValue(this);">
                                                        <asp:ListItem Value="myself">Assign to myself</asp:ListItem>
                                                        <asp:ListItem Value="manager">Assign to my manager</asp:ListItem>
                                                        <asp:ListItem Value="sca">Assign to another site collection administrator [Choose one]</asp:ListItem>
                                                    </asp:RadioButtonList> 
                                                    <img src="../Styles/loadingcirclests16.gif?rev=41" id="img_ScaLoading" runat="server" style="width:16px; height:16px; display: none; margin-left:10px;" alt="loading..."/>
                                                    <asp:TextBox ID="txtboxUser" runat="server" Enabled="false" Style="width: 400px" class="contoso-app-branding"></asp:TextBox>
                                                    <asp:DropDownList ID="ddlistSCA" runat="server" Style="width: 413px" class="contoso-app-branding" AutoPostBack="true" OnSelectedIndexChanged="ddlistSCA_SelectedIndexChanged"></asp:DropDownList>
                                                    <br />
                                                    <asp:Label ID="ddSelectedUser" runat="server" class="contoso-app-branding"></asp:Label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <span id="spanChangeOwnerOption" runat="server" style="display: none; width: 74%" class="confirm-error-message">No site collection administrators defined, please click link below to add.</span>
                                            <div id="divAdministrators" class="cam-peoplepicker-userlookupOwner ms-fullWidth">
                                                <span id="spanAdministrators"></span>
                                                <asp:TextBox ID="inputAdministrators" ClientIDMode="Static" runat="server" CssClass="cam-peoplepicker-edit" Width="170" placeholder="[Name or Email address]"></asp:TextBox>
                                            </div>
                                            <div id="divAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
                                            <asp:HiddenField ID="hdnAdministrators" runat="server" ClientIDMode="Static" />
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
            <!--site collection administrators line-->
            <tr>
                <td class="ms-formdescriptioncolumn-wide" valign="top" style="width:40%">
                    <table border="0" cellpadding="1" cellspacing="0" width="100%" summary="" role="presentation">
                        <tr>
                            <td class="ms-sectionheader" style="padding-top: 4px;" height="22" valign="top">
                                <h3 class="ms-standardheader ms-inputformheader">
                                    Site Collection Administrator
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="ms-descriptiontext ms-inputformdescription">
                                <!--Section description-->
                                The Site Collection Administrator has full adminstrative rights to this site and all sites under it.  If the Site Owner cannot be located or is unresponsive, the Site Collection Administrator may be contacted by IT for issues such as quota management, migration, and policy compliance.
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
                                            <a id="SCALink" runat="server" class="contoso-app-branding">Add Site Collection Administrators...</a>
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
                                If you remove yourself as the Site Owner or Site Collection Administrator of the site, you may lose adminstrative permissions to the site. 
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
                                        <asp:Button ID="btnCreate" runat="server" ClientIDMode="Static" Text="OK" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancel" runat="server" ClientIDMode="Static" Text="Cancel" CssClass="ms-ButtonHeightWidth"  OnClick="btnCancel_Click" UseSubmitBehavior="false" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td class="ms-descriptiontext" colspan="2"><span id="spanErrorMsg" style="display:none;" class="ms-error">Sorry something went wrong, please try again.  If this problem persists, please contact <a href='https://myitsupport.ext.contoso.com/myITsupport/ITSService?categorySelected=Sharepoint' target='_blank'> IT Support</a></span></td></tr>
            <tr><td height="10" class="ms-descriptiontext" colspan="2"><img src="../Styles/blank.gif?rev=41" width="1" height="10" alt="" data-accessibility-nocheck="true"/></td></tr>
            <tr><td height="40" class="ms-descriptiontext s4-notdlg" colspan="2"><img src="../Styles/blank.gif?rev=41" width="1" height="40" alt="" data-accessibility-nocheck="true"/></td></tr>
        </table>
        
        <br />

        <div class="contoso-app-version" style="margin-top: 15px !important;">
            <span > Version 1.0 <span style="color:white">Copyright © Contoso 2016</span></span>
        </div>

    </div>

</asp:Content>
