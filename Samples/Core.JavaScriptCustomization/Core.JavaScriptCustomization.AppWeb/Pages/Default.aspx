<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.JavaScriptCustomization.AppWeb.Default" %>

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
        <h1>Scenario1: Basic JavaScript customizations</h1>
        In this scenario you'll learn the basics of setting up a JavaScript customization for SharePoint. Following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to "inject" the JavaScript file to the sites you want to customize</li>
            <li>How to make your script MDS (Minimal Download Strategy) compliant</li>
            <li>How to ensure dependant script files, such as jQuery, are loaded</li>
            <li>How to perform basic customization like hiding elements</li>
        </ul>
        Click <asp:HyperLink id="hplScenario1" Runat="server" Text="here"></asp:HyperLink> to learn more and test out scenario 1.
        <br />
        <br />
        <h1>Scenario2: JavaScript customizations for translating site contents</h1>
        This scenario further builds up on the elements explained in scenario 1 and addresses:
        <ul style="list-style-type: square;">
            <li>A JavaScript based resource file model</li>
            <li>More advanced jQuery techniques to "select" the correct HTML dom elements on the loaded page</li>
        </ul>
        Click <asp:HyperLink id="hplScenario2" Runat="server" Text="here"></asp:HyperLink> to learn more and test out scenario 2.
        <br />
        <br />
        <h1>Scenario3: Advanced JavaScript customizations</h1>
        Finally this scenario shows some more advanced customizations that allow to:
        <ul style="list-style-type: square;">
            <li>Apply JavaScript customizations on asynchronously loaded content (e.g. XSLT based listview rendering)</li>
            <li>Apply JavaScript customizations on content that is asynchronously loaded after a user manipulation</li>
        </ul>
         Click <asp:HyperLink id="hplScenario3" Runat="server" Text="here"></asp:HyperLink> to learn more and test out scenario 3.
   </div>
    </form>
</body>
</html>
