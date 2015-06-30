<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js"></script>
    <SharePoint:ScriptLink Name="sp.runtime.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink Name="sp.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink Name="SP.UI.Dialog.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink Name="SP.RequestExecutor.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink Name="PickerTreeDialog.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/Core.js"></script>
    <script type="text/javascript" src="../Scripts/Common.js"></script>
    <script type="text/javascript" src="../Scripts/Data.js"></script>
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    PnP Header & Footer
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div id="Status"><b>Current Status:</b></div>
    <table>
        <tr>
            <th>Title</th>
            <th>Object</th>
            <th>Status</th>
        </tr>

        <tr>
            <td class="TBTitle">PropertyBag</td>
            <td class="TBObject"><b>PnPGlobalBreadcrumbRibbon</b></td>
            <td class="TVStatus">
                <img id="TBPropertyBag" />
            </td>
        </tr>
        <tr>
            <td class="TBTitle">Javascript File</td>
            <td class="TBObject"><b>PnPGlobal.js</b><a id="FileFolder">Link</a></td>
            <td class="TVStatus">
                <img id="TBFile" />
            </td>
        </tr>
        <tr>
            <td class="TBTitle">SiteCollection</td>
            <td class="TBObject"><b>PnPGlobalBreadcrumbRibbon</b></td>
            <td class="TVStatus">
                <img id="TBUserCustomAction" />
            </td>
        </tr>
    </table>
    <br />
    <div>
        <b>Provision Sequence:</b>
        <ol>
            <li>PropertyBag <b>PnPGlobalBreadcrumbRibbon</b> is created in RootWeb of the SiteCollection, this item will have the JSON data from the <b>JSON Editor Area</b>, if the JSON it's not correct then assumes the <b>default</b> JSON Data:<br />
                <b>{"Breadcrumb": [{"title": "Home","description": "Home","url":"https://github.com/OfficeDev"},{"title": "Product Category","description": "Product Category","url":"https://github.com/OfficeDev"},{"title": "Product","description": "Product","url":"https://github.com/OfficeDev"},{"title": "Example","description": "Example","url":"https://github.com/OfficeDev"}]}</b></li>
            <li>Copy of File <b>PnPGlobal.js</b> to <b>"_catalogs/masterpage/Display Template"</b>, this file supports the "ScriptLink" UserCustomAction</li>
            <li>SiteCollection ScriptLink with name <b>PnPGlobalBreadcrumbRibbon</b> to <b>PnPGlobal.js</b></li>
        </ol>
        <br />
        <b>Breadcrumb JSON Editor</b><br />
        <textarea id="PropertyBagJSON" rows="5" cols="150">{"Breadcrumb": [{"title": "Home","description": "Home","url":"https://github.com/OfficeDev"},{"title": "Product Category","description": "Product Category","url":"https://github.com/OfficeDev"},{"title": "Product","description": "Product","url":"https://github.com/OfficeDev"},{"title": "Example","description": "Example","url":"https://github.com/OfficeDev"}]}
    </textarea>
        
    </div>
    <br />
    <div id="ProvisionTable">
        <div>
            <b>Add UserCustomAction Global Custom Breadcrumb and Ribbon in Site Collection</b>
            <br />
            <input type="submit" class="ms-input" style="width: 140px;" value="Add Provision" id="BtAddProvision" onclick="PnPApp.AddProvisionOfUserCustomAction(); return false;" />
        </div>
        <div>
            <b>Remove UserCustomAction Global Custom Breadcrumb and Ribbon in Site Collection</b>
            <br />
            <input type="submit" class="ms-input" style="width: 140px;" value="Remove Provision" id="BtRemoveProvision" onclick="PnPApp.RemoveProvisionOfUserCustomAction(); return false;" />
        </div>
    </div>

</asp:Content>
