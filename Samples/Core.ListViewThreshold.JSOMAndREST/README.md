# Retrieve more items than Threshold limit with JSOM and REST #
----------------------------------------------------------
### Summary ###
<br><br>
In SharePoint, when you execute query on Large List, you will receive "The attempted operation is prohibited because it exceeds the list view threshold enforced by the administrator". This solution implements the retrieving SharePoint list items more than threshold limit by using JSOM and REST.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Core.ListViewThreshold.JSOMAndREST | Anil Lakhagoudar

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 7th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #

In SharePoint, when you execute query on Large List, you will receive "The attempted operation is prohibited because it exceeds the list view threshold enforced by the administrator". To avoid this exception and read list items by batch we can use Content Iterator class which is available in Server Object Model. 

In SharePoint 2013 App Model JavaScript Object Modal and REST is used for interacting with SharePoint. To retrieve SharePoint list items more than Threshold limit from JSOM (SharePoint JavaScript Object Modal) need to extend client side CamlQuery functionalities which can support the retrieving of list items batch by batch. In REST there is “RenderListData” end point for retrieving the lists items by batch by using extended CamlQuery functionalities.  

This Client Side “SharePointClient.min.Js” file can be used to read the SharePoint list items batch by batch by using the client side CamlQuery extension methods to modify the query which can override the Throttle limit.

##### The "SharePointClient" JavaScript file has two functionalities #####

1. JSOM – SharePoint JavaScript Object Model
2. REST – SharePoint REST service end points

## How to Use? ##

##### Configuration #####

To work with both JSOM and REST components in this SharePointClient.min.js default configurations can be overridden according to the requirement.
Default configuration are below

1. **IsApp** : (default false) – This property can be set true if working on SharePoint App else false
2. **SPHostUrl** : (optional) – This property has the value for host Url working with SharePoint Apps
3. **SPAppWebUrl** : (optional) – This property has the value for App Web Url working with SharePoint Apps.
4. **IsCrossDomainRequest** : (default false) -  This property can be set true if working on SharePoint App and trying                                                   to access hostweb data because this is cross domain access.
5. **SPUrl** : (default null) – This property can be set if the SharePoint context can be created for this particular                                 Url.
6. **REST.AccessToken**  : (default null) – This property can be set if working provider hosted app where AccessToken can         be retrieved from Token helper class. And this Access token can used for all sub sequent REST calls.


### Configuration for SharePoint page ###
No need of overriding the configuration for SharePoint page, default configuration will work for this scenario.

### Configuration for SharePoint Apps ###
_Connecting to App Web_

```javascript
    //Modify the default configurations 
    var configuration = SharePointClient.Configurations;
    configuration.IsApp = true; //This configuration will verify whether working on SharePoint App or Page
```
_Connecting to HostWeb from AppWeb_

```javascript
    //Modify the default configurations 
    var configuration = SharePointClient.Configurations;
    configuration.IsApp = true; //This configuration will verify whether working on SharePoint App or Page
    configuration.IsCrossDomainRequest = true; //Cross domain request, for example app web can request data from host     web.
```





### Using SharePointClient Js for JSOM ###

```javascript
//Modify the default configurations 
    var configuration = SharePointClient.Configurations;
    var utility = new SharePointClient.Utilities.Utility();
    configuration.IsApp = true; //This configuration will verify whether working on SharePoint App or Page
    
    //Initialize the required Js files to download for example SP.Js, SP.Runtime.js
    SharePointClient.Services.JSOM.Initialize(function () {
        var listServices = new SharePointClient.Services.JSOM.ListServices();

        //Get SP clientContext
        var context = new SharePointClient.Services.JSOM.Context();

        //Create Caml object
        var camlConstant = SharePointClient.Constants.CAML_CONSTANT;
        var camlQuery = new SharePointClient.CamlExtension.JSOM.CamlQuery();
        camlQuery.ViewAttribute(camlConstant.CAML_QUERY_SCOPE.recursiveAll)
        .Query("<Where><Geq><FieldRef Name=\"Modified\" /><Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\" StorageTZ=\"TRUE\">2015-08-05T15:50:08</Value></Geq></Where>")
        .ViewFieldsXml("<FieldRef Name='ID'/><FieldRef Name='Title'/>")
        .QueryThrottleMode(camlConstant.CAML_QUERY_THROTTLE_MODE.override)
        .OrderByIndex()
        .RowLimit(5000);

        var listTitle = "";
        listServices.GetLargeListItemsByBatch(context, listTitle, camlQuery.BuildQuery(), function (result) {
            alert(result.get_count());
        });

```
### Using SharePointClient Js for REST ###

```javascript
//Modify the default configurations
    var configuration = SharePointClient.Configurations;
    var utility = new SharePointClient.Utilities.Utility();
    configuration.IsApp = true;//This configuration will verify whether working on SharePoint App or Page
    

    var listServices = new SharePointClient.Services.REST.ListServices();

    //Create Caml object
    var camlConstant = SharePointClient.Constants.CAML_CONSTANT;
    var camlQuery = new SharePointClient.CamlExtension.REST.CamlQuery();
    camlQuery.SetViewScopeAttribute(camlConstant.CAML_QUERY_SCOPE.recursiveAll)
    .SetViewFieldsXml("<FieldRef Name='ID'/><FieldRef Name='Title'/>")
    .SetQuery("<Where><Geq><FieldRef Name=\"Modified\" /><Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\" StorageTZ=\"TRUE\">2014-08-05T15:50:08</Value></Geq></Where>")
    .OverrideQueryThrottleMode(camlConstant.CAML_QUERY_THROTTLE_MODE.override)
    .OverrideOrderByIndex()
    .SetRowLimit(5000);

    var listTitle = "";
    var responseType = SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON;
    listServices.GetListItemsByListName(listTitle, camlQuery.BuildQuery(), responseType,
        function (result) {
            var finalResult;
            if (responseType == SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                if (!SharePointClient.Configurations.IsCrossDomainRequest) {
                    finalResult = $.parseJSON(result.d.RenderListData);
                } else {
                    finalResult = $.parseJSON($.parseJSON(result).d.RenderListData);
                }

            } else {
                finalResult = $.parseJSON($($.parseXML(result).lastChild).text());
            }

            alert(finalResult.Row.length);
            var str = "";
            $.each(finalResult.Row, function (index, value) {
                str += value.ID + "/" + value.Title + ";";
            });
            alert(str);
        });
```
