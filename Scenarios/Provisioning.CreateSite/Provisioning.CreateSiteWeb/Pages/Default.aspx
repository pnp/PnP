<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.CreateSiteWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sub site and site collection creation</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: Create a sub site</h1>
            In this scenario you'll learn how to create a sub site to host web usign CSOM. Here's the topics which are addressed:
            <ul style="list-style-type: square;">
                <li>How to create new a new sub site using CSOM.</li>
                <li>How to check sub sites from the host web.</li>
                <li>How to define used sub site template.</li>
                <li>How to include additional customizations to the created site.</li>
            </ul>
            Click
            <asp:HyperLink ID="hplScenario1" runat="server" Text="here" />
            to learn more and test out scenario 1.
            <br />
            <br />
            <h1>Scenario 2: Create new site collection in tenant</h1>
            In this scenario you'll learn how to create a new site collection to the curretn tenant.
            <ul style="list-style-type: square;">
                <li>Create new site collection with desiared template</li>
                <li>Include other customizations to the created site collection</li>
            </ul>
            Click
                <asp:HyperLink ID="hplScenario2" runat="server" Text="here"></asp:HyperLink>
            to learn more and test out scenario 2.
            <br />
            <br />
        </div>
    </form>
</body>
</html>
