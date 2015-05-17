'use strict';

var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

// Extends jquery ajaxTransport too support binary reading of files
// Method taken from http://www.henryalgus.com/reading-binary-files-using-jquery-ajax/
$.ajaxTransport("+binary", function (options, originalOptions, jqXHR) {
    // check for conditions and support for blob / arraybuffer response type
    if (window.FormData && ((options.dataType && (options.dataType == 'binary')) ||
        (options.data && ((window.ArrayBuffer && options.data instanceof ArrayBuffer) ||
        (window.Blob && options.data instanceof Blob))))) {
        return {
            // create new XMLHttpRequest
            send: function (headers, callback) {
                // setup all variables
                var xhr = new XMLHttpRequest(),
		        url = options.url,
		        type = options.type,
		        async = options.async || true,
		        // blob or arraybuffer. Default is blob
		        dataType = options.responseType || "blob",
		        data = options.data || null,
		        username = options.username || null,
		        password = options.password || null;

                xhr.addEventListener('load', function () {
                    var data = {};
                    data[options.dataType] = xhr.response;
                    // make callback and send data
                    callback(xhr.status, xhr.statusText, data, xhr.getAllResponseHeaders());
                });

                xhr.open(type, url, async, username, password);

                // setup custom headers
                for (var i in headers) {
                    xhr.setRequestHeader(i, headers[i]);
                }

                xhr.responseType = dataType;
                xhr.send(data);
            },
            abort: function () {
                jqXHR.abort();
            }
        };
    }
});

var hostWebUrl,
    appWebUrl,
    hostWebContext,
    destinationServerRelativeUrl,
    destinationFileName;

// Define base object
Type.registerNamespace("BinaryUpload");

BinaryUpload.Uploader = function () {

    var hostWebUrl,
        hostWebContext,
        appWebUrl;

    var state = {
        ERROR: "error",
        WARNING: "warning",
        SUCCESS: "success"
    }

    // Read the Host Web Url and the App Web Url from query string
    var init = function () {
        var hostWebUrlFromQS = $.getUrlVar("SPHostUrl");
        hostWebUrl = (hostWebUrlFromQS !== undefined) ? decodeURIComponent(hostWebUrlFromQS) : undefined;

        var appWebUrlFromQS = $.getUrlVar("SPAppWebUrl");
        appWebUrl = (appWebUrlFromQS !== undefined) ? decodeURIComponent(appWebUrlFromQS) : undefined;
    };

    var logMessage = function (logMessage, status) {
        var message = String.format("<div class='{0}'>{1}</div>", status, logMessage);
        $("#msgBinUpload").append(message);
    };
    var clearMessages = function () {
        $("#msgBinUpload").text("");
    };
    var executeUpload = function (sourcePath, targetPath) {

        // initialise base variables base from query string
        init();
        clearMessages();

        hostWebContext = new SP.ClientContext(getRelativeUrlFromAbsolute(hostWebUrl));
        var web = hostWebContext.get_web();

        hostWebContext.load(web);
        hostWebContext.executeQueryAsync(
            // in case of success
            function () {
                logMessage("Host Web successfully loaded", state.SUCCESS);

                var sourceFile = appWebUrl + sourcePath;
                logMessage("Reading file from App Web <a href='" + sourceFile + "' target='_blank'>" + sourcePath + "</a><br /><br />", state.SUCCESS);
                logMessage("<img src='" + sourceFile + "'><br />", state.SUCCESS);
                // Read file from app web
                $.ajax({
                    url: sourceFile,
                    type: "GET",
                    dataType: "binary",
                    processData: false,
                    responseType: 'arraybuffer',
                    cache: false
                }).done(function (contents) {

                    var fileName = getFilenameFromUrl(targetPath);
                    var folder = getPathFromUrl(targetPath);

                    logMessage("Create file at<br>    " + hostWebUrl + "/" + folder + fileName, state.SUCCESS);

                    // Create new file
                    var createInfo = new SP.FileCreationInformation();

                    // Convert ArrayBuffer to Base64 string
                    createInfo.set_content(arrayBufferToBase64(contents));

                    // Overwrite if already exists
                    createInfo.set_overwrite(true);

                    // set target url
                    createInfo.set_url(fileName);

                    // retrieve file collection of folder
                    var files = hostWebContext.get_web().getFolderByServerRelativeUrl(getRelativeUrlFromAbsolute(hostWebUrl) + folder).get_files();

                    // load file collection from host web
                    hostWebContext.load(files);

                    // add the new file
                    files.add(createInfo);

                    // upload file
                    hostWebContext.executeQueryAsync(function () {

                        var loadImage = hostWebUrl + "/" + folder + fileName;
                        logMessage("File uploaded succeeded", state.SUCCESS);
                        logMessage("<b>Try to embed file from host web</b><br><br>", state.SUCCESS);
                        logMessage("<img src='" + loadImage + "'>", state.SUCCESS);
                        logMessage("<a href='" + loadImage + "' target='_blank'>" + folder + fileName + "</a>", state.SUCCESS);
                        logMessage("<b>File was successfully uploaded as binary file<br>Image can be loaded successfully.</b>", state.SUCCESS);

                    }, function (sender, args) {

                        logMessage("File upload failed "+args.get_message(), state.ERROR);

                    });


                }).fail(function (jqXHR, textStatus) {
                    logMessage(textStatus, state.ERROR);
                    logMessage("File '" + appWebUrl + sourcePath + "' failed.<br>"+textStatus);
                });

            },
            // in case of error
            function (sender, args) {
                logMessage(args.get_message(), state.ERROR);
            })

    }

    var getRelativeUrlFromAbsolute = function (absoluteUrl) {
        absoluteUrl = absoluteUrl.replace('https://', '');

        var parts = absoluteUrl.split('/');
        var relativeUrl = '/';

        for (var i = 1; i < parts.length; i++) {
            relativeUrl += parts[i] + '/';
        }

        return relativeUrl;

    }

    var getFilenameFromUrl = function (url) {

        var filename = url.substring(url.lastIndexOf('/') + 1);
        return filename;

    }

    var getPathFromUrl = function (url) {

        var path = url.substring(1, url.lastIndexOf('/') + 1);
        return path;

    }

    var arrayBufferToBase64 = function (buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    return {
        Upload: function (sourcePath, targetPath) {

            executeUpload(sourcePath, targetPath);

        }
    }

}