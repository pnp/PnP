<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>

    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/Common.js"></script>
    <script type="text/javascript" src="../Scripts/OfficeDevPnP.Core.Navigation.js"></script>
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Office 365 Developer PnP JSOM Navigation Sample 
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table>
        <thead></thead>
        <tbody>
            <tr>
                <td>This will add a node named 'Test' to the top nav.</td>
                <td>
                    <button id="addTopNavNodeButton" onclick="navApp.addTopNavNode();" type="submit">Add Top Nav Navigation Node</button></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>The will delete the node named 'Test' created in the step above.</td>
                <td>
                    <button id="deleteTopNavNodeButton" onclick="navApp.deleteTopNavNode();" type="submit">Delete Top Nav Navigation Node</button></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>This will add a node named 'Parent' and a node named 'Child' to the quick launch.
                </td>
                <td>
                    <button id="addQuickLaunchNodesButton" onclick="navApp.addQuickLaunchNodes();" type="submit">Add Quick Launch Nodes</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>This will delete the nodes 'Parent' and 'Child' created in the step above.
                </td>
                <td>
                    <button id="deleteQuickLaunchNodesButton" onclick="navApp.deleteQuickLaunchNodes();" type="submit">Delete Quick Launch Nodes</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td class="td-warning">This will delete all quick launch nodes.</td>
                <td>
                    <button id="deleteAllQuickLaunchNodesButton" onclick="navApp.deleteAllQuickLaunchNodes();" type="submit">Delete All Quick Launch Nodes</button></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>Sets navigation inheriteance to true.</td>
                <td>
                    <button id="updateNavigationInheritanceTrueButton" onclick="navApp.updateNavigationInheritanceTrue();" type="submit">Set Navigation Inheritance to true</button></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>Sets navigation inheriteance to false.</td>
                <td>
                    <button id="updateNavigationInheritanceFalseButton" onclick="navApp.updateNavigationInheritanceFalse();" type="submit">Set Navigation Inheritance to false</button></td>
            </tr>
        </tbody>
    </table>

    <div id="statusArea" class="div-status">
        <label class="label-status">Status: </label>
        <label id="statusMessage" class="message-status"></label>
    </div>

</asp:Content>
