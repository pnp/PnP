// List New and Edit Forms – Disable Input Control Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office AMS
(function () {

    // Create object that has the context information about the field that we want to render differently
    var disableFiledContext = {};
    disableFiledContext.Templates = {};
    disableFiledContext.Templates.Fields = {
        // Apply the new rendering for the field on New and Edit forms
        "Age": {
            "EditForm": disableFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(disableFiledContext);

})();


// This function provides the rendering logic
function disableFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
}

