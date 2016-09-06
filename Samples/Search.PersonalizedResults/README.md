# Search PersonalizedResults #

### Summary ###
This sample demonstrates how the usage of the Search API in apps and personalizing search results.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Search.PersonalizedResults| Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  |  August 7, 2014  | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General Comments #
This demonstrates a simple search example using the API, as well as a personalized search results example. The simple search example allows the user to provide a search filter to be used for a tenant-wide search and is looking for sites that apply to the user-supplied filter. Example code:

```C#
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

using (var clientContext = spContext.CreateUserClientContextForSPHost())
{
	// Since in this case we want only site collections, filter based on result type
    string query = searchtext.Text + " contentclass:\"STS_Site\"";
    ClientResult<ResultTableCollection> results = ProcessQuery(clientContext, query);
    lblStatus1.Text = FormatResults(results);
}

```

The **ProcessQuery** method builds a **KeywordQuery** object which is used by the **SearchExecutor** class and results are returned to the **ClientResult** object as shown below:

```C#
SearchExecutor searchExec = new SearchExecutor(ctx);
ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
ctx.ExecuteQuery();

```

The personalized search results example loads your user profile properties and checks for “Apptest” in the AboutMe profile property. If it is found, the search results return a list of any type of site template. If it is not found, the results will return only results of STS web templates:

```C#
private string ResolveAdditionalFilter(string aboutMeValue)
{
    if (!aboutMeValue.Contains("AppTest"))
    {
        return "WebTemplate=STS";
    }
    
    return "";
}
```

## Dependencies ##
- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- Microsoft.SharePoint.Client.Search.dll
- Microsoft.SharePoint.Client.UserProfiles.dll


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Search.PersonalizedResults" />