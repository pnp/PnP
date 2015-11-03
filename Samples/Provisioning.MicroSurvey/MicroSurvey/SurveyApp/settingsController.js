(function () {

    'use strict';

    angular
      .module('microSurvey')

      // Settings controller 
      .controller('settings', ['surveyService',
        function controller(surveyService) {

            var vm = this;

            // Ensure content is ready
            vm.displayMessages = [];

            var pageUrl = window.location.href.toLowerCase();
            // Strip off the protocol and page
            var baseUrl = pageUrl.substring(9, pageUrl.lastIndexOf('/'));
            // Strip off the host
            baseUrl = baseUrl.substring(baseUrl.indexOf('/'));
            if (baseUrl.lastIndexOf('/pages') > 0) {
                // The only time this controller runs in a /pages folder is when it's in a SharePoint add-in; adjust the base URL
                var baseUrl = baseUrl.substring(0, baseUrl.lastIndexOf('/pages')) + "/surveyapp";
            }

            surveyService.ensureContent(function addToMessage(message) {
                vm.displayMessages.push(message);
            }, baseUrl);

            // Surface binding properties for the view
            vm.questionsListUrl = surveyService.surveyInfo.questionsListUrl;
            vm.answersListUrl = surveyService.surveyInfo.answersListUrl;
        }
      ])

}());