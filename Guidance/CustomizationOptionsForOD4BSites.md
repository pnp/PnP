# Customization of OneDrive for Business sites #

### Summary ###
OneDrive for Business sites can be customized in Office 365 or with app model in general, based on company requirements. Actual techniques to perform this customization are different than in the on-premises, since only app model techniques can be used. This page contains details on the actual patterns which can be used with app mdoel to customize OneDrive for Business sites. 

# Why would you customize OneDrive for Business sites? #

There are numerous different aspects on applying customizations to OneDrive for Business (OD4B) sites. You certainly can customize these sites, since they are SharePoint sites, but at the same time you should always consider the short and long term impact of the customizations. As a rule of a thumb, we would like to give following high level guidelines for customizing OD4B sites. 

- Apply branding customizations using Office 365 themes or SharePoint site theming engine
- If theme engines are not enough, you can adjust some CSS settings using alternate CSS option
- Avoid customizing OD4B sites using custom master pages, since this will cause you additional long term costs and challenges with future updates
  + In most of the cases, you can achieve all common branding scenarios with themes and alternate CSS, so this is not really that limiting factor
  + If you chose to use custom master pages, be prepared on applying changes to the sites when major functional updates are applied to Office 365
- You can use JavaScript injection to modify or hide functionalities from the site
- You can use CSOM to control for example language or regional settings in the OD4B sites (see new APIs)
- We do not recommend usage of content types and site columns in OD4B sites to avoid challenges with the 
  + Think OD4B sites as for personal un-structural data and documents. Team sites and collaboration sits are then for company data and documents where you can certainly use whatever information management policies and metadata you want.

As a summary, customizations are definitely supported in Office 365 and you can keep on using them with OD4B sites. We just truly want to ensure that you consider the short and long term impact of these customizations from operational and maintenance perspective. This is not really specific for SharePoint, rather a rule of thumb for any IT solution build with any platform. 

Here’s an example of OD4B site, which has been customized using above guidelines. In this case the end result has been achieved with combination of Office 365 themes, site theme and usage of so called JavaScript injection pattern.

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/3286.image_5F00_thumb_5F00_20BE5FEA.png)

# Challenge with applying OneDrive for Business site customizations? #

Let’s start with defining what is the challenge and what are we trying to solve here. Technically each OneDrive for Business site is currently using identical architecture as what the personal or my sites used  back in SharePoint 2007 or 2010 version. This means that technically each OneDrive for Business site is their own site collection and we do not have any centralized location to apply branding or any other customizations.

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/1273.image_5F00_thumb_5F00_390B96FC.png)

Classic solution to apply needed configuration to the OneDrive for Business sites (including my or personal sites) was based on feature stapling in farm level. This meant that you deployed farm solution to your SharePoint farm and used feature framework to associate your custom feature to be activated each time a my site is crated, which was then responsible of applying needed customizations. This similar approach does not work in Office 365, since it requires farm solution to be deployed and that is simply impossible with Office 365 sites. Therefore we need to look alternatives to apply the needed changes to the sites.

In Office 365 there is no centralized event raised, which we could attach our custom code to when OD4B site is created. This means that we need to think about alternative solutions, which is quite common with app model approaches. Do not get stuck on old models, think about how to achieve same end result using new APIs and technologies. From pure requirement perspective, it does not really matter how we apply the customizations to the sites, as long as they are applied, since business requirement is not to use feature stapling, it’s about applying needed customizations using whatever supported technical mechanism. 

# Different options for applying customizations #

In practice we do have four different mechanisms to apply centralized customizations to OD4B sites in the Office 365. You could also consider manual option as the fifth one, but in the case of having hundreds or thousands of OD4B sites, using manual options is not really a realistic option. Here’s the different options we have.
 
1. Office 365 suite level s
ettings (Office 365 themes and other settings)
2. Hidden app part with user context
3. Pre-create and apply configuration
4. Remote timer job based on user profile updates

Each of the options have advantages and disadvantages in them and the right option depends on your detailed business requirements. Some of the settings you can also apply from the Office 365 suite level, but often you would be looking for some more specifics, so actual customizations are needed. It obviously all comes down on exact requirements and business case analyses on their impact on short and long term.

 

## Office 365 suite level settings ##

Office 365 is much more than just SharePoint, like you know. You can find more and more additional services which are not based on even the SharePoint architecture, like Delve, Yammer and many upcoming services. This means that the enterprise branding and configuration is not just about controlling what we have in the SharePoint sites, rather we should be thinking the overall end user experience and how we provide consistent configurations cross different services.

Classic example of these enterprise requirements is branding and for that we have already Office 365 theming introduced, which can be used to control some level of branding. We have also other upcoming features, which will help to control your site governance and other settings, from centralized location outside of the site collection settings, like the upcoming Compliance Center for Office 365, which is currently listed in the roadmap of the Office 365.

Following picture shows the different settings right now for the Office 365 theming, which will be then applied cross all Office 365 services.

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/6237.image_5F00_thumb_5F00_45C310CF.png)

