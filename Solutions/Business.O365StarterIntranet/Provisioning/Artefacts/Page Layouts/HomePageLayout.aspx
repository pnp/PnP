<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:progid="SharePoint.WebPartPage.Document" meta:webpartpageexpansion="full" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@Register Tagprefix="PageFieldRadioButtonChoiceField" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

	<!-- Custom styles are loaded only on display mode -->
    <PublishingWebControls:EditModePanel runat="server" id="DisplayModeControl" SuppressTag="true" PageDisplayMode="display">
            <SharePoint:CssRegistration runat="server" ID="PortalCss" Name="&lt;% $SPUrl:~sitecollection/Style Library/PnP/layouts.css %&gt;" After="corev15.css"/>
	</PublishingWebControls:EditModePanel>

    <!-- Custom styles are loaded only on edit mode -->
	<PublishingWebControls:EditModePanel runat="server" id="EditModeControl" SuppressTag="true">
        <SharePoint:CssRegistration runat="server" ID="PortalLayoutEditCss" Name="&lt;% $SPUrl:~sitecollection/Style Library/PnP/layouts-edit.css %&gt;" After="corev15.css"/>

		<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %>"
			After="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>

    </PublishingWebControls:EditModePanel>

</asp:Content>

<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">

	<style>
		/* Hide the title of the Home Page (only for this layout) */
	
	    #DeltaPlaceHolderPageTitleInTitleArea {
	    	display: none;
	    }
				
	</style>

	<div class="page-layout">
		<div id="homepage-layout">      

			<div class="row">
				<div class="col-md-12">
					<!-- Page Properties (edit mode only) -->
					<PublishingWebControls:EditModePanel runat="server">
					
						<div class="edit-panel">
							
							<div id="title" class="field">
								<SharePointWebControls:TextField runat="server" FieldName="Title"/>
							</div>

							<div id="language" class="field">
								<PageFieldRadioButtonChoiceField:RadioButtonChoiceField FieldName="IntranetContentLanguage" runat="server"/>
							</div>

							<div id="translation" class="field">
								<component-translationcontrol params='availableLanguages: ["EN","FR"], languageFieldName:"IntranetContentLanguage", associationKeyFieldName:"IntranetContentAssociationKey"'></component-translationcontrol>
							</div>
						</div>
						
					</PublishingWebControls:EditModePanel>
					<!--/Page Properties (edit mode only) -->
				</div>
			</div>

			<div class="row">
				<div class="col-md-8">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Left%>" ID="LeftColum"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>
				<div class="col-md-4">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Right%>" ID="RightColumn" Orientation="Vertical"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>
			</div>
		</div>
	</div>

</asp:Content>