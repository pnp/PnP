<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.OfficeWebWidgetsWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/Office.Controls.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script src="<%=Request.QueryString["SPHostUrl"] %>/_layouts/15/SP.RequestExecutor.js" type="text/javascript"></script>
    <script src="../Scripts/Office.Controls.js" type="text/javascript"></script>
    <script src="../Scripts/Office.Controls.ListView.js" type="text/javascript"></script>
    <script src="../Scripts/Office.Controls.PeoplePicker.js" type="text/javascript"></script>
    <script src="../Scripts/app.js" type="text/javascript"></script>
</head>

<body style="display: none; overflow: auto;" >
    <form id="form1" runat="server">
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>General information</h1>
        This sample shows how you can use the experimental Office Web Widgets in your provider hosted SharePoint apps. In order to use this sample you'll need to install the <b>Office Web Widgets – Experimental</b> NuGet package from Visual Studio.
        <br />
        References:<br />
        <ul style="list-style-type: square;">
            <li><a href="http://msdn.microsoft.com/en-us/library/office/dn636913(v=office.15).aspx">Office Web Widgets - Experimental overview</a></li>
            <li><a href="http://blogs.msdn.com/b/officeapps/archive/2014/03/07/office-web-widgets-experimental.aspx">Office Web Widgets—Experimental Office developer blog post</a></li>
            <li><a href="http://code.msdn.microsoft.com/office/SharePoint-2013-Office-Web-6d44aa9e#content">SharePoint 2013: Office Web Widgets - Experimental Demo on MSDN code gallery</a></li>
        </ul> 
        <br />
        <br />
        <h1>People picker demo</h1>
        Select a site Owner:<br />
        <%--Shows two peoplepickers: peoplePickerSiteOwner will be instantiated at runtime via JS, peoplePickerBackupSiteOwners is done declarative --%>
        <div id="peoplePickerSiteOwner"></div>
        <div style="display:none">
            <asp:TextBox ID="txtSiteOwner" runat="server"></asp:TextBox>
        </div>
        <br />
        <div id="peoplePickerBackupSiteOwners" data-office-control="Office.Controls.PeoplePicker" data-office-options='{ "placeholder" : "Please choose one or more backup site owner", 
                                                                                                                         "allowMultipleSelections" : true,
                                                                                                                         "onChange" : handleSiteOwnerBackupChange
                                                                                                                        }'></div>
        <div style="display:none">
            <asp:TextBox ID="txtBackupSiteOwners" runat="server" ClientIDMode="Static"></asp:TextBox>
        </div>
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit to server" OnClick="btnSubmit_Click" />
        <br />
        You've selected as site owner:<br />
        <asp:Label ID="lblSiteOwner" runat="server"></asp:Label> <br />
        You've selected as backup site owners:<br />
        <asp:Label ID="lblBackupSiteOwners" runat="server"></asp:Label> <br />
        <br />
        <h1>List view demo</h1>
        The list view widget can show (currently no inline editing) data from a list on the app web:<br />
        <div id="listViewAppWeb"></div>
        <br />
        <br />
        But the same applies for a list on the host web web:<br />
        <div id="listViewHostWeb"></div>
    </div>
    </form>
</body>
</html>
