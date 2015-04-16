
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var peoplePicker;


$(document).ready(function () {
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Get the isDialog from url parameters
    var isDialog = decodeURIComponent(getQueryStringParameter('IsDlg'));

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
                        $('body').show();
                    }

                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);

                        //Make a people picker control
                        //1. context = SharePoint Client Context object
                        //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
                        //3. $('#inputAdministrators') = INPUT that will be used to capture user input
                        //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                        //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                        peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
                        // required to pass the variable name here!
                        peoplePicker.InstanceName = "peoplePicker";
                        // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                        // Do not set the Language property if you do not have foreseen javascript resource file for your language
                        peoplePicker.Language = spLanguage;
                        // optionally show more/less entries in the people picker dropdown, 4 is the default
                        peoplePicker.MaxEntriesShown = 5;
                        // Can duplicate entries be selected (default = false)
                        peoplePicker.AllowDuplicates = false;
                        // Show the user loginname
                        peoplePicker.ShowLoginName = true;
                        // Show the user title
                        peoplePicker.ShowTitle = true;
                        // Set principal type to determine what is shown (default = 1, only users are resolved). 
                        // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                        // Set ShowLoginName and ShowTitle to false if you're resolving groups
                        peoplePicker.PrincipalType = 1;
                        // start user resolving as of 2 entered characters (= default)
                        peoplePicker.MinimalCharactersBeforeSearching = 2;
                        // Hookup everything
                        peoplePicker.Initialize();
                    });

                });
        });

    $('#btnCancel').click(function () {
        if (isDialog == '1') {
            window.opener = null;
            window.open("", "_self");
            window.close();
            try {
                window.frameElement.cancelPopUp();
            }
            catch (e) {
                if (Boolean(top) && Boolean(top.postMessage))
                    top.postMessage('CloseDialog', '*');
            }
            return false;
        }
    });

    //hookup a validation function on submit to perform client side validation
    $("form").submit(function () {
        // show a visual indication that the request is processing
        $('#divLoadingDialog').show();
        // change cursor
        $("body").css("cursor", "progress");

        // disable the create and cancel buttons to prevent second time submission and to avoid confusion
        $('#btnCreate').hide();
        $('#btnCancel').hide();

        return true;
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

function navigateParent(url) {
    var target = parent.postMessage ? parent : (parent.document.postMessage ? parent.document : undefined);
    target.postMessage('NavigateParent=' + url, '*');
}
