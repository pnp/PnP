(function () {
    'use strict';

    angular
        .module('app.wizard')
        .service('$SharePointProvisioningService', function ($q, $http) {
            this.getSiteTemplates = function ($scope) {
                var deferred = $.Deferred();

                $http({
                    method: 'GET',
                    url: '/api/provisioning/availabletemplates',
                    headers:
                    {
                        'accept': 'application/json'
                    }
                }).success(function (data, status, headers, config) {
                    console.debug("SERVICE", data);
                    deferred.resolve(data)
                }).error(function (data, status) {
                    deferred.reject(data);
                });
                return deferred;
            }
            this.saveRequest = function (request) {
                var deferred = $q.defer();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    url: '/api/provisioning/siterequest',
                    data: "=" + formData,
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log(data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/provisioning/siterequest Request Failed " + data);
                });
                return deferred;
            }
            this.isExternalSharingEnabled = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/provisioning/externalSharingEnabled',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("Request Succssess to api/provisioning/externalSharingEnabled result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("Request Failed to api/provisioning/externalSharingEnabled " + data );
                });
                return deferred;
            }
        });
})();
