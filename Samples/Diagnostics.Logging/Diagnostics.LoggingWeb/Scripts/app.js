$(document).ready(function () {

    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));
    var idFromURL = decodeURIComponent(getQueryStringParameter('SPListItemId'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //Create a Link element for the defaultcss.ashx resource
    var linkElement = document.createElement('link');
    linkElement.setAttribute('rel', 'stylesheet');
    linkElement.setAttribute('href', layoutsRoot + 'defaultcss.ashx');

    //Add the linkElement as a child to the head section of the html
    var headElement = document.getElementsByTagName('head');
    headElement[0].appendChild(linkElement);

    $.getScript(layoutsRoot + "sp.ui.controls.js", renderSPChrome);

});

//function callback to render chrome after SP.UI.Controls.js loads
function renderSPChrome() {
    var options =
          {
              "appHelpPageUrl": "home/Index?" + document.URL.split("?")[1],
              "appTitle": "PnP Logging Demo",
              "settingsLinks": [
               {
                   "linkUrl": "home/Index?" + document.URL.split("?")[1],
                   "displayName": "Home Page"
               },
              ]
          }

    var nav = new SP.UI.Controls.Navigation("divSPChrome", options);
    nav.setVisible(true);
};


function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}
