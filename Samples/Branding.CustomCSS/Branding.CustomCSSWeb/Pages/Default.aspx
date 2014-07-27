<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Branding.CustomCSSWeb.Default" %>

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
        <h1>Scenario 1: Set custom CSS to host site</h1>
        In this scenario you'll learn how to set custom CSS to the host site using custom user action and JavaScript injection pattern.
        <ul style="list-style-type: square;">
            <li>How to deploy needed CSS file to host web</li>
            <li>How to embed custom JavaScript block to be executed as part of site executions in host web</li>
            <li>How to apply custom CSS to the site using embedded JavaScript</li>
        </ul>
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />
        <asp:Button runat="server" ID="btnScenario1Remove" Text="Remove custom action from site" OnClick="btnScenario1Remove_Click" />   <asp:Label ID="lblStatus" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
