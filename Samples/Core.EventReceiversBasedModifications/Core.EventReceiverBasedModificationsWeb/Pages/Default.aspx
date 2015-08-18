<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.EventReceiverBasedModificationsWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario 1: What event receivers are defined for the host web</h1>
        <br />
        <asp:Label ID="lblEventReceivers" runat="server"></asp:Label>
        <br />
        <br />
        <h1>Scenario 2: Update host web lists on creation via an injected ListAdded remote event receiver</h1>
        Steps to test this functionality are:<br />
        <ul style="list-style-type: square;">
            <li>Navigate back to the host web</li>
            <li>Add a new document library</li>
            <li>Verify that after the library has been added that versioning is enabled</li>
        </ul> 
        <br />
        <br />
        To uninstall this app you'll need to uninstall it via the host web ("apps in testing" when a using a developer site, "site contents" otherwise) as this will the appuninstall remote event receiver to fire.
        <br />
    </div>
    </form>
</body>
</html>
