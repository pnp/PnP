
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

