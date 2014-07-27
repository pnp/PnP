
$(document).ready(function () {
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    //Get the URI decoded SharePoint app web url from the SPAppWebUrl parameter.
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    //Get the isDialog from url parameters
    var isDialog = decodeURIComponent(getQueryStringParameter('IsDlg'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Create a Link element for the defaultcss.ashx resource
                    var linkElement = document.createElement('link');
                    linkElement.setAttribute('rel', 'stylesheet');
                    linkElement.setAttribute('href', layoutsRoot + 'defaultcss.ashx');

                    ////Add the linkElement as a child to the head section of the html
                    var headElement = document.getElementsByTagName('head');
                    headElement[0].appendChild(linkElement);

                    //Execute the correct script based on the isDialog
                    if (isDialog == '0') {
                        //Load the SP.UI.Controls.js file to render the App Chrome
                        $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                        //These buttons are hidden in full screen mode as they're only relevant for dialog mode is this case
                        //$('#btnOk').hide();
                        //$('#btnCancel').hide();
                    }
                    else if (isDialog == '1') {
                        $('body').show();
                    }

                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', printAllListNamesFromHostWeb);
                });
        });

    // below function has been copied from http://www.mavention.com/blog/sharepoint-app-reading-data-from-host-web
    function printAllListNamesFromHostWeb() {
        var context;
        var factory;
        var appContextSite;
        var collList;

        context = new SP.ClientContext(appWebUrl);
        factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
        context.set_webRequestExecutorFactory(factory);
        appContextSite = new SP.AppContextSite(context, spHostUrl);

        this.web = appContextSite.get_web();
        collList = this.web.get_lists();
        context.load(collList);

        context.executeQueryAsync(
            Function.createDelegate(this, successHandler),
            Function.createDelegate(this, errorHandler)
        );

        function successHandler() {
            var listInfo = '';
            var listEnumerator = collList.getEnumerator();

            while (listEnumerator.moveNext()) {
                var oList = listEnumerator.get_current();
                listInfo += '<li>' + oList.get_title() + '</li>';
            }

            document.getElementById("siteTitle").innerHTML = 'Lists found on the host web:<ul>' + listInfo + '</ul>';
        }

        function errorHandler(sender, args) {
            document.getElementById("message").innerText =
                "Could not complete cross-domain call: " + args.get_message();
        }
    }

    $('#btnCancel').click(function () {
        if (isDialog == '1') {
            top.postMessage('CloseDialog', '*');
            return false;
        }
    });    

});

function navigateParent(url) {
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : undefined);
    target.postMessage('NavigateParent=' + url, '*');
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

