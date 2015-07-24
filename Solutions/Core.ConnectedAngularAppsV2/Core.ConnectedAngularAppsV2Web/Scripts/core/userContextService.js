(function () {
    'use strict';

    // remote user context service
    angular
        .module('app.core')
        .service('$SharePointJSOMService', function ($q, $http) {
            var vm = this;

            vm.get_userProperties = function () {
                var deferred = $.Deferred();              

                var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                var appweburl =
                   decodeURIComponent(
                       getQueryStringParameter("SPAppWebUrl")
                );

                // resources are in URLs in the form:
                // web_url/_layouts/15/resource
                var scriptbase = hostweburl + "/_layouts/15/";

                // Load the js files and continue to the successHandler
                $.getScript(scriptbase + "SP.Runtime.js",
                    function () {
                        $.getScript(scriptbase + "SP.js",
                            function () {
                                $.getScript(scriptbase + "SP.RequestExecutor.js",
                                     function () {
                                         var executor = new SP.RequestExecutor(appweburl);
                                         var url = appweburl + "/_api/SP.AppContextSite(@t)/web/currentUser?@t='" + hostweburl + "'";
                                         executor.executeAsync({
                                             url: url,
                                             method: "GET",
                                             headers: { "Accept": "application/json; odata=verbose" },
                                             success: function (data) {
                                                 deferred.resolve(data);
                                             },
                                             error: function (data, errorCode, errorMessage) {
                                                 deferred.reject(data, errorCode, errorMessage);
                                             }
                                         });
                                     }
                                );
                            }
                        );
                    }
                );

                return deferred.promise();
            };

            //function to get a parameter value by a specific key
            function getQueryStringParameter(urlParameterKey) {
                var params = document.URL.split('?')[1].split('&');
                var strParams = '';
                for (var i = 0; i < params.length; i = i + 1) {
                    var singleParam = params[i].split('=');
                    if (singleParam[0] == urlParameterKey)
                        return singleParam[1];
                }
            }
            
        })
})();