$(document).ready(function () {
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Execute the correct script based on the isDialog
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);
                });
        });
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

// Set the style of the page to be consistent with the host web. 
(function () {
    'use strict';

    var hostUrl = '';
    if (document.URL.indexOf('?') != -1) {
        var params = document.URL.split('?')[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var p = decodeURIComponent(params[i]);
            if (/^SPHostUrl=/i.test(p)) {
                hostUrl = p.split('=')[1];
                var dclink;
                var head;
                dclink = document.createElement("link");
                dclink.setAttribute("rel", "stylesheet");
                dclink.setAttribute("href", hostUrl + "/_layouts/15/defaultcss.ashx");
                head = document.getElementsByTagName("head");
                head[0].appendChild(dclink);
                break;
            }
        }
    }
    if (hostUrl == '') {
        document.write('<link rel="stylesheet" href="/_layouts/15/1033/styles/themable/corev15.css" />');
    }
})();