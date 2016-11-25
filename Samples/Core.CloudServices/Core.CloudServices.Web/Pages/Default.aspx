<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.CloudServices.Web.Pages.Default" %>

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
        <h1>Azure Cloud Services and SharePoint (Apps) play nicely together!</h1>
        <br />
        <h2>Scenario 1: An Azure cloud services worker role is using the SharePoint Tenant Administration CSOM by specifying a tenant administrator user and password</h2>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Current site title:"></asp:Label>
        <asp:Label ID="lblCurrentTitle1" runat="server" Text="" Font-Bold="True"></asp:Label>
        <asp:Label ID="Label4" runat="server" Text="     "></asp:Label>
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh site title" OnClick="btnRefresh_Click" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="New site title:"></asp:Label>
        <asp:TextBox ID="txtNewTitle1" runat="server"></asp:TextBox>
        <asp:Label ID="Label6" runat="server" Text="     "></asp:Label>
        <asp:Button ID="btnChangeTitle1" runat="server" Text="Ask Azure worker role to perform the title change" OnClick="btnChangeTitle1_Click" />
        <br />
        <br />
        See <asp:HyperLink runat="server" NavigateUrl="~/Pages/scenario1.html" Text="the scenario 1 help"></asp:HyperLink> for more information.
        <br />
        <br />
        <h2>Scenario 2: An Azure cloud services worker role is using the SharePoint Tenant Administration CSOM via OAUTH</h2>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Current site title:"></asp:Label>
        <asp:Label ID="lblCurrentTitle2" runat="server" Text="" Font-Bold="True"></asp:Label>
        <asp:Label ID="Label7" runat="server" Text="     "></asp:Label>
        <asp:Button ID="btnRefresh2" runat="server" Text="Refresh site title" OnClick="btnRefresh2_Click" />
        <br />
        <br />
        <asp:Label ID="Label5" runat="server" Text="New site title:"></asp:Label>
        <asp:TextBox ID="txtNewTitle2" runat="server"></asp:TextBox>
        <asp:Label ID="Label8" runat="server" Text="     "></asp:Label>
        <asp:Button ID="btnChangeTitle2" runat="server" Text="Ask Azure worker role to perform the title change" OnClick="btnChangeTitle2_Click" />
        <br />
        <br />
        See <asp:HyperLink runat="server" NavigateUrl="~/Pages/scenario2.html" Text="the scenario 2 help"></asp:HyperLink> for more information.
        <br />
    </div>
    </form>
</body>
</html>
