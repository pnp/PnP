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

        // Get sample divisions reference metadata
        function getAppSettings() {

            return $http.get('../scripts/data/json/appsettings.json')
               .then(getAppSettingsComplete)
               .catch(getAppSettingsFailed);

            function getAppSettingsComplete(response) {
                return response.data.appsettings;
            }

            function getAppSettingsFailed(error) {
                $log.error('XHR Failed for getAppSettings.' + error.data);
            }
        }
    }
})();