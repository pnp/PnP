(function () {
    'use strict';

    angular
        .module('app.wizard')
        .service('$SiteMetadataEditService', function ($q, $http) {
                        
            this.GetSiteClassifications = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'GET',
                    data: "=" + formData,
                    url: '/api/provisioning/metadata/getSiteClassifications',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/provisioning/metadata/getSiteClassifications result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/provisioning/metadata/getSiteClassifications " + data);
                });
                return deferred;
            }

            this.GetSitePolicies = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/provisioning/metadata/getSiteClassifications',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/provisioning/metadata/getSiteClassifications result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/provisioning/metadata/getSiteClassifications " + data);
                });
                return deferred;
            }

            this.GetSiteMetadata = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/getSiteMetadata',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/getSiteMetadata result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/getSiteMetadata " + data);
                });
                return deferred;
            }

            this.SetSiteMetadata = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/setSiteMetadata',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/setSiteMetadata result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/setSiteMetadata " + data);
                });
                return deferred;
            }

            this.isExternalSharingEnabled = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/getTenantExternalSharingStatus',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/getTenantExternalSharingStatus result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/getTenantExternalSharingStatus " + data);
                });
                return deferred;
            }

            this.isSiteExternalSharingEnabled = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/getSiteExternalSharingStatus',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/getSiteExternalSharingStatus result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/getSiteExternalSharingStatus " + data);
                });
                return deferred;
            }

            this.SetSiteExternalSharingStatus = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/setSiteExternalSharingStatus',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/setSiteExternalSharingStatus result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/setSiteExternalSharingStatus " + data);
                });
                return deferred;
            }

            this.CheckUserPermissions = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/checkUserPermission',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/checkUserPermission result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/checkUserPermission " + data);
                });
                return deferred;
            }

            this.GetPropertyBagItem = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/getPropertyBagItem',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/getPropertyBagItem result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/getPropertyBagItem " + data);
                });
                return deferred;
            }

            this.UpdatePropertyBagItem = function (request) {
                var deferred = $.Deferred();
                var formData = JSON.stringify(request);
                $http({
                    method: 'POST',
                    data: "=" + formData,
                    url: '/api/siteedit/metadata/updatePropertyBagItem',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status, headers, config) {
                    deferred.resolve(data);
                    console.log("/api/siteedit/metadata/updatePropertyBagItem result is " + data);
                }).error(function (data, status) {
                    deferred.reject(data);
                    console.log("/api/siteedit/metadata/updatePropertyBagItem " + data);
                });
                return deferred;
            }
            
            
        });
})();
