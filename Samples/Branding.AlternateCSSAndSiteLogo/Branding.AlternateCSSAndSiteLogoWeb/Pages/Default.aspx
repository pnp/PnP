<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Branding.AlternateCSSAndSiteLogoWeb.Default" %>

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
        <h1>Scenario: Set alternate CSS and custom logo to host site</h1>
        In this scenario you'll learn how to set custom CSS to the host site using new Web.AlternateCSS property. We also set the site logo using new property.
        <ul style="list-style-type: square;">
            <li>How to deploy needed CSS file and site logo to host web</li>
            <li>How to set alternate CSS and site logo using CSOM</li>
            <li>How to move alternate CSS and logo from the web configuration</li>
        </ul>
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario" Text="Run scenario" OnClick="btnScenario_Click" />
        <asp:Button runat="server" ID="btnScenario1Remove" Text="Remove custom action from site" OnClick="btnScenarioRemove_Click" />   <asp:Label ID="lblStatus" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
