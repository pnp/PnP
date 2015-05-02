// create namespace
var jslinkTemplates = window.jslinkTemplates || {};

jslinkTemplates.Generics = function () {

    // renders ANY field as a read-only value
    // NOTE - does not work with multi-select fields!
    _renderPlainText = function (ctx) {

        var fieldValue = ctx.CurrentFieldValue.toString();

        if (fieldValue.indexOf(";#") != -1) {
            // handle Lookup Fields
            fieldValue = fieldValue.substring(fieldValue.indexOf(";#") + 2);
        }

        // place in a div to make sure the description field (if present)
        // wraps to the next line
        return "<div>" + fieldValue + "</div>";
    };

    return {
        "renderPlainText": _renderPlainText
    }
}();