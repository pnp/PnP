<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../Scripts/jsviews.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/modern.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/modern-responsive.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/SPListmanager.js"></script>
    <script type="text/javascript" src="../Scripts/SPNewList.js"></script>

    <script type="text/javascript" src="../Scripts/Resources.<SharePoint:EncodedLiteral runat='server' text='<%$Resources:wss,language_value%>' EncodeMethod='HtmlEncode' />.js"></script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
   Contoso List Manager
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="s4-bodypadding">
        <table class="propertysheet" border="0" cellspacing="0" cellpadding="0">
            <!-- Name and Description-->
            <tbody>
                <tr>
                    <td height="1" class="ms-sectionline" colspan="6">
                        <img width="1" height="1" alt="" src="/_layouts/15/images/blank.gif?rev=31" data-accessibility-nocheck="true"></td>
                </tr>
                <tr>
                    <td nowrap="nowrap" rowspan="5"></td>
                    <td class="ms-descriptiontext" id="align02" valign="top" style="padding-top: 24px;" rowspan="5">
                        <table border="0" cellspacing="0" cellpadding="1">
                            <tbody>
                                <tr>
                                    <td class="ms-sectionheader" id="700" valign="top">
                                        <h3 class="ms-standardheader" id="SPListmanagerNewListTitleAndDescription">
                                            
                                        </h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" id="onetidListDescriptionText">
                                    </td>
                                    <td width="10">&nbsp;</td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td class="ms-authoringcontrols" colspan="3">
                        <img width="1" height="1" alt="" src="/_layouts/15/images/blank.gif?rev=31" data-accessibility-nocheck="true"></td>
                </tr>
                <tr valign="top">
                    <td width="10" class="ms-authoringcontrols">&nbsp;</td>
                    <td class="ms-authoringcontrols" id="800" colspan="2">
                        <label for="onetidListTitle" id="SPListmanagerNewListTitle"></label><font size="3">&nbsp;</font><br>
                        <table border="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td class="ms-authoringcontrols" colspan="2">
                                        <div id="ctl00_PlaceHolderMain_mainPanel" onkeypress="javascript:return WebForm_FireDefaultButton(event, 'ctl00_PlaceHolderMain_onetidCreateList')">

                                            <input name="Title" title="Naam" class="ms-input" id="txtNewListTitle" type="Text" size="35" maxlength="255" value="">
                                        </div>

                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>

                <tr class="ms-authoringcontrols">
                    <td class="ms-authoringcontrols" colspan="3">&nbsp;</td>
                </tr>

                <tr>
                    <td width="10" class="ms-authoringcontrols">&nbsp;</td>
                    <td class="ms-authoringcontrols" id="900" colspan="2">
                        <label for="onetidListDescription" id="SPListmanagerNewListDescription"></label><font size="3">&nbsp;</font><br>
                        <table border="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td class="ms-authoringcontrols" colspan="2">
                                        <textarea name="Description" title="Beschrijving" class="ms-input" id="txtNewListDescription" rows="5" cols="35"></textarea>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td width="10" class="ms-authoringcontrols">&nbsp;</td>
                    <td class="ms-authoringcontrols" id="Td1" colspan="2">
                        <label for="onetidListDescription" id="SPListmanagerNewListTemplate"></label><font size="3">&nbsp;</font><br>
                        <table border="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td class="ms-authoringcontrols" colspan="2">
                                        <select id="cmbNewListTemplate" class="ms-input">
                                            <option value="100" id="cmbNewListTemplate_100">Custom List</option>
                                            <option value="101" id="cmbNewListTemplate_101">Document Library </option>
                                            <option value="102" id="cmbNewListTemplate_102">Survey </option>
                                            <option value="103" id="cmbNewListTemplate_103">Links </option>
                                            <option value="104" id="cmbNewListTemplate_104">Announcements </option>
                                            <option value="105" id="cmbNewListTemplate_105">Contacts</option>
                                            <option value="106" id="cmbNewListTemplate_106">Calendar </option>
                                            <option value="107" id="cmbNewListTemplate_107">Tasks </option>
                                            <option value="171" id="cmbNewListTemplate_171">Tasks (2013)</option>
                                            <option value="108" id="cmbNewListTemplate_108">Discussion Board </option>
                                            <option value="109" id="cmbNewListTemplate_109">Picture Library </option>
                                        </select>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td height="21" class="ms-authoringcontrols" colspan="3">&nbsp;</td>
                </tr>

                <!--OK/Cancel-->
                <tbody>
                    <tr>
                        <td height="2" class="ms-sectionline" colspan="5">
                            <img width="1" height="1" alt="" src="/_layouts/15/images/blank.gif?rev=31" data-accessibility-nocheck="true"></td>
                    </tr>
                    <tr>
                        <td class="ms-spaceBetContentAndButton" colspan="5">
                            <img width="1" height="1" alt="" src="/_layouts/15/images/blank.gif?rev=31" data-accessibility-nocheck="true"></td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <table width="100%" cellspacing="0" cellpadding="0">
                                <colgroup>
                                    <col width="99%">
                                    <col width="1%">
                                </colgroup>
                                <tbody>
                                    <tr>
                                        <td>&nbsp;

                                        </td>
                                        <td align="right" id="align01" nowrap="">
                                            <input name="FeatureId" id="onetidFeatureId" type="Hidden" value="{00bfea71-de22-43b2-a848-c05709900100}">
                                            <input name="ListTemplate" id="onetidListTemplate" type="Hidden" value="">

                                            <input id="btnCreateList" class="ms-ButtonHeightWidth" accesskey="O" type="button" value="create" onclick="return false;">

                                            <input class="ms-ButtonHeightWidth" id="onetidClose" accesskey="C" onclick="location.href = '../'; return false;" type="button" value="Annuleren">
                                            <input name="Project" id="onetidProject" type="Hidden" value="https://tomvan.sharepoint.com">
                                            <input name="Cmd" id="onetidCmd" type="Hidden" value="NewList">
                                            
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
        </table>
    </div>




</asp:Content>
