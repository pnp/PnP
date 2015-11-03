<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.PublishingFeaturesWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Publishing features</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: Deploy page layouts</h1>
            In this scenario you'll learn how to deploy page layouts to the host web and use them in pages.
            <ul style="list-style-type: square;">
                <li>How to create content type for page layouts</li>
                <li>Deploy page layouts (associated to content type) to publishing site</li>
                <li>Use embedded JavaScript in the page layouts for dynamic functionalities</li>
                <li>Create a new publishing page based on the deployed page layout with some content</li>
            </ul>
            <br />
            <asp:Button runat="server" ID="btnScenario1" Text="Deploy page layouts" OnClick="btnScenario1_Click" /><br />
            <asp:Label ID="lblStatus1" runat="server" />
            <br />
            <br />
            <h1>Scenario 2: Master page vs. Theme</h1>
            In this scenario you'll learn the difference between custom master page and the theme with publishing sites.
            <ul style="list-style-type: square;">
                <li>Deploy custom theme to publishing site</li>
                <li>Deploy custom master page to publishing site</li>
                <li>Switch between theme and custom master page</li>
            </ul>
            <br />
            <asp:Button runat="server" ID="btnScenario2Master" Text="Deploy master and use it" OnClick="btnScenario2Master_Click" />
            <asp:Button runat="server" ID="btnScenario2Theme" Text="Deploy theme as use it" OnClick="btnScenario2Theme_Click" />
            <br />
            <asp:Label ID="lblStatus2" runat="server" />
            <br />
            <hr />
            <br />
            <asp:Button runat="server" ID="btnReset" Text="Reset host settings" OnClick="btnReset_Click" />
            <br />
            <asp:Label ID="lblReset" runat="server" />
            <br />
            <br />
            <h1>Scenario 3: Set available site templates and page layouts for the host web</h1>
            In this scenario you'll learn how to control available site templates and page layouts in the host web using CSOM.
            <ul style="list-style-type: square;">
                <li>Assign available sub site tempaltes for the host web</li>
                <li>Assign supported page layouts for the host web</li>
                <li>Set specific page layout as default page layout for publishing site</li>
            </ul>
            <br />
            <asp:Button runat="server" ID="btnScenario3Apply" Text="Apply filters to host web" OnClick="btnScenario3Apply_Click" />
            <asp:Button runat="server" ID="btnScenario3Clear" Text="Remove settings for host web" OnClick="btnScenario3Clear_Click" />
            <br />
            <asp:Label ID="lblStatus3" runat="server" />
            <br />
            <br />
        </div>
    </form>
</body>
</html>