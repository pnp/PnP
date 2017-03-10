# Replacement of team site feed with Yammer feed #

### Summary ###
This sample shows how provision sites with Yammer feed associated as the default news feed for the site. Shows options with OpenGraph objects or by using groups (existing or create new).


*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

*Since integration to the Yammer feeds actually happens with browser, this does work in the on-premises if the browsers have Internet connectivity.*

### Prerequisites ###
You will have to create Yammer access token for the add-in for your Yammer network. Additional details and link to get this done below in this document.

### Solution ###
Solution | Author(s)
---------| ----------
Provisioning.Yammer | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | June 30th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Create sub site and replace default news feed with Yammer Feed #
This sample shows how to use the newly added Yammer capabilities in the core component to create new Yammer group as part of site provisioning and to associate that as the discussion feed for the newly created collaborations site. 

To be able to use the sample, follow guidance for this URL to register access token for your Yammer add-in. This access token in updated to the web.config of the provider hosted add-in.

- Get the access token from here: [https://developer.yammer.com/authentication](https://developer.yammer.com/authentication)

Update access token to the web.config of the provider hosted add-in for the key called YammerAccessToken.
```XML
<!-- Details on how to get your access token - check following https://developer.yammer.com/authentication -->
<add key="YammerAccessToken" value="PutYourOwnYammerKeyHere" />
```

Sample provides simple provisioning UI for sub sites where you are able to define if you want to use OpenGraph or Group feed. If group feed option is selected, you can choose to associate the feed to any existing group or to create a new group for the site.


----------
**Notice**. We do recommend usage of OpenGraph objects with the team site feeds rather than using groups. If you automatically provision groups for each of the created team site, you will end up having massive amount of groups which will pollute your solution story in Yammer. Usage of the REST APIs for creating groups is also not documented and could have unexpected issues.  

----------

![Add-in UI for site collection creation](http://i.imgur.com/n7G6tUJ.png)

Once the provisioning is completed, you will see the following type of a site with associated Yammer feed running in it. Notice that you will need to be logged to the particular Yammer network in the browser session to show the news feed, since integration of the yammer feed happens in browser side. 

![Custom branded site - orange](http://i.imgur.com/NH4WMdL.png)

*Notice that by default feed is added without header or footer elements from the Yammer, but that can be controlled from the code which is calling the embed capability.*

This is how the UI looks if you have not signed to the Yammer network. You can click the login button, which will make the yammer feed to work again as expected.

![Site UI with login button in the Yammer section](http://i.imgur.com/ITVpzXL.png)

This example creates new Yammer group for each of the team site. We could actually also create new OpenGraph objects if that’s preferred. Either way the group or OpenGraph object is visible also on the Yammer side as follows and any updates from either side is updated to the feed. Here’s what gets created to the Yammer side as result of the site provisioning. 

![Yammer group created for the site](http://i.imgur.com/FwMqzxY.png)

Notice also that since this configuration is dynamically applied during provisioning time, there’s no impact on removing the provisioning add-in away from the SharePoint side.

# Used Core component extensions #

Actual provisioning logic and site modifications are using [PnP Core component](https://github.com/OfficeDev/PnP/tree/master/OfficeDevPnP.Core) extension methods. As you can see we can perform the required actions with only few lines of code due the encapsulated reusable methods from the core component.

```C#
public void CreateSubSite(Web hostWeb, string url, string template,
                            string title, string description, string feedType, string yammerGroupName)
{
    // Create new sub site
    Web newWeb = hostWeb.CreateWeb(title, url, description, template, 1033);

    //Remove the out of the box "NewsFeed" web part
    newWeb.DeleteWebPart("SitePages", "Site feed", "home.aspx");

    // Let's first get the details on the Yammer network using the access token
    WebPartEntity wpYammer;
    YammerUser user = YammerUtility.GetYammerUser(ConfigurationManager.AppSettings["YammerAccessToken"]);

    // Created Yammer web part with needed configuration
    wpYammer = CreateYammerWebPart(feedType, user, yammerGroupName, title);

    // Add Yammer web part to the page
    newWeb.AddWebPartToWikiPage("SitePages", wpYammer, "home.aspx", 2, 1, false);

    // Add theme to the site and apply that
    ApplyThemeToSite(hostWeb, newWeb);
}
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Yammer" />