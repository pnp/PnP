Type.registerNamespace('OfficeDev.PnP.JavaScriptInjectionJSOM');

OfficeDev.PnP.JavaScriptInjectionJSOM = function () {
    'use strict';
    // init: initializes the JavaScript injection class, connects click events to buttons
    var init = function (sourceUrl, target, messageDiv) {
        messageElement = messageDiv;
        sourceFile = sourceUrl;
        targetFolder = target;
        // bind click events to the buttons
        $('#btnInjection').click(provision);
        $('#btnEjection').click(unprovision);
        $('#' + messageElement).text('')
    },
    // provision: starts with uploading the JavaScript file to the host we, once done it will continue with the provisionScriptLink() method
    provision = function () {
        $('#' + messageElement).text('Uploading file to host web...')
        context = SP.ClientContext.get_current();
        hostContext = new SP.AppContextSite(context, decodeURIComponent(getQueryStringParameter('SPHostUrl')));
        // load the custom actions from the host web
        actions = hostContext.get_web().get_userCustomActions();
        context.load(actions)
        web = hostContext.get_web();
        context.load(web)
        // download the JavaScript file from the app web
        var req = jQuery.ajax({
            url: sourceFile + '?ver=' + ((new Date()) * 1), // add unique query string variable to prevent browser caching issues
            type: 'GET',
            cache: false,
            dataType: 'text'
        }).done(function (contents) {
            // when downloaded create a new file in the host web
            var createInfo = new SP.FileCreationInformation();
            createInfo.set_content(new SP.Base64EncodedByteArray());
            for (var i = 0; i < contents.length; i++) {
                createInfo.get_content().append(contents.charCodeAt(i));
            }
            createInfo.set_overwrite(true);
            createInfo.set_url(sourceFile.substring(sourceFile.lastIndexOf('/') + 1));
            // add the file to the filder/library specified in init()
            var files = hostContext.get_web().getFolderByServerRelativeUrl(targetFolder).get_files();
            files.add(createInfo);
            // add the file and continue with the custom action provisioning
            context.executeQueryAsync(provisionScriptLink, failure);

        }).fail(function (jqXHR, status) {
            // we don't like being here, because then we couldn't download the javascript file
            failureJQuery(jqXHR, status)
        });
    },
    // unprovision: removes the custom action and the JavaScript file
    unprovision = function () {
        $('#' + messageElement).text('Removing custom action and file from host web...')
        context = SP.ClientContext.get_current();
        hostContext = new SP.AppContextSite(context, decodeURIComponent(getQueryStringParameter('SPHostUrl')));
        // load the custom actions from the host web
        actions = hostContext.get_web().get_userCustomActions();
        context.load(actions)
        web = hostContext.get_web();
        context.load(web)
        context.executeQueryAsync(unprovisionEx, failure)
    },
    // unprovisionEx: internal methot to remove the custom action and JavaScript file
    unprovisionEx = function () {
        $('#' + messageElement).text('Adding custom action to host web...')
        var enumerator = actions.getEnumerator();
        var removeThese = []
        // find the custom action
        while (enumerator.moveNext()) {
            var action = enumerator.get_current();
            if (action.get_description() == 'OfficeDev.PnP.JavaScriptInjectionJSOM' &&
                action.get_location() == 'ScriptLink') {
                // add it to a temporary array (we cannot modify an enumerator while enumerating)
                removeThese.push(action)
            }
        }
        // do the actual removal of the custom action
        for (var i in removeThese) {
            removeThese[i].deleteObject()
            delete removeThese[i]
        }

        // delete the file
        var file = hostContext.get_web().getFileByServerRelativeUrl(web.get_serverRelativeUrl() + '/' + targetFolder + '/' + sourceFile.substring(sourceFile.lastIndexOf('/') + 1));
        file.deleteObject();
        context.executeQueryAsync(
           function () {
               // ...and we're done
               $('#' + messageElement).text('Ejection done!')
           },
           failure);
    },
    // provisionScriptLink: internal method that adds the custom action
    provisionScriptLink = function () {
        $('#' + messageElement).text('Adding custom action to host web...')
        var enumerator = actions.getEnumerator();
        var removeThese = []
        // check if the custom action already exists, if it do then remove it before adding the new one
        while (enumerator.moveNext()) {
            var action = enumerator.get_current();
            if (action.get_description() == 'OfficeDev.PnP.JavaScriptInjectionJSOM' &&
                action.get_location() == 'ScriptLink') {
                removeThese.push(action)
            }
        }
        for (var i in removeThese) {
            removeThese[i].deleteObject()
            delete removeThese[i]
        }

        // create the custom action
        var newAction = actions.add();
        // the 'description' is what we'll use to uniquely identify our custom action
        newAction.set_description('OfficeDev.PnP.JavaScriptInjectionJSOM')
        newAction.set_location('ScriptLink')
        var scriptBlock = 'var headID = document.getElementsByTagName("head")[0];var newScript = document.createElement("script");newScript.type = "text/javascript";newScript.src = "'
        scriptBlock += web.get_url() + '/' + targetFolder + '/' + sourceFile.substring(sourceFile.lastIndexOf('/') + 1) + '?ver=' + ((new Date()) * 1);
        scriptBlock += '";headID.appendChild(newScript);';
        newAction.set_scriptBlock(scriptBlock)

        newAction.update();
        context.executeQueryAsync(
            function () {
                // all is done, if we want to we can actually remove the app from the host web and still be happy about our JavaScript injection
                $('#' + messageElement).text('Injection done!')
            },
            failure);
    },
    // failure: internal method to show error messages for CSOM
    failure = function (sender, args) {
        $('#' + messageElement).text(args.get_message() + ' [' + args.get_errorCode() + ']');
    },
    // failureJQuery: internal method to show error messages for jQuery
    failureJQuery = function (sender, args) {
        $('#' + messageElement).text(status)
    },
    // getQueryStringParameter: internal method to retrieve query string parameter values
    getQueryStringParameter = function (param) {
        var params = document.URL.split('?')[1].split('&');
        var strParams = '';
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split('=');
            if (singleParam[0] == param) {
                return singleParam[1];
            }
        }
    },
    // local parameters
    context = null,
    hostContext = null,
    messageElement = null,
    sourceFile = null,
    targetFolder = null,
    web = null,
    actions = null;
    return {
        init: init,
        provision: provision,
        unprovision: unprovision
    }
}();


// This code runs when the DOM is ready
$(document).ready(function () {
    // Usage: 
    //      sourceUrl: apspecify the JS file relative to the Pages folder (or the page where the script is executing)
    //      target: web relative folder/library where to store the file
    //      messageDiv: id of html element where messages are shown
    OfficeDev.PnP.JavaScriptInjectionJSOM.init('../Content/scenario1.js', 'SiteAssets', 'message')
});
