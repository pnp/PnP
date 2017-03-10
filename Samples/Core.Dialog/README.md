# Show Add-Ins in a dialog #

### Summary ###
This scenario shows how you can show an add-in inside a dialog.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.Dialog | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | July 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
This sample scenario uses the OfficeDevPnP core library to inject links for opening a SharePoint add-in inside a dialog. We’ll show how to open the add-in inside a dialog from a custom action and how to do the same from a link on a SharePoint wiki page. The add-in that will be shown in a dialog is the same add-in that you’ll use to setup the demo, meaning that you’ll be able to experience how one and the same add-in can be used in a full page immersive experience (using the chromecontrol) and in a modal dialog experience. Some special attention has been given to the button click handling: the same OK and Cancel buttons behave differently when the add-in is shown in a dialog or as a full page immersive experience. Finally the add-in shows how you can use JSOM to obtain data from the host web regardless of the whether the add-in is shown in a dialog (=uses iframe) or not. Next chapters provide more details on this.

## ISDLG URL PARAMETER ##
To specify whether the add-in is shown in a dialog or not we’ve foreseen an additional URL parameter named IsDlg. If this one has a value of 1 then this is an indication that the add-in is shown in a dialog, value 0 indicates the default full page experience. This IsDlg parameter is added as additional query string: 

![AppManifest editor with URL parameter](http://i.imgur.com/GFWpp7m.png)

# SCENARIO 1: INSERT A CUSTOM ACTION TO OPEN THE ADD-IN IN A DIALOG FROM THE SITE SETTINGS MENU #

This scenario uses the OfficeDevPnP core method “AddCustomAction” to insert a custom action to the site actions menu of the hosting web. In order to open the add-in in a dialog it uses JavaScript instead of a static url for the url value of the custom action. In the JavaScript we make use of the SharePoint SP.UI.ModalDialog.showModalDialog class to show a modal dialog.

```C#
StringBuilder modelDialogScript = new StringBuilder(10);
modelDialogScript.Append("javascript:var dlg=SP.UI.ModalDialog.showModalDialog({url: '");
modelDialogScript.Append(String.Format("{0}", SetIsDlg("1")));
modelDialogScript.Append("', dialogReturnValueCallback:function(res, val) {} });");       

//Create a custom action
CustomActionEntity customAction = new CustomActionEntity()
{
  Title = "Office AMS Dialog sample",                
  Description = "Shows how to launch an add-in inside a dialog",
  Location = "Microsoft.SharePoint.StandardMenu",
  Group = "SiteActions",
  Sequence = 10000,
  Url = modelDialogScript.ToString(),
};

//Add the custom action to the site
cc.Web.AddCustomAction(customAction);

//SetIsDlg method constructs the add-in URL with the IsDlg parameter set
private string SetIsDlg(string isDlgValue)
{
    var urlParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());
    urlParams.Set("IsDlg", isDlgValue);
    return string.Format("{0}://{1}:{2}{3}?{4}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Request.Url.AbsolutePath, urlParams.ToString());
}
```

See [here](http://msdn.microsoft.com/en-us/library/office/bb802730(v=office.15).aspx) for more information on the custom action settings.

# SCENARIO 2: INSERT A SCRIPT EDITOR WEB PART TO OPEN THE ADD-IN IN A DIALOG FROM A SITE WIKI PAGE #
Here we use the OfficeDevPnP Core page and web part manipulation methods to create a new page and add a configured script editor web part to it.

```C#
string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
string scenario1PageUrl = cc.Web.AddWikiPage("Site Pages", scenario1Page);
cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.OneColumn, scenario1Page);
WebPartEntity scriptEditorWp = new WebPartEntity();
scriptEditorWp.WebPartXml = ScriptEditorWebPart();
scriptEditorWp.WebPartIndex = 1;
scriptEditorWp.WebPartTitle = "Script editor test"; 
cc.Web.AddWebPartToWikiPage("SitePages", scriptEditorWp, scenario1Page, 1, 1, false);
```

In above sample the WebPartXml of the script editor is generated via the ScriptEditorWebPart method which is shown below. Pay attention to the **Content** web part property as that's the one that contains the actual JavaScript that launches the add-in in a dialog.

```C#
private string ScriptEditorWebPart()
{
    StringBuilder sb = new StringBuilder(20);
    sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
    sb.Append("<webParts>");
    sb.Append("	<webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\">");
    sb.Append("		<metaData>");
    sb.Append("			<type name=\"Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" />");
    sb.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
    sb.Append("		</metaData>");
    sb.Append("		<data>");
    sb.Append("			<properties>");
    sb.Append("				<property name=\"ExportMode\" type=\"exportmode\">All</property>");
    sb.Append("				<property name=\"HelpUrl\" type=\"string\" />");
    sb.Append("				<property name=\"Hidden\" type=\"bool\">False</property>");
    sb.Append("				<property name=\"Description\" type=\"string\">Allows authors to insert HTML snippets or scripts.</property>");
    sb.Append("             <property name=\"Content\" type=\"string\">&lt;a id=\"newSiteLink\" onclick=\"javascript: var dlg=SP.UI.ModalDialog.showModalDialog({url:'" + HttpUtility.HtmlEncode(SetIsDlg("1")) + "', dialogReturnValueCallback:function(res, val) {} }); CancelEvent(event); return false;\" href=\"#\"&gt;Open in dialog&lt;/a&gt;");
    sb.Append("</property>");
    sb.Append("				<property name=\"CatalogIconImageUrl\" type=\"string\" />");
    sb.Append("				<property name=\"Title\" type=\"string\">Script Editor</property>");
    sb.Append("				<property name=\"AllowHide\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"AllowMinimize\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"AllowZoneChange\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"TitleUrl\" type=\"string\" />");
    sb.Append("				<property name=\"ChromeType\" type=\"chrometype\">None</property>");
    sb.Append("				<property name=\"AllowConnect\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"Width\" type=\"unit\" />");
    sb.Append("				<property name=\"Height\" type=\"unit\" />");
    sb.Append("				<property name=\"HelpMode\" type=\"helpmode\">Navigate</property>");
    sb.Append("				<property name=\"AllowEdit\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"TitleIconImageUrl\" type=\"string\" />");
    sb.Append("				<property name=\"Direction\" type=\"direction\">NotSet</property>");
    sb.Append("				<property name=\"AllowClose\" type=\"bool\">True</property>");
    sb.Append("				<property name=\"ChromeState\" type=\"chromestate\">Normal</property>");
    sb.Append("			</properties>");
    sb.Append("		</data>");
    sb.Append("	</webPart>");
    sb.Append("</webParts>");

    return sb.ToString();
}
```

# ENSURING THAT YOUR JSOM CODE WORKS, EVEN WHEN THE ADD-IN IS SHOWN IN A DIALOG #
When an add-in is running inside another add-in domain and it needs data from the host then we’re dealing with a cross domain call. To realize this one needs to use the ProxyWebRequestExecutorFactory class as shown below. This technique allows the add-in to make the cross domain call, regardless of whether the add-in is loaded as a dialog or not.

```C#
context = new SP.ClientContext(appWebUrl);
factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
context.set_webRequestExecutorFactory(factory);
appContextSite = new SP.AppContextSite(context, spHostUrl);
this.web = appContextSite.get_web();
```

See [here](http://msdn.microsoft.com/en-us/library/office/fp179927(v=office.15).aspx) for more information on cross domain calls.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Dialog" />