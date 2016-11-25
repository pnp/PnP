'use strict';

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    
    //REST call
    $("#btnREST").click(function(){
        REST();
    });

    //JSOM call
    $("#btnJSOM").click(function () {
        JSOM();
    });
    

});

//This function for testing ListView Threshold for REST
function REST() {
    var configuration = SharePointClient.Configurations;
    var utility = new SharePointClient.Utilities.Utility();
    configuration.IsApp = true;
    

    var listServices = new SharePointClient.Services.REST.ListServices();

     //REST Listservices class for accessing list services
    var listServices = new SharePointClient.Services.REST.ListServices();

    //Set response type
    var responseType = SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON;

    var listTitle = "xyz";
    //Create Caml object
    var camlConstant = SharePointClient.Constants.CAML_CONSTANT;
    var camlQuery = new SharePointClient.CamlExtension.REST.CamlQuery();
    camlQuery.SetViewScopeAttribute(camlConstant.CAML_QUERY_SCOPE.RECURSIVE_ALL)
    .SetViewFieldsXml("<FieldRef Name='ID'/><FieldRef Name='Title'/>")
    .SetQuery("<Where><Geq><FieldRef Name=\"Modified\" /><Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\""
    + "StorageTZ=\"TRUE\">2014-08-05T15:50:08</Value></Geq></Where>")
    .OverrideQueryThrottleMode(camlConstant.CAML_QUERY_THROTTLE_MODE.OVERRIDE)
    .OverrideOrderByIndex()
    .SetRowLimit(5000);

    //Get All list items batch by list name
    listServices.GetListItemsBatchByListName(listTitle, camlQuery.BuildQuery(), responseType).Execute(function (result) {
        //logic for working with returned result set
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

        var listTitle = "xyz";
        //Create Caml object
        var camlConstant = SharePointClient.Constants.CAML_CONSTANT;
        var camlQuery = new SharePointClient.CamlExtension.JSOM.CamlQuery();
        camlQuery.ViewAttribute(camlConstant.CAML_QUERY_SCOPE.RECURSIVE_ALL)
        .Query("<Where><Geq><FieldRef Name=\"Modified\" /><Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\""
        + "StorageTZ=\"TRUE\">2015-08-05T15:50:08</Value></Geq></Where>")
        .ViewFieldsXml("<FieldRef Name='ID'/><FieldRef Name='Title'/>")
        .QueryThrottleMode(camlConstant.CAML_QUERY_THROTTLE_MODE.OVERRIDE)
        .OrderByIndex()
        .RowLimit(5000);

        //Get All list items batch by list name
        listServices.GetListItemsBatchByListName(context, listTitle, camlQuery.BuildQuery()).Execute(function (result) {
            //Read all items
        }); 
    });
}
