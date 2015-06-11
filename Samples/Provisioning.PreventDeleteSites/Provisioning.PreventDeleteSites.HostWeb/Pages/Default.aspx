<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.PreventDeleteSites.HostWeb.Default" %>

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
        <h1>1. Deploy and activate the sandbox solution</h1>
        <p>
            <ul>
                <li><asp:LinkButton runat="server" OnClick="DeploySandboxSolution_Click" Text="Deploy solution" /></li>
                <li><asp:LinkButton runat="server" OnClick="ActivateSandboxSolution_Click" Text="Activate solution" /></li>
            </ul>
        </p>

        <h1>2. Test deleting the your site collection or web</h1>
        <p>
            <h2>Choose a site to delete:</h2>
            <asp:DropDownList runat="server" ID="SitesList1" DataTextField="Title" DataValueField="ServerRelativeUrl" />
            <asp:LinkButton runat="server" ID="NavButton1" OnClick="NavigateToDeleteSitePage_Click" Text="Try to delete site/web" />
        </p>

        <h1>3. Deactivate the sandbox solution to delete the site collection or web</h1>
        <p>
            <ul>
                <li><asp:LinkButton runat="server" OnClick="DeactivateSandboxSolution_Click" Text="Deactivate solution" /></li>
                <li><asp:LinkButton runat="server" OnClick="RemoveSandboxSolution_Click" Text="Remove solution" /></li>
            </ul>
        </p>

        <h1>4. Test deleting the your site collection or web</h1>
        <p>
            <h2>Choose a site to delete:</h2>
            <asp:DropDownList runat="server" ID="SitesList2" DataTextField="Title" DataValueField="ServerRelativeUrl" />
            <asp:LinkButton runat="server" ID="NavButton2" OnClick="NavigateToDeleteSitePage_Click" Text="Try to delete site/web" />
        </p>
    </div>
    </form>
</body>
</html>
