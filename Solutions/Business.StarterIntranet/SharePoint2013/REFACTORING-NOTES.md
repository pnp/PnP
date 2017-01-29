# Refactoring notes to update the PnP Starter Intranet to SharePoint 2013  #

The following refactoring steps have been followed to adapt the original the PnP Starter Intranet for Office365/SharePoint Online to SharePoint 2013 On-Premise.

### Environement setup ###
The following environment has been used to refactor the solution:

Version  | Version | Date
---------| -----| --------
PnP PowerShell cmdlets | 2.11.1701.1 | January 2017
SharePoint 2013 | 15.0.4893.1000 | January 2017 CU
PnP Core NuGet package for the extensibility provider | 2.11.1701.1 | January 2017

### Refactoring tasks ###

- Replaced all "16.0.0.0" (SharePoint Online) references to "15.0.0.0" (SharePoint 2013) in the whole project. These references are mainly used in the page layouts and also the master page.
- In the *RootSiteTemplate.xml* file, removed all references to the *BSN* property. This property is specific to SharePoint Online.
- In the *RootSiteTemplate.xml* file, removed all WebParts provisioning code from *aspx* files. When used with SharePoint 2013, the following issue appears [https://github.com/SharePoint/PnP-Sites-Core/issues/866](https://github.com/SharePoint/PnP-Sites-Core/issues/866).
- In page layouts and master page, replaced the Tagprefix "SharePointWebControls" to "SharePoint". Updated also all references accordingly in these files. The correct tag can be retrieved in the native *seattle.master*:

**Before**
```csharp
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
```
**After**
```csharp
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
```
- Updated the solution to TypeScript 2.1.5 to simplify typings management (see the *tsconfig.json* file)
- Added the following code to the *main.ts* file to set the correct HTTP header for SharePoint REST call. By default, SharePoint responds with ATOM XML instead of JSON:
```javascript
pnp.setup({
    headers: {
        Accept: "application/json; odata=verbose",
    },
});
```
- Recreated the extensibility provider Visual Studio solution to reference the correct PnP NuGet package. **Be careful, the NuGet package version need to be the same as the PnP PowerShell one (2.11.1701.1 )**.
- Set the version of the npm module *sp-pnp-js* to 1.0.6 (fixed) instead of 2.0.0. The latest version outputs incorrect *const* keywords in the bundle created by Webpack. *const* is not interpreted by Internet Explorer (but Google Chrome does).
- Updated the **pageinfo.viewmodel.ts** component to work with SharePoint 2013. With SharePoint 2013, values must be retrieved via the "results" property of itemValue:

**Before**
```javascript
if (Array.isArray(itemValue)) {
...
```
**After**
```javascript
if (Array.isArray(itemValue.results)) {
...
```