Since by default Office 365 theme settings are for controlling OD4B site suite bar, you will most likely be using this options together with other options to ensure that you can provide at least the right branding elements cross your OD4B sites. Notice that when you change for example Office 365 theme settings in Office 365 admin tool, it does take a quite a long time to get the settings applied for OD4B sites, so be patience. 

 

## Hidden app part with user context ##

This is an approach where use centralized landing page as the location for starting the needed customization process. This means that you would have to have one centralized location, like company intranet front page, where the users are always landing when they open up their browser. This is pretty typical process with midsized and larger enterprises where corporate landing page is then controlled using group policy settings in the AD. This will ensure that end users cannot override default welcome page of the company domain joined browsers.

When user arrives to the intranet, we will have hidden app part in the page, which will start the customization process. It can actually be responsible of the whole OD4B site creation as well, since normally user would have to visit the OD4B site once time, before the site creation process will be started. Hidden app part is actually hosting a page from provider hosted app hosted in Azure. This page is then responsible of starting the customization process.

Let’s have a closer look on the logical design of this approach.

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/4174.image_5F00_thumb_5F00_2D109734.png)

1. Place hidden app part to centralized site where end users will land. Typically this is the corporate intranet front page.
2. App part is hosting a page from provider hosted app, where in the server side code we initiate the customization process by adding needed metadata to the azure storage queue. This means that this page will only receive the customization request, but will not actually apply any changes to keep the processing time normal.
3. This is the actual azure storage queue, which will receive the messages to queue for processing. This way we can handle the customization controlling process asynchronously so that it does not really matter how long end user will stay on the front page of the Intranet. If the customization process would be synchronous, we would be dependent on end user to keep the browser open in the Intranet front page until page execution is finalized. This would not definitely be optimal end user experience. 
4. WebJob hooked to follow the storage queue, which is called when new item is placed to the storage queue. This WebJob will receive the needed parameters and metadata from the queued message to access right site collection. WebJob is using app only token and have been granted the needed permissions to manipulate site collections in the tenant level.
5. Actual customizations are applied one-by-one to those people’s sites who visit the intranet front page to start the process.

This is definitely the most reliable process of ensuring that there’s right configurations in the OD4B sites. You can easily add customization versioning logic to the process, which is also applying any needed updates to the OD4B sites, when there is an update needed and user visits the Intranet front page next time. This option does however require that you have that centralized location where your end users are landing.

If you are familiar of classic SharePoint development models with farm solutions, this is pretty similar process as one time executing timer jobs.

 

## Pre-create and apply configuration ##

This option relies on the pre-creation of the OD4B sites before users will access them. This can be achieved by using relatively new API which provides us away to create OD4B sites for specific users in batch process, using either CSOM or REST. Needed code can be initiated using a PowerShell script or by writing actual code which is calling the remote APIs. 

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/7382.image_5F00_thumb_5F00_08A645B6.png)

1. Administrator is using the remote creation APIs to create OD4B sites for users and is applying the needed customizations to the OD4B sites as part of the script process.
2. Actual OD4B sites are created to the Office 365 for specific users and associated to their user profiles

In some sense this is also really reliable process, but you would have to manage new persons and updates “manually”, which could mean more work then using the hidden app part approach. This is definitely valid approach which can be taken and especially useful if you are migrating from some other file sharing solution to the OD4B and want to avoid the need of end users to access the OD4B site once, before actuals site creation is started.

 

## Remote timer job based on user profile updates ##

This approach means scanning through user profiles for checking to whom the OD4B site has been created and then apply the changes to the sites as needed. This would mean scheduled job running outside of the SharePoint, which will periodically check the status and perform needed customizations. Scheduled job could be running as a WebJob in Azure or as simple as PowerShell script scheduled in your own windows scheduler. Obviously the scale of the deployment has huge impact on the chosen scheduling option.

![](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/0702.image_5F00_thumb_5F00_3AC52246.png)

1.Scheduled task is initiated which will access user profiles of the users for checking who has OD4B site provisioned
2.Actual sites are customized one-by-one based on the business requirements

One of the key downsides of this option is that there can clearly be a situations where user can access the OD4B sites before the customizations have been applied. At the same time this option is interesting add-on for other options to ensure that end users have not changed any of the required settings on the sites or to check that the OD4B site content aligns with the company policies.


----------

### Related links ###
-  [Customizing OneDrive for Business sites with app model (MSDN blog article)](http://blogs.msdn.com/b/vesku/archive/2015/01/01/customizing-onedrive-for-business-sites-with-app-model.aspx)

### Related PnP samples ###
-  [Customizing OD4B sites using Async pattern](#)
-  [Classic app part and sync process for OD4B site customization](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.OneDrive)
-  [Pre-create OD4B sites for users](https://github.com/OfficeDev/PnP/tree/master/Samples/Provisioning.OneDriveProvisioning)

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D) - *partly*
-  SharePoint 2013 on-premises - *partly*

*Patterns for Dedicated and on-premises are identical with app model techniques, but there are differences on the possible technologies which can be used.*

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 2nd, 2015 | Initial release | Vesa Juvonen (Microsoft)


