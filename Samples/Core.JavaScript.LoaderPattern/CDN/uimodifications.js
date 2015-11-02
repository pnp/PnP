// we can rely on jquery being loaded due to our embedder script loading it
$(function () {

    // Example 1: hide the new site link always
    $('#createnewsite').hide();


    // Example 2: hide the new site link conditionally based on group membership
    // to test this example you need to create a group named "TestGroup"

    //var newSiteLink = $('#createnewsite');

    //if (newSiteLink.length > 0) {

    //    // we have use of the global page vars here, such as _spPageContextInfo:    
    //    var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/sitegroups/getbyname('TestGroup')/Users/getbyid(" + _spPageContextInfo.userId + ")";

    //    $.ajax({
    //        headers: { 'accept': 'application/json;odata=verbose' },
    //        method: 'GET',
    //        url: restUrl
    //    }).fail(function () {
    //        // if the user is not found, we will end up here and we hide the link
    //        newSiteLink.hide();
    //    });
    //}

});