<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>
<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.requestexecutor.js"></script>
    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following files -->
    <script type="text/javascript" src="../Scripts/provision.js"></script>
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Site Provisioning using a SharePoint Hosted App
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h1>This page demonstrates the JSOM provisioning capabilities of the CSOM libraries</h1> <br /><br />
    The provision.js file loaded by this page will scrape the following values out of the page and provision site assets as described.<br /><br />
    <h2>Site Collection Root</h2>
    <h3>Site Column</h3>
    Site Column Display Name: <label id="sitecolumndisplayname">Favorite Color</label><br />
    Site Column Name: <label id="sitecolumnname">FavoriteColor</label><br />
    Site Column Type: <label id="sitecolumntype">Text</label><br /><br />
    <h3>Content Type</h3>
    Content Type Name: <label id="contenttypename">Provisioned Content Type</label><br />
    Content Type GUID: <label id="contenttypeid">0x010035B6BB0664654138A3FEDE70948A1D28</label><br />
    Content Type Definition: This will consist of two columns, Title and the new Favorite Color site column<br />
    This is defined based on the format of {Base Content Type ID} + '00' + {New Guid}. In this case, this type is based<br />
    on 0x01 which is Item.<br /><br />
    <h2>Sub-Site</h2>
    <h3>Site</h3>
    Site Title: <label id="sitetitle">JSOM Provisioned Subsite</label><br />
    Site URL: <label id="siteurl">jsomprovisionedsubsite</label><br />
    Site Template: <label id="sitetemplate">STS</label><br /><br />
    <h3>Document Library:</h3>
    Document Library Name: <label id="doclibname">Contoso Documents</label><br />
    Document Library Configuration: The document library will be provisioned with the new content type as the default.<br /><br />
    <h3>File</h3>
    File Name: <label id="docname">JSOM-Provisioned-Document.txt</label><br />
    File Title: <label id="doctitle">JSOM Provisioned Document</label><br />
    File Favorite Color: <label id="doccolor">Red</label><br /><br />
    <input id="createsite" type="button" value="Provision Site" /><input id="deletesite" type="button" value="Delete Site" /><br />
</asp:Content>
