# SharePoint Governance Site Provisioning #

### Summary ###

### Walkthrough Video ###

### Applies to ###
- Office 365 Dedicated (D)
- Office 365 Multi-Tenant (MT)
- SharePoint 2013 On-Premises

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
Framework.Provisioning | Frank Marasco (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0  | 2-22-2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Solution Overview #

# Configuration Files #
Actual applying of the theme is done with single line of code as long as the URLs to the file are properly created.
    
```C#
//Set the properties for applying custom theme which was just uploaded
string spColorURL = URLCombine(rootWeb.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spcolor");
string spFontURL = URLCombine(rootWeb.ServerRelativeUrl, "/_catalogs/theme/15/contoso.spfont");
string backGroundImage = URLCombine(rootWeb.ServerRelativeUrl, "/_catalogs/theme/15/contosobg.jpg");

// Use the Red theme for demonstration
rootWeb.ApplyTheme(spColorURL,
    spFontURL,
    backGroundImage,
    false);
clientContext.ExecuteQuery();
```
## Lists ##