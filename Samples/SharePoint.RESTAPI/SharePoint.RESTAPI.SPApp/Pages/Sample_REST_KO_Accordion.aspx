<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:ScriptLink name="sp.js" runat="server" OnDemand="true" LoadAfterUI="true" Localizable="false" />
    <!-- Javascript links -->
    <script type="text/javascript" src="../Scripts/accordion/libraries/jquery-2.2.3.min.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/libraries/jquery-ui.min.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/libraries/knockout-3.1.0.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/libraries/modernizr.2.8.2.custom.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/helpers/browserRequirements.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/helpers/namespace.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/helpers/cache.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/datamodels/slideModel.js"></script>
    <script type="text/javascript" src="../Scripts/accordion/data/main.js"></script>

    <!-- CSS links -->
    <link href="../Content/accordion/progressbar.css" rel="stylesheet" />
    <link href="../Content/accordion/jquery-ui.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <div style="width: 600px; height: 400px">
        <!-- Unsupported Browsers and Browser Versions -->
        <div id="unsupported" style="text-align: center; color: white; background-color: red; display: none; cursor: pointer">
            Your web browser does not support this page. Please view this page using a newer version of Internet Explorer, Firefox or Chrome.
        </div>
        <div class="progress progress-striped active" data-bind="visible: !loadingComplete()" >
            <div id="accordionContentCurrentLoadingStatus" class="progress-bar" role="progressbar" style="width: 100%; font-weight: bold">
            </div>
        </div>
        <div style="display: none" data-bind="visible: loadingComplete()">
            <div id="accordion">
                <h2>Popular XBOX Games</h2>
                <div id="accordionContentNoDataFound" style="display: none" data-bind="visible: loadingComplete() && !resultsFound()">There were no Popular XBOX Games found in the Accordion List.</div>
                <div data-bind="foreach: bindedAllSlideItems">
                    <h3 data-bind="text: slideWhat"></h3>
                    <div>
                        <div style="font-weight: bolder">Description:</div>
                        <div style="margin-bottom: 10px" data-bind="text: slideDescription"></div>
                        <div style="font-weight: bolder">Audience:</div>
                        <div data-bind="text: slideWho"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
