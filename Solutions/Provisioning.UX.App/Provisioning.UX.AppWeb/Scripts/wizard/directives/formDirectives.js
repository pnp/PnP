(function () {
    //'use strict';

    angular
        .module('app')
        .directive('siteAvailabilityValidator', ['$http', '$SharePointJSOMService', function ($http, $SharePointJSOMService) {

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
                        if (!value || value.length == 0) return;

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

        

})();