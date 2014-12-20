# Control regional and language settings using CSOM #

### Summary ###
Documentation in progress.
Notice that there is a bug in the lanuguage controlling API with 2014 December CU. This will be fixed with future releases.

### Applies to ###
-  Office 365 Multi Tenant (MT) - With upcoming CSOM package
-  Office 365 Dedicated (D) 
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.Settings.LocaleAndLanguage | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 19th 2014 (to update) | Draft version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Controlling regional settings #
In progress


## Sub level 1.1 ##
Description:
Code snippet:
```C#
string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
string scenario1PageUrl = csomService.AddWikiPage("Site Pages", scenario1Page);
```


# Controlling supported languages #
Documentation in progress.