<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/REST-APIs-Examples.js"></script>
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div>
        <div id="message"></div>
        <div id="error" style="color: red;"></div>

        <div class="Command"><input type="button" value="Create a new list" onclick="createNewList()" /></div>
        <div class="Command"><input type="button" value="Create a new list item" onclick="createNewListItem()" /></div>
        <div class="Command"><input type="button" value="Update a list item" onclick="updateListItem()" /></div>
        <div class="Command"><input type="button" value="Delete a list item" onclick="deleteListItem()" /></div>
        <div class="Command"><input type="button" value="Query for list items" onclick="queryListItems()" /></div>
        <br /><hr /><br />
        <div class="Command"><input type="button" value="Create a new library" onclick="createNewLibrary()" /></div>        
        <div class="Command"><input type="button" value="Upload a new file" onclick="uploadFile()" /></div>
        <div class="Command"><input type="button" value="Update a file" onclick="updateFile()" /></div>
        <div class="Command"><input type="button" value="Check-out a file" onclick="checkOutFile()" /></div>
        <div class="Command"><input type="button" value="Check-in a file" onclick="checkInFile()" /></div>
        <div class="Command"><input type="button" value="Delete a file" onclick="deleteFile()" /></div>
        <div class="Command"><input type="button" value="Query for documents in a library" onclick="queryDocuments()" /></div>
        <br /><hr /><br />
        <h3>Accordion, REST, and Knockout example</h3>
        <div class="Command"><input type="button" value="View Accordion" onclick="location.href = 'Sample_REST_KO_Accordion.aspx'" /></div>
        <div class="Command"><input type="button" value="View Accordion List" onclick="location.href = '../Lists/Accordion'" /></div>
    </div>

</asp:Content>