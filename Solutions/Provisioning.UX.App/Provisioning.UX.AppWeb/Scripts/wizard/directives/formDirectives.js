(function () {
    //'use strict';
        
    var app = angular.module('app');

    app.directive('siteAvailabilityValidator', ['$http', '$SharePointJSOMService', function ($http, $SharePointJSOMService) {

        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {

                function setAsLoading(bool) {
                    ngModel.$setValidity('site-loading', !bool);
                    scope.$apply();
                }

                function setAsAvailable(bool) {
                    ngModel.$setValidity('site-available', bool);
                    scope.$apply();
                }

                ngModel.$parsers.push(function (value) {
                    if (!value || value.length == 0) return;  // removed this for custom url checks -> "|| !scope.allowCustomUrl"
                    setAsLoading(true);
                    setAsAvailable(false);

                    if (value === undefined)
                        return ''
                    cleanInputValue = value.replace(/[^\w\s]/gi, '').replace(/\s+/g, '');

                    if (cleanInputValue != value) {
                        ngModel.$setViewValue(cleanInputValue);
                        ngModel.$render();
                    }

                    setTimeout(function () {
                        // use the SP service to query for the user's inputted site URL
                        $.when($SharePointJSOMService.checkUrlREST(scope, cleanInputValue))
                            .done(function (data) {

                                // web service call was successful - site already exists
                                // double check its status code and set as unavailable
                                if (data.statusCode == 200) {
                                    console.log(data);
                                    setAsLoading(false);
                                    setAsAvailable(false);
                                }

                            })
                            .fail(function (err) {

                                // web service call failed - site does not already exist
                                // set as a valid site
                                setAsLoading(false);
                                setAsAvailable(true);

                            });
                    }, 2000);

                    return value;

                })
            }
        }
    }]);

    app.directive('siteTitleValidator', ['$http', '$SharePointJSOMService', function ($http, $SharePointJSOMService) {

        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {

                ngModel.$parsers.push(function (inputValue) {
                    if (inputValue === undefined)
                        return ''
                    cleanInputValue = inputValue.replace(/[^\w\s]/gi, '');

                    if (cleanInputValue != inputValue) {
                        ngModel.$setViewValue(cleanInputValue);
                        ngModel.$render();
                    }
                    return cleanInputValue;
                })
            }
        }
    }]);

    app.directive('specialCharsValidator', ['$http', '$SharePointJSOMService', function ($http, $SharePointJSOMService) {

        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {

                ngModel.$parsers.push(function (inputValue) {
                    if (inputValue === undefined)
                        return ''
                    cleanInputValue = inputValue.replace(/[^\w\s]/gi, '');

                    if (cleanInputValue != inputValue) {
                        ngModel.$setViewValue(cleanInputValue);
                        ngModel.$render();
                    }
                    return cleanInputValue;
                })
            }
        }
    }]);

    app.directive('ccSpinner', ['$window', function ($window) {
        // Description:
        //  Creates a new Spinner and sets its options
        // Usage:
        //  <div data-cc-spinner="vm.spinnerOptions"></div>
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs) {
            scope.spinner = null;
            scope.$watch(attrs.ccSpinner, function (options) {
                if (scope.spinner) {
                    scope.spinner.stop();
                }
                scope.spinner = new $window.Spinner(options);
                scope.spinner.spin(element[0]);
            }, true);
        }
    }]);

})();