<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../Scripts/jsviews.js"></script>

    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/modern.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/modern-responsive.css" />

    <script type="text/javascript" src="../Scripts/log.aspx.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    SharePoint List Manager - Write to the app log
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMain" runat="server">

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
                                        <h3 class="ms-standardheader">The log information can be found in the Site Contents under the "Details" of the List Manager App.


                                        </h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" id="onetidListDescriptionText"></td>
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
                        <label for="onetidListTitle">Message: </label>
                        <font size="3">&nbsp;</font>
                        <br>
                        <table border="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td class="ms-authoringcontrols" colspan="2">
                                        <div id="ctl00_PlaceHolderMain_mainPanel" onkeypress="javascript:return WebForm_FireDefaultButton(event, 'ctl00_PlaceHolderMain_onetidCreateList')">
                                            <input name="LogMessage" title="LogMessage" class="ms-input" id="txtLogMessage" type="Text" size="35" maxlength="255" value="">
                                        </div>
                                    </td>
                                </tr>
                                    <td class="ms-authoringcontrols" colspan="2">
                                        <div id="lblMessage">
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

                                            <input id="btnlog" class="ms-ButtonHeightWidth" accesskey="O" type="button" value="log" onclick="return false;">

                                            <input class="ms-ButtonHeightWidth" id="onetidClose" accesskey="C" onclick="history.go(-1); return false;" type="button" value="Annuleren">
                                            <input name="Project" id="onetidProject" type="Hidden" value="https://tomvan.sharepoint.com">
                                            <input name="Cmd" id="onetidCmd" type="Hidden" value="NewList">
                                            </p>
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
