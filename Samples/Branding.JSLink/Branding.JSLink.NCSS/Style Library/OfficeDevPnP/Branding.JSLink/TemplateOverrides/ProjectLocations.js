(function () {
    var fieldOverrides = {
        Templates: {
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
        }
    };

    // register our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldOverrides);
})();

