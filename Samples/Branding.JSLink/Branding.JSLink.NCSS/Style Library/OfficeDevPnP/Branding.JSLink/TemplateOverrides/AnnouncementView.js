/* 

 This is a standalone filed, intended to be plugged into a Web Part

 Create an Announcements List, add it to a page (as a List View Web Part) 
 and make sure the "Body" is included in the view.

 The open Web Part Properties and go to "Miscellaneous > JSLink" and enter the value:
 
 ~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/AnnouncementView.js

 Enjoy! 

 */


// create a safe namespace
var jslinkViews = window.jslinkViews || {};

jslinkViews.itemHtml = function (ctx) {
    var modifiedDate = new Date(Date.parse(ctx.CurrentItem.Modified));

    // start with a <tr> and a <td>
    var returnHtml = "<tr><td colspan='3'>";
    returnHtml += "<h2>" + ctx.CurrentItem.Title + " | " + modifiedDate.toLocaleDateString() + "</h2>";
    returnHtml += "<p>" + ctx.CurrentItem.Body + "</p>";
    returnHtml += "</td></tr>";

    return returnHtml;
};

(function () {
    var viewTemplate = {};
    viewTemplate.Templates = {};

    // use my own custom header / footer
    // note - you can also return the HTML here, instead of using a function
    viewTemplate.Templates.Header = "<div id='MyCustomView'>";
    viewTemplate.Templates.Item = jslinkViews.itemHtml;
    viewTemplate.Templates.Footer = "</div>";

    // 104 is the template type for Announcements
    viewTemplate.ListTemplateType = 104;
    viewTemplate.BaseViewID = 1;
    
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(viewTemplate);
})();