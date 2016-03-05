<%@ Page language="C#" masterpagefile="~masterurl/default.master" inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <sharepoint:encodedliteral runat="server" text="<%$Resources:wss,multipages_homelink_text%>" encodemethod="HtmlEncode" />
    -
    <sharepoint:projectproperty property="Title" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageImage" runat="server">
    <img src="/_layouts/15/images/blank.gif?rev=23" width='1' height='1' alt="" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderTitleAreaClass" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <meta name="CollaborationServer" content="SharePoint Team Web Site" />
    <sharepoint:styleblock runat="server">
        .s4-nothome {
            display:none;
        }
    </sharepoint:styleblock>
    <sharepoint:scriptblock runat="server">
        var navBarHelpOverrideKey = "WSSEndUser";
    </sharepoint:scriptblock>
    <SharePoint:CssRegistration Name="&lt;% $SPUrl:~sitecollection/siteassets/nfdasd/css/frontpage.css %&gt;" runat="server" ></SharePoint:CssRegistration>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderSearchArea" runat="server">
    <SharePoint:DelegateControl runat="server" ControlId="SmallSearchInputBox" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderLeftActions" runat="server" />
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server" />
<asp:Content ContentPlaceHolderID="PlaceHolderBodyAreaClass" runat="server">
    <sharepoint:styleblock runat="server">
        .ms-bodyareaframe {
        padding: 0px;
        }
    </sharepoint:styleblock>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script src="https://code.jquery.com/jquery-2.2.1.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/require.js/2.1.22/require.min.js" type="text/javascript"></script>
    <script type="text/javascript">
       require(['../siteassets/scripts/pnp'], function(pnp) {           
          window.pnp = pnp;
       });
    </script>

</asp:Content>