<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceholderID="PlaceHolderAdditionalPageHead" runat="server">
	<style type="text/css">
		.v4master #s4-leftpanel { display: none; }
		.v4master .s4-ca { margin-left: 0px; }
	</style>
	<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
	<PublishingWebControls:EditModePanel runat="server" id="editmodestyles">
		<!-- Styles for edit mode only-->
		<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %>"
			After="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
	</PublishingWebControls:EditModePanel>
    
    <SharePointWebControls:ScriptLink runat="server" Name="SP.js" Localizable="false"  ID="s3" LoadAfterUI="true"/>
	<SharePointWebControls:ScriptLink runat="server" Name="SP.Core.js" Localizable="false"  ID="s1" LoadAfterUI="true"/>
	<SharePointWebControls:ScriptLink runat="server" Name="SP.Runtime.js" Localizable="false"  ID="s2" LoadAfterUI="true"/>

    <SharePointWebControls:Scriptlink ID="Scriptlink1" runat="server" Name="~SiteCollection/Style Library/Contoso/Scripts/UserProfileData.js" Language="javascript" />
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePointWebControls:FieldValue FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderTitleBreadcrumb" runat="server">
	<div class="breadcrumb">
		<asp:SiteMapPath runat="server" SiteMapProvider="CurrentNavigation"
			RenderCurrentNodeAsLink="false" SkipLinkText="" CurrentNodeStyle-CssClass="current" NodeStyle-CssClass="ms-sitemapdirectional"/>
	</div>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
	<div class="welcome welcome-links">
		<PublishingWebControls:EditModePanel runat="server" CssClass="edit-mode-panel title-edit">
			<SharePointWebControls:TextField runat="server" FieldName="Title"/>
		</PublishingWebControls:EditModePanel>
		<div class="welcome-image">
			<PublishingWebControls:RichImageField FieldName="PublishingPageImage"  runat="server"/>
            <br />
            <div id="ContosoUserProfileDataInformation" style="width:450px""></div>
		</div>
		<div class="welcome-content">
			<PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" HasInitialFocus="True" MinimumEditHeight="400px" runat="server"/>
		</div>
    		<div class="col-50 clearer">
			<div class="left-column-links">
				<PublishingWebControls:SummaryLinkFieldControl FieldName="SummaryLinks" runat="server"/>
			</div>
		</div>
		<div class="col-50">
			<div class="right-column-links">
				<PublishingWebControls:SummaryLinkFieldControl FieldName="SummaryLinks2" runat="server"/>
			</div>
		</div>
		<div class="clearer">
			<div class="links-top-zone">
				<WebPartPages:WebPartZone runat="server" AllowPersonalization="true" ID="TopColumnZone" FrameType="TitleBarOnly"
					Title="<%$Resources:cms,WebPartZoneTitle_Top%>" Orientation="Vertical"></WebPartPages:WebPartZone>
			</div>
		</div>
		<div class="ms-table ms-fullWidth">
		    <div class="tableCol-50">
			    <div class="left-column-area">
				    <WebPartPages:WebPartZone runat="server" AllowPersonalization="true" ID="LeftColumnZone" FrameType="TitleBarOnly"
					    Title="<%$Resources:cms,WebPartZoneTitle_LeftColumn%>" Orientation="Vertical"></WebPartPages:WebPartZone>
			    </div>
		    </div>
		    <div class="tableCol-50">
			    <div class="right-column-area">
				    <WebPartPages:WebPartZone runat="server" AllowPersonalization="true" ID="RightColumnZone" FrameType="TitleBarOnly"
					    Title="<%$Resources:cms,WebPartZoneTitle_RightColumn%>" Orientation="Vertical"></WebPartPages:WebPartZone>
			    </div>
		    </div>
		</div>
	</div>
</asp:Content>
