// List View - Substring Long String Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office PnP

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterBodyFiledContext();
}

function RegisterInMDS() {
    // RegisterBodyFiledContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/SubstringLongText.js", RegisterBodyFiledContext);
    //RegisterBodyFiledContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterBodyFiledContext();
}


function RegisterBodyFiledContext() {

    // Create object that has the context information about the field that we want to render differently 
    var bodyFiledContext = {};
    bodyFiledContext.Templates = {};
    bodyFiledContext.Templates.Fields = {
        // Apply the new rendering for Body field in list view
        "Body": { "View": bodyFiledTemplate }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(bodyFiledContext);

}

// This function provides the rendering logic
function bodyFiledTemplate(ctx) {

    var bodyValue = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];

    //This regex expression use to delete html tags from the Body field
    var regex = /(<([^>]+)>)/ig;

    bodyValue = bodyValue.replace(regex, "");

    var newBodyValue = bodyValue;

    if (bodyValue && bodyValue.length >= 100)
    {
        newBodyValue = bodyValue.substring(0, 100) + " ...";
    }

    return "<span title='" + bodyValue + "'>" + newBodyValue + "</span>";
       
}

