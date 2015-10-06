'use strict';

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    
    //REST call
    $("#btnREST").click(function(){
        REST();
    });

    //JSOM call
    $("#btnREST").click(function(){
        JSOM();
    });
    

});

//This function for testing ListView Threshold for REST
function REST() {
    var configuration = SharePointClient.Configurations;
    var utility = new SharePointClient.Utilities.Utility();
    configuration.IsApp = true;
    

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
                if (result.d != "undefined") {
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
}

//This function for testing ListView Threshold for JSOM
function JSOM() {
    var configuration = SharePointClient.Configurations;
    var utility = new SharePointClient.Utilities.Utility();
    configuration.IsApp = true;
    

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
    });
}
