var additionalOwnersPicker;
var membersPicker;
var visitorsPicker;


(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('SiteOwnersPeoplePickerController', SiteOwnersPeoplePickerController);
    //.value('urlparams', null);

    SiteOwnersPeoplePickerController.$inject = ['$scope', '$log', 'utilservice', 'peoplepickerfactory'];

    function SiteOwnersPeoplePickerController($scope, $log, $utilservice, $peoplepickerfactory) {
        $scope.title = 'SiteOwnersPeoplePickerController';

        $scope.updateSecondaryOwners = function (data) {
            $scope.siteConfiguration.secondaryOwners = data;
        }

        $scope.updateMembers = function (data) {
            $scope.siteConfiguration.members = data;
        }

        $scope.updateVisitors = function (data) {
            $scope.siteConfiguration.visitors = data;
        }

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
            initSiteOwnersPeoplePickers(context);

        }

        $scope.getEmailFromPicker = function (e) {
            var elem = angular.element(e.srcElement);
            alert(elem.val());
        }


        function initSiteOwnersPeoplePickers(context) {

            //Make a people picker control
            //1. context = SharePoint Client Context object
            //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
            //3. $('#inputAdministrators') = INPUT that will be used to capture user input
            //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
            //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users

            additionalOwnersPicker = $peoplepickerfactory.getPeoplePickerInstance(context, $('#spanAdditionalOwnersPrimary'), $('#inputAdditionalOwnersPrimary'), $('#divAdditionalOwnersPrimarySearch'), $('#hdnAdditionalOwnersPrimary'), "additionalOwnersPicker", spLanguage);

            //Removed fjm if you wish to add additional people pickers this shows the pattern and look at the view_owners.html file
            //membersPicker = $peoplepickerfactory.getPeoplePickerInstance(context, $('#spanMembers'), $('#inputMembers'), $('#divMembersSearch'), $('#hdnMembers'), "membersPicker", spLanguage);
            //visitorsPicker = $peoplepickerfactory.getPeoplePickerInstance(context, $('#spanVisitors'), $('#inputVisitors'), $('#divVisitorsSearch'), $('#hdnVisitors'), "visitorsPicker", spLanguage);


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