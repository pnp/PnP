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

This JavaScript file has two functionalities
a)	JSOM – SharePoint JavaScript Object Model
b)	REST – SharePoint REST service end points

## How to Use? ##
### Using SharePointClient Js for JSOM ###

```C#
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

```C#
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
