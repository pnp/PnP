# My Colleagues App using the Office Graph #

### Summary ###
This sample shows how to consume the Office Graph API to return the collegues with whom you most closely work with.


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
All Office 365 tenants should have the Office Graph API now enabled.

### Solution ###
Solution | Author(s)
---------|----------
OfficeGraph.Demo.App | Vardhaman Deshpande

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 3rd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Introduction #

The App queries the Office Graph for the colleagues with whom you closely work with and displays them. Basically, it does the same thing which you see on the left hand side when you log in to Delve. This app can be a good starting point if you want to further develop apps which consume the Office Graph API. You can pick and choose the Delve elements which you want in your app. Also, you can create customized scenarios which leverage the Office Graph and deliver them as Apps for SharePoint. 

![Add-in UI](http://i.imgur.com/MHTSLS5.png)


# Required Details #
Since the App requires Search permissions to be granted in the AppManifest, you will need to have the Tenant Admin credentials to install the App. I think this might be a blocker for some people in installing the App. Now fortunately, if you just want to test the functionality out, you can take all the code in the App.js and run it from inside a SharePoint Online page. You do not need tenant admin permissions to execute the code in the page.


# Search Query to get the closest Colleagues of the current user  #

```JS

var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/search/query?Querytext='*'" +
                    "&Properties='GraphQuery:ACTOR(ME\\, action\\:1019),GraphRankingModel:{\"features\"\\:[{\"function\"\\:\"EdgeWeight\"}]}'" +
                    "&RankingModelId='0c77ded8-c3ef-466d-929d-905670ea1d72'" +
                    "&SelectProperties='Title,UserName,Path'" +
                    "&RowLimit=10";
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/OfficeGraph.Demo.App" />