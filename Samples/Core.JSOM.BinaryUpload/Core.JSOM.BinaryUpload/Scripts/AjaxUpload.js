'use strict';

var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

// extends jquer for easier fetching url parameter
$.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return $.getUrlVars()[name];
    }
});


var hostWebUrl,
    appWebUrl,
    hostWebContext,
    destinationServerRelativeUrl,
    destinationFileName;

// Define base object and namespace
Type.registerNamespace("AjaxUpload");

AjaxUpload.Uploader = function () {

    var hostWebUrl,
        hostWebContext,
        appWebUrl;

    // Logging states
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
    // Message Logger
    var logMessage = function (logMessage, status) {
        var message = String.format("<div class='{0}'>{1}</div>", status, logMessage);
        $("#msgAjaxUpload").append(message);
    };
    var clearMessages = function () {
        $("#msgAjaxUpload").text("");
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
                    cache: false
                }).done(function (contents) {

                    var fileName = getFilenameFromUrl(targetPath);
                    var folder = getPathFromUrl(targetPath);

                    logMessage("Create file at<br>    " + hostWebUrl + "/" + folder + fileName, state.SUCCESS);
                    // Create new file
                    var createInfo = new SP.FileCreationInformation();
                                        
                    // Convert ArrayBuffer to Base64 string
                    createInfo.set_content(new SP.Base64EncodedByteArray());
                    for (var i = 0; i < contents.length; i++) {

                        createInfo.get_content().append(contents.charCodeAt(i));
                    }

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
                        logMessage("Try to embed file from host web", state.SUCCESS);
                        logMessage("<img src='" + loadImage + "'>", state.SUCCESS);
                        logMessage("<a href='" + loadImage + "' target='_blank'>" + folder + fileName + "</a>", state.SUCCESS);
                        logMessage("<b>File was successfully uploaded but corrupted due invalid reading</b><br>", state.SUCCESS);

                    }, function (sender, args) {

                        logMessage("File upload failed", state.ERROR);

                    });


                }).fail(function (jqXHR, textStatus) {
                    logMessage(textStatus, state.ERROR);
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