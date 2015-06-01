(function () {
    var app = angular.module('app.wizard');

    app.factory("peoplepickerfactory", function () {


        return {

            getPeoplePickerInstance: function (context, spanControl, inputControl, searchDivControl, hiddenControl, variableName, spLanguage) {
                var newPicker;

                //Make a people picker control
                //1. context = SharePoint Client Context object
                //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
                //3. $('#inputAdministrators') = INPUT that will be used to capture user input
                //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                newPicker = new CAMControl.PeoplePicker(context, spanControl, inputControl, searchDivControl, hiddenControl, variableName);
                // required to pass the variable name here!
                newPicker.InstanceName = variableName;
                // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                // Do not set the Language property if you do not have foreseen javascript resource file for your language
                //newPicker.Language = spLanguage;
                // optionally show more/less entries in the people picker dropdown, 4 is the default
                newPicker.MaxEntriesShown = 5;
                // Can duplicate entries be selected (default = false)
                newPicker.AllowDuplicates = false;
                // Show the user loginname
                newPicker.ShowLoginName = true;
                // Show the user title
                newPicker.ShowTitle = true;
                // Set principal type to determine what is shown (default = 1, only users are resolved). 
                // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                // Set ShowLoginName and ShowTitle to false if you're resolving groups
                newPicker.PrincipalType = 1;
                // start user resolving as of 2 entered characters (= default)
                newPicker.MinimalCharactersBeforeSearching = 2;

                // Hookup everything
                newPicker.Initialize();

                return newPicker;
            }
        };
    });
})();