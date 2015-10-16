<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.AppScriptPartWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add-in Script Part Usage</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
 <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario: Add script Add-in part to host web</h1>
        In this scenario you'll learn how to inject Add-in script part to host web, which still uses scripts and processes from the provider hosted Add-in without Add-in parts.
        <ul style="list-style-type: square;">
            <li>How to create Add-in script part which is referencing scripts from provider hosted app</li>
            <li>How to deploy Add-in script web part to be available for use from web part gallery</li>
        </ul>
        <br />
        <i>Notice that technically you could also upload the needed script(s) to the host web from the provider hosted Add-in or during provisioning and reference scripts from there.</i>
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario" Text="Run Scenario" OnClick="btnScenario_Click" />
        <asp:Label ID="lblStatus" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>

