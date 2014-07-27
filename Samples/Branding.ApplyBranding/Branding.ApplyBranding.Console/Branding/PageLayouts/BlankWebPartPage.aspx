<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceholderID="PlaceHolderAdditionalPageHead" runat="server">
	<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server" __designer:Preview="&lt;link rel=&quot;stylesheet&quot; type=&quot;text/css&quot; href=&quot;/sites/pub/Style%20Library/en-US/Themable/Core%20Styles/pagelayouts15.css&quot;/&gt;
" __designer:Values="&lt;P N=&#39;Name&#39; Bound=&#39;True&#39; T=&#39;SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css&#39; /&gt;&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl00&#39; /&gt;&lt;P N=&#39;EnableCssTheming&#39; Serial=&#39;AAEAAAD/////AQAAAAAAAAAEAQAAAA5TeXN0ZW0uQm9vbGVhbgEAAAAHbV92YWx1ZQABAQs&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
	<PublishingWebControls:EditModePanel runat="server" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&amp;#xD;&amp;#xA;		&amp;lt;!-- Styles for edit mode only--&amp;gt;&amp;#xD;&amp;#xA;		&amp;lt;SharePointWebControls:CssRegistration name=&amp;quot;&amp;lt;% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %&amp;gt;&amp;quot;&amp;#xD;&amp;#xA;			After=&amp;quot;&amp;lt;% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %&amp;gt;&amp;quot; runat=&amp;quot;server&amp;quot; __designer:Preview=&amp;quot;&amp;amp;lt;link rel=&amp;amp;quot;stylesheet&amp;amp;quot; type=&amp;amp;quot;text/css&amp;amp;quot; href=&amp;amp;quot;/sites/pub/Style%20Library/en-US/Themable/Core%20Styles/editmode15.css&amp;amp;quot;/&amp;amp;gt;&amp;#xA;&amp;quot; __designer:Values=&amp;quot;&amp;amp;lt;P N=&amp;amp;#39;Name&amp;amp;#39; Bound=&amp;amp;#39;True&amp;amp;#39; T=&amp;amp;#39;SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;After&amp;amp;#39; Bound=&amp;amp;#39;True&amp;amp;#39; T=&amp;amp;#39;SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;ID&amp;amp;#39; T=&amp;amp;#39;ctl00&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;EnableCssTheming&amp;amp;#39; Serial=&amp;amp;#39;AAEAAAD/////AQAAAAAAAAAEAQAAAA5TeXN0ZW0uQm9vbGVhbgEAAAAHbV92YWx1ZQABAQs&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;InDesign&amp;amp;#39; T=&amp;amp;#39;False&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;Page&amp;amp;#39; ID=&amp;amp;#39;1&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;TemplateControl&amp;amp;#39; R=&amp;amp;#39;1&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;AppRelativeTemplateSourceDirectory&amp;amp;#39; R=&amp;amp;#39;-1&amp;amp;#39; /&amp;amp;gt;&amp;quot;/&amp;gt;&amp;#xD;&amp;#xA;	&quot; /&gt;&lt;/Regions&gt;
		&lt;!-- Styles for edit mode only--&gt;
		&lt;SharePointWebControls:CssRegistration name=&quot;&lt;% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %&gt;&quot;
			After=&quot;&lt;% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %&gt;&quot; runat=&quot;server&quot;/&gt;
	" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl01&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;&lt;P N=&#39;Visible&#39; T=&#39;False&#39; /&gt;">
		<!-- Styles for edit mode only-->
		<SharePointWebControls:CssRegistration name="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/editmode15.css %>"
			After="<% $SPUrl:~sitecollection/Style Library/~language/Themable/Core Styles/pagelayouts15.css %>" runat="server"/>
	</PublishingWebControls:EditModePanel>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:ListProperty Property="Title" runat="server" __designer:Preview="Master Page Gallery" __designer:Values="&lt;P N=&#39;Property&#39; T=&#39;Title&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl02&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/> - <SharePointWebControls:FieldValue FieldName="Title" runat="server" __designer:Preview="Blank Web Part page" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl03&#39; /&gt;&lt;P N=&#39;ItemFieldValue&#39; ID=&#39;2&#39; Serial=&#39;AAEAAAD/////AQAAAAAAAAAGAQAAABNCbGFuayBXZWIgUGFydCBwYWdlCw&#39; /&gt;&lt;P N=&#39;ListItemFieldValue&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Visible&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;FieldName&#39; T=&#39;Title&#39; /&gt;&lt;P N=&#39;ControlMode&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	G-Apps&nbsp;<SharePointWebControls:FieldValue FieldName="Title" runat="server" __designer:Preview="Blank Web Part page" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl04&#39; /&gt;&lt;P N=&#39;ItemFieldValue&#39; ID=&#39;2&#39; Serial=&#39;AAEAAAD/////AQAAAAAAAAAGAQAAABNCbGFuayBXZWIgUGFydCBwYWdlCw&#39; /&gt;&lt;P N=&#39;ListItemFieldValue&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Visible&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;FieldName&#39; T=&#39;Title&#39; /&gt;&lt;P N=&#39;ControlMode&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderTitleBreadcrumb" runat="server"> <SharePointWebControls:ListSiteMapPath runat="server" SiteMapProviders="CurrentNavigationSwitchableProvider" RenderCurrentNodeAsLink="false" PathSeparator="" CssClass="s4-breadcrumb" NodeStyle-CssClass="s4-breadcrumbNode" CurrentNodeStyle-CssClass="s4-breadcrumbCurrentNode" RootNodeStyle-CssClass="s4-breadcrumbRootNode" NodeImageOffsetX=0 NodeImageOffsetY=289 NodeImageWidth=16 NodeImageHeight=16 NodeImageUrl="/_layouts/15/images/fgimg.png?rev=33" HideInteriorRootNodes="true" SkipLinkText="" __designer:Preview="&lt;ul class=&quot;s4-breadcrumb&quot;&gt;

