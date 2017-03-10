# Display template management using add-in model #

### Summary ###
The Display Templates sample demonstrates how to use Display Templates to render a hero image and content rotator in a Content By Search Web Part. Additionally, the Display Templates target mobile devices using Responsive Web Design (RWD) and Device Channels.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Branding.DisplayTemplates | Enny Zhang, Cindy Yan, Lucas Smith, Todd Baginski (**Canviz LLC**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 16th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Configuration & Deployment #
In order for the Display Templates sample to render correctly, you must first create and configure your SharePoint site collection.

## SharePoint ##

1.Navigate to your SharePoint tenancy and create a new site collection using the Developer Site template in the Collaboration tab.

![Creation of dev site colelction](http://i.imgur.com/odV8QKQ.png)

2.Once the site collection is created, navigate to Site settings and select Site collection features under the Site Collection Administration heading.

![Site collection features link in site settings](http://i.imgur.com/dPVRbZ7.png)

3.In the Site collection features, locate the SharePoint Server Publishing Infrastructure feature and click Activate.  Be patient, sometimes this feature may take several minutes to activate.

![Publishing features in site collection features list](http://i.imgur.com/Rokg2DE.png)

4.Return to Site settings and select Manage Site Features under the Site Actions heading.

![Manage site features in site settings](http://i.imgur.com/7qF70va.png)

5.Locate the SharePoint Server Publishing feature and select Activate. 

![Publishing feature in site level](http://i.imgur.com/VNohlms.png)

6.Deactivate the Mobile Browser View feature.

![Mobile Browser View Feature in site features list](http://i.imgur.com/rH0Or8b.png)


## Visual Studio ##

1.Open the Branding.DisplayTemplates.sln file with Visual Studio 2013 or 2015. 

![Opened solution in Visual Studio](http://i.imgur.com/jScRPO4.png)

2.In the Properties window, set the “Site URL” property to the site collection you previously created and configured.

![Site URL property in add-in project properties](http://i.imgur.com/YkYmC5c.png)

3.Press F5 or click the Start button in Visual Studio 2013.
4.Enter you user name and password to connect to your SharePoint site collection. 

![Signing in to Office 365](http://i.imgur.com/Sd0PS58.png)

5.After your username and password have been verified, the trust dialog is displayed. Click the “Trust It” button.

![Trust operation in Office 365](http://i.imgur.com/KPVha7Z.png)

6.After add-in installation, a new page will be displayed, click the Deploy button to create the site columns, content type, list, initialize the list with data, and upload the master pages, images, CSS files and Display Template JavaScript files. 

![UI of add-in](http://i.imgur.com/JqBDFRl.png)

7.To install all the artifacts again, first click the Delete Artifacts button to delete the site columns, content type, list, master pages, CSS, images, and display template JavaScript files.  Then, click the Deploy button again.

Note: The code behind in the default.aspx.cs file contains all the code used to deploy the artifacts which support this sample.  This code sample uses the remote provisioning pattern to deploy the artifacts.  The remote provisioning pattern uses the SharePoint Client Side Object Model to deploy the artifacts.  There are many other Office 365 Developer PnP samples which demonstrate this same approach.

## Configure Device Channels ##
To configure the Device Channels, perform the following tasks.

1.Navigate to the SharePoint site.
2.Log into the site with Site Owner credentials.
3.Click the gear icon in the top right portion of the page and select Site Settings in the dropdown list.
4.In the Look and Feel section, click the Device Channels link.

![Site settings page with Device Channels link](http://i.imgur.com/1KjlhZH.png)

5.Click the new item link.
6.Enter the information to create the iPad device channel.  See the following screenshot for reference. 

![New device channel creation for iPad](http://i.imgur.com/NAQXEBk.png)

7.Click Save.
8.Enter the information to create the iPhone device channel.  See the following screenshot for reference. 

![iPad entry creation](http://i.imgur.com/SfdJTEi.png)

9.Click Save.

## Configure Master Pages ##
To configure which Master Pages associated with each Device Channel, perform the following tasks.

1.Click the gear icon in the top right portion of the page and select Site Settings in the dropdown list.
2.In the Look and Feel section, click the Master page link.

![Master page link in site settings](http://i.imgur.com/bQ8qXzN.png)

3.Configure the appropriate Master Page for each device channel.  Refer to the following screenshot for reference.

![Master page selection for channel](http://i.imgur.com/Tomglbm.png)

4.Click OK

# Viewing the Home Hero Slider #
After you have successfully configured your SharePoint environment and deployed the artifacts via the add-in, you can view the Home Hero slider in different pages and Device Channels.

## HOME HERO SLIDER LIST ##

1.To access the content of the Home Hero slider, navigate to Site contents.

![Site contents page](http://i.imgur.com/vI4o7S8.png)

2.Select the Home Hero list.

![Home Hero list in site contents page](http://i.imgur.com/bEdhrFJ.png)

3.Four list items are displayed in the Home Hero slider control.  These list items were created by the add-in after it provisioned the Home Hero list.  You can edit these items or leave the default content.  We encourage you to at least view the properties of a list item to see all the different thngs you can configure such as text, images, colors, opacity, and URL.

**Note:** If you edit the properties of a list item you must wait until the Search Service crawls the list again to see the changes appear in the Home Hero slider.

![List of entries in list](http://i.imgur.com/gYWj7gl.png)

4.Navigate back to Site contents and select the Pages library. You will find 3 newly created pages.  These pages were created by the add-in.

- desktop.aspx
- rwd.aspx 
- channels.aspx

![Home Hero list](http://i.imgur.com/EZgyJjl.png)

Continue to the next section to interact with the pages.

## Pages and Channels ##
The pages within the Pages library render the Home Hero slider with different views. The Home Hero slider is a Content By Search Web Part.  Customized Display Templates are used to render the data returned via the Search Service.

Important Note: The Content By Search Web Part will only display the data from the SharePoint list after the list items have been crawled and indexed.

In an on-premises SharePoint 2013 installation you can start a crawl manually in the Search Administration section in Central Administration.

In an O365 site collection you cannot start the crawl manually.  It may take a long time in an O365 site collection to crawl and index the data. This MSDN article describes how long it may take to crawl and index the data.  After the crawling and indexing process is complete the hero control will display properly on the pages.

### DESKTOP.ASPX ###
This page displays the desktop view of the Home Hero slider. Click it to open a page with desktop version of the hero control.

![Desktop experience](http://i.imgur.com/Lzcs4xf.png)

The Content By Search Web Part on the desktop.aspx page is configured to use the HomePageHeroControlSlideshow.js Control Display Template.  All of the Content By Search Web Parts share the same Control Display Template.

The Content By Search Web Part on the desktop.aspx page is configured to use the HomePageHeroItemTemplate.js Item Display Template.  The HomePageHeroItemTemplate.js Item Display Template implements a design targeted for desktop web browsers.  This approach is typically used in an Intranet scenario where the page is not targeting mobile devices. Each Content By Search Web Part uses a different item template.

### RWD.ASPX ###
Click the rwd.aspx page in the Pages library.  This page utilizes responsive web design and displays different views when a browser’s screen size changes.  Resize your browser to see the responsive web design adapt to your browser’s width.

In the largest viewport (greater than 768 pixels wide & less than or equal to 1168 pixels), the responsive design renders this view.  This view is the same as the desktop version on the desktop.aspx page.

![Responsive experience](http://i.imgur.com/aSB4Ibn.png)

In the smaller viewport (less than or equal to 768 pixels wide), the responsive design renders this view.  Notice the layout and the content changes as well as the overall width of the display.

![Smaller viewport experience](http://i.imgur.com/fCTvsLN.png)

The Content By Search Web Part on the rwd.aspx page is configured to use the HomePageHeroControlSlideshow.js Control Display Template.  All of the Content By Search Web Parts share the same Control Display Template.

The Content By Search Web Part on the rwd.aspx page is configured to use the HomePageHeroItemTemplate_rwd.js Item Display Template.  The HomePageHeroItemTemplate_rwd.js Item Display Template implements a responsive web design targeted for desktop and mobile web browsers.  This approach is typically used in a scenario where the page targets desktop browser and mobile devices.

### CHANNELS.ASPX ###
Click the channels.aspx in the Pages library and you will access the default Device Channel.  The default Device Channel renders the desktop version of the Home Hero slider.

This sample includes three Device Channels for desktop, iPad, and iPhone devices.

To access the different Device Channels and see the different views, add the parameter “?devicechannel=<channel>” to the URL to display the different views.  You may also use a developer tool to spoof the User Agent String to access different Device Channels from a desktop web browser or use an iPad or iPhone to access the different device channels.  Keep in mind, when you access the different Device Channels in your web browser they may appear to be larger than they would in a native iPad or iPhone.  If you access the site via an iPad or iPhone you will see the designs fit perfectly.

1.To render the default channel browse to either: 
- https://<site URL>/channels.aspx or
- https://<site URL>/channels.aspx?devicechannel=default

![Larges screen experience](http://i.imgur.com/uyYbpsq.png)

2.To render the iPhone channel browse to: https://<site URL>/channels.aspx?devicechannel=iphone

*Notice at the top of the page the Master Page displays some text to let you know which Master Page is being rendered by the Device Channel.*

![iPhone experience - smaller](http://i.imgur.com/nb2hhX7.png)

```XML
<div style="background-color:lightgreen; width:220px;text-align:center;">This is the iPhone Master Page</div>;
```

Notice the layout and the content changes as well as the overall width of the display.

![UI with iPhone](http://i.imgur.com/2p7Pimx.png)

3.To render the iPad channel browse to: https:/<site URL>/channels.aspx?devicechannel=ipad.  The images on the hero control are rendered larger than iPhone. 

Notice at the top of the page the Master Page displays some text to let you know which Master Page is being rendered by the Device Channel.

![This is the iPad Master Page entry](http://i.imgur.com/8XMly9m.png)

The following code in the Master Page is used to display this text.

```XML
<div style="background-color:lightsalmon; width:220px;text-align:center;">This is the iPad Master Page</div>
```

Notice the layout and the content changes as well as the overall width of the display.

![UI with iPad - larger](http://i.imgur.com/35NkvLa.png)

# How the Device Channels, Master Pages, CSS and Display Templates work together #
The Content By Search Web Part on the channels.aspx page is configured to use the HomePageHeroControlSlideshow.js Control Display Template.  All of the Content By Search Web Parts share the same Control Display Template.  This control template sets up the overall container for the content displayed int he Content By Search Web Part.  The control template also includes the code and logic used to cycle the different list items like a slide show.

The Content By Search Web Part on the channels.aspx page is configured to use the HomePageHeroItemTemplate_channel.js Item Display Template.  The HomePageHeroItemTemplate_channel.js Item Display Template implements a design targeted for desktop and mobile web browsers.  This approach is typically used in a scenario where the page targets desktop browsers and mobile devices and you wish to deliver the smallest amount of page payload as possible to make pages load as fast as possible.

Three master pages are used to implement to Device Channel approach.
- Desktop.master
- iPad.master
- iPhone.master

When the page is loaded in the default Device Channel, the Desktop.master Master Page is used.  The Desktop.master Master Page is also used for the default.aspx and rwd.aspx pages.  This master page loads the hero_desktop.css CSS file.  This CSS file contains styles specific to the desktop version of the Home Hero slider.

When the page is loaded in the iPad Device Channel, the iPad.master Master Page is used. This master page loads the hero_ipad.css CSS file.  This CSS file contains styles specific to the iPad version of the Home Hero slider.

When the page is loaded in the iPhone Device Channel, the iPhone.master Master Page is used. This master page loads the hero_iphone.css CSS file.  This CSS file contains styles specific to the iPhone version of the Home Hero slider.

The HomePageHeroItemTemplate_channel.js Item Display Template is used for all three Device Channels, it uses the CSS styles it inherits from the CSS file the Master Page includes. 


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.DisplayTemplates" />