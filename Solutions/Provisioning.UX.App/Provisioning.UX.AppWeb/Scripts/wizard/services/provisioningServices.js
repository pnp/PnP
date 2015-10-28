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
                    console.debug("/api/provisioning/templates/getAvailableTemplates", data);
                    deferred.resolve(data, status)
                }).error(function (data, status) {
                    deferred.reject(data, status);
                });
                return deferred;
            }
            this.createNewSiteRequest = function (request) {
                var deferred = $q.defer();
    
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    url: '/api/provisioning/siteRequests/create',
                    data: "=" + formData,
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    console.debug("/api/provisioning/siteRequests/createSiteRequest ", data);
                    deferred.resolve(data, status);
                }).error(function (data, status) {
                    console.log("/api/provisioning/createSiteRequest " + data);
                    deferred.reject(data, status);
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
                    console.log("/api/provisioning/getOwnerRequests " + data);
                    deferred.resolve(data);
                }).error(function (data, status) {
                    console.log("/api/provisioning/getOwnerRequests " + data);
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
                    console.log("api/provisioning/externalSharingEnabled result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("api/provisioning/externalSharingEnabled " + data );
                });
                return deferred;
            }
            this.getSiteRequestByUrl = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/provisioning/siteRequests/getSiteRequest/url',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data, status);
                    console.log("api/provisioning/siteRequests/getSiteRequest/id result is " + data);
                }).error(function (data, status) {
                    if (status == 404) {
                        deferred.resolve(data, status);
                    }
                    deferred.reject(data, status);
                    console.log("Request Failed to api/provisioning/siteRequests/getSiteRequest/id " + data);
                });
                return deferred;
            }
        });
})();
