<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <!-- utility js -->
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>

    <!-- Polyfills to ensure browser functionality -->
    <script type="text/javascript" src="../Scripts/es6-promise.min.js"></script>
    <script type="text/javascript" src="../Scripts/fetch.js"></script>

    <script type="text/javascript" src="../Scripts/sample.js"></script>
    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/bootstrap.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <script type="text/javascript">

        $(function () {
            sample.ensureSPHostUrlInLinks($("a"));
        });

    </script>


</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    PnP Sample Viewer Add-In : Help
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container-fluid" style="margin-top: 30px;">
        <div class="row">

            <div class="col-md-2">
                <ul id="sample-nav" class="list-group">
                    <!-- This is where additional samples will be added, the href should point to the sample content -->
                    <li class="list-group-item"><a href="default.aspx" class="directLink">Home</a></li>
                    <li class="list-group-item active">Help</li>
                </ul>
            </div>

            <div class="col-md-10">
                <div class="page-header">
                    <h1>Help + FAQ</h1>
                </div>

                <p>As we gather common questions and areas of concern we will note them here. If you have found an issue please do report an issue for us to have a look.

            </div>
        </div>
    </div>

</asp:Content>
