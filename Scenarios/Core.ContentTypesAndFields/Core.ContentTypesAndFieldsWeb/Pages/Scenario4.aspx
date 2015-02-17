<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scenario4.aspx.cs" Inherits="Core.ContentTypesAndFieldsWeb.Pages.Scenario4" %>

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
            <h1>Scenario 4: Localize content type and site column</h1>
            In this scenario you'll learn how to localize content types and site columns.
            <div style="color: #FF0000">Notice that there seems to be a bug related on the list level translation capability.</div>
            <br />
            <i>Notice that there's no automatic cleaning with these scenarios.</i>
            <ul style="list-style-type: square;">
                <li>Creation of a new content type</li>
                <li>Adding site columns to the new content type</li>
                <li>Associate the content type to list</li>
                <li>Adding localization entries for Finnish and Spanish languages</li>
            </ul>
            <br />
            Choose a list name for the newly created list which we will manipulate as needed. If the list exists already, it will be modified.
            <br />
            <asp:TextBox runat="server" ID="txtListName" Text="Sample" />
            <br />
            <br />
            <br />
            You must enable alternate languages in sites to make this work. 
            For more details about enabling languages, click to read the following blog post
            <div><a href="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx" target="_blank">Office365 – Multilingual content types, site columns and other site elements</a></div>
            <br />
            <asp:Button runat="server" ID="btnScenario4" Text="Run scenario 4" OnClick="btnScenario4_Click" />
            <asp:Label ID="lblStatus4" runat="server" />
            <br />
            <br />
        </div>
    </form>
</body>
</html>
