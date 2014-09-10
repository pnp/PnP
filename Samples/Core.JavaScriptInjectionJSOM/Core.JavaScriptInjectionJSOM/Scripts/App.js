
Type.registerNamespace('OfficeDev.PnP.JavaScriptInjectionJSOM');

OfficeDev.PnP.JavaScriptInjectionJSOM = function () {
    'use strict';
    var init = function (sourceUrl, target, messageDiv) {
        messageElement = messageDiv;
        sourceFile = sourceUrl;
        targetFolder = target;
        $('#btnInjection').click(provision);
        $('#btnEjection').click(unprovision);
        $('#' + messageElement).text('')
    },
    provision = function () {
        $('#' + messageElement).text('Uploading file to host web...')
        context = SP.ClientContext.get_current();
        hostContext = new SP.AppContextSite(context, decodeURIComponent(getQueryStringParameter('SPHostUrl')));
        actions = hostContext.get_web().get_userCustomActions();
        context.load(actions)
        web = hostContext.get_web();
        context.load(web)
        var req = jQuery.ajax({
            url: sourceFile + '?ver=' + ((new Date()) * 1),
            type: 'GET',
            cache: false,
            dataType: 'text'
        }).done(function (contents) {
            var createInfo = new SP.FileCreationInformation();
            createInfo.set_content(new SP.Base64EncodedByteArray());
            for (var i = 0; i < contents.length; i++) {
                createInfo.get_content().append(contents.charCodeAt(i));
            }
            createInfo.set_overwrite(true);
            createInfo.set_url(sourceFile.substring(sourceFile.lastIndexOf('/') + 1));

            var files = hostContext.get_web().getFolderByServerRelativeUrl(targetFolder).get_files();
            files.add(createInfo);
            context.executeQueryAsync(provisionScriptLink, failure);

        }).fail(function (jqXHR, status) {
            failureJQuery(jqXHR, status)
        });
    },
    unprovision = function () {
        $('#' + messageElement).text('Removing custom action and file from host web...')
        context = SP.ClientContext.get_current();
        hostContext = new SP.AppContextSite(context, decodeURIComponent(getQueryStringParameter('SPHostUrl')));
        actions = hostContext.get_web().get_userCustomActions();
        context.load(actions)
        web = hostContext.get_web();
        context.load(web)
        context.executeQueryAsync(unprovisionEx, failure)
    },
    unprovisionEx = function () {
        $('#' + messageElement).text('Adding custom action to host web...')
        var enumerator = actions.getEnumerator();
        var removeThese = []
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

        var file = hostContext.get_web().getFileByServerRelativeUrl(web.get_serverRelativeUrl() + '/' + targetFolder + '/' + sourceFile.substring(sourceFile.lastIndexOf('/') + 1));
        file.deleteObject();
        context.executeQueryAsync(
           function () {
               $('#' + messageElement).text('Ejection done!')
           },
           failure);
    },
    provisionScriptLink = function () {
        $('#' + messageElement).text('Adding custom action to host web...')
        var enumerator = actions.getEnumerator();
        var removeThese = []
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


        var newAction = actions.add();
        newAction.set_description('OfficeDev.PnP.JavaScriptInjectionJSOM')
        newAction.set_location('ScriptLink')
        var scriptBlock = 'var headID = document.getElementsByTagName("head")[0];var newScript = document.createElement("script");newScript.type = "text/javascript";newScript.src = "'
        scriptBlock += web.get_url() + '/' + targetFolder + '/' + sourceFile.substring(sourceFile.lastIndexOf('/') + 1) + '?ver=' + ((new Date()) * 1);
        scriptBlock += '";headID.appendChild(newScript);';
        newAction.set_scriptBlock(scriptBlock)

        newAction.update();
        context.executeQueryAsync(
            function () {
                $('#' + messageElement).text('Injection done!')
            },
            failure);
    },    
    failure = function (sender, args) {
        $('#' + messageElement).text(args.get_message() + ' [' + args.get_errorCode() + ']');
    },
    failureJQuery = function (sender, args) {
        $('#' + messageElement).text(status)
    },
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
    OfficeDev.PnP.JavaScriptInjectionJSOM.init('../Content/scenario1.js', 'SiteAssets', 'message')
});
