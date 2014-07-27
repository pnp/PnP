<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scenario1.aspx.cs" Inherits="Core.ContentTypesAndFieldsWeb.Pages.Scenario1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario 1: Create New Content Type</h1>
        In this scenario you'll learn how to create content type and site column using CSOM to host web. Here's the topics which are addressed:
        <br />
        <i>Notice that there's no automatic cleaning with these scenarios.</i>
        <ul style="list-style-type: square;">
            <li>How to create new content types usign CSOM.</li>
            <li>How to add site columsn to the newly added content type using CSOM.</li>
        </ul>
        <br />
        <h2>Demo</h2>
        Choose a parent content type to your new content type.
        <br />
        <asp:DropDownList runat="server" ID="drpContentTypes" Width="400px" />
        <br />
        Give a name for your content type
        <br />
        <asp:TextBox runat="server" ID="txtContentTypeName" Text="Sample" Width="400px" />
        <br />
        Give an content type ID extension for you content type. <i>This will be used to create unique ID for the content type.</i>
        <br />
        <asp:TextBox runat="server" ID="txtContentTypeExtension" Text="Sample" Width="400px" />
        <br />
        <br />
        <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />
        <br />
        <asp:Label ID="lblStatus1" runat="server" />
        <br />
    </div>
    </form>
</body>
</html>

