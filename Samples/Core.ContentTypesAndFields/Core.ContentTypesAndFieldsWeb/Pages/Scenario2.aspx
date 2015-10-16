<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scenario2.aspx.cs" Inherits="Core.ContentTypesAndFieldsWeb.Pages.Scenario2" %>

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
            <h1>Scenario 2: Taxonomy field to host web</h1>
            In this scenario you'll learn how to create content type with taxonomy field to the host web.
            <br />
            <i>Notice that there's no automatic cleaning with these scenarios.</i>
            <ul style="list-style-type: square;">
                <li>Access taxonomy service for getting group and site column information</li>
                <li>Creation of taxonomy field which could be also added to any existing content type</li>
            </ul>
            <br />
            Choose a group and taxonomy term set to which the field is associated.
            <br />
            Group: <asp:DropDownList runat="server" ID="drpGroups" OnSelectedIndexChanged="drpGroups_SelectedIndexChanged" AutoPostBack="true" Width="200px" /> 
            &nbsp;
            Term Set: <asp:DropDownList runat="server" ID="drpTermSets" Width="200px" />
            <br />
            <br />
            If there is no suitable term set, you can create a Samples group, select it and import a sample term set (to the currently selected group).
            <br />
            <asp:Button runat="server" ID="btnCreateGroup" Text="Create samples group" OnClick="btnCreateGroup_Click" />
            &nbsp;
            <asp:Button runat="server" ID="btnUploadTermSet" Text="Import sample term set" OnClick="btnUploadTermSet_Click" />
            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" />
            <asp:Label ID="lblStatus2" runat="server" />
        </div>
    </form>
</body>
</html>
