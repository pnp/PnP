# Page manipulation scenario#

### Summary ###
This sample shows how to add wiki pages to a SharePoint site, how to add/remove web parts and HTML content to the created wiki pages.

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
Provisioning.Pages | Bert Jansen (**Microsoft**) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
2.1  | August 5th 2015 | Nuget update
2.0  | March 21st 2014 | Documentation updates
1.0  | November 6th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Scenario 1: Basic Wiki page manipulation #
This scenario shows two basic steps: first step is creating a wiki page in a wiki library. The sample uses the SitePages library which is default available in non-publishing sites, but the same sample can be easily adapted to create pages in a custom wiki page library. In the last step the empty wiki page is filled with HTML content. In the sample you can specify your HTML content and then click the “Run scenario 1” button to create the wiki page and add the HTML:
![Add-in UI for scenario 1](http://i.imgur.com/FX5KtQX.png)

Once the page has been created the “here” link points to the created page. Clicking on the link brings you to the created page:
![UI of the newly created page](http://i.imgur.com/sudk0er.png)

## Create Wiki page ##
To create a wiki page the following code is used:
```C#
string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
string scenario1PageUrl = csomService.AddWikiPage("Site Pages", scenario1Page);
```

## Add HTML to a Wiki page ##
This is simply done by using the extension method AddHtmlToWikiPage from Office 365 PnP Core:
```C#
cc.Web.AddHtmlToWikiPage("SitePages", txtHtml.Text, scenario1Page);
```
Adding HTML to a wiki page is simple: just put your HTML in the WikiField cell:
```C#
listItem["WikiField"] = html;
```

# Scenario 2: Advanced Wiki page manipulation #

![Add-in UI for scenario 2](http://i.imgur.com/pTg47rx.png)

This scenario further extends the scenario 1 with the option to create wiki pages using different layouts:
-  One column
-  One column with sidebar
-  Two columns
-  Two columns with header
-  Two columns with header and footer
-  Three columns
-  Three columns with header
-  Three columns with header and footer

In the sample the existing OOB layouts are reused, but it’s perfectly possible to use custom layouts as well (e.g. a “four columns” layout). Once the layout is created we have actually created a table with rows and columns and optionally headers and footers. In a next step the sample shows how you can place either a web part or HTML content into a cell of the created table. When you click on “Run scenario 2” a wiki page will be created with the layout you’ve selected and:
-  An XsltListViewWebPart is placed in the first content row on column 1. If there’s no header this means row1, column 1. If there’s a layout with a header then this will be row 2, column 1. The XsltListView web part is created for a promoted links list which will be automatically created if it does not yet exist
-  If there are two or more colums than the HTML content listed for scenario 1 will be inserted in the column next to the web part

Once the page is created you can use the here link to show the created page. Below screenshot shows the page in edit mode so that you can see the used layout:
![UI of newly created page](http://i.imgur.com/Jw5A4RO.png)

Once a page has been created the button “Remove webpart from the last page created during scenario 2 run” is enabled. Clicking on this button removes the XsltListViewWebPart web part from the page.

## Apply layouts to a Wiki page ##
Below sample shows how to insert a two columns with header and footer layout into a created wiki page:
```C#
cc.Web.AddLayoutToWikiPage("SitePages", WikiPageLayout.TwoColumnsHeaderFooter, scenario2Page);
```
Technically this is nothing more than inserting the correct HTML table structure as HTML content. The HTML table structure for the OOB layouts is included in the Office 365 PnP Core library:
![Text layout options in code](http://i.imgur.com/ezwBsPv.png)

## Inserting a web part ##
To insert a web part you need to create a webpartentity class instance, fill it with the web part data and insert it:
```C#
Guid linksID = csomService.GetListID("Links");
WebPartEntity wp2 = new WebPartEntity();
wp2.WebPartXml = WpPromotedLinks(linksID, string.Format("{0}/Lists/{1}", Request.QueryString["SPHostUrl"], "Links"), string.Format("{0}/{1}", Request.QueryString["SPHostUrl"], scenario2PageUrl), "$Resources:core,linksList");
wp2.WebPartIndex = 1;
wp2.WebPartTitle = "Links";

cc.Web.AddWebPartToWikiPage("SitePages", wp2, scenario2Page, 1, 1, false);
```
1, 1 in the above sample tell the method to insert on row 1, column 1.

In above sample the webpart XML for the promoted links list is adapted to work for the created list:
```C#
private string WpPromotedLinks(Guid listID, string listUrl, string pageUrl, string title)
{
    StringBuilder wp = new StringBuilder(100);
    wp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
    wp.Append("<webParts>");
    wp.Append("	<webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\">");
    wp.Append("		<metaData>");
    wp.Append("			<type name=\"Microsoft.SharePoint.WebPartPages.XsltListViewWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" />");
    wp.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
    wp.Append("		</metaData>");
    wp.Append("		<data>");
    wp.Append("			<properties>");
    wp.Append("				<property name=\"ShowWithSampleData\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"Default\" type=\"string\" />");
    wp.Append("				<property name=\"NoDefaultStyle\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"CacheXslStorage\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"ViewContentTypeId\" type=\"string\" />");
    wp.Append("				<property name=\"XmlDefinitionLink\" type=\"string\" />");
    wp.Append("				<property name=\"ManualRefresh\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"ListUrl\" type=\"string\" />");
    wp.Append(String.Format("				<property name=\"ListId\" type=\"System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">{0}</property>", listID.ToString()));
    wp.Append(String.Format("				<property name=\"TitleUrl\" type=\"string\">{0}</property>", listUrl));
    wp.Append("				<property name=\"EnableOriginalValue\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"Direction\" type=\"direction\">NotSet</property>");
    wp.Append("				<property name=\"ServerRender\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"ViewFlags\" type=\"Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">None</property>");
    wp.Append("				<property name=\"AllowConnect\" type=\"bool\">True</property>");
    wp.Append(String.Format("				<property name=\"ListName\" type=\"string\">{0}</property>", ("{" + listID.ToString().ToUpper() + "}")));
    wp.Append("				<property name=\"ListDisplayName\" type=\"string\" />");
    wp.Append("				<property name=\"AllowZoneChange\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"ChromeState\" type=\"chromestate\">Normal</property>");
    wp.Append("				<property name=\"DisableSaveAsNewViewButton\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"ViewFlag\" type=\"string\" />");
    wp.Append("				<property name=\"DataSourceID\" type=\"string\" />");
    wp.Append("				<property name=\"ExportMode\" type=\"exportmode\">All</property>");
    wp.Append("				<property name=\"AutoRefresh\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"FireInitialRow\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"AllowEdit\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"Description\" type=\"string\" />");
    wp.Append("				<property name=\"HelpMode\" type=\"helpmode\">Modeless</property>");
    wp.Append("				<property name=\"BaseXsltHashKey\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"AllowMinimize\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"CacheXslTimeOut\" type=\"int\">86400</property>");
    wp.Append("				<property name=\"ChromeType\" type=\"chrometype\">Default</property>");
    wp.Append("				<property name=\"Xsl\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"JSLink\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"CatalogIconImageUrl\" type=\"string\">/_layouts/15/images/itgen.png?rev=26</property>");
    wp.Append("				<property name=\"SampleData\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"UseSQLDataSourcePaging\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"TitleIconImageUrl\" type=\"string\" />");
    wp.Append("				<property name=\"PageSize\" type=\"int\">-1</property>");
    wp.Append("				<property name=\"ShowTimelineIfAvailable\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"Width\" type=\"string\" />");
    wp.Append("				<property name=\"DataFields\" type=\"string\" />");
    wp.Append("				<property name=\"Hidden\" type=\"bool\">False</property>");
    wp.Append(String.Format("				<property name=\"Title\" type=\"string\">{0}</property>", title));
    wp.Append("				<property name=\"PageType\" type=\"Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">PAGE_NORMALVIEW</property>");
    wp.Append("				<property name=\"DataSourcesString\" type=\"string\" />");
    wp.Append("				<property name=\"AllowClose\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"InplaceSearchEnabled\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"WebId\" type=\"System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">00000000-0000-0000-0000-000000000000</property>");
    wp.Append("				<property name=\"Height\" type=\"string\" />");
    wp.Append("				<property name=\"GhostedXslLink\" type=\"string\">main.xsl</property>");
    wp.Append("				<property name=\"DisableViewSelectorMenu\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"DisplayName\" type=\"string\" />");
    wp.Append("				<property name=\"IsClientRender\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"XmlDefinition\" type=\"string\">");
    wp.Append(string.Format("&lt;View Name=\"{1}\" Type=\"HTML\" Hidden=\"TRUE\" ReadOnly=\"TRUE\" OrderedView=\"TRUE\" DisplayName=\"\" Url=\"{0}\" Level=\"1\" BaseViewID=\"1\" ContentTypeID=\"0x\" &gt;&lt;Query&gt;&lt;OrderBy&gt;&lt;FieldRef Name=\"TileOrder\" Ascending=\"TRUE\"/&gt;&lt;FieldRef Name=\"Modified\" Ascending=\"FALSE\"/&gt;&lt;/OrderBy&gt;&lt;/Query&gt;&lt;ViewFields&gt;&lt;FieldRef Name=\"Title\"/&gt;&lt;FieldRef Name=\"BackgroundImageLocation\"/&gt;&lt;FieldRef Name=\"Description\"/&gt;&lt;FieldRef Name=\"LinkLocation\"/&gt;&lt;FieldRef Name=\"LaunchBehavior\"/&gt;&lt;FieldRef Name=\"BackgroundImageClusterX\"/&gt;&lt;FieldRef Name=\"BackgroundImageClusterY\"/&gt;&lt;/ViewFields&gt;&lt;RowLimit Paged=\"TRUE\"&gt;30&lt;/RowLimit&gt;&lt;JSLink&gt;sp.ui.tileview.js&lt;/JSLink&gt;&lt;XslLink Default=\"TRUE\"&gt;main.xsl&lt;/XslLink&gt;&lt;Toolbar Type=\"Standard\"/&gt;&lt;/View&gt;</property>", pageUrl, ("{" + Guid.NewGuid().ToString() + "}")));
    wp.Append("				<property name=\"InitialAsyncDataFetch\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"AllowHide\" type=\"bool\">True</property>");
    wp.Append("				<property name=\"ParameterBindings\" type=\"string\">");
    wp.Append("  &lt;ParameterBinding Name=\"dvt_sortdir\" Location=\"Postback;Connection\"/&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"dvt_sortfield\" Location=\"Postback;Connection\"/&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"dvt_startposition\" Location=\"Postback\" DefaultValue=\"\"/&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"dvt_firstrow\" Location=\"Postback;Connection\"/&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"OpenMenuKeyAccessible\" Location=\"Resource(wss,OpenMenuKeyAccessible)\" /&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"open_menu\" Location=\"Resource(wss,open_menu)\" /&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"select_deselect_all\" Location=\"Resource(wss,select_deselect_all)\" /&gt;");
    wp.Append("            &lt;ParameterBinding Name=\"idPresEnabled\" Location=\"Resource(wss,idPresEnabled)\" /&gt;&lt;ParameterBinding Name=\"NoAnnouncements\" Location=\"Resource(wss,noXinviewofY_LIST)\" /&gt;&lt;ParameterBinding Name=\"NoAnnouncementsHowTo\" Location=\"Resource(wss,noXinviewofY_DEFAULT)\" /&gt;</property>");
    wp.Append("				<property name=\"DataSourceMode\" type=\"Microsoft.SharePoint.WebControls.SPDataSourceMode, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\">List</property>");
    wp.Append("				<property name=\"AutoRefreshInterval\" type=\"int\">60</property>");
    wp.Append("				<property name=\"AsyncRefresh\" type=\"bool\">False</property>");
    wp.Append("				<property name=\"HelpUrl\" type=\"string\" />");
    wp.Append("				<property name=\"MissingAssembly\" type=\"string\">Cannot import this Web Part.</property>");
    wp.Append("				<property name=\"XslLink\" type=\"string\" null=\"true\" />");
    wp.Append("				<property name=\"SelectParameters\" type=\"string\" />");
    wp.Append("			</properties>");
    wp.Append("		</data>");
    wp.Append("	</webPart>");
    wp.Append("</webParts>");
    return wp.ToString();
}
```
### Note: ###
The demo does not show how to add a web part to a layouts page, but this is possible using the AddWebPartToWebPartPage method:
```C#
public void AddWebPartToWebPartPage(WebPartEntity webPart, string page)
```

## Inserting HTML to a wiki page cell ##
```C#
cc.Web.AddHtmlToWikiPage("SitePages", txtHtml.Text, scenario2Page, 1, 2);
```
1, 2 in the above sample tell the method to insert on row 1, column 2.

## Deleting a web part ##
To delete a web part use the following code:
```C#
cc.Web.DeleteWebPart("SitePages", "Links", "LastPageName.aspx");
```
This method deletes the Links webpart from the page LastPageName.aspx which is stored in the wiki page library “SitePages”.
### Note: ###
This method can also be used to remove a web part from a layouts page. In this case you should specify an empty library name:
```C#
cc.Web.DeleteWebPart("", "About this blog", "Default.aspx");
```
This sample removes the “About this blog” web part from the blog site home page.
# Scenario cleanup #
If you play around with this sample you’ll have created a collection of pages in the sitepages library. Clicking on the “Cleanup created pages” button removes all these pages. Note that the created Promoted Links list will not be automatically deleted.

![Cleanup created pages button](http://i.imgur.com/TihwUb0.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Pages" />