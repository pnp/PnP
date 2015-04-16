(function (angular) {
    "use strict";

    angular
        .module('sharepointproxy.service', [])
        .factory('SharepointProxyService', SharepointProxyService);

    function SharepointProxyService($http) {

        var proxyEndpointOptions = {
            url: 'api/sharepoint',
            method: 'POST',
            data: {}
        };

        var defaultProxyOptions = {

        };

        var service = {
            transformRequest: transformRequest,
            sendRequest: sendRequest
        };

        function getData(o) {
            return o.data;
        }

        function transformRequest(request) {
            var transformedRequest = proxyEndpointOptions;
            transformedRequest.data = request;

            return transformedRequest;
        }

        function sendRequest(request) {
            return $http(request)
                .then(getData)
                .then(function (data) {
                    return data;
                })
            ;
        }

        return service;
    }

    SharepointProxyService.$inject = ['$http'];

})(angular);
