// List add and edit – Email Regex Validator Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office PnP

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterEmailFiledContext();
}

function RegisterInMDS() {
    // RegisterEmailFiledContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/RegexValidator.js", RegisterEmailFiledContext);
    //RegisterEmailFiledContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterEmailFiledContext();
}


function RegisterEmailFiledContext() {

    // Create object that has the context information about the field that we want to render differently
    var emailFiledContext = {};
    emailFiledContext.Templates = {};
    emailFiledContext.Templates.Fields = {
        // Apply the new rendering for Email field on New and Edit Forms
        "Email": {
            "NewForm": emailFiledTemplate,
            "EditForm": emailFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(emailFiledContext);

}

// This function provides the rendering logic
function emailFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // Register a callback just before submit.
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return document.getElementById('inpEmail').value;
    });

    //Create container for various validations
    var validators = new SPClientForms.ClientValidation.ValidatorSet();
    validators.RegisterValidator(new emailValidator());

    // Validation failure handler.
    formCtx.registerValidationErrorCallback(formCtx.fieldName, emailOnError);

    formCtx.registerClientValidator(formCtx.fieldName, validators);

    return "<span dir='none'><input type='text' value='" + formCtx.fieldValue + "'  maxlength='255' id='inpEmail' class='ms-long'> \
            <br><span id='spnError' class='ms-formvalidation ms-csrformvalidation'></span></span>";
}

// Custom validation object to validate email format
emailValidator = function () {
    emailValidator.prototype.Validate = function (value) {
        var isError = false;
        var errorMessage = "";

        if (!validateEmail(value)) {
            isError = true;
            errorMessage = "Invalid email address";
        }

        //Send error message to error callback function (emailOnError)
        return new SPClientForms.ClientValidation.ValidationResult(isError, errorMessage);
    };
};

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}
// Add error message to spnError element under the input field element
function emailOnError(error) {
    document.getElementById("spnError").innerHTML = "<span role='alert'>" + error.errorMessage + "</span>";
}
