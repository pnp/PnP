// create a safe namespace
Type.registerNamespace('jslinkOverride')
var jslinkOverride = window.jslinkOverride || {};
jslinkOverride.ProjectLocations = {};

jslinkOverride.ProjectLocations.Templates = {
    Fields: {
        'Region': {
            'NewForm': jslinkTemplates.Lookups.Generic.SingleItem.editForm,
            'EditForm': jslinkTemplates.Lookups.Generic.SingleItem.editForm,
            'View': jslinkTemplates.Lookups.Generic.view,
            'DisplayForm': jslinkTemplates.Lookups.Generic.displayForm
        },
        'Country': {
            'NewForm': jslinkTemplates.Lookups.Filtered.editForm.bind(null, "Region"),
            'EditForm': jslinkTemplates.Lookups.Filtered.editForm.bind(null, "Region"),
            'View': jslinkTemplates.Lookups.Generic.view,
            'DisplayForm': jslinkTemplates.Lookups.Generic.displayForm
        },
        'AssociatedCountries': {
            'NewForm': jslinkTemplates.Lookups.Filtered.editForm.bind(null, "Region"),
            'EditForm': jslinkTemplates.Lookups.Filtered.editForm.bind(null, "Region"),
            'View': jslinkTemplates.Lookups.Generic.view,
            'DisplayForm': jslinkTemplates.Lookups.Generic.displayForm
        }
    }
};

jslinkOverride.ProjectLocations.Functions = {};
jslinkOverride.ProjectLocations.Functions.RegisterTemplate = function () {
    // register our object, which contains our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkOverride.ProjectLocations);
};
jslinkOverride.ProjectLocations.Functions.MdsRegisterTemplate = function () {
    // register our custom template
    jslinkOverride.ProjectLocations.Functions.RegisterTemplate();

    // and make sure our custom view fires each time MDS performs
    // a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/ProjectLocations.js";
    RegisterModuleInit(thisUrl, jslinkOverride.ProjectLocations.Functions.RegisterTemplate);
};

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkOverride.ProjectLocations.Functions.MdsRegisterTemplate();
} else {
    // normal page load
    jslinkOverride.ProjectLocations.Functions.RegisterTemplate();
}