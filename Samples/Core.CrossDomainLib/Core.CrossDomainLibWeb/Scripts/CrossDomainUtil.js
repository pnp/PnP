function CrossDomainUtil()
{
    var _initializeEventFunction = null;
    var _proxyPageUrl = null;
    var _proxyObjectId = "";

    var _requestCallBackFunctionArray = [];
    var _requestCallBackCounter = 0;
    var _self;

    this.Initialized = false;

    this.Init = function (proxyPageUrl)
    {
        _self = this;
        _proxyPageUrl = proxyPageUrl;

        //hook messageListener
        if (window.addEventListener) {
            addEventListener("message", SpCDUMessageListener, false);
        } else {
            attachEvent("onmessage", SpCDUMessageListener);
        }

        _proxyPageUrl = proxyPageUrl;
        _proxyObjectId = proxyPageUrl.split('?')[0].replace(/[\/:.]/g, '');

        if ($("#" + _proxyObjectId).length == 0) {
            var iframeHtml = "<iframe src='" + _proxyPageUrl + "' id='" + _proxyObjectId + "' style='display: none;'></iframe>";
            $("body").append($(iframeHtml));
        }
    }

    this.OnInitialized = function(initializeEventFunction)
    {
        _initializeEventFunction = initializeEventFunction;
    }

    this.ajax = function (requestObject) {
        //set default settings
        if (requestObject != null && typeof requestObject != 'undefined') {
            if (requestObject.method == null || typeof requestObject.method == 'undefined') {
                requestObject.method = "GET";
            }
            if (requestObject.data == null || typeof requestObject.data == 'undefined') {
                requestObject.data = {};
            }
            if (requestObject.dataType == null || typeof requestObject.dataType == 'undefined') {
                requestObject.dataType = "json";
            }
            if (typeof requestObject.success == 'undefined') {
                requestObject.success = null;
            }
            if (typeof requestObject.error == 'undefined') {
                requestObject.error = null;
            }
        }

        requestObject.CallBackFunctionId = _requestCallBackCounter;

        var requestObjectString = JSON.stringify(requestObject);
        SetCallBackFunction(requestObject.success, requestObject.error);

        $("#" + _proxyObjectId)[0].contentWindow.postMessage(requestObjectString, "*");
    }

    var SetCallBackFunction = function (requestCallBackFunction, requestErrorCallBackFunction) {
        var callBackFunction = new SpCrossDomainCallBack();
        callBackFunction.Id = _requestCallBackCounter;
        callBackFunction.RequestCallBackFunction = requestCallBackFunction;
        callBackFunction.RequestErrorCallBackFunction = requestErrorCallBackFunction;
        _requestCallBackCounter++;
        _requestCallBackFunctionArray.push(callBackFunction);
    }

    var SpCDUMessageListener = function (event)
    {
        if (event.data != null && !(typeof event.data === "undefined") && event.data.indexOf("#/CDU#") > -1) {
            var endTag = event.data.indexOf("#/CDU#");
            var metaData = event.data.substring(5, endTag);
            var contentData = event.data.substring(endTag + 6, event.data.length);

            var splittedMetaData = metaData.split("@");
            var messageCode = splittedMetaData[0];
            var messageIdentifier = splittedMetaData[1];

            if (messageCode == "1") {  //initialized
                _self.Initialized = true;
                if (_initializeEventFunction != null) {
                    _initializeEventFunction();
                }
            }
            else if (messageCode == "2") { //returndata recieved 
                var id = parseInt(messageIdentifier);
                callbackFunction = GetCallBackFunction(id);

                if (callbackFunction != null) {
                    callbackFunction.RequestCallBackFunction(contentData);
                }
            }
            else if (messageCode == "3") {  //a error occured
                var id = parseInt(messageIdentifier);
                callbackFunction = GetCallBackFunction(id);

                if (callbackFunction != null && callbackFunction.RequestErrorCallBackFunction != null && !((typeof callbackFunction.RequestErrorCallBackFunction === "undefined"))) {
                    callbackFunction.RequestErrorCallBackFunction(contentData);
                }
            }
        }
    }

    var GetCallBackFunction = function (id) {
        var foundCallBackIndex = -1;
        for (var i = 0; i < _requestCallBackFunctionArray.length; i++) {
            if (_requestCallBackFunctionArray[i].Id == id) {
                foundCallBackIndex = i;
                break;
            }
        }

        var callBackFunction = _requestCallBackFunctionArray[foundCallBackIndex];
        _requestCallBackFunctionArray.splice(foundCallBackIndex, 1);
        return callBackFunction;
    }
}

function SpCrossDomainCallBack() {
    this.Id;
    this.RequestCallBackFunction;
    this.RequestErrorCallBackFunction;
}