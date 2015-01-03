<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.Settings.AuditWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Control Auditing Settings</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
 <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario: Control Auditing Settings</h1>
        In this scenario you'll learn how to control audting settings in site collection level at the host site collection.
        <ul style="list-style-type: square;">
            <li>How to enable auditing in site collection level using CSOM</li>
            <li>How to control audting settings using CSOM</li>
            <li>How to disable auditing in site collection level</li>
        </ul>
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario" Text="Run scenario" OnClick="btnScenario_Click" />
        <asp:Button runat="server" ID="btnScenario1Remove" Text="Disable auditing settings" OnClick="btnScenarioRemove_Click" />   <asp:Label ID="lblStatus" runat="server" />
        <br />
        <br />
    </div>
         <asp:HiddenField ID="SPAppToken" ClientIDMode="Static" runat="server" />
    </form>
</body>
</html>
