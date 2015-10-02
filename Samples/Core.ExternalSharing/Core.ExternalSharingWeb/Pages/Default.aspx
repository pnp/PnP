<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.ExternalSharingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>External Sharing</title>
    <link rel="stylesheet" type="text/css" href="../styles/app.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="chrome_ctrl_placeholder"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: External Sharing at Site Collection level</h1>
            In this scenario you'll learn how to manage site collection sharing settings with managed code.
            <ul style="list-style-type: square;">
                <li>Check if the external sharing is enabled at tenant level.</li>
                <li>Configure external sharing settings at site collection level.</li>
            </ul>
            <asp:HyperLink ID="hplScenario1" runat="server" Text="Learn more and test out scenario 1" />
            <br />
            <br />
            <h1>Scenario 2: Share a Site externally</h1>
            In this scenario you'll learn how to share site externally.
            <ul style="list-style-type: square;">
                <li>Share sites to external accounts.</li>
                <li>Provide permissions based on default site groups.</li>
                <li>Check current sharing details for the site</li>
            </ul>
            <asp:HyperLink ID="hplScenario2" runat="server" Text="Learn more and test out scenario 2"></asp:HyperLink>
            <br />
            <br />
            <h1>Scenario 3: Share Documents</h1>
            In this scenario you'll learn how to share individual documents externally, including getting anonymous links.
            <ul style="list-style-type: square;">
                <li>Create anonymous link with view or edit permissions to specific document, including option expiration time</li>
                <li>Share document externally with authentication requirement</li>
                <li>Unshare document</li>
                <li>Check sharing settings of specific document</li>
            </ul>
            <asp:HyperLink ID="hplScenario3" runat="server" Text="Learn more and test out scenario 3"></asp:HyperLink>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
