
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;

//Wait for the page to load
$(document).ready(function () {
        
    var hostweburl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appweburl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

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
                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                             context.set_webRequestExecutorFactory(factory);
                                                         

                         }
                    );
                }
            );
        }
    );
    
});


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
