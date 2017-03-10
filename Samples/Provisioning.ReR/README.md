# Site collection creation from remote event receiver #

### Summary ###
This sample shows how to attach remote event receivers to a list in the host web and how to use that as the initiating action to create new site collections. 

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.SiteCollectionReR | Vesa Juvonen ,Bert Jansen & Frank Marasco (Microsoft) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
The APIs for creating site collections, subsites, and OneDrive for Business sites are different. Only the on-demand pattern applies to OneDrive for Business sites, see [Frank’s blog](http://blogs.msdn.com/b/frank_marasco/archive/2014/03/25/so-you-want-to-programmatically-provision-personal-sites-one-drive-for-business-in-office-365.aspx) for additional information. You can apply the other two patterns to creation of all other types of SharePoint sites. The S[elf-Service Site Provisioning using Apps for SharePoint 2013](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2013/04/04/self-service-site-provisioning-using-apps-for-sharepoint-2013.aspx) sample by Richard diZerega demonstrates this by enabling creation of both subsites and site collections through a [customization form](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2013/04/04/self-service-site-provisioning-using-apps-for-sharepoint-2013.aspx). You may also visit Vesa "vesku" Juvonen blog for addition information [SharePoint 2013 site provisioning](http://blogs.msdn.com/b/vesku/archive/2014/03/02/sharepoint-online-solution-pack-for-branding-and-provisioning-released.aspx) techniques presentation video recording.

This samples demonstrates

- Association of remote event receiver with custom list on the host web during the add-in install event
- Removal of the remote event receiver with the add-in is uninstalled
- Creation of a Site Request List on the host web
- Remote site collection provisioning in a remote event receiver using the add-in-only policy.

This sample assumes that you have a workflow deployed to the Host web.
This code only works on an Office 365 Multi-Tenant (MT) SharePoint site.  With slight modifications this sample will work in an on-premises installation of SharePoint or the current version of SharePoint Online Dedicated.  See Vesa’s Blog for additional information. 

```C#
public static string CreateSiteCollection(ClientContext ctx, string hostWebUrl, string template, string title, string description, string userEmail)
{
//get the base tenant admin urls
var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8);
       tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

       //create site collection using the Tenant object
       var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", title);
        var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                var properties = new SiteCreationProperties()
                {
                    Url = webUrl,
                    Owner = userEmail,
                    Title = title,
                    Template = template,
                    StorageMaximumLevel = 100,
                    UserCodeMaximumLevel = 100
                };

                //start the SPO operation to create the site
                SpoOperation op = tenant.CreateSite(properties);
                adminContext.Load(tenant);
                adminContext.Load(op, i => i.IsComplete);
                adminContext.ExecuteQuery();

                //check if site creation operation is complete
                while (!op.IsComplete)
                {
                    //wait 30seconds and try again
                    System.Threading.Thread.Sleep(30000);
                    op.RefreshLoad();
                    adminContext.ExecuteQuery();
                }
            }

            //get the new site collection
            var siteUri = new Uri(webUrl);
            token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var newWebContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                var newWeb = newWebContext.Web;
                newWebContext.Load(newWeb);
                newWebContext.ExecuteQuery();

                new LabHelper().SetThemeBasedOnName(newWebContext, newWeb, newWeb, "Orange");

                // All done, let's return the newly created site
                return newWeb.Url;
            }
        }
    }

```

# Solution #
![Visual Studio solution pcture](http://i.imgur.com/fnh9LY9.png)

Provisioning.SiteCollectionReRWeb– SharePoint Provider Hosted Application 
Because the add-in needs the ability to create site collections anywhere in the tenancy, it will need FullControl permission on the entire tenancy.  The add-in will also need to make add-in only calls to SharePoint, so it can work with tenant objects or sites outside the context.  Both these settings can be configured in the Permissions tab of the AppManifest.xml.

**NOTE:** *You should typically avoid requesting tenancy permissions in your apps…especially with FullControl.  It is a best practice for apps to request the minimum permissions they need to function.  The “tenancy” permission scope is in place specifically for scenarios like provisioning.*


# RUNNING THE SAMPLE #
Navigate to the Application and fill in the supplied form. The Site information will be saved to the Site Requests list in the host web.

![Add-in UI for creating new site collection](http://i.imgur.com/TcD3OMd.png)

Navigate to the Site Request List and you may change the State of the given item from New to “Approved”

![Site request list in SharePoint](http://i.imgur.com/olcECBg.png)

Once the item has been set to approved, this will invoke the remote event receiver to handle the provisioning logical.


```C#
private void HandleItemUpdated(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if(clientContext != null)
                {
                    List requestList = clientContext.Web.Lists.GetById(properties.ItemEventProperties.ListId);
                    ListItem item = requestList.GetItemById(properties.ItemEventProperties.ListItemId);
                    clientContext.Load(item);
                    clientContext.ExecuteQuery();

                    if (String.Compare(item[SiteRequestFields.State].ToString(), "Approved", true) == 0)
                    {
                        try
                        {
                            string site_title = item[SiteRequestFields.Title].ToString();
                            string site_description = item[SiteRequestFields.Description].ToString();
                            string site_template = item[SiteRequestFields.Template].ToString();
                            string site_url = item[SiteRequestFields.Url].ToString();
                            SharePointUser site_owner = LabHelper.BaseSetUser(clientContext, item, SiteRequestFields.Owner);
                            LabHelper.CreateSiteCollection(clientContext, site_url, site_template, site_title, site_description, site_owner.Email);
                            item[SiteRequestFields.State] = "COMPLETED";
                        }
                        catch(Exception ex)
                        {
                            item[SiteRequestFields.State] = "ERROR";
                            item[SiteRequestFields.StatusMessage] = ex.Message;
                        }
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }

```
After the site is provisioned the site request will have its status updated to “COMPLETED” or “ERROR” based on the outcome.


## AppUninstalling event receiver ##
When the add-in is uninstalled we’re also removing the event receiver. In order to make this work during debugging you’ll need to ensure that you navigate to the “Apps in testing” library and use the remove option on the add-in. This remove will trigger the add-in uninstalling event with the proper permissions to remove the created remote event handler. If you just close the browser or uninstall the add-in from the “site contents” then either the event receiver never fires or the event receivers runs with unsufficient permissions to remove the list added event receiver. The reason for this behavior is differences in add-in deployment when the add-in gets side loaded which is what Visual Studio does when you press F5.

When a user uninstalls a deployed add-in this moves the add-in to the site's recycle bin and will NOT trigger the appuninstalling event handler. The add-in needs to be removed from all recycle bins in order to trigger the appuninstalled event.

# SHAREPOINT ONLINE SETUP #

The first step to create the application principal. The add-in principal is an actual principal in SharePoint 2013 for the add-in that can be granted permissions.  To register the add-in principal, we will use the “_layouts/AppRegNew.aspx”. 

Now we need to grant permissions to the add-in principal.  You will have to navigate to another page in SharePoint which is the “_layouts/AppInv.aspx”. This is where you will grant the application Tenant permissions, so that our Site Provisioning application may create site collections.

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
 <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

# DEPENDENCIES 

- Microsoft.Online.SharePoint.Client.Tenant
- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- [Setting up provider hosted add-in to Windows Azure for Office365 tenant](http://blogs.msdn.com/b/vesku/archive/2013/11/25/setting-up-provider-hosted-app-to-windows-azure-for-office365-tenant.aspx)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.ReR" />