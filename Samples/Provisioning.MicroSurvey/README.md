# MicroSurvey Web Part and List Forms #

### Summary ###
This is a "microsurvey" web part, which will display a single question and
gather up the answers. The app provisions SharePoint lists to hold the questions
and answers, including custom New, Display, and Edit forms for one of the lists.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Provisioning.MicroSurvey | Bob German

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | January 1st 2016 | Update to use Widget Wrangler
1.0  | July 31st 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# MicroSurvey Web Part and List Forms #

This is a "microsurvey" web part, which will display a single question and
gather up the answers. The app provisions SharePoint lists to hold the questions
and answers, including custom New, Display, and Edit forms for one of the lists.

![Add-in UI](http://i.imgur.com/AGGFagj.png)

In addition to providing a useful "microsurvey", this Visual Studio solution demonstrates how to build
widgets and forms that provision their own content and are flexible to be deployed in more than one way.
There are four ways to deploy this app - with more to come!

 1. __As a SharePoint Hosted App:__ If you simply install the .app file, or let Visual Studio deploy the MicroSurvey project
    to a Developer (Dev#0) site, the MicroSurvey will run as a SharePoint Hosted App. Manage the app in the full page view; use the
    included App Part to place it on the page.

 2. __As a Drag and Drop App:__ This is a style of application used by end users who want to add something to a SharePoint site.
    Simply copy the contents of the SurveyApp folder to SiteAssets/SurveyApp/ within any SharePoint web.
    Manage the app by visiting the SiteAssets/SurveyApp/ folder in a web browser (click the Default.aspx file if needed.)
    To use the web part, place a Content Editor Web Part on any page pointing to SiteAssets/SurveyApp/webPart.html.
    You must turn off the "Minimal Download Strategy" feature on the site in order for the web part to work.

 3. __As a centrally deployed app:__ If you have many sites that share a common host name (such as site collections that run under
    managed paths), you can install the code on one site and reference it from all the sites that share the host name. This allows you
    to update the application centrally, by changing the scripts and files in the central site; the other sites only have a few files
    needed to host the app. The MicroSurveyInstaller project contains PowerShell scripts to deploy the centrally managed scripts and
    to add the app to sites where it will be used.

     - Edit Get-Settings.ps1 with the URL of the site that will house the scripts,
       the name of the script library, and the title to be placed on the web part when
       the web part is added to a site

     - Run the Install-MicrosurveyScripts script to deploy the centrally hosted scripts.
       NOTE: This requires the PnP PowerShell.Commands project to be deployed

     - Run the  Install-Microsurvey script to add the Microsurvey to a site using the centrally
       hosted scripts. Remember - the scripting site must be in the same DNS host name.

   The management page can be accessed under Site Settings, and the web part will be placed on the site home page.
   (This assumes a wiki page site such as Team site; the Install-Microsurvey script will need to be modified to handle
   a publishing page.)

 4. __Variation on a centrally deployed app:__ If you have many sites that don't share a common host name, you can deploy the centrally
    hosted scripts and other files as part of an IIS site or provider hosted app. This requires adding CORS headers needed to load an
    AngularJS html template across domain boundaries. A sample web.config file is included in the Solution Items to show how
    to set this up.

The code shows several useful patterns including:

* How to write an AngularJS "widget" (web part) that allows more than one copy on a page, and shares
  the page with other AngularJS code. In a single-page application, it's fine to assume you're the only
  developer using AngularJS on a page, but that's not so with a web part or widget.

* The JavaScript code provisions its own SharePoint content using a "desired state" pattern - that is, it can be run
  again and again without error to ensure that the needed content is present. This makes upgrading very
  easy, since the code just adds any new lists, columns, etc. when they're needed.

* How to use the PnP PowerShell commands to deploy a widget and custom site settings page

* How to associate custom client-side forms with a list using remote provisioning

## Table of query string parameters passed to List Forms ##

<table>
<tr><td>Query String Parameter</td><td>New Form</td><td>Display Form</td><td>Edit Form</td></tr>
<tr><td>======================</td><td>========</td><td>============</td><td>=========</td></tr>
<tr><td>List={guid}</td><td>Y</td><td>Y<td>Y</td></tr>
<tr><td>ID={int}</td><td>N</td><td>Y<td>Y</td></tr>
<tr><td>Source={Url}</td><td>Y</td><td>Y<td>Y</td></tr>
<tr><td>ContentTypeId={guid}</td><td>N</td><td>Y<td>Y</td></tr>
<tr><td>RootFolder={path}</td><td>Y</td><td>Y<td>N</td></tr>
<tr><td>Web={guid}</td><td>Y</td><td>N<td>N</td></tr>
</tr></table>

______________________
Clever URL Parsing is brought to you by: https://saikiran78.wordpress.com/2014/01/17/getting-list-data-in-sharepoint-2013-using-rest-api-and-angular-js/


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.MicroSurvey" />