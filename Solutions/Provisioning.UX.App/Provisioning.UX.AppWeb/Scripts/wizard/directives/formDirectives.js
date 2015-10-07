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
                        if (!value || value.length == 0 || scope.allowCustomUrl) return;

                        setAsLoading(true);
                        setAsAvailable(false);

                        // use the SP service to query for the user's inputted site URL
                        $.when($SharePointJSOMService.checkUrlREST(scope, value))
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

                        return value;

                    })
                }
            }
        }])

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