<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:progid="SharePoint.WebPartPage.Document" meta:webpartpageexpansion="full" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@Register Tagprefix="PageFieldTaxonomyFieldControl" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldPublishingScheduleFieldControl" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldTextField" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldUserField" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldDateTimeField" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldRadioButtonChoiceField" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@Register Tagprefix="PageFieldPublishingScheduleFieldControl" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>

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
<asp:Content ContentPlaceholderID="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePointWebControls:FieldValue FieldName="Title" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
      
    <div class="page-layout">
        <div id="static-page-layout">
            <div class="row">
                <div class="col-md-3 hidden-xs hidden-sm">   

                    <component-contextualmenu params='siteMapFieldName: "IntranetSiteMapPosition"' ></component-contextualmenu>
                </div>
                <div class="col-md-9">
                    <div class="row">
                        <div class="col-md-12">
                            <component-breadcrumb params='siteMapFieldName: "IntranetSiteMapPosition"' ></component-breadcrumb>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                    
                            <div class="content">

                                <div id="title" class="field">
                                    <SharePointWebControls:TextField runat="server" FieldName="Title"/>
                                </div>
                                
                                <!-- Page Properties (edit mode only) -->
                                <PublishingWebControls:EditModePanel runat="server">

                                    <div id="date" class="field">
                                        <PageFieldDateTimeField:DateTimeField FieldName="IntranetContentPublishingDate" runat="server"/>
                                    </div>

                                    <div id="author" class="field">
                                        <PageFieldUserField:UserField FieldName="IntranetContentAuthor" runat="server"/>
                                    </div>

                                    <div class="edit-panel">
                                        
                                        <div id="publishing-start-date" class="field">
                                            <PageFieldPublishingScheduleFieldControl:PublishingScheduleFieldControl FieldName="PublishingStartDate" runat="server"/>
	                           			</div>

                                        <div id="publishing-expiration-date" class="field">
                                            <PageFieldPublishingScheduleFieldControl:PublishingScheduleFieldControl FieldName="PublishingExpirationDate" runat="server"/>
	                           			</div>

                                    	<div id="tags" class="field">
                                			<PageFieldTaxonomyFieldControl:TaxonomyFieldControl FieldName="IntranetKeywords" runat="server"/>
                               			</div>

                                        <div id="language" class="field">
                                           <PageFieldRadioButtonChoiceField:RadioButtonChoiceField FieldName="IntranetContentLanguage" runat="server"/>
                                        </div>

                                        <div id="translation" class="field">
                                            <component-translationcontrol params='availableLanguages: ["EN","FR"], languageFieldName:"IntranetContentLanguage", associationKeyFieldName:"IntranetContentAssociationKey"'></component-translationcontrol>
                                        </div>

                                        <div id="sitemap-position" class="field">
                                			<PageFieldTaxonomyFieldControl:TaxonomyFieldControl FieldName="IntranetSiteMapPosition" runat="server"/>
                               			</div>

                                    </div>
                                    
                                </PublishingWebControls:EditModePanel>
                                <!--/Page Properties (edit mode only) -->

                                <div id="page-content" class="field">
                                    <PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" HasInitialFocus="True" MinimumEditHeight="400px" runat="server"/>
								</div>
												
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>