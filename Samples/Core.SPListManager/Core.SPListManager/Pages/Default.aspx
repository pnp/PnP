<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../Scripts/jsviews.js"></script>

    <script type="text/javascript" src="../Scripts/SPListmanager.js"></script>
    <script type="text/javascript" src="../Scripts/App.js"></script>

    <script type="text/javascript" src="../Scripts/Resources.<SharePoint:EncodedLiteral runat='server' text='<%$Resources:wss,language_value%>' EncodeMethod='HtmlEncode' />.js"></script>

    <script id="tmpList" type="text/x-jsrender">
        <div class="ms-vl-apptile ms-vl-apptilehover" onclick="SPListmanager.Default.ShowListDetails({{:InstanceID}});return false;">
            <div class="ms-vl-appimage">
                <a tabindex="-1" class="ms-storefront-selectanchor ms-storefront-appiconspan ms-draggable" dragid="1">
                    <img class="ms-storefront-appiconimg" style="border: 0px currentColor;" alt="Afbeeldingen van siteverzameling" src="{{:ImageUrl}}">
                </a>
            </div>
            <div class="ms-vl-appinfo ms-vl-pointer">
                <div>
                    <div class="ms-vl-apptitleouter">
                        <a title="{{:Title}}" class="ms-draggable ms-vl-apptitle ms-listLink" dragid="0" onclick="SPListmanager.Default.ShowListDetails({{:InstanceID}});return false;">
                            <span data-link="Title"></span>
						</a>
                    </div>
                    <a name="Open List Manager" title="Klik voor meer informatie." class="ms-vl-calloutarrow ms-calloutLink ms-ellipsis-a ms-pivotControl-overflowDot js-callout-launchPoint" onclick="SPListmanager.Default.ShowListDetails({{:InstanceID}});return false;">
                        <img class="ms-ellipsis-icon" alt="Menu openen" src="/_layouts/15/images/spcommon.png?rev=31">
                    </a>
                </div>
                <div class="ms-metadata ms-vl-appstatus">
                    {{:ItemCount}} items
			    </div>
            </div>
        </div>
    </script>

    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Contoso List Manager
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table class="ms-viewlsts" id="appsTable" style="border-collapse: collapse; width: 100%" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr class="ms-vl-sectionHeaderRow">
                <td colspan="3">

                    <!-- START SUBTITLE -->
                    <span class="ms-vl-sectionHeader">
                        <h2 id="SPListmanagerDefaultTitle" class="ms-webpart-titleText"></h2>
                    </span>
                    <!-- END SUBTITLE -->

                </td>
                <td class="ms-alignRight">

                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div class="ms-vl-applist" id="applist">
                        <div class="ms-vl-apptile ms-vl-apptilehover ms-vl-pointer" id="apptile-appadd">
                            <div class="ms-vl-appimage">
                                <a tabindex="-1" title="{SPListmanager.Default.CreateNewList}" id="appadd" style="width: 97px; height: 97px; overflow: hidden; display: inline-block; position: relative;" href="#">
                                    <img class="ms-vl-appadd-img" alt="<SharePoint:EncodedLiteral runat='server' text='<%$Resources:wss,allapps_addAnApp%>' EncodeMethod='HtmlEncode'/>" src="/_layouts/15/images/spcommon.png?rev=31">
                                </a>
                            </div>
                            <div class="ms-vl-appinfo">
                                <div style="height: 96px; vertical-align: middle; display: table-cell;">
                                    <a id="SPListmanagerDefaultCreateNewList" title="{SPListmanager.Default.CreateNewList}" class="ms-verticalAlignMiddle ms-textLarge ms-vl-apptitle" id="appadd-link" href="#"></a>
                                </div>
                            </div>
                        </div>

                        <span id="results"></span>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
</asp:Content>
