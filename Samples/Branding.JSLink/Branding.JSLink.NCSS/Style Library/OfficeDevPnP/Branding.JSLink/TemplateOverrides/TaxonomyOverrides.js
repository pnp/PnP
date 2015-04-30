/* 
  Note - the "MicrosoftProducts" field is not included
  in this solution.

  Either change this to the name of your Taxonomy field, 
  or add a new field of type Taxonomy using the name "MicrosoftProducts"
  (create it without a space or the internal name will be wrong!)

  Note - If you want to use the sample term set then you can import 
  it into your Term Store using the "Products.csv" file in the
  TermSet solution folder. Just go to the Term Management tool and select
  "import terms".

  You'll then need to include two JavaScript files on the page (either by
  binding it to a field in the list using JSLink, or just by adding it to 
  the Edit and New forms by updating the JSLink property on the List Form 
  Web Parts).

    ~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/Generics/ManagedMetadata.js|~sitecollection/Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/TaxonomyOverrides.js
  
  For more information on doing this please refer to the documentation
  associated with this sample.
 */

// create a safe namespace
Type.registerNamespace('jslinkOverride')
var jslinkOverride = window.jslinkOverride || {};
jslinkOverride.Taxonomy = {};

jslinkOverride.Taxonomy.Templates = {
    Fields: {
        'MicrosoftProducts': {
            'NewForm': jslinkTemplates.Taxonomy.editMode,
            'EditForm': jslinkTemplates.Taxonomy.editMode
        }
    }
};

jslinkOverride.Taxonomy.Functions = {};
jslinkOverride.Taxonomy.Functions.RegisterTemplate = function () {
    // register our object, which contains our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(jslinkOverride.Taxonomy);
};
jslinkOverride.Taxonomy.Functions.MdsRegisterTemplate = function () {
    // register our custom template
    jslinkOverride.Taxonomy.Functions.RegisterTemplate();

    // and make sure our custom view fires each time MDS performs
    // a page transition
    var thisUrl = _spPageContextInfo.siteServerRelativeUrl + "Style Library/OfficeDevPnP/Branding.JSLink/TemplateOverrides/TaxonomyOverrides.js";
    RegisterModuleInit(thisUrl, jslinkOverride.Taxonomy.Functions.RegisterTemplate)
};

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    // its an MDS page refresh
    jslinkOverride.Taxonomy.Functions.MdsRegisterTemplate()
} else {
    // normal page load
    jslinkOverride.Taxonomy.Functions.RegisterTemplate()
}