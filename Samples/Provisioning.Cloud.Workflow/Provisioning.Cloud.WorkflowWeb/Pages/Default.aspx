<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.Cloud.WorkflowWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button runat="server" ID="CreateSiteButton" Text="Create site" OnClick="CreateSiteButton_Click" />
        <p></p>
        <asp:TextBox runat="server" ID="SiteName"></asp:TextBox>
    </div>
    </form>
</body>
</html>
