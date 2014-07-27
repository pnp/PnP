<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.DialogWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js?rev=7"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario 1: Launch this app in a dialog by adding a site settings menu item</h1>
        In this scenario you'll learn how to use a custom action to add a menu item to the site settings menu. This menu item will then launch this app in a dialog. Demo steps:
        <ul style="list-style-type: square;">
            <li>Click on "Add menu item"</li>
            <li>Go back to parent site, and use "Site Settings" --> "Office AMS Dialog sample" to launch this app <b>inside a dialog</b></li>
            <li>Click on "Remove menu item" to remove the custom menu again</li>
            <li>Close the dialog</li>
        </ul>   
        <br />
        <asp:Button runat="server" ID="btnAddCustomAction" Text="Add menu item" OnClick="btnAddCustomAction_Click" />
        <asp:Button runat="server" ID="btnRemoveCustomAction" Text="Remove menu item" OnClick="btnRemoveCustomAction_Click" />
        <br />
        <br />
        <h1>Scenario 2: Launch this app in a dialog from a link on a page</h1>
        This scenario will show you how you can use the scripteditor web part to insert an "open in dialog" link to an wiki page. Follow these steps to test this functionality:
        <ul style="list-style-type: square;">
            <li>Click on "Add page with script editor web part"</li>
            <li>Click on "here" to navigate to the created page</li>
            <li>On the opened page click on the "Open in dialog" link to open this app <b>inside a dialog</b></li>
            <li>Close the dialog</li>
            <li>Click on "Cleanup created pages" to remove the created demo page(s)</li>
        </ul>   
        <br />
        <asp:Button runat="server" ID="btnAddDialogLinkOnPage" Text="Add page with script editor web part" OnClick="btnAddDialogLinkOnPage_Click" />
         Click <asp:HyperLink id="hplScenario1" Runat="server" Text="here" Target="_blank"></asp:HyperLink> to go to the created page.
        <br />
        <br />
        <asp:Button runat="server" ID="btnCleanup" Text="Cleanup created pages" OnClick="btnCleanup_Click" />
        <br />
        <br />
        <h1>JSOM test</h1>
        <div id="siteTitle"></div>
        <br />
        <br />
        <h1>Button test</h1>
        Use the below buttons to test how they behave when the app is either shown inside a dialog or used as a full page immersive app
        <br />
        <div id="divButtons" style="float: left;">
            <asp:Button ID="btnOk" runat="server" Text="OK" OnClick="btnOk_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"  />
        </div>

    </div>
    </form>
</body>
</html>
