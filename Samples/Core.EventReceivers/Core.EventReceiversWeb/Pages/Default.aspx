<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.EventReceiversWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Remote Event Receiver</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
 <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario: Demonstrate usage of the remote event receivers with list events</h1>
        In this scenario you'll learn how to set remote event receiver to the list in site and how to process those requests. Remote event receiver is added during the AppInstalled event.
        <ul style="list-style-type: square;">
            <li>Create remote event receiver to host web for handling remote events to provider hosted app</li>
            <li>How to handle events in the remote service</li>
            <li>How to uninstall events from the host web</li>
        </ul>
        <i>Notice that you could associate this example code also to site collection provisioning, which ensure that all new sites have automatically needed customizations.</i>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
