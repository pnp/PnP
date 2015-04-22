var jslinkTemplates = window.jslinkTemplates || {};

jslinkTemplates.Colours = function () {

    var selectIdPrefix = "COL_";
    var errorIdPrefix = "ERR_";

    var _display = function (ctx) {

        var returnHtml = "";
        returnHtml += "<div style='background: " + ctx.CurrentItem["Colour"] + "; width: 20px; float: left'>&nbsp;</div>";
        returnHtml += "<div style='margin-left: 30px;'>" + ctx.CurrentItem["Colour"] + "</div>";

        // return our string
        return returnHtml;
    };
    var _edit = function (ctx) {

        // create form context object
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

        // register our custom validator
        var validator = new SPClientForms.ClientValidation.ValidatorSet();
        validator.RegisterValidator(new jslinkTemplates.Colours.validator());
        formCtx.registerClientValidator(formCtx.fieldName, validator);

        // register our callbacks
        formCtx.registerGetValueCallback(formCtx.fieldName, jslinkTemplates.Colours.getValue.bind(null, formCtx.fieldName));
        formCtx.registerValidationErrorCallback(formCtx.fieldName, jslinkTemplates.Colours.onError.bind(null, formCtx.fieldName));
        
        var returnHtml = "";

        // hard coding the colours
        var colours = new Array("Red", "Green", "Blue");

        returnHtml += "<select id='" + selectIdPrefix + formCtx.fieldName + "'>";
        for (var i = 0; i < colours.length; i++) {
            // create a drop-down option
            returnHtml += "<option value='" + colours[i] + "'";

            if (ctx.CurrentItem["Colour"] == colours[i]) {
                // make sure the current field value is selected
                returnHtml += " selected='true' ";

            }
            returnHtml += ">" + colours[i] + "</option>";
        }
        returnHtml += "</select>";

        // add an error span for our validator to use
        returnHtml += "<br/><span id='" + errorIdPrefix + formCtx.fieldName + "' class='ms-formvalidation ms-csrformvalidation'></span><br/>";

        return returnHtml;
    };

    var _getValue = function (fieldName) {
        var selector = '#' + selectIdPrefix + fieldName;
        return $(selector).val();
    };
    var _validator = function () {
        jslinkTemplates.Colours.validator.prototype.Validate = function (value) {
            var isError = false;
            var errorMessage = "";

            if (value == "Blue") {
                isError = true;
                errorMessage = "Sorry, we don't like the colour blue";
            }

            return new SPClientForms.ClientValidation.ValidationResult(isError, errorMessage);
        };
    };
    var _onError = function (fieldName, error) {
        var selector = '#' + errorIdPrefix + fieldName;
        $(selector).html("<span role='alert'>" + error.errorMessage + "</span>");
    };

    return {
        "display": _display,
        "edit": _edit,
        "getValue": _getValue,
        "validator": _validator,
        "onError": _onError
    }
}();