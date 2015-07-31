/* 

 This is a "standalone" file, intended to be plugged into a Web Part

 Create an Announcements List, add it to a page (as a List View Web Part) 
 and make sure the "Body" is included in the view.

 The open Web Part Properties and go to "Miscellaneous > JSLink" and enter the value:
 
 ~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/jquery-1.10.2.min.js|~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/Generics/AnnouncementAccordion.js|~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/AnnouncementAccordionView.js

 */

// Create a safe namespace
Type.registerNamespace('jslinkViews')
var jslinkViews = window.jslinkViews || {};
jslinkViews.AnnouncementAccordion = {};

jslinkViews.AnnouncementAccordion.Templates = {};
jslinkViews.AnnouncementAccordion.OnPreRender = jslinkTemplates.Announcements.Accordion.onPreRender;
jslinkViews.AnnouncementAccordion.Templates.Header = '<div class="accordion" role="tablist"><ul style="list-style-type: none;">';
jslinkViews.AnnouncementAccordion.Templates.Item = jslinkTemplates.Announcements.Accordion.item;
jslinkViews.AnnouncementAccordion.Templates.Footer = '</ul></div>';
jslinkViews.AnnouncementAccordion.OnPostRender = jslinkTemplates.Announcements.Accordion.onPostRender;

jslinkViews.AnnouncementAccordion.Functions = {};
jslinkViews.AnnouncementAccordion.Functions.RegisterTemplate = function () {
    // Register our object, which contains our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkViews.AnnouncementAccordion);
};
jslinkViews.AnnouncementAccordion.Functions.MdsRegisterTemplate = function () {
    // Register our custom template
    jslinkViews.AnnouncementAccordion.Functions.RegisterTemplate();

    // And make sure our custom view fires each time MDS performs a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/AnnouncementAccordionView.js";
    RegisterModuleInit(thisUrl, jslinkViews.AnnouncementAccordion.Functions.RegisterTemplate)
};
if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkViews.AnnouncementAccordion.Functions.MdsRegisterTemplate()
} else {
    // normal page load
    jslinkViews.AnnouncementAccordion.Functions.RegisterTemplate()
}
