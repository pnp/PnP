(function () {

    'use strict';

    angular
      .module('microSurvey', [])

      // Add configuration for cross-domain execution - change the URL to the location where the files will
      // be deployed, or remove the config section if all files will be deployed in the same domain.
      .config(['$sceDelegateProvider', function ($sceDelegateProvider) {
          $sceDelegateProvider.resourceUrlWhitelist(['self', 'http://install.dev13.local/surveyapp/**']);
      }])

      // Main controller 
      .controller('main', ['$log', 'surveyService',
        function controller($log, surveyService) {

            var vm = this;

            // Get the question to display
            vm.questionsListUrl = surveyService.surveyInfo.questionsListUrl;
            vm.currentUrl = document.location.href;
            surveyService.getQuestion()

            // Now add the question and answer choices to the scope
            .then(function (question) {
                vm.question = question.text;                // The text of the question being asked
                vm.answerChoices = question.answerChoices;  // A collection of answer choices

                vm.questionMode = true;
                vm.resultsMode = false;
                vm.errorMode = false;
            })
            .catch(function (message) {
                $log.debug('Error reading question: ' + message);

                vm.questionMode = false;
                vm.resultsMode = false;
                vm.errorMode = true;
            });

            // Display the question, hide the responses

            // Set up click handler
            vm.answerClick = function answerClick(choiceIndex) {

                // If here, user clicked an answer. Log it to SharePoint
                surveyService.recordAnswer(vm.question, vm.answerChoices, choiceIndex)

                // Then update the UI
                .then(function (answerString) {

                    // Split the answer string and total it up
                    vm.questionMode = false;
                    vm.resultsMode = true;
                    vm.errorMode = false;

                    var choices = answerString.split(',');
                    var totalChoices = 0;
                    for (var i = 0; i < choices.length; i++) {
                        totalChoices += parseInt(choices[i]);
                    };
                    // Calculate percentages and update scope
                    for (var j = 0; j < choices.length; j++) {

                        vm.answerChoices[j].percent = 100 * parseInt(choices[j]) / totalChoices;
                    };
                    // Refresh the chart canvas
                    if (vm.refreshChart) { vm.refreshChart(answerString); }

                })
                .catch(function (message) {
                    $log.debug(message);
                });
            };
        }
      ]) // End Main controller

}());