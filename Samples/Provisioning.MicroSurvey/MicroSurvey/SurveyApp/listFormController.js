(function () {

    'use strict';

    angular
      .module('microSurvey')

      // Controller for new list items
      .controller('listNewForm', ['surveyService',
        function controller(surveyService) {

            var vm = this;

            vm.message = "";
            vm.isNewForm = true;
            vm.isDisplayForm = false;
            vm.isEditForm = false;
            vm.inputEnabled = true;

            vm.questionMode = true;
            vm.resultsMode = false;
            vm.errorMode = false;

            vm.question = "";
            vm.answerChoices = [];
            for (var i = vm.answerChoices.length; i < 5; i++) {
                vm.answerChoices.push({ index: i, text: "", percent: 0 });
            }

            vm.saveButtonClick = function () {
                vm.inputEnabled = false;
                surveyService.addQuestion(vm.question, vm.answerChoices)
                .then(function (message) {
                    window.location.href = surveyService.getQueryStringParam("Source");
                })
                .catch(function (message) {
                    vm.inputEnabled = true;
                    vm.message = message;
                });
            }

            vm.cancelButtonClick = function () {
                vm.inputEnabled = false;
                window.location.href = surveyService.getQueryStringParam("Source");
            }
        }
      ])

          // Controller for displaying a list item
      .controller('listDisplayForm', ['surveyService',
        function controller(surveyService) {

            var vm = this;

            vm.message = "";
            vm.isNewForm = false;
            vm.isDisplayForm = true;
            vm.isEditForm = false;
            vm.inputEnabled = true;

            vm.questionMode = false;
            vm.resultsMode = true;
            vm.errorMode = false;

            var itemId = surveyService.getQueryStringParam("ID");
            surveyService.getQuestionById(itemId)
            .then(function (question) {
                vm.question = question.text;
                vm.answerChoices = question.answerChoices;
                return surveyService.getAnswers(vm.question);
            })
            .then(function (answers) {
                var a;
                var totalChoices = 0;

                for (a in answers) {
                    totalChoices += parseInt(answers[a]);
                };
                // Calculate percentages and update scope
                var j = 0;
                for (a in answers) {
                    vm.answerChoices[j++].percent = 100 * parseInt(answers[a]) / totalChoices;
                };
                vm.refreshChart();
            })
            .catch(function (message) {
                vm.message = message;
            });

            vm.editButtonClick = function () {
                vm.inputEnabled = false;
                window.location.href = "ListEditForm.aspx" + window.location.search;
            }

            vm.closeButtonClick = function () {
                vm.inputEnabled = false;
                window.location.href = surveyService.getQueryStringParam("Source");
            }
        }
      ])

      // Controller for editing list items
      .controller('listEditForm', ['surveyService',
        function controller(surveyService) {

            var vm = this;

            vm.message = "";
            vm.isNewForm = false;
            vm.isDisplayForm = false;
            vm.isEditForm = true;
            vm.inputEnabled = true;

            vm.questionMode = true;
            vm.resultsMode = false;
            vm.errorMode = false;

            var itemId = surveyService.getQueryStringParam("ID");
            surveyService.getQuestionById(itemId)
            .then(function (question) {
                vm.question = question.text;
                vm.answerChoices = question.answerChoices;
                for (var i = vm.answerChoices.length; i < 5; i++) {
                    vm.answerChoices.push({ index: i, text: "", percent: 0 });
                }
            })
            .catch(function (message) {
                vm.message = message;
            });

            vm.saveButtonClick = function () {
                vm.inputEnabled = false;
                surveyService.updateQuestion(itemId, vm.question, vm.answerChoices)
                .then(function (message) {
                    window.location.href = surveyService.getQueryStringParam("Source");
                })
                .catch(function (message) {
                    vm.inputEnabled = true;
                    vm.message = message;
                });
            }

            vm.cancelButtonClick = function () {
                vm.inputEnabled = false;
                window.location.href = surveyService.getQueryStringParam("Source");
            }
        }
      ])

}());