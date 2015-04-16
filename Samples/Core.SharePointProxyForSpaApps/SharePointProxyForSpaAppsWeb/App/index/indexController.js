(function (angular) {
    "use strict";

    angular
        .module('index.controller', [
            'common.services',
            'common.directives'
        ])
        .controller('IndexController', IndexController)
    ;

    IndexController['$inject'] = ['$scope', 'SharepointProxyService'];
    function IndexController($scope, SharepointProxyService) {
        // Instance Variables
        $scope.queryParameters = {};

        $scope.appWebRequestOptions = null;
        $scope.transformedAppWebRequestOptions = null;
        $scope.appWebResponse = null;

        $scope.hostWebRequestOptions = null;
        $scope.transformedHostWebRequestOptions = null;
        $scope.hostWebResponse = null;

        // Observers
        $scope.$watch('queryParameters', function () {
            $scope.queryParametersForDisplay = JSON.stringify($scope.queryParameters, null, '  ');
        });

        // Instance Methods
        $scope.buildAppWebRequestOptions = function () {
            var appWebUrl = $scope.queryParameters.SPAppWebUrl;
            var restUrl = appWebUrl + '/_api/web/title';

            return {
                url: restUrl,
                method: 'GET',
                headers: {
                    Accept: 'application/json;odata=verbose'
                }
            };
        };

        $scope.buildHostWebOptions = function () {
            var hostWebUrl = $scope.queryParameters.SPHostUrl;
            var restUrl = hostWebUrl + '/_api/web/title';

            return {
                url: restUrl,
                method: 'GET',
                headers: {
                    Accept: 'application/json;odata=verbose'
                }
            };
        }

        $scope.requestAppWebData = function () {
            console.log("Request App Web Data!");

            $scope.transformedAppWebRequestOptions = SharepointProxyService.transformRequest($scope.appWebRequestOptions);
            var promise = SharepointProxyService.sendRequest($scope.transformedAppWebRequestOptions)
                .then(function (data) {
                    $scope.appWebResponse = data;
                });
        };

        $scope.requestHostWebData = function () {
            console.log("Request Host Web Data!");

            $scope.transformedHostWebRequestOptions = SharepointProxyService.transformRequest($scope.hostWebRequestOptions);
            var promise = SharepointProxyService.sendRequest($scope.transformedHostWebRequestOptions)
                .then(function (data) {
                    $scope.hostWebResponse = data;
                });
        };

        // Init
        (function () {
            var queryParameterString = (window.location.search[0] === '?') ? window.location.search.slice(1) : window.location.search;
            $scope.queryParameters = deparam(queryParameterString);

            $scope.hostWebRequestOptions = $scope.buildHostWebOptions();
            $scope.appWebRequestOptions = $scope.buildAppWebRequestOptions();
        })();
    }


})(angular);

    
