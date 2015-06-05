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

            //var req = {
            //    method: 'GET',
            //    url: '../scripts/data/templates.json',
            //    headers: { "Content-Type": "application/json; odata=verbose" },
            //}

            //return $http(req)
            //   .then(getTemplatesComplete)
            //   .catch(getTemplatesFailed);
            
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