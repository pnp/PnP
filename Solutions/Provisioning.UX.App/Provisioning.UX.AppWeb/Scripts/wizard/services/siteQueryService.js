(function () {
    'use strict';

    angular
        .module('app.wizard')
        .service('$SharePointJSOMService', function ($q, $http) {
            this.checkUrlREST = function ($scope, value) {
                var deferred = $.Deferred();                

                var executor = new SP.RequestExecutor($scope.spAppWebUrl);
                executor.executeAsync({
                    url: $scope.spAppWebUrl + "/_api/SP.AppContextSite(@target)/web/url" + "?@target='" + $scope.siteConfiguration.spNewSitePrefix + value + "'",
                    method: "GET",
                    headers: { "Accept": "application/json; odata=verbose" },                    
                    success: function (data, textStatus, xhr) {                       

                        deferred.resolve(data);
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        deferred.reject(JSON.stringify(xhr));
                    }
                });
                return deferred;
            };

           
        });
})();
    
