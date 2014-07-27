<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.GroupManagementWeb.Default" %>

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
        <h1>Scenario 1: List the SharePoint groups defined in the host web</h1>
        <asp:Button ID="btnLoadGroups" runat="server" Text="Load existing groups" OnClick="btnLoadGroups_Click"/>
        <br />
        <asp:Label ID="lblExistingGroups" runat="server" />
        <br />
        <br />
        <h1>Scenario 2: Create group and add users to groups...and delete the group again</h1>
        <asp:Button ID="btnCreateGroupAndAddUsers" runat="server" Text="Create group 'Test' and you to it" OnClick="btnCreateGroupAndAddUsers_Click" />
        <br />
        <br />
        <asp:Button ID="btnRemoveUserFromGroup" runat="server" Text="Remove yourself from group 'Test'" OnClick="btnRemoveUserFromGroup_Click" />
        <br />
        <br />
        <asp:Button ID="btnRemoveGroup" runat="server" Text="Remove group'Test'" OnClick="btnRemoveGroup_Click" />
        <br />
        <br />
        <h1>Scenario 3: Add permission level (e.g. Contribute) to groups and users...and remove again</h1>
        <asp:Button ID="btnAddContributePermissionLevel" runat="server" Text="Add 'Contribute' permission level to group 'Test'" OnClick="btnAddContributePermissionLevel_Click" />
        <br />
        <br />
        <asp:Button ID="btnAddReadPermissionLevel" runat="server" Text="Add 'Reader' permission level to group 'Test'" OnClick="btnAddReadPermissionLevel_Click" />
        <br />
        <br />
        <asp:Button ID="btnAddReadPermissionLevelToCurrentUser" runat="server" Text="Add 'Reader' permission level to yourself" OnClick="btnAddReadPermissionLevelToCurrentUser_Click" />
        <br />
        <br />
        <asp:Button ID="btnRemoveReadPermissionLevel" runat="server" Text="Remove 'Reader' permission from group 'Test'" OnClick="btnRemoveReadPermissionLevel_Click" />
        <br />
        <br />
        <asp:Button ID="btnRemoveReadPermissionLevelFromCurrentUser" runat="server" Text="Remove 'Reader' permission from yourself" OnClick="btnRemoveReadPermissionLevelFromCurrentUser_Click" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
