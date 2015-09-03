(function () {

    'use strict';

    var module = angular
      .module('microSurvey')
      .factory('surveyService', ['spDataService', '$log', '$q',

        // surveyService - Retrieves survey questions and uploads answers
        function surveyService(spDataService, $log, $q) {

            // ensureContent() - Ensure lists and content are initialized
            var ensureContent = function ensureContent(displayMessage, baseUrl) {

                // Ensure there is a Questions list
                spDataService.ensureList('Questions', 'List to hold microsurvey questions')
                .then( // Ensure there is an Answers field in the Questions list
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                        return spDataService.ensureColumn('Questions', 'Answers', spDataService.fieldTypes.Text);
                    }) 
                .then( // Ensure the Answers field is in the All Items view
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                        return spDataService.ensureColumnInView('Questions', 'Answers', 'All Items');
                    })
                .then( // Ensure the Questions List forms are set up
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                        return spDataService.setContentTypeForms('Questions', 'Item',
                            baseUrl + '/ListNewForm.aspx',
                            baseUrl + '/ListDisplayForm.aspx',
                            baseUrl + '/ListEditForm.aspx')
                    })
                .then( // Ensure there is an Answers list
                    function (message) {
                        $log.debug('Success' + message);
                        displayMessage(message);
                        return spDataService.ensureList('Answers', 'User answers to survey data');
                    })
                .then( // Ensure the Answers list has a Data field
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                        return spDataService.ensureColumn('Answers', 'Data', spDataService.fieldTypes.Text);
                    })
                .then( // Ensure the Data field is in the All Items view
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                        return spDataService.ensureColumnInView('Answers', 'Data', 'All Items');
                    })
                .then(  // Done!
                    function (message) {
                        $log.debug('Success: ' + message);
                        displayMessage(message);
                    })
                .catch (
                    function (message) {
                        $log.debug('Failure: ' + message);
                    });
            };

            // getQuestion() - Returns a promise to deliver a question to pose
            function getQuestion() {

                var deferred = $q.defer();

                spDataService.readMostRecentListItem("Questions")
                .then(function (data) {
                    var question = {
                        text: data.Title,
                        answerChoices: []
                    };
                    var answers = data.Answers.split(',');
                    for (var a in answers) {
                        question.answerChoices.push({ index: a, text: answers[a], percent: 0 });
                    }
                    deferred.resolve(question);
                })
                .catch(function (message) {
                    deferred.reject(message);
                });

                return deferred.promise;
            };

            // getQuestionById() - Returns a promise to deliver a question to pose
            function getQuestionById(itemId) {

                var deferred = $q.defer();
                
                spDataService.readListItemById("Questions", itemId)
                .then(function (data) {
                    var question = {
                        text: data.Title,
                        answerChoices: []
                    };
                    var answers = data.Answers.split(',');
                    var i = 0;
                    for (var a in answers) {
                        question.answerChoices.push({ index: i++, text: answers[a], percent: 0 });
                    }
                    deferred.resolve(question);
                })
                .catch(function (message) {
                    deferred.reject(message);
                });

                return deferred.promise;
            };

            // addQuestion() Adds a question to the Questions list
            function addQuestion(question, answerChoices) {
                var deferred = $q.defer();

                var answerString = "";
                for (var i in answerChoices) {
                    if (answerChoices[i].text) {
                        answerString += (answerString != "") ? "," : "";
                        answerString += answerChoices[i].text;
                    }
                }
                spDataService.addListItem("Questions", question, { Answers: answerString })
                .then(function (data) {
                    deferred.resolve("Saved");
                })
                .catch(function (status) {
                    $log.debug(status);
                    deferred.reject("Error " + status);
                });

                return deferred.promise;
            }

            // updateQuestion() Updates a question in the Questions list
            function updateQuestion(itemId, question, answerChoices) {
                var deferred = $q.defer();

                var answerString = "";
                for (var i in answerChoices) {
                    if (answerChoices[i].text) {
                        answerString += (answerString != "") ? "," : "";
                        answerString += answerChoices[i].text;
                    }
                }
                spDataService.updateListItemById("Questions", itemId, question, { Answers: answerString })
                .then(function (data) {
                    deferred.resolve("Saved");
                })
                .catch(function (status) {
                    $log.debug(status);
                    deferred.reject("Error " + status);
                });

                return deferred.promise;
            }

            // getAnswers() Reads the answers from the server
            function getAnswers(question) {

                var deferred = $q.defer();

                // Remove unsupported characters from question
                question = question.replace("'", "");

                spDataService.readListItemByTitle("Answers", question)
                .then(function (data) {

                    // OK we read the data - now bump the item
                    var answers = data.Data.split(',');

                    deferred.resolve(answers);
                })

                return deferred.promise;
            }

            // recordAnswer() Part 1: Read the answer from the server
            function recordAnswer(question, answerChoices, choiceIndex) {

                var deferred = $q.defer();

                // Remove unsupported characters from question
                question = question.replace("'", "");

                // First try to read the data
                spDataService.readListItemByTitle("Answers", question)
                // Then calculate the new answers and update the item
                .then(function (data) {

                    // OK we read the data - now bump the item
                    var choices = data.Data.split(',');
                    if (choices[choiceIndex]) {
                        choices[choiceIndex] = parseInt(choices[choiceIndex]) + 1;;
                    } else {
                        choices.push(1);
                    }
                    var result = choices[0];
                    for (var i = 1; i < answerChoices.length; i++) {
                        result += choices[i] ? "," + choices[i] : ",0";
                    };

                    // Now update the item
                    recordAnswer2(data.Id, question, result, deferred);

                })
                // Then return the new answers to the controller for display
                .then(function (data) {
                    deferred.resolve(data.Answers);
                })
                // Catch any exceptions
                .catch(function (status) {

                    if (status === 404) {

                        // If here, we couldn't find the answers record, so make a new one
                        var result = "";
                        if (parseInt(choiceIndex) === 0) {
                            result = "1"
                        } else {
                            result = "0"
                        }
                        for (var i = 1; i < answerChoices.length; i++) {
                            result += (parseInt(choiceIndex) === i) ? ",1" : ",0";
                        }
                        recordAnswer3(question, result, deferred);
                    };
                    $log.debug(status);
                });

                return deferred.promise;
            }

            // recordAnswer() Part 2: Update the answer (if found)
            function recordAnswer2(itemId, question, answerString, deferred) {
                spDataService.updateListItemById("Answers", itemId, question, { Data: answerString })
                .then(function (data) {
                    deferred.resolve(answerString);
                })
                .catch(function (status) {
                    $log.debug(status);
                    deferred.reject(status);
                });
            };

            // recordAnswer() Part 3: Create the answer (if not found)
            function recordAnswer3(question, answerString, deferred) {
                spDataService.addListItem("Answers", question, { Data: answerString })
                .then(function (data) {
                    deferred.resolve(answerString);
                })
                .catch(function (status) {
                    $log.debug(status);
                    deferred.reject(status);
                });
            };

            // getQueryStringParam() - Gets a query string parameter
            function getQueryStringParam(name) {
                var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
                return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
            }

            // Public properties
            var surveyInfo = {
                questionsListUrl: spDataService.getSiteUrl() + '/lists/questions/',
                answersListUrl: spDataService.getSiteUrl() + '/lists/answers/'
            };

            // Expose public members of survey service
            return {
                ensureContent: ensureContent,
                getQuestion: getQuestion,
                getQuestionById: getQuestionById,
                addQuestion: addQuestion,
                updateQuestion: updateQuestion,
                getAnswers: getAnswers,
                recordAnswer: recordAnswer,
                getQueryStringParam: getQueryStringParam,
                surveyInfo: surveyInfo
            };

        }
      ])

          // Data charting directive - decorate a canvas element with bg-show-data to render
      // a bar chart showing survey responses
      .directive('bgShowData', function bgShowDataDirective() {

          function renderChart(scope, element, attrs, clientWidth, clientHeight) {

              // Get the canvas DOM element
              var canvas = element[0];
              var context = canvas.getContext('2d');

              // Scale the inner drawling surface to the same aspect ratio as the canvas element
              canvas.width = canvas.height * (clientWidth / clientHeight);

              // We're drawing a horizontal bar chart
              // Calculate the height of each bar 
              var barHeight = Math.ceil(canvas.height / scope.answerChoices.length) - 1;
              var barTop = 0;

              // For each bar in the chart
              for (var a in scope.answerChoices) {

                  // Draw a rectangle for the bar
                  context.beginPath();
                  context.rect(0, barTop, (scope.answerChoices[a].percent / 100) * canvas.width, barHeight);
                  context.strokeStyle = '55a5e4';
                  context.fillStyle = '#55a5e4';
                  context.fill();

                  // Draw a rectangle the width of the canvas as a border for each line
                  context.beginPath();
                  context.rect(0, barTop, canvas.width, barHeight);
                  context.strokeStyle = '#999';
                  barTop = barTop + barHeight;
                  context.stroke();

                  // Add text with the answer and percentage
                  context.beginPath();
                  context.font = '16pt Helvetica, Arial, sans-serif';
                  context.fillStyle = 'black';
                  context.textAlign = 'left';
                  context.fillText(scope.answerChoices[a].text.toUpperCase() + ' - ' +
                      Math.round(scope.answerChoices[a].percent) + '%', 10, barTop - 10);

              } // for loop

          };

          return {

              // Directive can only be used as an attribute
              restrict: 'A',

              // This function is called on initial binding. There won't be data so soon but we can
              // set up for when the data arrives
              link: function link(scope, element, attrs) {

                  // The clientWidth and clientHeight values are lost after the initial canvas renders. 
                  // Since we need to refresh it asynchronously, save these values in variables and pass
                  // them to the render function
                  var clientWidth = element[0].clientWidth;
                  var clientHeight = element[0].clientHeight;

                  // Don't render the chart now - set up a function that the controller can call when
                  // a user clicks
                  scope.vm.refreshChart = function () {
                      renderChart(scope.vm, element, attrs, clientWidth, clientHeight);
                  };
              }
          }

      }); // End bg-show-data directive

}());