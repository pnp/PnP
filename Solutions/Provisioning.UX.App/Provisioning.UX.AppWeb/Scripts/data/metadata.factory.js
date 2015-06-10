
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

        // Get sample divisions reference metadata
        function getDivisions() {
                       
            return $http.get('../scripts/data/json/divisions.json')
               .then(getDivisionsComplete)
               .catch(getDivisionsFailed);

            function getDivisionsComplete(response) {
                return response.data.divisions;
            }

            function getDivisionsFailed(error) {
                $log.error('XHR Failed for getDivions.' + error.data);
            }
        }

        // Get sample regions reference metadata
        function getRegions() {

            return $http.get('../scripts/data/json/regions.json')
               .then(getRegionsComplete)
               .catch(getRegionsFailed);

            function getRegionsComplete(response) {
                return response.data.regions;
            }

            function getRegionsFailed(error) {
                $log.error('XHR Failed for getRegions.' + error.data);
            }
        }

        // Get sample functions reference metadata
        function getFunctions() {

            return $http.get('../scripts/data/json/functions.json')
               .then(getFunctionsComplete)
               .catch(getFunctionsFailed);

            function getFunctionsComplete(response) {
                return response.data.functions;
            }

            function getFunctionsFailed(error) {
                $log.error('XHR Failed for getFunctions.' + error.data);
            }
        }

        // Get sample languages reference metadata
        function getLanguages() {

            return $http.get('../scripts/data/json/languages.json')
               .then(getLanguagesComplete)
               .catch(getLanguagesFailed);

            function getLanguagesComplete(response) {
                return response.data.languages;
            }

            function getLanguagesFailed(error) {
                $log.error('XHR Failed for getLanguages.' + error.data);
            }
        }

        // Get sample timezone reference metadata
        function getTimeZones() {

            return $http.get('../scripts/data/json/timezones.json')
               .then(getTimeZonesComplete)
               .catch(getTimeZonesFailed);

            function getTimeZonesComplete(response) {
                return response.data.timezones;
            }

            function getTimeZonesFailed(error) {
                $log.error('XHR Failed for getTimeZones.' + error.data);
            }
        }

        // Get sample site classifications reference metadata
        function getSiteClassifications() {

            return $http.get('../scripts/data/json/siteclassifications.json')
               .then(getSiteClassificationsComplete)
               .catch(getSiteClassificationsFailed);

            function getSiteClassificationsComplete(response) {
                return response.data.siteclassifications;
            }

            function getSiteClassificationsFailed(error) {
                $log.error('XHR Failed for getSiteClassifications.' + error.data);
            }
        }
    }
})();