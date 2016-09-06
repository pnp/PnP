# Site classification #

### Summary ###
This sample demonstrates how to implement a site classification solution using various samples found in Patterns and Practices as well as leverage Site Policies.

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
Core.SiteClassification | Brian Michely, Vesa Juvonen, Bert Jansen, Frank Marasco (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | July 21th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
Even with good governance, SharePoint sites can proliferate and grow out of control. Sites are created as they are needed, but sites are rarely deleted. Many organization have search crawl burdened by unused site collections, difficulty with outdated and irrelevant results. With site classification it allows sensitive data in the environment to be identified. In this scenario, will demonstrate on how to implement a site classification solution that leverages many of the existing Patterns and Practices samples. This solution will also leverage SharePoint Site Policies to enforce the deletion. This solution can also be integrated into your existing Site Provisioning solution to form a unified solution for your governance needs.

# SETUP #
First of we need to define the site policies that will be available in all your sites collections. We are going to define the Site Policies in the content type hub and publish. In this example we are using SharePoint Online MT, but this same approach is available in SharePoint Online Dedicated as well as SharePoint on-premises. If your environment is hosted in SharePoint Online MT, your content type hub would be located at the following URL. https://[tenanatname]/sites/contentTypeHub. Navigate to Settings, then Site Policies under Site Collection Administration, and then finally create. 

See **Overview of site policies in SharePoint 2013** at http://technet.microsoft.com/en-US/library/jj219569(v=office.15).aspx for an overview of Site Policies.

We are going to create three site policies, HBI, MBI and then LBI.  Create an HBI Policy that mimics the below screen.

![Creation of new policies](http://i.imgur.com/sKI5csC.png)

Repeat the above setup 2 more times for MBI and LBI. You should end up with the below:

![List of policy entries](http://i.imgur.com/QUqir7I.png)

Once we have the policies we are going to publish the Site Policies. 

# SCENARIO 1: INSERT A CUSTOM ACTION #
Here we are going to add the custom action to the Settings page and the SharePoint gear. That is only available to users with ManageWeb Permission.

![Custom option in the site actions menu](http://i.imgur.com/ix5lU4b.png)

```C#
/// <summary>
/// Adds a custom Action to a Site Collection
/// </summary>
/// <param name="ctx">The Authenticaed client context.</param>
/// <param name="hostUrl">The Provider hosted URL for the Application</param>
static void AddCustomAction(ClientContext ctx, string hostUrl)
{
    var _web = ctx.Web;
    ctx.Load(_web);
    ctx.ExecuteQuery();

    //we only want the action to show up if you have manage web permissions
    BasePermissions _manageWebPermission = new BasePermissions();
    _manageWebPermission.Set(PermissionKind.ManageWeb);

    CustomActionEntity _entity = new CustomActionEntity()
    {
        Group = "SiteTasks",
        Location = "Microsoft.SharePoint.SiteSettings",
        Title = "Site Classification",
        Sequence = 1000,
        Url = string.Format(hostUrl, ctx.Url),
        Rights = _manageWebPermission,
    };

    CustomActionEntity _siteActionSC = new CustomActionEntity()
    {
        Group = "SiteActions",
        Location = "Microsoft.SharePoint.StandardMenu",
        Title = "Site Classification",
        Sequence = 1000,
        Url = string.Format(hostUrl, ctx.Url),
        Rights = _manageWebPermission
    };
    _web.AddCustomAction(_entity);
    _web.AddCustomAction(_siteActionSC);
}
```

See [here](http://msdn.microsoft.com/en-us/library/office/bb802730(v=office.15).aspx) for more information on the custom action settings.

# SCENARIO 2: CUSTOM SITE CLASSIFICATION #

![Policy option](http://i.imgur.com/MKETcx9.png)

This pages defines the necessary options that are available. Here we define the intended Audience reach of the Site as well as the defined Site Policy. We also show the Expiration date of the site, which is based on the site policy that you created earlier. Both the audience reach and Site classification are searchable and will have managed properties associated after a crawl has taken place. You can then use these properties to search for specific types of sites from. 

![UI of policy options](http://i.imgur.com/rfCqoyW.png)

These are searchable via a custom hidden list that is implemented in the Site Collection. This implement in the Core.SiteClassification.Common project in the SiteManagerImpl class.

```C#
private void CreateSiteClassificationList(ClientContext ctx)
{
    var _newList = new ListCreationInformation()
    {
        Title = SiteClassificationList.SiteClassificationListTitle,
        Description = SiteClassificationList.SiteClassificationDesc,
        TemplateType = (int)ListTemplateType.GenericList,
        Url = SiteClassificationList.SiteClassificationUrl,
        QuickLaunchOption = QuickLaunchOptions.Off
    };

    if(!ctx.Web.ContentTypeExistsById(SiteClassificationContentType.SITEINFORMATION_CT_ID))
    {
        //ct
        ContentType _contentType = ctx.Web.CreateContentType(SiteClassificationContentType.SITEINFORMATION_CT_NAME,
            SiteClassificationContentType.SITEINFORMATION_CT_DESC,
            SiteClassificationContentType.SITEINFORMATION_CT_ID,
            SiteClassificationContentType.SITEINFORMATION_CT_GROUP);

        FieldLink _titleFieldLink = _contentType.FieldLinks.GetById(new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"));
        _titleFieldLink.Required = false;
        _contentType.Update(false);

        //Key Field
        FieldCreationInformation fldCreate = new FieldCreationInformation(FieldType.Text)
        {
            Id = SiteClassificationFields.FLD_KEY_ID,
            InternalName = SiteClassificationFields.FLD_KEY_INTERNAL_NAME,
            DisplayName = SiteClassificationFields.FLD_KEY_DISPLAY_NAME,
            Group = SiteClassificationFields.FIELDS_GROUPNAME,
        };
        ctx.Web.CreateField(fldCreate);

        //value field
        fldCreate = new FieldCreationInformation(FieldType.Text)
        {
            Id = SiteClassificationFields.FLD_VALUE_ID,
            InternalName = SiteClassificationFields.FLD_VALUE_INTERNAL_NAME,
            DisplayName = SiteClassificationFields.FLD_VALUE_DISPLAY_NAME,
            Group = SiteClassificationFields.FIELDS_GROUPNAME,
        };
        ctx.Web.CreateField(fldCreate);

        //Add Key Field to content type
        ctx.Web.AddFieldToContentTypeById(SiteClassificationContentType.SITEINFORMATION_CT_ID, 
            SiteClassificationFields.FLD_KEY_ID.ToString(), 
            true);
        //Add Value Field to content type
        ctx.Web.AddFieldToContentTypeById(SiteClassificationContentType.SITEINFORMATION_CT_ID,
            SiteClassificationFields.FLD_VALUE_ID.ToString(),
            true);
    }
    var _list = ctx.Web.Lists.Add(_newList);
    _list.Hidden = true;
    _list.ContentTypesEnabled = true;
    _list.Update();
    ctx.Web.AddContentTypeToListById(SiteClassificationList.SiteClassificationListTitle, SiteClassificationContentType.SITEINFORMATION_CT_ID, true);
    this.CreateCustomPropertiesInList(_list);
    ctx.ExecuteQuery();
    this.RemoveFromQuickLaunch(ctx, SiteClassificationList.SiteClassificationListTitle);

}

```

By default, when you create list either using out-of-box or if you are using CSOM, the list will be available in the Recent menu. Now, we don’t want that right? It’s supposed to be hidden. We need some simple code to remove the item from the recent menu. 
See for more information:  http://blogs.technet.com/b/speschka/archive/2014/05/07/create-a-list-in-the-host-web-when-your-sharepoint-app-is-installed-and-remove-it-from-the-recent-stuff-list.aspx

```C#
private void RemoveFromQuickLaunch(ClientContext ctx, string listName)
{
    Site _site = ctx.Site;
    Web _web = _site.RootWeb;

    ctx.Load(_web, x => x.Navigation, x => x.Navigation.QuickLaunch);
    ctx.ExecuteQuery();

    var _vNode = from NavigationNode _navNode in _web.Navigation.QuickLaunch
                 where _navNode.Title == "Recent"
                 select _navNode;

    NavigationNode _nNode = _vNode.First<NavigationNode>();
    ctx.Load(_nNode.Children);
    ctx.ExecuteQuery();

    var vcNode = from NavigationNode cn in _nNode.Children
                 where cn.Title == listName
                 select cn;
    NavigationNode _cNode = vcNode.First<NavigationNode>();
    _cNode.DeleteObject();
    ctx.ExecuteQuery();    
}
```

So you’re probably thinking that site admin or someone with permission can remove that list. Well we thought about that too. When this page is accessed we will create the recreate the list, however in the sample we don’t set the properties back. If the values in the list are not present, we know that someone deleted the list and you can leverage the Patterns and Practices sample Core.SiteEnumeration to do checks on the list and send nasty emails to your site administrators. You may also extend this sample and modify the permissions on the list so that only site collections administrators have access. 

The list verification check is also implemented in the SiteManagerImpl in the Initialize member:

```C#
internal void Initialize(ClientContext ctx)
{
    try 
    {
        var _web = ctx.Web;
        var lists = _web.Lists;
        ctx.Load(_web);
        ctx.Load(lists, lc => lc.Where(l => l.Title == SiteClassificationList.SiteClassificationListTitle));
        ctx.ExecuteQuery();
          
        if (lists.Count == 0) 
        {
            this.CreateSiteClassificationList(ctx); 
        }
    }
    catch(Exception _ex)
    {

    }
}
```

# SCENARIO 3: SITE CLASSIFICATION DISPLAY #
In the final scenario, we are going to add an indicator. I chose to inject an image next to the Site Title. In the old days we would use a Delegate control or a custom master page and add some JavaScript. Delegate controls are Server Side implemented we don’t want to use that. Modifying the master page would work, but I chose our friendly JavaScript injection pattern.  When you change the Site Policy in the Edit Site Information page, this will change the site indicator like below.

## LBI ##

![LBI icon in the site welcome page](http://i.imgur.com/yMpuyhR.png)

## MBI ##

![MBI icon in the site welcome page](http://i.imgur.com/abGbUMk.png)

## HBI ##

![HBI icon in the site welcome page](http://i.imgur.com/rpDpO6K.png)

The below method is defined in the Core.SiteClassificationWeb project, scripts and classifier.js. I chose to store the images in an Azure Web Site, so you will have to change the URL’s to match your environment. Maybe in the next release I can remove the hard-coded urls. 

```JavaScript
function setClassifier() {
    if (!classified)
    {
        var clientContext = SP.ClientContext.get_current();
        var query = "<View><Query><Where><Eq><FieldRef Name='SC_METADATA_KEY'/><Value Type='Text'>sc_BusinessImpact</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID'/><FieldRef Name='SC_METADATA_KEY'/><FieldRef Name='SC_METADATA_VALUE'/></ViewFields></View>";
        var list = clientContext.get_web().get_lists().getByTitle("Site Information");
        clientContext.load(list);
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(query);
        var listItems = list.getItems(camlQuery);
        clientContext.load(listItems);

        clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            var listItemInfo;
            var listItemEnumerator = listItems.getEnumerator();

            while (listItemEnumerator.moveNext()) {
                listItemInfo = listItemEnumerator.get_current().get_item('SC_METADATA_VALUE');
                
                var pageTitle = $('#pageTitle')[0].innerHTML;
                if (pageTitle.indexOf("img") > -1) {
                    classified = true;
                }
                else {
                    var siteClassification = listItemInfo;
                    if (siteClassification == "HBI") {
                        var img = $("<a href='http://insertyourpolicy' target=_blank><img id=classifer name=classifer src='https://spmanaged.azurewebsites.net/content/img/hbi.png' title='Site contains personally identifiable information (PII), or unauthorized release of information on this site would cause severe or catastrophic loss to Contoso.' alt='Site contains personally identifiable information (PII), or unauthorized release of information on this site would cause severe or catastrophic loss to Contoso.'></a>");
                        $('#pageTitle').prepend(img);
                        classified = true;
                    }
                    else if (siteClassification == "MBI") {
                        var img = $("<a href='http://insertyourpolicy' target=_blank><img id=classifer name=classifer src='https://spmanaged.azurewebsites.net/content/img/mbi.png' title='Unauthorized release of information on this site would cause severe impact to Contoso.' alt='Unauthorized release of information on this site would cause severe impact to Contoso.'></a>");
                        $('#pageTitle').prepend(img);
                        classified = true;
                    }
                    else if (siteClassification == "LBI") {
                        var img = $("<a href='http://insertyourpolicy' target=_blank><img id=classifer name=classifer src='https://spmanaged.azurewebsites.net/content/img/lbi.png' title='Limited or no impact to Contoso if publically released.' alt='Limited or no impact to Contoso if publically released.'></a>");
                        $('#pageTitle').prepend(img);
                        classified = true;
                    }
                }
            }
        }));
    }
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SiteClassification" />