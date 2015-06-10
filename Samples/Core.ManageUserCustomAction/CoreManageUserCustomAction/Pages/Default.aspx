<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>
<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js"></script>
    
	<SharePoint:ScriptLink name="sp.runtime.js" runat="server" LoadAfterUI="true" Localizable="false"/>
	<SharePoint:ScriptLink name="sp.js" runat="server" LoadAfterUI="true" Localizable="false"/>
	<SharePoint:ScriptLink name="SP.UI.Dialog.js" runat="server" LoadAfterUI="true" Localizable="false"/>
	<SharePoint:ScriptLink name="SP.RequestExecutor.js" runat="server" LoadAfterUI="true" Localizable="false"/>
	<SharePoint:ScriptLink name="PickerTreeDialog.js" runat="server" LoadAfterUI="true" Localizable="false"/>
	<!-- Add your JavaScript to the following file -->
	<script type="text/javascript" src="../Scripts/Core.js"></script>
	<script type="text/javascript" src="../Scripts/Common.js"></script>
	<script type="text/javascript" src="../Scripts/Data.js"></script>
	<script type="text/javascript" src="../Scripts/App.js"></script>
	
	<!-- Add your CSS styles to the following file -->
	<link rel="Stylesheet" type="text/css" href="../Content/App.css" />
	

</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	Manage UserCustomAction
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div style="margin:10px">

