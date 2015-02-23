<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.SiteInformationWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <title>Site Information</title>
</head>
<body style="overflow: auto;display:none;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <table>
            <tr><td><strong>Site Title:</strong> <asp:Label ID="lblTitle" runat="server" /></td></tr>
            <tr><td><strong>Storage Quota:</strong> <asp:Label ID="lblStorageQuota" runat="server" /></td></tr>
            <tr><td><strong>Used Storage:</strong> <asp:Label ID="lblUsedStorage" runat="server" /></td></tr>
            <tr><td><strong>Read-Only:</strong> <asp:Label ID="lblReadOnly" runat="server" /></td></tr>
            <tr><td><strong>Primary Owner:</strong> <asp:Label ID="lblOwner" runat="server" /></td></tr>
            <tr><td><strong>Site Collection Administrators:</strong> <asp:Label ID="lblSiteCollectionAdmins" runat="server" /></td></tr>
            <tr><td><strong>External Sharing by Email Enabled:</strong> <asp:Label ID="lblExternalSharingByEmail" runat="server" /></td></tr>
            <tr><td><strong>External Sharing by Link Enabled:</strong> <asp:Label ID="lblExternalSharingByLink" runat="server" /></td></tr>
            <tr><td><strong>Last Content Modified Date:</strong> <asp:Label ID="lblLastModified" runat="server" /></td></tr>
            <tr><td><strong>Webs in Site Collection:</strong> <asp:Label ID="lblWebsCount" runat="server" /></td></tr>
        </table>
    </form>
</body>
</html>
