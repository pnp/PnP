# Taxonomy navigation components for SharePoint Online using Office UI Fabric and JSOM #

### Summary ###

This is an example of common taxonomy navigation components (main menu, contextual menu and breadcrumb) for SharePoint Online using Office UI Fabric and JSOM.

Here is what you get after deploying this example:

![Final result](http://thecollaborationcorner.com/wp-content/uploads/2016/02/final_taxonomy_menu.png)

 - A responsive navigation main menu wired to a taxonomy term set and using the Office Ui Fabric CSS classes for rendering.
 - A contextual menu and breadcrumb menu to insert directly in your pages (see below for instructions).

![Responsive by default](http://thecollaborationcorner.com/wp-content/uploads/2016/02/final_taxonomy_menu_responsive.png)

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Prerequisites ###

Before starting, you need to install some prerequisites:

- Install the [SharePoint Online Client Components SDK](https://www.microsoft.com/en-ca/download/details.aspx?id=42038). CSOM dlls are deployed in the GAC and used by the `Utility\Navigation.ps1` script via `Add-Type`
- Install [PnP PowerShell cmdlets for SharePoint Online v16](https://github.com/OfficeDev/PnP-PowerShell/tree/master/Binaries)
- Provision a site collection with the "Publishing site" template or with the publishing infrastructure features activated

### Solution ###
Solution | Author(s)
---------|----------
Core.TaxonomyNavigationComponents | Franck Cornu

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 4th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Installation #

- Download the PnP source code as ZIP from GitHUb and extract it to your destination folder
- Start a PowerShell session as an **administrator** an call the `Deploy.ps1` script with your parameters like this:

```csharp
$UserName = "username@<your_tenant>.onmicrosoft.com"
$Password = "<your_password>"
$SiteUrl = "https://<your_tenant>.sharepoint.com/sites/<your_site_collection>"

Set-Location "<your_installation_folder>\Components\Core.TaxonomyNavigationComponents"

$Script = ".\Deploy.ps1" 
& $Script -SiteUrl $SiteUrl -UserName $UserName -Password $Password

```
# Key points of this solution #

- This is a generic solution that you can use without modification except your own term set configuration. 

- You can add your own components by following the same pattern.

- All CSS and menu logic come from the [Office UI Fabric components](http://dev.office.com/fabric/components). The navigation main menu is responsive by default.
 
- All navigation components are added through the [KnockoutJs 'component' binding mechanism](http://knockoutjs.com/documentation/component-binding.html) with Require JS

- All script dependencies are managed through [RequireJS](http://requirejs.org/), avoiding us to use the SP.SOD nightmare.

- The deployment sequence is done via PnP cmdlets and SharePoint CSOM.

- We use the default "**oslo**" master page from a publishing site with JavaScript injection to insert the main menu in all pages.

- The navigation menus support multiple languages and friendly/simple link URLs .

- The default search box is integrated to the navigation bar only by jQuery manipulations.

- Component synchronization (contextual menu and breadcrumb) is done via the Javascript Pub/Sub pattern. We use [AmplifyJS](http://amplifyjs.com/) to manage this behavior.

- Global and current navigation visibility settings for each term are supported. You can use these properties in your HTML views (see `Templates\template.mainmenu.html` to see an example).

- The source term set for the menu don't have to be necessarily the term set used for the web navigation (for example in the case you only want simple links in your menu). However, to benefit of the friendly URLs, you can use directly the navigation term set configured for the navigation (like this example) **OR** a term set that reuses terms from it to get friendly URLs work.

- The browser local storage is used in this example to show how to cache the main menu navigation nodes. You can reset the cache just by adding a custom property directly on the term set (see below).

# Use #

## Use navigation menu with your own term set ##

By default, the term set used for the menu is the sample term set provisioned with this example. You can use your own term set by specifying the the id in the DOM element for the component in the `main.js` script:

```javascript
...
$("<div class=\"ms-NavBar\"><component-mainmenu params='termSetId: \"<your_termset_id>\"'></component-mainmenu></div>").insertBefore(tableRow);
...
```

Warning: your term set must be flagged as a navigation term set!

## Caching ##

The main menu nodes are automatically cached during the first load in the browser local storage under the value `mainMenuNodes`. Any other subsequent requests will use the cache value instead of making a CSOM call. To clear the cache for every users, you can set the custom property of the term set `NoCache` to `true`. No custom property or other value will result to continue using the cached value.

![Local storage value](http://thecollaborationcorner.com/wp-content/uploads/2016/02/final_local_storage2.png)

![Reset the local storage value from term set](http://thecollaborationcorner.com/wp-content/uploads/2016/02/final_nocache.png)

## Icons configuration ##

For each navigation term, you can configure an specific icon from the [Office UI Fabric styles](http://dev.office.com/fabric/styles), to do so, just add the "IconCssClas" custom property with the desired Css class. 

![Icon configuration for a term](http://thecollaborationcorner.com/wp-content/uploads/2016/02/icon_configuration.png)

In this example, only the first level of the main menu display icons but this mechanism is easily extendable.

## Contextual menu and breadcrumb ##

You can add a contextual menu and breadcrumb components to your page just by adding the following HTML markup in a SharePoint page (in a Script Editor Web Part).

`<component-contextualmenu></component-contextualmenu>`

`<component-breadcrumb></component-breadcrumb>`

The main JavaScript will look for these specific DOM elements add will add them dynamically to your page.

Notes:

- You don't need to pass the term set id because the nodes are always deduced form the main menu (whatever if they've been retrieved from the cache or directly from a CSOM call). To do this, we use the Pub/Sub pattern via AmplifyJS library.

- For the contextual menu, only the siblings and children who have been flagged to appear in the current navigation are displayed (the display is controlled directly in the HTML of the template view).

- The breadcrumb and the contextual menu work with both friendly and physical (simple link) URLs.

## Troubleshooting ##

If you encounter some troubles to get the navigation main menu in your page, make sure the term set is flagged as navigation term set and wired correctly to your web site for navigation.
The SharePoint Online taxonomy term store may be sometimes capricious ;)

Enjoy!

----------

<img  src="https://telemetry.sharepointpnp.com/pnp/components/Core.TaxonomyNavigationComponents" />