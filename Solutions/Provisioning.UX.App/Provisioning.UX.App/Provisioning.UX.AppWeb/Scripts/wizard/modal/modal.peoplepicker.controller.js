var additionalOwnersPicker;
var membersPicker;
var visitorsPicker;

(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('PeoplePickerController', PeoplePickerController);
        //.value('urlparams', null);

    PeoplePickerController.$inject = ['$scope', '$log', 'utilservice'];

    function PeoplePickerController($scope, $log, $utilservice) {
        $scope.title = 'PeoplePickerController';

        $scope.additionalOwnersPicker;

        //var spHostWebUrl = $scope.spHostWebUrl;
        //var spAppWebUrl = $scope.spAppWebUrl;

        var context;
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
                                 context = new SP.ClientContext(appweburl);
                                 var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                 context.set_webRequestExecutorFactory(factory);

                                 activate();

                             }
                        );
                    }
                );
            }
        );

        
        
        function activate() {

            $log.info($scope.title + ' Activated');
            initPeoplePickers(context);                     

        }                      

        function initPeoplePickers(context) {

            //Make a people picker control
            //1. context = SharePoint Client Context object
            //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
            //3. $('#inputAdministrators') = INPUT that will be used to capture user input
            //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
            //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
            additionalOwnersPicker = getPeoplePickerInstance(context, $('#spanAdditionalOwnersPrimary'), $('#inputAdditionalOwnersPrimary'), $('#divAdditionalOwnersPrimarySearch'), $('#hdnAdditionalOwnersPrimary'), "additionalOwnersPicker", spLanguage);
            membersPicker = getPeoplePickerInstance(context, $('#spanMembers'), $('#inputMembers'), $('#divMembersSearch'), $('#hdnMembers'), "membersPicker", spLanguage);
            visitorsPicker = getPeoplePickerInstance(context, $('#spanVisitors'), $('#inputVisitors'), $('#divVisitorsSearch'), $('#hdnVisitors'), "visitorsPicker", spLanguage);
            
        }

        function getPeoplePickerInstance(context, spanControl, inputControl, searchDivControl, hiddenControl, variableName, spLanguage) {
            var newPicker;

            //Make a people picker control
            //1. context = SharePoint Client Context object
            //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
            //3. $('#inputAdministrators') = INPUT that will be used to capture user input
            //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
            //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
            newPicker = new CAMControl.PeoplePicker(context, spanControl, inputControl, searchDivControl, hiddenControl);
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

    }

})();
