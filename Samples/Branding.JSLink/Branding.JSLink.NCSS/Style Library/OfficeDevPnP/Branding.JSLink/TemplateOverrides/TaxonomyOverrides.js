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

(function () {
    var fieldOverrides = {
        Templates: {
            Fields: {
                'MicrosoftProducts': {
                    'NewForm': jslinkTemplates.Taxonomy.editMode,
                    'EditForm': jslinkTemplates.Taxonomy.editMode
                }
            }
        }
    }

    // register our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldOverrides);
})();