(function () {
    var app = angular.module('app.wizard');

    app.factory("utilservice", function () {

        this.DialogMessage = "";
        this.DialogState = 0;

        return {
            getQueryStringParameter: function (paramToRetrieve) {
                var params;
                var strParams;
                params = document.URL.split("?")[1].split("&");
                strParams = "";
                for (var i = 0; i < params.length; i = i + 1) {
                    var singleParam = params[i].split("=");
                    if (singleParam[0] == paramToRetrieve)
                        return singleParam[1];
                }
            },
            replaceAll: function (find, replace, str) {
                find = find.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
                return str.replace(new RegExp(find, 'g'), replace);
            },
            layoutsRoot: function () {
                var spHostUrl = decodeURIComponent(this.getQueryStringParameter('SPHostUrl'));

                var layoutsRoot = spHostUrl + '/_layouts/15/';

                return layoutsRoot;
            },
            spRootHostName: function (url) {                
                var match = url.match(/:\/\/(www[0-9]?\.)?(.[^/:]+)/i);
                if (match != null && match.length > 2 && typeof match[2] === 'string' && match[2].length > 0) {
                    return match[2];
                }
                else {
                    return null;
                }                
            },
            spHostUrl: function () {
                return decodeURIComponent(this.getQueryStringParameter('SPHostUrl'));
            },
            spAppWebUrl: function () {
                return decodeURIComponent(this.getQueryStringParameter('SPAppWebUrl'));
            },
            spContext: function () {
                var context;
                var hostweburl = decodeURIComponent(this.getQueryStringParameter('SPHostUrl'));
                var appweburl = decodeURIComponent(this.getQueryStringParameter('SPAppWebUrl'));
                var spLanguage = decodeURIComponent(this.getQueryStringParameter('SPLanguage'));

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
                                         context = new SP.ClientContext(appweburl);                                    

                                         
                                     }
                                );
                            }
                        );
                    }
                );

                return context;
            }



        };
    });
})();