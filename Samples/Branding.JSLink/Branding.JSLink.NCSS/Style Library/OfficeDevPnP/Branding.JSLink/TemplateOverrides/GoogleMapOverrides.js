// create a safe namespace
Type.registerNamespace('jslinkOverride')
var jslinkOverride = window.jslinkOverride || {};
jslinkOverride.GoogleMaps = {};

jslinkOverride.GoogleMaps.Templates = {
    Fields: {
        // GOOGLE MAPS FIELDS
        'LocationPoint': {
            'NewForm': jslinkGoogleMaps.PointValue.editForm,
            'EditForm': jslinkGoogleMaps.PointValue.editForm,
            'DisplayForm': jslinkGoogleMaps.PointValue.displayForm,
            'View': jslinkGoogleMaps.PointValue.view
        },
        'LocationArea': {
            'NewForm': jslinkGoogleMaps.SpacialValue.editForm,
            'EditForm': jslinkGoogleMaps.SpacialValue.editForm,
            'DisplayForm': jslinkGoogleMaps.SpacialValue.displayForm,
            'View': jslinkGoogleMaps.SpacialValue.view
        }
    }
};

jslinkOverride.GoogleMaps.Functions = {};
jslinkOverride.GoogleMaps.Functions.RegisterTemplate = function () {
    // register our object, which contains our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkOverride.GoogleMaps);
};
jslinkOverride.GoogleMaps.Functions.MdsRegisterTemplate = function () {
    // register our custom template
    jslinkOverride.GoogleMaps.Functions.RegisterTemplate();

    // and make sure our custom view fires each time MDS performs a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/GoogleMapOverrides.js";
    RegisterModuleInit(thisUrl, jslinkOverride.GoogleMaps.Functions.RegisterTemplate)
};

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkOverride.GoogleMaps.Functions.MdsRegisterTemplate()
} else {
    // normal page load
    jslinkOverride.GoogleMaps.Functions.RegisterTemplate()
}