(function () {
    var fieldOverrides = {
        Templates: {
            Fields: {
                'Colour': {
                    'DisplayForm': jslinkTemplates.Colours.display,
                    'View': jslinkTemplates.Colours.display,
                    'EditForm': jslinkTemplates.Colours.edit,
                    'NewForm': jslinkTemplates.Colours.edit
                }
            }
        }
    }

    // register our templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldOverrides);
})();