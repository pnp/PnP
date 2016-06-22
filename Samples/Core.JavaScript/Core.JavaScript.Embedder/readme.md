#PnP JavaScript Core - Test Embedder Setup#

### Summary ###
This article describes how to get started using the test embedder in your projects and debugging your SharePoint JavaScript files - making use of the loader pattern, PnP dev dashboard, and configuration list. 

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.JavaScript | Patrick Rodgers (**Microsoft**) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 20th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------



#### 1. Determine the local IIS Express address for the Core.JavaScript.CDN project. You can do this by selecting the project and pressing "F4" to see the properties window, note the https address, here "https://localhost:44324/"

![The SSL URL field contains the text https://localhost:44324/](http://i.imgur.com/8EBraLo.png)

----------

#### 2. Determine the url to your SharePoint development site, this will be something in your company farm, or perhaps something like https://mycompany.sharepoint.com

----------

#### 3. In the Core.JavaScript.Embedder.ContextManager.cs file you need to update the variables as appropriate for your environment. You always need to update the "mySite" variable with your SharePoint dev site url.

![A screenshot of the ContextManager.cs file showing the placeholder values for the variables mySite, myLogin, and myPassword, and the value of true for isOnline, false for hasPartnerAccess, and true for useDefault.](http://imgur.com/eUqFBjM.png)

 Variable | Description
---------|----------
mySite | The absolute url to your dev site
myLogin | If not using default credentials you need to specify the login for your dev site
myPassword | If not using default credentials you need to specify the password for the above login
isOnline | Set to true if your site is hosted in SharePoint online, false if you are working on premises
hasPartnerAccess | Set to true if your site has partner access enabled (often false)
useDefault | If you connect to a site where you use your default credentials set this to true; otherwise, false. If you set this to true you do not need to specify the login and password variables as the credentials of the user executing the console application

These are the values I use to connect to my dev tenant located in SharePoint online, note the blacked out boxes cover my login and password:

![var mySite = https://318studios.sharepoint.com/sites/dev, var myLogin is redacted, var myPassword is redacted, var isOnline = true, var hasPartnerAccess = false, var useDefault = false.](http://i.imgur.com/3FEQYPY.png)

----------

#### 4. Next, update the Core.JavaScript.Embedder.Program.cs to ensure that the file references to localhost match the value from step 1. You will need to update the script block you are using, without making changes you will use script2 as a default, this is the one that handles MDS.

![Screenshot of the code which shows that the JavaScript in variable script2 has been modified so that every URL begins with https://localhost:44324/](http://i.imgur.com/kFWECOR.png)

----------

#### 5. Execute the console application by selecting the project, right clicking, selecting Debug -> Start New Instance. This will create the user custom action in your site.

![Screenshot of the context menu, highlighting Debug -> Start new instance.](http://i.imgur.com/UCITZkm.png)

----------

#### 6. In the Core.JavaScript.CDN project, set the Start URL to your development site url:

![The project properties, Web tab are displayed with the Start URL field set to https://318studios.sharepoint.com/sites/dev/](http://i.imgur.com/Ch18z8B.png)


----------

#### 7. Create a list named "Config" in your dev site and add a column named "Value". Add an item with a Title of "ClientCDNUrlBase" and a value matching the absolute url from step 1. You can also add any other configuration required by your applications to this centralized store - which can be accessed from both client and managed code.

![A screenshot of the SharePoint site showing the Config list, the title ClientCDNUrlBase and value https://localhost:44324](http://i.imgur.com/edijvUz.png)


----------


#### 8. Update the pnp-settings.js file, modifying the configLoadUrl value to point to your dev site where you created the config list in step 7. Ensure you leave the _api part unchanged so that the REST query will work.

![A screenshot of the pnp-settings.js file, showing that the configLoadUrl value has been modified to begin with the text https://318studios.sharepoint.com/sites/dev/_api/lists/getbytitle](http://i.imgur.com/iyj4yNt.png)


----------


#### 9. Set a break point in one of the JS files, for example pnp-uimods.js and hit F5. You should see your breakpoint get hit. You should also see the dev dashboard icon in the very bottom right of your page, clicking this will open the dev dashboard.

![Screenshot of a breakpoint set at a line which reads, modifications.push(new modification(Remove New Site Link](http://i.imgur.com/dCUHnDX.png)
