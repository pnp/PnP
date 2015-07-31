// create a safe namespace
Type.registerNamespace('jslinkOverride')
var jslinkOverride = window.jslinkOverride || {};
jslinkOverride.Colours = {};

jslinkOverride.Colours.Templates = {
    Fields: {
        'Colour': {
            'DisplayForm': jslinkTemplates.Colours.display,
            'View': jslinkTemplates.Colours.display,
            'EditForm': jslinkTemplates.Colours.edit,
            'NewForm': jslinkTemplates.Colours.edit
        }
    }
};

jslinkOverride.Colours.Functions = {};
jslinkOverride.Colours.Functions.RegisterTemplate = function () {
    // register our object, which contains our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkOverride.Colours);
};
jslinkOverride.Colours.Functions.MdsRegisterTemplate = function () {
    // register our custom template
    jslinkOverride.Colours.Functions.RegisterTemplate();

    // and make sure our custom view fires each time MDS performs
    // a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/FavouriteColours.js";
    RegisterModuleInit(thisUrl, jslinkOverride.Colours.Functions.RegisterTemplate)
};

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkOverride.Colours.Functions.MdsRegisterTemplate()
} else {
    // normal page load
    jslinkOverride.Colours.Functions.RegisterTemplate()
}