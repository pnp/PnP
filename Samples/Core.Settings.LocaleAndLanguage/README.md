# Control regional and language settings using CSOM #

### Summary ###
Demonstrates how to control regional and language settings in SharePoint site level using CSOM.

### Applies to ###
-  Office 365 Multi Tenant (MT) 
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
2014 December CU installed on farm for on-premises or new re-distributable package for cloud CSOM (April 2015 release).

### Solution ###
Solution | Author(s)
---------|----------
Core.Settings.LocaleAndLanguage | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 19th 2014 (to update) | Draft version
1.1  | April 13th 2015 | Updated to use latest Office 365 CSOM

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Controlling regional settings and languages #
Sample shows simple API calls to control these required settings. 

![Add-in UI](http://i.imgur.com/dbXy4Cf.png)

## Controlling regional settings ##
You can control regional settings by using LocaleId property in the regional settings object. 
```C#
Web web = clientContext.Web;
// Set regional settings to host web and execute the query
web.RegionalSettings.LocaleId = uint.Parse(ddlLocales.SelectedValue);
web.RegionalSettings.Update();
clientContext.ExecuteQuery();
```


## Controlling supported languages ##
Language settings can be easily controlled by using natively exposed properties and methods.

### Access currently supported languages ###

```C#
clientContext.Load(clientContext.Web, w => w.SupportedUILanguageIds);
clientContext.ExecuteQuery();

lblCurrentlySupportedLanguages.Text = "";
foreach (var item in clientContext.Web.SupportedUILanguageIds)
{
    lblCurrentlySupportedLanguages.Text = lblCurrentlySupportedLanguages.Text + " | " + item;
}
```

### Add new language ###

```C#
// Site needs to be set in multi lingual, without you can't add additional languages
clientContext.Web.IsMultilingual = true;
clientContext.Web.AddSupportedUILanguage(1035);
clientContext.Web.Update();
clientContext.ExecuteQuery();
```

### Remove language ###

```C#
clientContext.Web.RemoveSupportedUILanguage(1035);
clientContext.Web.Update();
clientContext.ExecuteQuery();
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Settings.LocaleAndLanguage" />