<div id="dlxShowDeclarativeXML" title="Declarative XML">
<div id="myDialogText"></div>
</div>
</div>
	<div id="dlxFormMenu" style="margin:10px;float: left; width: 200px;">
	<label style="font-weight:bold">Custom Actions</label><span class="s4-clust ms-promotedActionButton-icon" style="width: 16px; height: 16px; overflow: hidden; display: inline-block; position: relative;"><img onclick="PnPApp.GetInit();" id="dlxRefresh" style="left: -200px; top: -48px; position: absolute;cursor: pointer;" alt="Refresh" src="/_layouts/15/images/spcommon.png?rev=31"></img></span><span onclick="PnPApp.GetInit();" style="cursor:pointer;" class="ms-promotedActionButton-text">Refresh</span>
		<select id="dlxUserCustomActionsOptions" size="30">
			
		</select>
	</div>
	<div class="dlxCustomActionFormCSS" id="dlxCustomActionForm">
		<div id="dlxCustomActionFormMenu">
		<input value="Clean Form" type="button" onclick="PnPCommon.CleanForm()" /><input value="New Action" type="button" onclick="PnPApp.NewCustomAction()" /><input value="Update" type="button" id="dlxUpdateUserCustomAction" onclick="    PnPApp.UpdateCustomAction()" disabled="disabled" /><input type="button" id="dlxDeleteUserCustomAction" onclick="PnPApp.DeleteUserCustomAction()" value="Delete" disabled="disabled" /><a href="http://msdn.microsoft.com/en-us/library/office/ee556258(v=office.14).aspx" target="_blank" style="cursor: hand;"><span style="height:16px;width:16px;position:relative;display:inline-block;overflow:hidden;" class="s4-clust"><img src="/_layouts/15/1033/images/spintl.png?rev=31" alt="Help" style="border: 0px; position: absolute; left: -19px; top: -1px;"/></span></a>
		</div>
		<div>
			<label title="Gets a value that specifies the identifier of the custom action.">
			ID:</label><input readonly="readonly" id="dlxCustomActionID" title="ID" type="text" disabled /><br/>
		</div>
		<div>
			<label title="Gets or sets the name of the custom action.">Name:</label><input  id="dlxCustomActionName" title="NAME" type="text" /><br/>
			Example: &quot;MyCustomAction.NewButton&quot; or create new Guid
		</div>

		<div style="vertical-align:middle">
			<label title="Gets or sets the display title of the custom action.">
			Title: </label><input id="dlxCustomActionTitle" title="" type="text" /><br/>
			Example: &quot;My Custom Action&quot;
		</div>
		
		<div style="vertical-align:middle">
			<label title="Gets or sets the description of the custom action.">
			Description: </label><input id="dlxCustomActionDescription" title="" type="text" /><br/>
			Example: &quot;Description of my Custom Action&quot;
		</div>
		<div style="vertical-align:middle">
			<label title="Gets or sets the URL of the image associated with the custom action.">
			Image Url: </label><input id="dlxCustomActionImageUrl" class="tags" title="" type="text" /><div class="PnPTokensImageUrl dropdown"></div><input type="submit" class="ms-input" style="width:60px;" value="Browser" id="BtnBrowseImageUrl" onclick="PnPApp.OpenAssetPortalBrowserDialog('Image', 'dlxCustomActionImageUrl', 'true'); return false;" />
			
            <br/>
			Example: &quot;/_layouts/images/imageAction.png&quot;

		</div>

		<div>
			<label title="Location:Gets or sets the location of the custom action. Group:Gets or sets a value that specifies an implementation-specific value that determines the position of the custom action in the page.">
			Location:</label><select id="dlxCustomActionLocation">
					<option value="-1">Select User Custom Action Location</option>
					<optgroup label="ScriptLink">
						<option title="Reference Custom JS in SharePoint" value="">
						ScriptLink</option>
					</optgroup>
					<optgroup label="Menu Custom Action Location">
						<option title="Location corresponds to the display form toolbar of lists." value="">
						DisplayFormToolbar</option>
						<option title="Location corresponds to the per-item edit control block (ECB) menu." value="">
						EditControlBlock</option>
						<option title="Location corresponds to the edit form toolbar of lists." value="">
						EditFormToolbar</option>
						<option title="Location corresponds to the new form toolbar of lists." value="">
						NewFormToolbar</option>
						<option title="Location corresponds to the toolbar in list views." value="">
						ViewToolbar</option>
					</optgroup>
					<optgroup label="CommandUI">
						<option title="Customization appears everywhere for the specified RegistrationId." value="">
						CommandUI.Ribbon</option>
						<option title="Customization appears when the list view Web Part is present." value="">
						CommandUI.Ribbon.ListView</option>
						<option title="Customization appears on the edit form." value="">
						CommandUI.Ribbon.EditForm</option>
						<option title="Customization appears on the new form." value="">
						CommandUI.Ribbon.NewForm</option>
						<option title="Customization appears on the display form." value="">
						CommandUI.Ribbon.DisplayForm</option>
					</optgroup>
					<optgroup label="Microsoft.SharePoint.StandardMenu">
						<option title="Actions menu in list and document library views." value="Microsoft.SharePoint.StandardMenu">
						ActionsMenu</option>
						<option title="Site Actions menu for surveys." value="Microsoft.SharePoint.StandardMenu">
						ActionsMenuForSurvey</option>
						<option title="Site Settings links for surveys." value="Microsoft.SharePoint.StandardMenu">
						SettingsMenuForSurvey</option>
						<option title="Site Actions menu." value="Microsoft.SharePoint.StandardMenu">
						SiteActions</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.ContentTypeSettings">
						<option title="Columns section on site collection Content Type page." value="Microsoft.SharePoint.ContentTypeSettings">
						Fields</option>
						<option title="Settings section on site collection Content Type page." value="Microsoft.SharePoint.ContentTypeSettings">
						General</option>
					</optgroup> 
					<optgroup  label="Microsoft.SharePoint.ContentTypeTemplateSettings">
						<option title="Columns section on List Content Type page." value="Microsoft.SharePoint.ContentTypeTemplateSettings">
						Fields</option>
						<option title="Settings section on List Content Type page." value="Microsoft.SharePoint.ContentTypeTemplateSettings">
						General</option>
					</optgroup> 
					<optgroup  label="Microsoft.SharePoint.Create">
						<option title="Web Pages section on Create page." value="Microsoft.SharePoint.Create">
						WebPages</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.GroupsPage">
						<option title="New menu on site collection People and Groups page." value="Microsoft.SharePoint.GroupsPage">
						NewMenu</option>
						<option title="Settings menu on site collection People and Groups page." value="Microsoft.SharePoint.GroupsPage">
						SettingsMenu</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.ListEdit">
						<option title="Communications section on Customize page for list or document library." value="Microsoft.SharePoint.ListEdit">
						Communications</option>
						<option title="General Settings section on Customize page for list." value="Microsoft.SharePoint.ListEdit">
						GeneralSettings</option>
						<option title="Permissions and Management section on Customize page for list or document library." value="Microsoft.SharePoint.ListEdit">
						Permissions</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.ListEdit.DocumentLibrary">
						<option title="General Settings section on Customize page for document library." value="Microsoft.SharePoint.ListEdit.DocumentLibrary">
						GeneralSettings</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.PeoplePage">
						<option title="Actions menu on site collection People and Groups page." value="Microsoft.SharePoint.PeoplePage">
						ActionsMenu</option>
						<option title="New menu on site collection People and Groups page." value="Microsoft.SharePoint.PeoplePage">
						NewMenu</option>
						<option title="Settings menu on site collection People and Groups page." value="Microsoft.SharePoint.PeoplePage">
						SettingsMenu</option>
					</optgroup>
					<optgroup  label="Microsoft.SharePoint.SiteSettings">
						<option title="Look and Feel section on Site Settings page." value="Microsoft.SharePoint.SiteSettings">
						Customization</option>
						<option title="Galleries section on Site Settings page." value="Microsoft.SharePoint.SiteSettings">
						Galleries</option>
						<option title="Site Administration section on Site Settings page." value="Microsoft.SharePoint.SiteSettings">
						SiteAdministration</option>
						<option title="Site Collection Administration section on Site Settings page." value="Microsoft.SharePoint.SiteSettings">
						SiteCollectionAdmin</option>
						<option title="Users and Permissions section on Site Settings page." value="Microsoft.SharePoint.SiteSettings">
						UsersAndPermissions</option>
					</optgroup>
				</select><br/>
				Example: EditFormToolbar <a href="http://msdn.microsoft.com/en-us/library/office/bb802730(v=office.15).aspx">
			link</a>
		</div>
		
		<div>
			<label title="Gets or sets the value that specifies the type of object associated with the custom action." >
			RegistrationType:</label><select id="dlxCustomActionRegistrationType">
				<option value="0" title="Enumeration whose values specify that the object association is not specified.">
				None</option>
				<option value="1" title="Enumeration whose values specify that the custom action is associated with a list.">
				List</option>
				<option value="2" title="Enumeration whose values specify that the custom action is associated with a content type.">
				ContentType</option>
				<option value="3" title="Enumeration whose values specify that the custom action is associated with a ProgID.">
				ProgId</option>
				<option value="4" title="Enumeration whose values specify that the custom action is associated with a file extension.">
				FileType</option>
			</select><br />
			Example: List, ContentType <a href="http://msdn.microsoft.com/en-us/library/office/ee549057(v=office.14).aspx" target="_blank">
			Link</a>
		</div>
		<div>
            
			<label title="Gets or sets the value that specifies the identifier of the object associated with the custom action.">
			RegistrationId:</label><input  id="dlxCustomActionRegistrationId" title="" value="" type="text" style="width:400px" /><input type="submit" class="ms-input" style="width:60px;display:none;" value="Browser" id="BtnBrowseListFolder" onclick="PnPCommon.LaunchTargetPicker(); return false;" />
            <div id="PnPListTemplates" class="dropdown">
            </div>
            <div id="PnPContentTypes" class="dropdown">
                <ul id="PnPContentTypePicker" >
                    <li><img src="../_layouts/15/images/pageLayoutHS.png" >
                    Content Types
                        <ul id="PnPContentType" >
                        </ul>
                    </li>
                    <li><img src="../_layouts/15/images/itgen.png?rev=23" >
                        Lists
                        <ul id="PnPLists">
                        </ul>
                    </li>
                </ul>
            </div>
            <div id="PnPProgId" class="dropdown">
            </div>
            <div id="PnPFileTypes" class="dropdown">
            </div>  
            <br/>
			Content Type Example: System 0x;Item 0x01;Document 0x0101;Link 
			0x0105;Contact 0x0106,Task 0x0108... <a href="http://msdn.microsoft.com/en-us/library/office/aa543822(v=office.14).aspx" target="_blank">
			Link</a><br/>
			List Example: 100 Generic list, 101 Document library, 109 Picture... <a href="http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.splisttemplatetype.aspx">Link</a>. Select List in <b>"Browser"</b> option.<br/>
			FileType Example: docx, pdf
		</div>
		
		<div>
			<label title="Gets or sets the value that specifies the permissions needed for the custom action.">
			Rights:</label><input class="Rights " id="dlxCustomActionRights" title="" type="text" /><div id="PnPRights" class="dropdown">
    </div><br/>
			Example: OpenItems,ApproveItems <a href="http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.spbasepermissions.aspx">
			link</a>
		</div>
		<div id="ZoneScriptBlock">
			<label title="Gets or sets the value that specifies the ECMAScript to be executed when the custom action is performed.">
			ScriptBlock: </label><textarea class="tags" id="dlxCustomActionScriptBlock" cols="20" rows="2"></textarea><div class="PnPTokensScriptBlock dropdown"></div><br/>
			JScript code example: alert(&quot;Test Script!&quot;);
		</div>
		<div id="ZoneScriptSrc">
			<label title="Gets or sets a value that specifies the URI of a file which contains the ECMAScript to execute on the page">
			ScriptSrc: </label><input class="tags" id="dlxCustomActionScriptSrc" title="" type="text" /><input type="submit" class="ms-input" style="width:60px;" value="Browser" id="BtnBrowseScriptSrc" onclick="PnPApp.OpenAssetPortalBrowserDialog('JS', 'dlxCustomActionScriptSrc', 'true'); return false;" /><div class="PnPTokensScriptSrc dropdown"></div><br/>
			Example: ~SiteCollection/SiteAssets/Alert.js
		</div>
		<div>
			<label title="Gets or sets the value that specifies an implementation-specific value that determines the order of the custom action that appears on the page.">
			Sequence: </label><input id="dlxCustomActionSequence" title="" type="text" /><br/>
			Example: 0 - 99999
		</div>
		<div>
			<label title="Gets or sets the URL, URI, or ECMAScript (JScript, JavaScript) function associated with the action." >
			Url Action:</label><input id="dlxCustomActionUrl" class="tags" title="" type="text" /><div class="PnPTokensUrl dropdown"></div>
			<br/>example: 
			&quot;~site/_layouts/ItemAudit.aspx?ID={ItemId}&amp;List={ListId}&quot; dlxCustomActionImageUrl 
			javascript: 
			&quot;:SP.UI.ModalDialog.ShowPopupDialog(&#39;/_layouts/settings.aspx&#39;);&quot;
		</div>
		<div style="">
			<label title="Gets or sets a value that specifies an implementation specific XML fragment that determines user interface properties of the custom action.">
			Command UI Extension: </label><textarea id="dlxCustomActionCommandUIExtension" class="tags" cols="20" rows="2"></textarea><div class="PnPTokensUIExtension dropdown"></div>
			<br/>Server Ribbon XML Example: <a href="http://msdn.microsoft.com/en-us/library/ff407290(v=office.14).aspx" target="_blank">Link</a>
			<br/>Declarative Customization of the Server Ribbon: <a href="http://msdn.microsoft.com/en-us/library/ff407268(v=office.14).aspx" target="_blank">Link</a>
			<br/>Default Server Ribbon Customization Locations: <a href="http://msdn.microsoft.com/en-us/library/ee537543(v=office.14).aspx" target="_blank">Link</a>
		</div>
		<div>
			<label title="Gets a value that specifies an implementation specific version identifier.">
			Version of UCA: </label><input id="dlxCustomActionVersionOfUserCustomAction" disabled="disabled" title="" type="text" />
		</div>
</div>
    
</asp:Content>