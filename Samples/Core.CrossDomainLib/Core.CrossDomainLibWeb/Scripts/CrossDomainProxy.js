function CrossDomainProxy()
{
    var _allowedDomain = "*";

    this.Init = function(allowedDomain)
    {
        _allowedDomain = allowedDomain;

        //hook messageListener
        if (window.addEventListener) {
            addEventListener("message", SpCDPMessageListener, false);
        } else {
            attachEvent("onmessage", SpCDPMessageListener);
        }

        //tell parent that proxy is ready
        parent.postMessage("#CDU#1@0#/CDU#", _allowedDomain);
    }

    var SpCDPMessageListener = function (event)
    {
        if (event.origin == _allowedDomain || _allowedDomain == "*") {
            var requestObject = JSON.parse(event.data);

            //do request to server
            var callBackFunctionId = requestObject.CallBackFunctionId;

            $.ajax({
                url: requestObject.url,
                type: requestObject.method,
                dataType: requestObject.dataType,
                // we set cache: false because GET requests are often cached by browsers
                // IE is particularly aggressive in that respect
                cache: false,
                data: requestObject.data,
                success: function (response) {
                    var reponseMessage = "#CDU#2@" + callBackFunctionId + "#/CDU#" + response;
                    parent.postMessage(reponseMessage, _allowedDomain);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var reponseMessage = "#CDU#3@" + callBackFunctionId + "#/CDU#" + textStatus + " --> " + errorThrown;
                    parent.postMessage(reponseMessage, _allowedDomain);
                }
            });
        }
    }



    var PerformGet = function (requestObject)
    {
        var callBackFunctionId = requestObject.CallBackFunctionId;

            $.ajax({
                url: requestObject.url,
                type: 'GET',
                dataType: requestObject.dataType,
                // we set cache: false because GET requests are often cached by browsers
                // IE is particularly aggressive in that respect
                cache: false,
                data: requestObject.data,
                success: function (response) {
                    var reponseMessage = "#CDU#2@" + callBackFunctionId + "#/CDU#" + response;
                    parent.postMessage(reponseMessage, _allowedDomain);
                },
                error: function (jqXHR, textStatus, errorThrown)
                {
                    var reponseMessage = "#CDU#3@" + callBackFunctionId + "#/CDU#" + textStatus + " --> " + errorThrown;
                    parent.postMessage(reponseMessage, _allowedDomain);
                }
            });
    }

    var PerformPost = function (requestObject) {
        var callBackFunctionId = requestObject.CallBackFunctionId;

        $.ajax({
            url: requestObject.Url,
            type: 'POST',
            dataType: 'json',
            // we set cache: false because GET requests are often cached by browsers
            // IE is particularly aggressive in that respect
            cache: false,
            data: requestObject.DataObject,
            success: function (response) {
                var reponseMessage = "#CDU#2@" + callBackFunctionId + "#/CDU#" + response;
                parent.postMessage(reponseMessage, _allowedDomain);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var reponseMessage = "#CDU#3@" + callBackFunctionId + "#/CDU#" + textStatus + " --> " + errorThrown;
                parent.postMessage(reponseMessage, _allowedDomain);
            }
        });
    }
}

$(function () {
    var cdlProxy = new CrossDomainProxy();
    cdlProxy.Init("*");  //* means every domain can call this proxy method. By adding a domain, only this domain can use the proxy. 
});