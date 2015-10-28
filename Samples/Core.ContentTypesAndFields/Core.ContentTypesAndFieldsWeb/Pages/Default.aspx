<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.ContentTypesAndFieldsWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Content type and site columns</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: Create New Content Type</h1>
            In this scenario you'll learn how to create content type and site column using CSOM to host web. Here's the topics which are addressed:
            <ul style="list-style-type: square;">
                <li>How to create new content types using CSOM.</li>
                <li>How to add site columns to the newly added content type using CSOM.</li>
            </ul>
            <asp:HyperLink ID="hplScenario1" runat="server" Text="Learn more and test out scenario 1" />
            <br />
            <br />
            <h1>Scenario 2: Taxonomy field to host web</h1>
            In this scenario you'll learn how to create content type with taxonomy field to the host web.
            <ul style="list-style-type: square;">
                <li>Access taxonomy service for getting group and site column information</li>
                <li>Optional creation of taxonomy group and import of sample term set</li>
                <li>Creation of new content type</li>
                <li>Creation of taxonomy field associated to newly created content type</li>
                <li>Assocation of  of taxonomy field to newly created content type</li>
            </ul>
            <asp:HyperLink ID="hplScenario2" runat="server" Text="Learn more and test out scenario 2"></asp:HyperLink>
            <br />
            <br />
            <h1>Scenario 3: List and content types</h1>
            In this scenario you'll learn how to create content type and how to associate it as default content type to a list.
            <ul style="list-style-type: square;">
                <li>Creation of new content type</li>
                <li>Adding site columns to newly added content type</li>
                <li>Associating content type to newly created list</li>
                <li>Set content type as default content type for the list</li>
            </ul>
            <asp:HyperLink ID="hplScenario3" runat="server" Text="Learn more and test out scenario 3"></asp:HyperLink>
            <br />
            <br />
            <h1>Scenario 4: Localication of content types and site columns</h1>
            In this scenario you'll learn how to add localizations for the content types and site columns.
            <ul style="list-style-type: square;">
                <li>Creation of new content type</li>
                <li>Adding site columns to newly added content type</li>
                <li>Adding localization entries for Finnish, Spanish and German languages</li>
            </ul>
            <asp:HyperLink ID="hplScenario4" runat="server" Text="Learn more and test out scenario 4"></asp:HyperLink>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
