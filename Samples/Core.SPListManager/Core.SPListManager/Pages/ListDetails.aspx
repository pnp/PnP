<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.10.3.js"></script>
    <script type="text/javascript" src="../Scripts/SPListmanager.js"></script>
    <script type="text/javascript" src="../Scripts/SPListdetails.js"></script>
    
    <link rel="stylesheet" href="../Content/App.css"  />
    <link rel="stylesheet" href="../Content/jquery-ui.css" />

    <script>
        $(function () {
            $("#tabs").tabs();
        });
    </script>
</asp:Content>


<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">Properties</a></li>
            <li style="display:none;"><a href="#tabs-2">RoleAssignments</a></li> <%--NOT POSSIBLE WITH READ PERMISSIONS ON THE APP--%>
            <li><a href="#tabs-3">ContentTypes</a></li>
            <li><a href="#tabs-4">DefaultView</a></li>
            <li><a href="#tabs-5">EventReceivers</a></li>
            <li><a href="#tabs-6">Fields</a></li>
            <li><a href="#tabs-7">Forms</a></li>
            <li><a href="#tabs-8">InformationRightsManagementSettings</a></li>
            <li><a href="#tabs-9">Items</a></li>
            <li><a href="#tabs-10">ParentWeb</a></li>
            <li><a href="#tabs-11">RootFolder</a></li>
            <li><a href="#tabs-12">UserCustomActions</a></li>
            <li><a href="#tabs-13">Views</a></li>
            <li><a href="#tabs-14">WorkflowAssociations</a></li>
        </ul>
        <div id="tabs-1" class="show-workinprogress">
        </div>
        <div id="tabs-2" class="show-workinprogress">
        </div>
        <div id="tabs-3" class="show-workinprogress">
        </div>
        <div id="tabs-4" class="show-workinprogress">
        </div>
        <div id="tabs-5" class="show-workinprogress">
        </div>
        <div id="tabs-6" class="show-workinprogress">
        </div>
        <div id="tabs-7" class="show-workinprogress">
        </div>
        <div id="tabs-8" class="show-workinprogress">
        </div>
        <div id="tabs-9" class="show-workinprogress">
        </div>
        <div id="tabs-10" class="show-workinprogress">
        </div>
        <div id="tabs-11" class="show-workinprogress">
        </div>
        <div id="tabs-12" class="show-workinprogress">
        </div>
        <div id="tabs-13" class="show-workinprogress">
        </div>
        <div id="tabs-14" class="show-workinprogress">
        </div>
    </div>
</asp:Content>
