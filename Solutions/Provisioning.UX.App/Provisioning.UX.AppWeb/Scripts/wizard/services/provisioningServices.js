(function () {
    'use strict';

    angular
        .module('app.wizard')
        .service('$SharePointProvisioningService', function ($q, $http) {
            this.getSiteTemplates = function ($scope) {
                var deferred = $.Deferred();

                $http({
                    method: 'GET',
                    url: '/api/provisioning/templates/getAvailableTemplates',
                    headers:
                    {
                        'accept': 'application/json'
                    }
                }).success(function (data, status, headers, config) {
                    console.debug("Request Success /api/provisioning/templates/getAvailableTemplates", data);
                    deferred.resolve(data.templates)
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
                    url: '/api/provisioning/siteRequests/saveSiteRequest',
                    data: "=" + formData,
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    console.debug("Request Success /api/provisioning/siteRequests/saveSiteRequest ", data);
                    deferred.resolve(data);
                }).error(function (data, status) {
                    console.log("Request Failed /api/provisioning/siterequest Request " + data);
                    deferred.reject(data);
                });
                return deferred;
            }
            this.getSiteRequestsByOwners = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/provisioning/siteRequests/getOwnerRequests',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    console.log("Request Success /api/provisioning/getOwnerRequests " + data);
                    deferred.resolve(data);
                }).error(function (data, status) {
                    console.log("Request Failed /api/provisioning/getOwnerRequests " + data);
                    deferred.reject(data);
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
