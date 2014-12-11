<%@ Page language="C#" MasterPageFile="~masterurl/default.master"    Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c"  %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="ApplicationPages" Namespace="Microsoft.SharePoint.ApplicationPages.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server"><SharePoint:ListProperty Property="TitleOrFolder" runat="server"/> - <SharePoint:ListProperty Property="CurrentViewTitle" runat="server"/></asp:Content>
<asp:content contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:RssLink runat="server" />
</asp:content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageImage" runat="server"><SharePoint:ViewIcon Width="145" Height="54" runat="server" /></asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderLeftActions" runat="server">
<SharePoint:RecentChangesMenu runat="server" id="RecentChanges"/>
<SharePoint:ModifySettingsLink runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderId ="PlaceHolderBodyLeftBorder" runat="server">
 <div height="100%" class="ms-pagemargin"><img src="/_layouts/15/images/blank.gif?rev=23" width='6' height='1' alt="" /></div>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
		<WebPartPages:WebPartZone runat="server" FrameType="None" ID="Main" Title="loc:Main" />
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
<div class="ms-listdescription"><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,xmlformlib_repair_desc%>" EncodeMethod="HtmlEncode"/></div>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderCalendarNavigator" runat="server">
  <SharePoint:SPCalendarNavigator id="CalendarNavigatorId" runat="server"/>
  <ApplicationPages:CalendarAggregationPanel id="AggregationPanel" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderUtilityContent" runat="server">
	<form id="SubmitRepairDocsForm" method="POST" action="">
		<input id="SubmitRepairRedirectList" type="hidden" name="SubmitRepairRedirectList" />
		<input id="SubmitRepairRedirectFolder" type="hidden" name="SubmitRepairRedirectFolder" />
		<input id="SubmitRepairDocs" type="hidden" name="SubmitRepairDocs" />
		<SharePoint:FormDigest runat=server ForceRender="true"/>
	</form>
</asp:Content>
