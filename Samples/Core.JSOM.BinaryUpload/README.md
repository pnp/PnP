# Upload binary files from SharePoint Hosted Add-in #

### Summary ###
This sample demonstrates how to upload binary files from SharePoint Hosted Add-in to the host web. This sample project was merged from the blog post "[Deploy binary files from SharePoint Hosted App to Host Web - Stefan Bauer](http://www.n8d.at/blog/deploy-binary-files-from-sharepoint-hosted-app-to-host-web/)".
The first method shows how to upload a binary file to the host web using jquery ajax. The missing support for binary files will corrupt the uploaded file.
The second method extends jQuery capabilites to support binary file reading. This method makes sure that the file won't get corrupted.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
The following minimum version of web browsers are required.
-   Internet Explorer: 10.0+
-   Chrome: 20+
-   Safari: 6.0
-   Firefox: 13.0+
-   Opera: 12.10

### Solution ###
Solution | Author(s)
---------|----------
Core.JSOM.BinaryUpload | Stefan Bauer (**[N8D](http://www.n8d.at/)**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 17th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Sample Add-in #
This sample add-in demonstrates how to deploy binary files from SharePoint Hosted Add-in to the host web. 

First method uploads a file using default jQuery ajax read operation and successfully upload the file. Due missing binary support of jQuery the fill will be corrupted during upload.

The second method extends jQuery with binary support, upload the file successfully and loads image from host web.

## Initialize ##
Description:
To add binary support to jquery the following extension method is required.

## JQuery Extension method for binary support
```JavaScript
// Extends jquery ajaxTransport too support binary reading of files
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
```
[Source: Reading Binary Data using jQuery ajax - Henry Algus](http://www.henryalgus.com/reading-binary-files-using-jquery-ajax/)

### Code for uploading binary file to host web
After this extension have been add the read operation needs to be changed to the following code.

```JavaScript
var sourceFile = appWebUrl + sourcePath;
// Read file from add-in web
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

        logMessage("File uploaded succeeded", state.SUCCESS);

    }, function (sender, args) {

        logMessage("File upload failed "+args.get_message(), state.ERROR);

    });


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.JSOM.BinaryUpload" />