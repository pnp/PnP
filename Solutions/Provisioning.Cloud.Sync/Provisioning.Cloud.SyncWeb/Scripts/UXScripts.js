//postMessage to SharePoint for closing dialog
function closeDialog() {
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : undefined);
    target.postMessage('CloseDialog', '*');
}

function navigateParent(url) {
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : undefined);
    target.postMessage('NavigateParent=' + url, '*');
}

//Wait for the page to load
$(document).ready(function () {
    //Get the isDialog from url parameters
    var isDialog = decodeURIComponent(getQueryStringParameter('IsDlg'));

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Execute the correct script based on the isDialog
                    if (isDialog == '0') {
                        //Load the SP.UI.Controls.js file to render the App Chrome
                        $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);
                    }
                    else if (isDialog == '1') {
                        //Create a Link element for the defaultcss.ashx resource
                        var linkElement = document.createElement('link');
                        linkElement.setAttribute('rel', 'stylesheet');
                        linkElement.setAttribute('href', layoutsRoot + 'defaultcss.ashx');

                        //Add the linkElement as a child to the head section of the html
                        var headElement = document.getElementsByTagName('head');
                        headElement[0].appendChild(linkElement);

                        //load view 
                        chromeLoaded();
                    }
                });
        });
});

function chromeLoaded() {
    $('body').show();
}
//function callback to render chrome after SP.UI.Controls.js loads
function renderSPChrome() {
    //Get the host site logo url from the SPHostLogoUrl parameter
    var hostlogourl = decodeURIComponent(getQueryStringParameter('SPHostLogoUrl'));

    //Set the chrome options for launching Help, Account, and Contact pages
    var options = {
        'appIconUrl': hostlogourl,
        'appTitle': document.title,
        'settingsLinks': [],
        'onCssLoaded': 'chromeLoaded()'
    };

    //Load the Chrome Control in the divSPChrome element of the page
    var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
    chromeNavigation.setVisible(true);
}

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

//show body once the chrome is loaded
function chromeLoaded() {
    $('body').show();
}

//resize the UI
function PostResizeMessage(w, h) {
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : undefined);
    var regex = new RegExp(/[Ss]ender[Ii]d=([\daAbBcCdDeEfF]+)/);
    results = regex.exec(this.location.search);
    if (null !== results && null !== results[1]) {
        target.postMessage('<message senderId=' + results[1] + '>resize(' + w + ',' + h + ')</message>', '*');
    }
}

function MakeSSCDialogPageVisible() {
    var dlgMadeVisible = false;
    try {
        var dlg = window.top.g_childDialog;
        if (Boolean(window.frameElement) && Boolean(window.frameElement.makeVisible)) {
            window.frameElement.makeVisible();
            dlgMadeVisible = true;
        }
    }
    catch (ex) {
    }
    if (!dlgMadeVisible && Boolean(top) && Boolean(top.postMessage)) {
        var message = "MakePageVisible";
        top.postMessage(message, "*");
    }
}

function UpdateSSCDialogPageSize() {
    var dlgResized = false;
    try {
        var dlg = window.top.g_childDialog;
        if (!fIsNullOrUndefined(dlg)) {
            dlg.autoSize();
            dlgResized = true;
        }
    }
    catch (ex) {
    }
    if (!dlgResized && Boolean(top) && Boolean(top.postMessage)) {
        var message = "PageWidth=450;PageHeight=500";
        top.postMessage(message, "*");
    }
}