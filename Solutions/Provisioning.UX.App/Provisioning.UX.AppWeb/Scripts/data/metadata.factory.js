
// Note:
// This factory pulls sample data from json files to be used for wizard metadata.
// This approach can be simply changed to pull this reference data from other locations.

(function () {
    'use strict';

    angular
        .module('app')
        .factory('BusinessMetadata', BusinessMetadata);

    BusinessMetadata.$inject = ['$http', '$log'];

    function BusinessMetadata($http, $log) {
        var service = {
            getDivisions: getDivisions,
            getRegions: getRegions,
            getFunctions: getFunctions,
            getLanguages: getLanguages,
            getTimeZones: getTimeZones,
            getSiteClassifications: getSiteClassifications
        };

        return service;


        function getMetadata(method) {
            var deferred = $.Deferred();
            $http({
                method: 'GET',
                url: '/api/provisioning/metadata/' +method ,
                headers:{ 'accept': 'application/json'}
            }).success(function (data, status, headers, config) {
                console.debug("/api/provisioning/metadata/" + method, data);
                deferred.resolve(data, status)
            }).error(function (data, status) {
                deferred.reject(data, status);
            });
            return deferred;
        }

        function getDivisions() {
            return getMetadata('getDivisions');
        }

        function getRegions() {
            return getMetadata('getRegions');
        }

        function getFunctions() {
            return getMetadata('getFunctions');
        }

        function getLanguages() {
            return getMetadata('getLanguages');
        }

        function getTimeZones() {
            return getMetadata('getTimeZones');
        }
        
        function getSiteClassifications() {
            return getMetadata('getSiteClassifications');
          
        }
    }
})();