&lt;/ul&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl05&#39; /&gt;&lt;P N=&#39;CssClass&#39; T=&#39;s4-breadcrumb&#39; /&gt;&lt;P N=&#39;PathSeparator&#39; R=&#39;-1&#39; /&gt;&lt;P N=&#39;SiteMapProviders&#39; T=&#39;CurrentNavigationSwitchableProvider&#39; /&gt;&lt;P N=&#39;HideInteriorRootNodes&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;NodeImageOffsetY&#39; T=&#39;289&#39; /&gt;&lt;P N=&#39;NodeImageWidth&#39; T=&#39;16&#39; /&gt;&lt;P N=&#39;NodeImageHeight&#39; T=&#39;16&#39; /&gt;&lt;P N=&#39;NodeImageUrl&#39; T=&#39;/_layouts/15/images/fgimg.png?rev=33&#39; /&gt;&lt;P N=&#39;SkipLinkText&#39; R=&#39;-1&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;NodeTemplate&quot;&gt;&lt;Template Name=&quot;NodeTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;&lt;Group Name=&quot;CurrentNodeTemplate&quot;&gt;&lt;Template Name=&quot;CurrentNodeTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;&lt;Group Name=&quot;RootNodeTemplate&quot;&gt;&lt;Template Name=&quot;RootNodeTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;&lt;Group Name=&quot;PathSeparatorTemplate&quot;&gt;&lt;Template Name=&quot;PathSeparatorTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"/> </asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
	<SharePointWebControls:ProjectProperty Property="Description" runat="server" __designer:Preview="" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl07&#39; /&gt;&lt;P N=&#39;Property&#39; T=&#39;Description&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderBodyRightMargin" runat="server">
	<div height=100% class="ms-pagemargin"><IMG SRC="/_layouts/images/blank.gif" width=10 height=1 alt=""></div>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
	<div class="welcome blank-wp" style="border: 1px solid #848484">
		<PublishingWebControls:EditModePanel runat="server" CssClass="edit-mode-panel title-edit" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&amp;#xD;&amp;#xA;			&amp;lt;SharePointWebControls:TextField runat=&amp;quot;server&amp;quot; FieldName=&amp;quot;Title&amp;quot; __designer:Preview=&amp;quot;&amp;amp;lt;div align=&amp;amp;quot;left&amp;amp;quot; class=&amp;amp;quot;ms-formfieldcontainer&amp;amp;quot;&amp;amp;gt;&amp;amp;lt;div class=&amp;amp;quot;ms-formfieldlabelcontainer&amp;amp;quot; nowrap=&amp;amp;quot;nowrap&amp;amp;quot;&amp;amp;gt;&amp;amp;lt;span class=&amp;amp;quot;ms-formfieldlabel&amp;amp;quot; nowrap=&amp;amp;quot;nowrap&amp;amp;quot;&amp;amp;gt;Title&amp;amp;lt;/span&amp;amp;gt;&amp;amp;lt;/div&amp;amp;gt;&amp;amp;lt;div class=&amp;amp;quot;ms-formfieldvaluecontainer&amp;amp;quot;&amp;amp;gt;Blank Web Part page&amp;amp;lt;/div&amp;amp;gt;&amp;amp;lt;/div&amp;amp;gt;&amp;quot; __designer:Values=&amp;quot;&amp;amp;lt;P N=&amp;amp;#39;ID&amp;amp;#39; T=&amp;amp;#39;ctl00&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;ItemFieldValue&amp;amp;#39; ID=&amp;amp;#39;1&amp;amp;#39; Serial=&amp;amp;#39;AAEAAAD/////AQAAAAAAAAAGAQAAABNCbGFuayBXZWIgUGFydCBwYWdlCw&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;ListItemFieldValue&amp;amp;#39; R=&amp;amp;#39;1&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;Visible&amp;amp;#39; T=&amp;amp;#39;True&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;FieldName&amp;amp;#39; T=&amp;amp;#39;Title&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;ControlMode&amp;amp;#39; E=&amp;amp;#39;1&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;InDesign&amp;amp;#39; T=&amp;amp;#39;False&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;Page&amp;amp;#39; ID=&amp;amp;#39;2&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;TemplateControl&amp;amp;#39; R=&amp;amp;#39;2&amp;amp;#39; /&amp;amp;gt;&amp;amp;lt;P N=&amp;amp;#39;AppRelativeTemplateSourceDirectory&amp;amp;#39; R=&amp;amp;#39;-1&amp;amp;#39; /&amp;amp;gt;&amp;quot;/&amp;gt;&amp;#xD;&amp;#xA;		&quot; /&gt;&lt;/Regions&gt;
			&lt;SharePointWebControls:TextField runat=&quot;server&quot; FieldName=&quot;Title&quot;/&gt;
		" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl08&#39; /&gt;&lt;P N=&#39;CssClass&#39; T=&#39;edit-mode-panel title-edit&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;&lt;P N=&#39;Visible&#39; T=&#39;False&#39; /&gt;">
			<SharePointWebControls:TextField runat="server" FieldName="Title"/>
		</PublishingWebControls:EditModePanel>
		<div class="welcome-content">
			<PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" HasInitialFocus="True" MinimumEditHeight="400px" runat="server" __designer:Preview="&lt;div id=&quot;ctl09_label&quot; style=&#39;display:none&#39;&gt;Page Content&lt;/div&gt;&lt;div id=&quot;ctl09__ControlWrapper_RichHtmlField&quot; class=&quot;ms-rtestate-field&quot; style=&quot;display:inline&quot; aria-labelledby=&quot;ctl09_label&quot;&gt;&lt;div align=&quot;left&quot; class=&quot;ms-formfieldcontainer&quot;&gt;&lt;div class=&quot;ms-formfieldlabelcontainer&quot; nowrap=&quot;nowrap&quot;&gt;&lt;span class=&quot;ms-formfieldlabel&quot; nowrap=&quot;nowrap&quot;&gt;Page Content&lt;/span&gt;&lt;/div&gt;&lt;div class=&quot;ms-formfieldvaluecontainer&quot;&gt;&lt;div class=&quot;ms-rtestate-field&quot;&gt;Page Content field value. Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.&lt;/div&gt;&lt;/div&gt;&lt;/div&gt;&lt;/div&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl09&#39; /&gt;&lt;P N=&#39;V4RTEClientId&#39; T=&#39;ctl09_DefaultDataHolderHtmlEditorControl_displayContent&#39; /&gt;&lt;P N=&#39;ItemFieldValue&#39; ID=&#39;2&#39; Serial=&#39;AAEAAAD/////AQAAAAAAAAAGAQAAAP0DPGRpdiBjbGFzcz0ibXMtcnRlc3RhdGUtZmllbGQiPlBhZ2UgQ29udGVudCBmaWVsZCB2YWx1ZS4gTG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2ljaW5nIGVsaXQsIHNlZCBkbyBlaXVzbW9kIHRlbXBvciBpbmNpZGlkdW50IHV0IGxhYm9yZSBldCBkb2xvcmUgbWFnbmEgYWxpcXVhLiBVdCBlbmltIGFkIG1pbmltIHZlbmlhbSwgcXVpcyBub3N0cnVkIGV4ZXJjaXRhdGlvbiB1bGxhbWNvIGxhYm9yaXMgbmlzaSB1dCBhbGlxdWlwIGV4IGVhIGNvbW1vZG8gY29uc2VxdWF0LiBEdWlzIGF1dGUgaXJ1cmUgZG9sb3IgaW4gcmVwcmVoZW5kZXJpdCBpbiB2b2x1cHRhdGUgdmVsaXQgZXNzZSBjaWxsdW0gZG9sb3JlIGV1IGZ1Z2lhdCBudWxsYSBwYXJpYXR1ci4gRXhjZXB0ZXVyIHNpbnQgb2NjYWVjYXQgY3VwaWRhdGF0IG5vbiBwcm9pZGVudCwgc3VudCBpbiBjdWxwYSBxdWkgb2ZmaWNpYSBkZXNlcnVudCBtb2xsaXQgYW5pbSBpZCBlc3QgbGFib3J1bS48L2Rpdj4L&#39; /&gt;&lt;P N=&#39;MinimumEditHeight&#39; T=&#39;400px&#39; /&gt;&lt;P N=&#39;HasInitialFocus&#39; Serial=&#39;AAEAAAD/////AQAAAAAAAAAEAQAAAA5TeXN0ZW0uQm9vbGVhbgEAAAAHbV92YWx1ZQABAQs&#39; /&gt;&lt;P N=&#39;HideWikiLabel&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Visible&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;CssClass&#39; T=&#39;ms-long ms-rtestate-field ms-rtestate-field ms-rtestate-field&#39; /&gt;&lt;P N=&#39;ListItemFieldValue&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;FieldName&#39; T=&#39;PublishingPageContent&#39; /&gt;&lt;P N=&#39;ControlMode&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;InDesign&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
		</div>
		<div class="ms-table ms-fullWidth">
			<div class="tableCol-75">
				<div class="cell-margin">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Header%>" ID="Header" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;Header&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Header&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;Header&#39; /&gt;&lt;P N=&#39;HeaderText&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_Header&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>
				<div class="ms-table ms-fullWidth">
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_TopLeft%>" ID="TopLeftRow" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;TopLeftRow&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Top Left&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;TopLeftRow&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Top Left&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_TopLeft&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
					<div class="cell-margin tableCol-50">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_TopRight%>" ID="TopRightRow" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;TopRightRow&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Top Right&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;TopRightRow&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Top Right&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_TopRight&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
				</div>
				<div class="ms-table ms-fullWidth">
					<div class="cell-margin tableCol-33">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_CenterLeft%>" ID="CenterLeftColumn" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;CenterLeftColumn&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Center Left&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;CenterLeftColumn&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Center Left&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_CenterLeft&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
					<div class="cell-margin tableCol-33">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Center%>" ID="CenterColumn" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;CenterColumn&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Center&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;CenterColumn&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Center&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_Center&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
					<div class="cell-margin tableCol-33">
						<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_CenterRight%>" ID="CenterRightColumn" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;CenterRightColumn&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Center Right&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;CenterRightColumn&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Center Right&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_CenterRight&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
					</div>
				</div>
				<div class="cell-margin">
					<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Footer%>" ID="Footer" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;Footer&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Footer&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;Footer&#39; /&gt;&lt;P N=&#39;HeaderText&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_Footer&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
				</div>
			</div>
			<div class="cell-margin tableCol-25">
			<WebPartPages:WebPartZone runat="server" Title="<%$Resources:cms,WebPartZoneTitle_Right%>" ID="RightColumn" Orientation="Vertical" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;RightColumn&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Right&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;RightColumn&#39; /&gt;&lt;P N=&#39;HeaderText&#39; ID=&#39;2&#39; T=&#39;Right&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;Title&#39; Bound=&#39;True&#39; T=&#39;Resources:cms,WebPartZoneTitle_Right&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
			</div>
			<SharePointWebControls:ScriptBlock runat="server" __designer:Preview="&lt;script type=&quot;text/javascript&quot;&gt;// &lt;![CDATA[ 

if(typeof(MSOLayout_MakeInvisibleIfEmpty) == &quot;function&quot;) {MSOLayout_MakeInvisibleIfEmpty();}// ]]&gt;
&lt;/script&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl10&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;">if(typeof(MSOLayout_MakeInvisibleIfEmpty) == "function") {MSOLayout_MakeInvisibleIfEmpty();}</SharePointWebControls:ScriptBlock>
		</div>
	</div>
</asp:Content>
