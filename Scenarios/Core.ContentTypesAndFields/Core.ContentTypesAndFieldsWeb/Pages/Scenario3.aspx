<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scenario3.aspx.cs" Inherits="Core.ContentTypesAndFieldsWeb.Pages.Scenario3" %>

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
            <h1>Scenario 3: List and content types</h1>
            In this scenario you'll learn how to create content type and how to associate it as default content type to a list.
            <br />
            <i>Notice that there's no automatic cleaning with these scenarios.</i>
            <ul style="list-style-type: square;">
                <li>Creation of new content type</li>
                <li>Adding site columns to newly added content type</li>
                <li>Associating content type to newly created list</li>
                <li>Set content type as default content type for the list</li>
            </ul>
            <br />
            Choose a list name for the newly created list which we will manipulate as needed. If list exists already, it will be also modified.
            <br />
            <asp:TextBox runat="server" ID="txtListName" Text="Sample" />
            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario3" Text="Run scenario 3" OnClick="btnScenario3_Click" />
            <asp:Label ID="lblStatus3" runat="server" />
            <br />
        </div>
    </form>
</body>
</html>
