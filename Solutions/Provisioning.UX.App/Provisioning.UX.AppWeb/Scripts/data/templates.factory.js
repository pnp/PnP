(function () {
    'use strict';

    angular
        .module('app')
        .factory('Templates', Templates);

    Templates.$inject = ['$http', '$log'];

    function Templates($http, $log) {
        var service = {
            getData: getData
        };

        return service;

        function getData() {
                       
            return $http.get('../scripts/data/templates.json')
               .then(getTemplatesComplete)
               .catch(getTemplatesFailed);

            function getTemplatesComplete(response) {
                return response.data.templates;
            }

            function getTemplatesFailed(error) {
                $log.error('XHR Failed for getTemplates.' + error.data);
            }

        }
    }
})();