Retrieve more items than Threshold limit with JSOM and REST
----------------------------------------------------------
**Summary**
<br><br>
In SharePoint, when you execute query on Large List, you will receive "The attempted operation is prohibited because it exceeds the list view threshold enforced by the administrator". This solution implements the retrieving SharePoint list items more than threshold limit by using JSOM and REST. 
<br><br>
**Solution**
<br>
Core.ListViewThreshold.JSOMAndREST
<br>
<br>
How to Use?
-------------------------
<br>
<br>
*Using SharePointClient Js for JSOM*
<pre>
<code>
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
</cod>
</pre>
------------------------
*Using SharePointClient Js for REST*
<pre>
<code>
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
</code>
</pre>
