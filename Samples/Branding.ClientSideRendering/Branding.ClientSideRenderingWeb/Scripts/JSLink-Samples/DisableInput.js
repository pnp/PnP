// List New and Edit Forms – Disable Input Control Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office PnP

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterDisableFiledContext();
}

function RegisterInMDS() {
    // RegisterDisableFiledContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/DisableInput.js", RegisterDisableFiledContext);
    //RegisterDisableFiledContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterDisableFiledContext();
}

function RegisterDisableFiledContext () {

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

}


// This function provides the rendering logic
function disableFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
}

