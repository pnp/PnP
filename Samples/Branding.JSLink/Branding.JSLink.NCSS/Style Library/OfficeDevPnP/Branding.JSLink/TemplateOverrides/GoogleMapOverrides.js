(function () {
    var fieldOverrides = {
        Templates: {
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
        }
    };

    // register our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldOverrides);
})();

