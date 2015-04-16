<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Provisioning.Services.SiteManager.AppWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <title>Request new site collection</title>
    <link rel="Stylesheet" type="text/css" href="../Styling/app.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/ChromeLoader.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"></asp:ScriptManager>
        <div id="chrome_ctrl_placeholder"></div>
        <div id="ContentArea">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="ms-core-overlay" id="s4-workspace" style="width: 800px;">
                        <div id="s4-bodyContainer">
                            <div id="contentRow">
                                <div id="contentBox" aria-live="polite" aria-relevant="all">
                                    <div id="DeltaPlaceHolderMain">
                                        <table class="ms-propertysheet" border="0" f="0" cellpadding="0">
                                            <tbody>
                                                <tr>
                                                    <td height="1" class="ms-sectionline" colspan="2">
                                                        <img width="1" height="1" alt="" src="../styling/images/blank.gif" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="ms-formdescriptioncolumn-wide" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="1" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td height="22" class="ms-sectionheader" valign="top" style="padding-top: 4px;">
                                                                        <h3 class="ms-standardheader ms-inputformheader">Title and Description</h3>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ms-descriptiontext ms-inputformdescription"></td>
                                                                    <td>
                                                                        <img width="8" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <img width="150" height="19" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="ms-authoringcontrols ms-inputformcontrols" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td width="9">
                                                                        <img width="9" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <img width="150" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td class="ms-authoringcontrols">
                                                                        <table width="100%" class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" summary="">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <span>Title:</span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">&nbsp;
                                                                                        <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <span>Description:</span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">&nbsp;
                                                                                        <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td>
                                                                        <img width="150" height="13" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="1" class="ms-sectionline" colspan="2">
                                                        <img width="1" height="1" alt="" src="../styling/images/blank.gif" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="ms-formdescriptioncolumn-wide" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="1" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td height="22" class="ms-sectionheader" valign="top" style="padding-top: 4px;">
                                                                        <h3 class="ms-standardheader ms-inputformheader">Web Site Address					   
                                                                        </h3>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ms-descriptiontext ms-inputformdescription"></td>
                                                                    <td>
                                                                        <img width="8" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <img width="150" height="19" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="ms-authoringcontrols ms-inputformcontrols" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td width="9">
                                                                        <img width="9" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <img width="150" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td class="ms-authoringcontrols">
                                                                        <table width="100%" class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" summary="">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <span>URL name:</span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">
                                                                                        <table dir="ltr" border="0" cellspacing="0" cellpadding="0">
                                                                                            <tbody>
                                                                                                <tr nowrap="nowrap">
                                                                                                    <td class="ms-authoringcontrols" nowrap="nowrap" style="padding-right: 2px;">
                                                                                                        <asp:Label ID="lblHostUrl" runat="server" Text="Label"></asp:Label>
                                                                                                        /sites/&nbsp;</td>
                                                                                                    <td class="ms-authoringcontrols">&nbsp;<span class="ms-error" style="display: none;"><br />
                                                                                                        <span role="alert">You can't leave this blank.</span></span>
                                                                                                        <asp:TextBox ID="txtUrl" runat="server"></asp:TextBox>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </tbody>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td>
                                                                        <img width="150" height="13" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>

                                                <!-- 
                                                    #############################
                                                    Administrators
                                                    #############################
                                                    -->
                                                <tr>
                                                    <td class="ms-formdescriptioncolumn-wide" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="1" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td height="22" class="ms-sectionheader" valign="top" style="padding-top: 4px;">
                                                                        <h3 class="ms-standardheader ms-inputformheader">Administrators</h3>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ms-descriptiontext ms-inputformdescription"></td>
                                                                    <td>
                                                                        <img width="8" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <img width="150" height="19" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="ms-authoringcontrols ms-inputformcontrols" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td width="9">
                                                                        <img width="9" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <img width="150" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td class="ms-authoringcontrols">
                                                                        <table width="100%" class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" summary="">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <span>Primary Administrator</span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">&nbsp;
                                                                                        <asp:TextBox ID="txtAdminPrimary" runat="server"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <span>Secondary contact</span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">&nbsp;
                                                                                        <asp:TextBox ID="txtAdminSecondary" runat="server"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td>
                                                                        <img width="150" height="13" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>

                                                <!-- ############################################################################################################################## -->
                                                <!-- TEMPLATE PICKER -->
                                                <!-- ############################################################################################################################## -->

                                                <tr>
                                                    <td height="1" class="ms-sectionline" colspan="2">
                                                        <img width="1" height="1" alt="" src="../styling/images/blank.gif" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="ms-formdescriptioncolumn-wide" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="1" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td height="22" class="ms-sectionheader" valign="top" style="padding-top: 4px;">
                                                                        <h3 class="ms-standardheader ms-inputformheader">Template Selection          						   
                                                                        </h3>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ms-descriptiontext ms-inputformdescription"></td>
                                                                    <td>
                                                                        <img width="8" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <img width="150" height="19" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="ms-authoringcontrols ms-inputformcontrols" valign="top">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" summary="">
                                                            <tbody>
                                                                <tr>
                                                                    <td width="9">
                                                                        <img width="9" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <img width="150" height="7" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td class="ms-authoringcontrols">
                                                                        <table width="100%" class="ms-authoringcontrols" border="0" cellspacing="0" cellpadding="0" summary="">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td class="ms-authoringcontrols" colspan="2">
                                                                                        <label>Select a template:</label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="3" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                                <!-- End Right_Text -->
                                                                                <tr>
                                                                                    <td width="11">
                                                                                        <img width="11" height="1" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                    <td width="99%" class="ms-authoringcontrols">
                                                                                        <div class="ms-templatepicker">
                                                                                            &nbsp;<div class="ms-descriptiontext ms-floatLeft" style="width: 440px; display: inline;">
                                                                                                <span></span>
                                                                                            </div>
                                                                                            <asp:ListBox ID="listSites" runat="server" Width="321px"></asp:ListBox>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <img width="1" height="6" style="display: block;" alt="" src="../styling/images/blank.gif" />
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                    <td width="10">
                                                                        <img width="10" height="1" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td>
                                                                        <img width="150" height="13" alt="" src="../styling/images/blank.gif" />
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <!-- Save button -->
                                                <tr>
                                                    <td colspan="2">
                                                        <table width="100%" cellspacing="0" cellpadding="0">
                                                            <colgroup>
                                                                <col width="99%" />
                                                                <col width="1%" />
                                                            </colgroup>
                                                            <tbody>
                                                                <tr>
                                                                    <td>&nbsp;
                                                                    </td>
                                                                    <td nowrap="nowrap">&nbsp;<asp:Button ID="btnCreate" runat="server" Text="Create Site" OnClick="Create_Click" />&nbsp;
                                                                <input name="ctl00$PlaceHolderMain$ctl01$BtnCancel" class="ms-ButtonHeightWidth" type="button" value="Cancel" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <div style="position: fixed; top: 5%; left: 5%; width: 80%; height: 90%; border: 1px solid #ccc; background-color: white;">
                        <table style="width: 600px; height: 100%; text-align: center">
                            <tr>
                                <td valign="center">
                                    <h1>Working on it...<img src="../Styling/Images/gears_anv4.gif" /></h1>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </form>
</body>
</html>
