'use strict';

var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    $("#wrapper").on("click", "#btnAjaxUpload", function (evt) {
        evt.preventDefault();
        AjaxUpload.Uploader().Upload("/images/sampleimage.jpg", "/SiteAssets/ajaxUpload.jpg");
    })
    // Fetch files form images of SharePoint Hosted App and add it to site assets
    $("#wrapper").on("click", "#btnBinUpload", function (evt) {
        evt.preventDefault();
        BinaryUpload.Uploader().Upload("/images/sampleimage.jpg", "/SiteAssets/binUpload.jpg");
    })


});
