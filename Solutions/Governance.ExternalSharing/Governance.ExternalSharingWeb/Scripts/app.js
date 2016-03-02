
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;

//Wait for the page to load
$(document).ready(function () {

    setAppPageInitStatus();

    var hostweburl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appweburl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    // resources are in URLs in the form:
    // web_url/_layouts/15/resource
    var scriptbase = hostweburl + "/_layouts/15/";

    // Load the js files and continue to the successHandler
    $.getScript(scriptbase + "SP.Runtime.js",
        function () {
            $.getScript(scriptbase + "SP.js",
                function () {
                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                         function () {

                             if (appweburl != 'undefined') {
                                 context = new SP.ClientContext(appweburl);
                                 var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                 context.set_webRequestExecutorFactory(factory);
                             }
                         }
                    );
                }
            );
        }
    );

});


//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}

function ParseBool(val) {

    var returnValue = false;

    try {
        val = val.toString().toUpperCase();

        if (val == "1" || val == "TRUE") {
            returnValue = true;
        } else if (val == "0" || val == "FALSE") {
            returnValue = false;
        }
    } catch (ex)
    { }

    return returnValue;
}

var initial_external_sharing_enabled = false;

function setAppPageInitStatus() {

    $('input[id$=btnSave]').prop('disabled', true);
    initial_external_sharing_enabled = ParseBool($("input[id$=HiddenField_Init_ExternalSharing_Enabled]").val());
}

function GetRadioButtonListSelectedValue(radioButtonList) {

    for (var i = 0; i < radioButtonList.rows.length; ++i) {
        if (radioButtonList.rows[i].cells[0].querySelectorAll("input[type=radio]")[0].checked) {
            //isChecked = true;

            var selected_external_sharing_enabled = radioButtonList.rows[i].cells[0].querySelectorAll("input[type=radio]")[0].value == 'allowed';

            // value changed, allow to submit request
            if (selected_external_sharing_enabled != initial_external_sharing_enabled) {
                $('input[id$=btnSave]').prop('disabled', false);
            } else {
                $('input[id$=btnSave]').prop('disabled', true);
            }

            // disable feature, but it was active before, show warning message
            if (!selected_external_sharing_enabled && initial_external_sharing_enabled) {
                $('#disable_external_sharing_warning').show();
            } else {
                $('#disable_external_sharing_warning').hide();
            }
        }
    }
}

