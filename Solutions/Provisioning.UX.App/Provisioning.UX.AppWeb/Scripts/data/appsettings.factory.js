(function () {
    'use strict';

    angular
        .module('app')
        .factory('AppSettings', AppSettings);

    AppSettings.$inject = ['$http', '$log'];

    function AppSettings($http, $log) {
        var service = {
            getAppSettings: getAppSettings

        };

        return service;

        function getAppSettings() {
            var deferred = $.Deferred();
            $http({
                method: 'GET',
                url: '/api/provisioning/appSettings/get',
                headers: { 'accept': 'application/json' }
            }).success(function (data, status, headers, config) {
                console.debug("/api/provisioning/appSettings/get", data);
                deferred.resolve(data, status)
            }).error(function (data, status) {
                deferred.reject(data, status);
            });
            return deferred;            
        }
    }
})();