// List view, display, add and edit – Percent Complete Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office PnP

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterPercentCompleteFiledContext();
}

function RegisterInMDS() {
    // RegisterPercentCompleteFiledContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/PercentComplete.js", RegisterPercentCompleteFiledContext);
    //RegisterPercentCompleteFiledContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterPercentCompleteFiledContext();
}


function RegisterPercentCompleteFiledContext () {

    // Create object that has the context information about the field that we want to render differently
    var percentCompleteFiledContext = {};
    percentCompleteFiledContext.Templates = {};
    percentCompleteFiledContext.Templates.Fields = {
        // Apply the new rendering for PercentComplete field in List View, Display, New and Edit forms
        "PercentComplete": { 
            "View": percentCompleteViewFiledTemplate,
            "DisplayForm": percentCompleteViewFiledTemplate,
            "NewForm": percentCompleteEditFiledTemplate,
            "EditForm": percentCompleteEditFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(percentCompleteFiledContext);

}

// This function provides the rendering logic for View and Display form
function percentCompleteViewFiledTemplate(ctx) {

    var percentComplete = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    return "<div style='background-color: #e5e5e5; width: 100px;  display:inline-block;'> \
            <div style='width: " + percentComplete.replace(/\s+/g, '') + "; background-color: #0094ff;'> \
            &nbsp;</div></div>&nbsp;" + percentComplete;

}

// This function provides the rendering logic for New and Edit forms
function percentCompleteEditFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // Register a callback just before submit.
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return document.getElementById('inpPercentComplete').value;
    });

    return "<input type='range' id='inpPercentComplete' name='inpPercentComplete' min='0' max='100' \
            oninput='outPercentComplete.value=inpPercentComplete.value' value='" + formCtx.fieldValue + "' /> \
            <output name='outPercentComplete' for='inpPercentComplete' >" + formCtx.fieldValue + "</output>%";

}
