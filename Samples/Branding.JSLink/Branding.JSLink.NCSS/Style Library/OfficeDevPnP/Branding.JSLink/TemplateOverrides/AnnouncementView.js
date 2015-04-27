/* 

 This is a standalone filed, intended to be plugged into a Web Part

 Create an Announcements List, add it to a page (as a List View Web Part) 
 and make sure the "Body" is included in the view.

 The open Web Part Properties and go to "Miscellaneous > JSLink" and enter the value:
 
 ~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/AnnouncementView.js

 Enjoy! 

 */

// create a safe namespace
Type.registerNamespace('jslinkViews')
var jslinkViews = window.jslinkViews || {};

// our custom Template object
jslinkViews.Templates = {};
jslinkViews.Templates.Header = "<div id='MyCustomView'>";
jslinkViews.Templates.Item = jslinkViews.Functions.itemHtml;
jslinkViews.Templates.Footer = "</div>";
jslinkViews.ListTemplateType = 104; // target this at Announcements only!

jslinkViews.Functions = {};
jslinkViews.Functions.itemHtml = function (ctx) {
    var modifiedDate = new Date(Date.parse(ctx.CurrentItem.Modified));

    // start with a <tr> and a <td>
    var returnHtml = "<tr><td colspan='3'>";
    returnHtml += "<h2>" + ctx.CurrentItem.Title + " | " + modifiedDate.toLocaleDateString() + "</h2>";
    returnHtml += "<p>" + ctx.CurrentItem.Body + "</p>";
    returnHtml += "</td></tr>";

    return returnHtml;
};
jslinkViews.Functions.RegisterTemplate = function() {
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkViews);
};
jslinkViews.Functions.MdsRegisterTemplate = function () {

    // register our custom view
    jslinkViews.RegisterTemplate();

    // and make sure our custom view fires each time MDS performs
    // a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/AnnouncementView.js";
    RegisterModuleInit(thisUrl, jslinkViews.Functions.RegisterTemplate)
};

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkViews.Functions.MdsRegisterTemplate()
} else {
    // normal page load
    jslinkViews.Functions.RegisterTemplate()
}