(function ($, SP, SP2013, undefined) {

    SP2013.Constants = SP2013.Constants || {};

    // See documentation for list template types
    //http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.splisttemplatetype.aspx
    SP2013.Constants.ListTemplateTypes = {
        103: 'Links'
        , 104: 'Announcements'
        , 106: 'Calendar'
    };


    SP2013.Utilities = SP2013.Utilities || {};


    SP2013.UX = SP2013.UX || {};

    SP2013.UX.addHostStyleSheets = function (hostUrl, spClientTag) {
        var hostWebCssControlPath = hostUrl + '/_layouts/15/defaultcss.ashx?ctag=' + spClientTag
            , link
        ;

        // Create element which references sharepoint control
        link = document.createElement('link');
        link.rel = 'stylesheet';
        link.type = 'text/css';
        link.href = hostWebCssControlPath;

        // Append element to end of head element
        document.getElementsByTagName('head')[0].appendChild(link);

        return hostWebCssControlPath;
    };

    SP2013.UX.updateIframeSize = function (spHostUrl, senderId, width, height) {
        if (typeof spHostUrl !== "string" || spHostUrl.length === 0) {
            throw new Error('Value passed for spHostUrl is not valid. spHostUrl must be a non-empty string. value: ' + spHostUrl);
        }
        senderId = senderId.toString();
        if (typeof senderId !== "string" || senderId.length === 0) {
            throw new Error('Value passed for senderId is not valid. senderId must be a non-empty string. value: ' + senderId);
        }

        var postMessage = "<message senderId=" + senderId + ">resize(" + width + ", " + height + ")</message>";

        window.parent.postMessage(postMessage, spHostUrl);
    };


    SP2013.Ajax = SP2013.Ajax || {};

    SP2013.Ajax.getSpRequestExecutorPromise = function (appWebUri, hostWebUri, resourcePath, returnRawData) {
        console.log("getSpRequstExecutorPromise: ", arguments);

        var executor = new SP.RequestExecutor(appWebUri)
            // TODO: make uri construction accept resourcePaths which have already included querystring parameters such as OData filter,select
            , uriFragment = appWebUri + '/_api/SP.AppContextSite(@target)' + resourcePath
            , targetFragment = "@target='" + hostWebUri + "'"
            , uri = (uriFragment.indexOf('?') > 0) ? (uriFragment + '&' + targetFragment) : (uriFragment + '?' + targetFragment)
            , deferred = $.Deferred()
            , executorPromise
        ;

        executor.executeAsync({
            url: uri,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function () {
                deferred.resolve.apply(null, arguments);
            },
            error: function () {
                deferred.reject.apply(null, arguments);
            }
        });

        executorPromise = deferred.promise();

        if (returnRawData === undefined) {
            executorPromise = executorPromise.then(function (data) {
                // Request Executor does not automaticaly parse the responseText like jQuery does.
                // We are technically hiding some information by parsing here instead of as a
                // thenable method, the caller can request the raw form by padding any value for returnRawData

                var parsedData = JSON.parse(data.body).d
                    , returnData = parsedData
                ;

                // TODO: Determine if this is an acceptable assumption.
                // This is a shortcut to return the array if the response is an array otherwise return the object;
                // however in the case where the intended object happens to have a property named results this would
                // incorrectly only return that value
                // Possible solution would be to have the caller of the endpoint determine how to parse the data depending on if
                // it is expecting an array or not.
                if (parsedData.results) {
                    returnData = parsedData.results;
                }

                return returnData;
            });
        }

        return executorPromise;
    };

})(jQuery, SP, window.SP2013 = window.SP2013 || {});