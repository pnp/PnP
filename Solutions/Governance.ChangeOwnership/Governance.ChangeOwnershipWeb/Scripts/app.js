
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;
var csomPeoplePicker;

var peoplePickerSecondary;

//Wait for the page to load
$(document).ready(function () {

    setAppPageInitStatus();

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';
    
    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    //$.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {

                        if (appWebUrl != 'undefined') {
                            context = new SP.ClientContext(appWebUrl);
                            var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                            context.set_webRequestExecutorFactory(factory);

                            //Make a people picker control
                            //1. context = SharePoint Client Context object
                            //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
                            //3. $('#inputAdministrators') = INPUT that will be used to capture user input
                            //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                            //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                            peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
                            // required to pass the variable name here!
                            peoplePicker.InstanceName = "peoplePicker";
                            // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                            // Do not set the Language property if you do not have foreseen javascript resource file for your language
                            peoplePicker.Language = spLanguage;
                            // optionally show more/less entries in the people picker dropdown, 4 is the default
                            peoplePicker.MaxEntriesShown = 5;
                            // Can duplicate entries be selected (default = false)
                            peoplePicker.AllowDuplicates = false;
                            // Show the user loginname
                            peoplePicker.ShowLoginName = true;
                            // Show the user title
                            peoplePicker.ShowTitle = true;
                            // Set principal type to determine what is shown (default = 1, only users are resolved). 
                            // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                            // Set ShowLoginName and ShowTitle to false if you're resolving groups
                            peoplePicker.PrincipalType = 1;
                            // start user resolving as of 2 entered characters (= default)
                            peoplePicker.MinimalCharactersBeforeSearching = 2;
                            // Hookup everything
                            peoplePicker.Initialize();

                            ////SecondaryOwner
                            //peoplePickerSecondary = new CAMControl.PeoplePicker(context, $('#spanSecondaryAdministrators'), $('#inputSecondaryAdministrators'), $('#divSecondaryAdministratorsSearch'), $('#hdnSecondaryAdministrators'));
                            //peoplePickerSecondary.InstanceName = "peoplePickerSecondary";
                            //// Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                            //// Do not set the Language property if you do not have foreseen javascript resource file for your language
                            //peoplePickerSecondary.Language = spLanguage;
                            //// optionally show more/less entries in the people picker dropdown, 4 is the default
                            //peoplePickerSecondary.MaxEntriesShown = 5;
                            //// Can duplicate entries be selected (default = false)
                            //peoplePickerSecondary.AllowDuplicates = false;
                            //// Show the user loginname
                            //peoplePickerSecondary.ShowLoginName = true;
                            //// Show the user title
                            //peoplePickerSecondary.ShowTitle = true;
                            //// Set principal type to determine what is shown (default = 1, only users are resolved). 
                            //// See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                            //// Set ShowLoginName and ShowTitle to false if you're resolving groups
                            //peoplePickerSecondary.PrincipalType = 1;
                            //// start user resolving as of 2 entered characters (= default)
                            //peoplePickerSecondary.MinimalCharactersBeforeSearching = 2;
                            //// Hookup everything
                            //peoplePickerSecondary.Initialize();

                            ////Make a Csom people picker control
                            ////1. data url on the server (webmethod in webforms, controller action in MVC)
                            ////2. SpHostUrl
                            ////3. $('#spanCsomAdministrators') = SPAN that will 'host' the people picker control
                            ////4. $('#inputCsomAdministrators') = INPUT that will be used to capture user input
                            ////5. $('#divCsomAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                            ////6. $('#hdnCsomAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                            //csomPeoplePicker = new CAMControl.CsomPeoplePicker('Default.aspx/GetPeoplePickerData', spHostUrl, $('#spanCsomAdministrators'), $('#inputCsomAdministrators'), $('#divCsomAdministratorsSearch'), $('#hdnCsomAdministrators'));
                            //// required to pass the variable name here!
                            //csomPeoplePicker.InstanceName = "csomPeoplePicker";
                            //// Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                            //// Do not set the Language property if you do not have foreseen javascript resource file for your language
                            //csomPeoplePicker.Language = spLanguage;
                            //// optionally show more/less entries in the people picker dropdown, 4 is the default
                            //csomPeoplePicker.MaxEntriesShown = 5;
                            //// Can duplicate entries be selected (default = false)
                            //csomPeoplePicker.AllowDuplicates = false;
                            //// Show the user loginname
                            //csomPeoplePicker.ShowLoginName = true;
                            //// Show the user title
                            //csomPeoplePicker.ShowTitle = true;
                            //// Set principal type to determine what is shown (default = 1, only users are resolved). 
                            //// See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                            //// Set ShowLoginName and ShowTitle to false if you're resolving groups
                            //csomPeoplePicker.PrincipalType = 1;
                            //// start user resolving as of 2 entered characters (= default)
                            //csomPeoplePicker.MinimalCharactersBeforeSearching = 2;
                            //// Hookup everything
                            //csomPeoplePicker.Initialize();
                        }
                    });
                });
        });

    $("#GetValuesByJavascript").click(function (event) {
        event.preventDefault();
        //get json string from hidden field and parse it to PeoplePickerUser object
        var pickedPeople = $.parseJSON($('#hdnCsomAdministrators').val());

        var pickedPeopleString = "";

        //loop picked persons and create string to show
        $.each(pickedPeople, function (key, value) {
            pickedPeopleString += value.Name + " ";
        });

        alert(pickedPeopleString);
    });
});


//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {

    if (document.URL.split('?').length > 1) {
        var params = document.URL.split('?')[1].split('&');
        var strParams = '';
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split('=');
            if (singleParam[0] == urlParameterKey)
                return singleParam[1];
        }
    }
}

function setAppPageInitStatus()
{
    $("input[id$=txtboxUser]").hide();
    $("select[id$=ddlistSCA]").hide();
    $('#divAdministrators').hide();

    $('input[id$=btnCreate]').prop('disabled', true);
    //$('input[id$=btnCreate]').css('background-color', '#0096D6');
    //$('input[id$=btnCreate]').css('foreground-color', '#0096D6');
}

function GetRadioButtonListSelectedValue(radioButtonList) {
    if (typeof ($peoplePickerControl) != 'undefined') {
        $peoplePickerControl.PeoplePickerEdit.val('');
        $peoplePickerControl.PeoplePickerEdit.focus();
        $peoplePickerControl.HideSelectionBox();
    }

    //$('input[id$=btnCreate]').prop('disabled', false);

    for (var i = 0; i < radioButtonList.rows.length; ++i) {
        if (radioButtonList.rows[i].cells[0].querySelectorAll("input[type=radio]")[0].checked) {
            //isChecked = true;
            if (radioButtonList.rows[i].cells[0].querySelectorAll("input[type=radio]")[0].value == 'sca') {
                $('#img_ScaLoading').show();
                $("img[id$=img_ScaLoading]").show();
            }

            if (radioButtonList.rows[i].cells[0].querySelectorAll("input[type=radio]")[0].value == 'other') {
                $('#divAdministrators').show();
            }
            else { $('#divAdministrators').hide(); }
        }
    }
}
