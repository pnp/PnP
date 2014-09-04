/// <reference path="..\..\Scripts/Office.Controls.js" />
/// <reference path="..\..\Scripts/Office.Controls.PeoplePicker.js" />


// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;

var peoplePickerControls = {
    inputCtrlId: "OwnersPicker",
    dataCtrlId: "OtherOwners",
    allowMultiple: true
};

//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = __hostUrl;
    var appWebUrl = __appWebUrl;
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    $.getScript(layoutsRoot + 'SP.Runtime.js', function () {
        $.getScript(layoutsRoot + 'SP.js', function () {
            $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
            Office.Controls.Runtime.initialize({
                sharePointHostUrl: spHostUrl,
                appWebUrl: appWebUrl,
            });
            var pplPicker = new Office.Controls.PeoplePicker(
                    document.getElementById(peoplePickerControls.inputCtrlId), {
                        allowMultipleSelections: peoplePickerControls.allowMultiple,
                        displayErrors: false,
                        onAdded: function (ctrl, principal) {
                            var dataCtrl = $('#' + peoplePickerControls.dataCtrlId);
                            var currentVal = dataCtrl.val();
                            dataCtrl.val(currentVal + principal.loginName + ";");
                        },
                    });
            });
        });
    });
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