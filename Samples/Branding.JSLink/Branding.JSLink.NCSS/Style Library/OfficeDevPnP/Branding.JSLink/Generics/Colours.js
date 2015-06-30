var jslinkTemplates = window.jslinkTemplates || {};

jslinkTemplates.Colours = function () {

    var selectIdPrefix = "COL_";
    var errorIdPrefix = "ERR_";

    var _display = function (ctx) {

        // return our string
        return _getColourSpan(ctx.CurrentItem["Colour"]);
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

        //#region array of available colours
        var colours = new Array("AliceBlue",
                                "AntiqueWhite",
                                "Aqua",
                                "Aquamarine",
                                "Azure",
                                "Beige",
                                "Bisque",
                                "Black",
                                "BlanchedAlmond",
                                "Blue",
                                "BlueViolet",
                                "Brown",
                                "BurlyWood",
                                "CadetBlue",
                                "Chartreuse",
                                "Chocolate",
                                "Coral",
                                "CornflowerBlue",
                                "Cornsilk",
                                "Crimson",
                                "Cyan",
                                "DarkBlue",
                                "DarkCyan",
                                "DarkGoldenRod",
                                "DarkGray",
                                "DarkGreen",
                                "DarkKhaki",
                                "DarkMagenta",
                                "DarkOliveGreen",
                                "DarkOrange",
                                "DarkOrchid",
                                "DarkRed",
                                "DarkSalmon",
                                "DarkSeaGreen",
                                "DarkSlateBlue",
                                "DarkSlateGray",
                                "DarkTurquoise",
                                "DarkViolet",
                                "DeepPink",
                                "DeepSkyBlue",
                                "DimGray",
                                "DodgerBlue",
                                "FireBrick",
                                "FloralWhite",
                                "ForestGreen",
                                "Fuchsia",
                                "Gainsboro",
                                "GhostWhite",
                                "Gold",
                                "GoldenRod",
                                "Gray",
                                "Green",
                                "GreenYellow",
                                "HoneyDew",
                                "HotPink",
                                "IndianRed",
                                "Indigo",
                                "Ivory",
                                "Khaki",
                                "Lavender",
                                "LavenderBlush",
                                "LawnGreen",
                                "LemonChiffon",
                                "LightBlue",
                                "LightCoral",
                                "LightCyan",
                                "LightGoldenRodYellow",
                                "LightGray",
                                "LightGreen",
                                "LightPink",
                                "LightSalmon",
                                "LightSeaGreen",
                                "LightSkyBlue",
                                "LightSlateGray",
                                "LightSteelBlue",
                                "LightYellow",
                                "Lime",
                                "LimeGreen",
                                "Linen",
                                "Magenta",
                                "Maroon",
                                "MediumAquaMarine",
                                "MediumBlue",
                                "MediumOrchid",
                                "MediumPurple",
                                "MediumSeaGreen",
                                "MediumSlateBlue",
                                "MediumSpringGreen",
                                "MediumTurquoise",
                                "MediumVioletRed",
                                "MidnightBlue",
                                "MintCream",
                                "MistyRose",
                                "Moccasin",
                                "NavajoWhite",
                                "Navy",
                                "OldLace",
                                "Olive",
                                "OliveDrab",
                                "Orange",
                                "OrangeRed",
                                "Orchid",
                                "PaleGoldenRod",
                                "PaleGreen",
                                "PaleTurquoise",
                                "PaleVioletRed",
                                "PapayaWhip",
                                "PeachPuff",
                                "Peru",
                                "Pink",
                                "Plum",
                                "PowderBlue",
                                "Purple",
                                "RebeccaPurple",
                                "Red",
                                "RosyBrown",
                                "RoyalBlue",
                                "SaddleBrown",
                                "Salmon",
                                "SandyBrown",
                                "SeaGreen",
                                "SeaShell",
                                "Sienna",
                                "Silver",
                                "SkyBlue",
                                "SlateBlue",
                                "SlateGray",
                                "Snow",
                                "SpringGreen",
                                "SteelBlue",
                                "Tan",
                                "Teal",
                                "Thistle",
                                "Tomato",
                                "Turquoise",
                                "Violet",
                                "Wheat",
                                "White",
                                "WhiteSmoke",
                                "Yellow",
                                "YellowGreen");
        //#endregion

        returnHtml += "<select id='" + selectIdPrefix + formCtx.fieldName + "'>";
        for (var i = 0; i < colours.length; i++) {
            // create a drop-down option
            returnHtml += "<option value='" + colours[i] + "'";

            if (ctx.CurrentItem["Colour"] == colours[i]) {
                // make sure the current field value is selected
                returnHtml += " selected='true' ";

            }
            returnHtml += ">" + _getColourSpan(colours[i]) + "</option>";
        }
        returnHtml += "</select>";

        // add an error span for our validator to use
        returnHtml += "<br/><span id='" + errorIdPrefix + formCtx.fieldName + "' class='ms-formvalidation ms-csrformvalidation'></span><br/>";

        return returnHtml;
    };

    var _getColourSpan = function(colour) {
        var returnHtml = "";
        returnHtml += "<div style='background: " + colour + "; width: 20px; float: left'>&nbsp;</div>";
        returnHtml += "<div style='margin-left: 30px;'>" + colour + "</div>";

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