Type.registerNamespace('OfficeDevPnP');
Type.registerNamespace('OfficeDevPnP.Core');

OfficeDevPnP.Core.RibbonManager = (function () {
    // private members
    var scriptUrljQuery = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/js/libs/jquery/jquery-1.11.2.min.js";
    var scriptUrlZipJs = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/js/libs/zipjs/zip.js";
    var scriptUrlZipJsDeflate = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/js/libs/zipjs/deflate.js";
    var scriptUrlFileSaverJs = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/js/libs/filesaverjs/FileSaver.js";
    var scriptUrlMultiDownloadJs = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/js/libs/multidownload/browser.js";

    var fileLoaderModel = (function () {
        var webRestUrl;
        var listItemsRESTUrl;
        function initRESTUrls(listId) {
            webRestUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/Web/";
            listItemsRESTUrl = webRestUrl + "Lists('" + listId + "')/items";
        }

        return {
            loadSelectedFiles: function (onend, onabort) {
                var addIndex = 0;
                var fileList = [];

                var folderIndex = 0;
                var foldersToProcess = [];

                var curCtx = SP.ClientContext.get_current();
                var listItems = SP.ListOperation.Selection.getSelectedItems(curCtx);
                var listId = SP.ListOperation.Selection.getSelectedList();

                initRESTUrls(listId);

                function nextListItem() {
                    var listItem = listItems[addIndex];

                    if (abortingDownload) {
                        if (onabort) {
                            onabort();
                        }
                        return;
                    }

                    var url = listItemsRESTUrl + "(" + listItem.id + ")?$select=ID,FileLeafRef,FileRef,FSObjType";
                    $.ajax({
                        url: url,
                        type: "GET",
                        headers: { "accept": "application/json;odata=verbose" },
                        dataType: "json",
                        async: true,
                        success: function (data) {
                            var item = data.d;
                            fileList.push({ name: item.FileLeafRef, url: item.FileRef, fsObjType: item.FSObjType });
                            if (item.FSObjType == 1) {
                                // folder
                                foldersToProcess.push(item.FileRef);
                            }
                            addIndex++;
                            if (addIndex < listItems.length) {
                                nextListItem();
                            } else {
                                if (foldersToProcess.length > 0) {
                                    nextFolder();
                                } else {
                                    onend(fileList);
                                }
                            }
                        },
                        error: function (error) {
                            alert("Error: unable to load information for list item. " + error);
                        }
                    });
                }

                function nextFolder() {
                    var folderRef = foldersToProcess[folderIndex];
                    if (abortingDownload) {
                        if (onabort) {
                            onabort();
                        }
                        return;
                    }
                    processFilesForFolder(folderRef, function () {
                        processSubFolderForFolder(folderRef, function () {
                            folderIndex++;
                            if (folderIndex < foldersToProcess.length) {
                                nextFolder();
                            }
                            else {
                                onend(fileList);
                            }
                        })
                    });
                }

                function processFilesForFolder(folderRef, callback) {
                    var filesUrl = webRestUrl + "GetFolderByServerRelativeUrl('" + escapeProperly(folderRef.replace(/'/g, "''")) + "')/Files?$select=Name,ServerRelativeUrl";
                    $.ajax({
                        url: filesUrl,
                        type: "GET",
                        headers: { "accept": "application/json;odata=verbose" },
                        dataType: "json",
                        async: true,
                        success: function (data) {
                            $.each(data.d.results, function (index, item) {
                                fileList.push({ name: item.Name, url: item.ServerRelativeUrl, fsObjType: 0 });
                            });
                            callback();
                        },
                        error: function (error) {
                            alert("Error: unable to load information about files in folder. " + error);
                        }
                    });
                }

                function processSubFolderForFolder(folderRef, callback) {
                    var foldersUrl = webRestUrl + "GetFolderByServerRelativeUrl('" + escapeProperly(folderRef.replace(/'/g, "''")) + "')/Folders?$select=Name,ServerRelativeUrl";
                    $.ajax({
                        url: foldersUrl,
                        type: "GET",
                        headers: { "accept": "application/json;odata=verbose" },
                        dataType: "json",
                        async: true,
                        success: function (data) {
                            $.each(data.d.results, function (index, item) {
                                fileList.push({ name: item.Name, url: item.ServerRelativeUrl, fsObjType: 1 });
                                foldersToProcess.push(item.ServerRelativeUrl);
                            });
                            callback();
                        },
                        error: function (error) {
                            alert("Error: unable to load information about subfolders. " + error);
                        }
                    });
                }

                nextListItem();
            },
            downloadAllFiles: function (files, onend, onabort) {
                var urlsToDownload = [];
                var downloadPageUrl = _spPageContextInfo.webAbsoluteUrl + "/" + _spPageContextInfo.layoutsUrl + "/download.aspx?SourceUrl=";
                for(var i=0; i< files.length; i++)
                {
                    var file = files[i];
                    if (abortingDownload) {
                        if (onabort) {
                            onabort();
                        }
                        return;
                    }
                    if (file.fsObjType == 0) {
                        // is file
                        urlsToDownload.push(downloadPageUrl + escapeProperly(file.url));
                    }
                }
                multiDownload(urlsToDownload);
                if(onend)
                    onend();
            },
            downloadBlob: function(url, onprogress, onend, onabort){
                var xmlhttp = window.XMLHttpRequest
                                    ? new XMLHttpRequest()
                                    : new ActiveXObject("Microsoft.XMLHTTP");
                xmlhttp.onreadystatechange = function () {
                    if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                        onend(xmlhttp.response);
                    }
                }
                xmlhttp.onprogress = onprogress;
                xmlhttp.onabort = onabort;
                xmlhttp.open('GET', url, true);
                xmlhttp.responseType = 'blob';
                xmlhttp.send();
            }
        }
    })();

    var zipModel = (function () {
        var zipWriter, writer;
        return {
            addFiles: function (files, oninit, onadd, onfiledownloadingprogress, onfilezipprogress, onend, onabort) {
                var addIndex = 0;

                function nextFile() {
                    var file = files[addIndex];
                    onadd(file);
                    if (file.fsObjType == 0) {
                        // is file
                        fileLoaderModel.downloadBlob(file.url,
                            onfiledownloadingprogress,
                            function (response) {
                                addItemToZip(file, response, onabort);
                            },
                            onabort);
                    } else {
                        // is folder
                        addItemToZip(file, null, onabort);
                    }
                }

                function addItemToZip(file, blob, onabort) {
                    var relativePath = getFolderRelativePath(file);
                    zipWriter.add(relativePath, file.fsObjType == 0 ? new zip.BlobReader(blob) : null, function () {
                        addIndex++;
                        if (addIndex < files.length)
                            nextFile();
                        else
                            onend();
                    },
                    onfilezipprogress,
                    { directory: file.fsObjType == 1 });
                }

                function createZipWriter() {
                    zip.createWriter(writer, function (writer) {
                        zipWriter = writer;
                        oninit();
                        nextFile();
                    }, onerror);
                }

                if (zipWriter)
                    nextFile();
                else {
                    writer = new zip.BlobWriter();
                    createZipWriter();
                }
            },
            getBlobURL: function (callback) {
                zipWriter.close(function (blob) {
                    var blobURL = URL.createObjectURL(blob);
                    callback(blobURL);
                    zipWriter = null;
                });
            },
            getBlob: function (callback) {
                zipWriter.close(callback);
                zipWriter = null;
            },
            abort: function (callback) {
                zipWriter.close(callback);
                zipWriter = null;
            }
        };
    })();
    function onerror(error) {
        alert('Error occured. Please try refreshing the page, or closing all opened internet browser windows and restarting your browser.\nPlease also ensure that sufficient memory is available to generate ZIP archive, and close unused programs if needed. Alternatively you can split selected files into smaller groups and download separate ZIP archive for every group. This requires less memory.');
        zipModel.abort(function () {
            handleDownloadAborted();
        });
    }

    function getFolderRelativePath(file) {
        var currentFolderUrl = ctx.rootFolder == "" ? decodeURIComponent(ctx.listUrlDir) : decodeURIComponent(ctx.rootFolder);
        var relativePath = file.name;
        if (file.url.slice(0, currentFolderUrl.length) == currentFolderUrl) {
            // starts with current folder url
            relativePath = file.url.slice(currentFolderUrl.length - file.url.length + 1);
        }
        return relativePath;
    }

    var downloadInProgress = false;
    var abortingDownload = false;
    var progressNotification = null;
    var htmlProgressIcon = "<img src='" + _spPageContextInfo.webAbsoluteUrl + "/" + _spPageContextInfo.layoutsUrl + "/images/loadingcirclests16.gif' /> ";
    var htmlCancelDownloadButton = " <button onclick=\"OfficeDevPnP.Core.RibbonManager.invokeCommand('CancelDownload', {'event': event}); return false;\">Cancel download</button>";
    function showMsgDownloadInProgress() {
        alert("Another download is in progress. Please wait for it to complete, before starting new download.");
    }
    function handleDownloadAborted() {
        handleDownloadCompleted();
        abortingDownload = false;
    }
    function showProgressNotification(msg, showDetails) {
        var htmlNotification = "<div style='width:400px;' id='progressNotificationBlock'>" + htmlProgressIcon + msg + htmlCancelDownloadButton;
        if (showDetails) {
            htmlNotification += "<div style='margin-top: 10px;' id='progressNotificationDetails'>Initializing...</div>";
        }
        htmlNotification += "</div>"
        hideProgressNotification();
        progressNotification = SP.UI.Notify.addNotification(htmlNotification, true);
        var divNotificationBlock = document.getElementById("progressNotificationBlock");
        if (divNotificationBlock != null) {
            divNotificationBlock.parentElement.onclick = function (event) {
                stopEventPropagation(event);
            }
        }
    }
    function updateProgressNotificationDetails(msg) {
        var detailsDiv = document.getElementById('progressNotificationDetails');
        if (detailsDiv) {
            detailsDiv.innerHTML = msg;
        }
    }
    function hideProgressNotification(){
        if (progressNotification != null) {
            SP.UI.Notify.removeNotification(progressNotification);
            progressNotification = null;
        }
    }
    function handleDownloadStarted() {
        downloadInProgress = true;
    }
    function handleDownloadCompleted() {
        hideProgressNotification();
        downloadInProgress = false;
    }
    function stopEventPropagation(event) {
        event.cancelBubble = true;
        if (event.stopPropagation) {
            event.stopPropagation();
        }
    }
    return {
        // public interface
        init: function () {
            OfficeDevPnP.Core.loadScript(scriptUrljQuery, function () {
                OfficeDevPnP.Core.loadScript(scriptUrlZipJs, function () {
                    OfficeDevPnP.Core.loadScript(scriptUrlZipJsDeflate, function () {
                        OfficeDevPnP.Core.loadScript(scriptUrlFileSaverJs, function () {
                            OfficeDevPnP.Core.loadScript(scriptUrlMultiDownloadJs, function () {
                                zip.useWebWorkers = false;
                            });
                        });
                    });
                });
            });
        },
        mdsInit: function(){
            var thisUrl = _spPageContextInfo.siteServerRelativeUrl
                + "/SiteAssets/ribbonmanager.js";
            OfficeDevPnP.Core.RibbonManager.init();
            RegisterModuleInit(thisUrl, OfficeDevPnP.Core.RibbonManager.init);
        },
		isListViewButtonEnabled: function (cmd, attrs) {
			var ctx = SP.ClientContext.get_current();
			var items = SP.ListOperation.Selection.getSelectedItems(ctx);
			if (items.length == 0)
				return false;
			return true;
		},
		invokeCommand: function (cmd, attrs) {
		    var htmlNotification;
		    switch (cmd) {
		        case 'CancelDownload':
		            if (attrs && attrs.event) {
		                stopEventPropagation(attrs.event);
		            }
		            if (confirm("Do you want to cancel downloading files?")) {
		                abortingDownload = true;
		            }
		            break;
		        case 'DownloadAll':
		            if (downloadInProgress) {
		                showMsgDownloadInProgress();
		                break;
		            }
		            if (confirm("Do you want to download all selected files, including files in all selected folders?")) {
		                handleDownloadStarted();
		                showProgressNotification("Preparing files for download...");
		                fileLoaderModel.loadSelectedFiles(
                            function (fileList) {
		                        fileLoaderModel.downloadAllFiles(fileList, function () {
		                            handleDownloadCompleted();
		                        });
                            }, handleDownloadAborted);
		            }
			        break;
		        case 'DownloadAllAsZip':
		            if (downloadInProgress) {
		                showMsgDownloadInProgress();
		                break;
		            }
		            if (confirm("Do you want to download all selected files and folders as single ZIP archive?")) {
		                handleDownloadStarted();
		                showProgressNotification("Preparing ZIP archive for download...", true);
		                var currentFile = null;
		                var totalFiles = 0;
		                var currentFileIndex = 0;
		                fileLoaderModel.loadSelectedFiles(
                            function (fileList) {

                                for (i in fileList) {
                                    if (fileList[i].fsObjType == 0) {
                                        totalFiles++;
                                    }
                                }

		                        zipModel.addFiles(fileList, function () {
		                        }, function (file) {
		                            if (file.fsObjType == 0) {
		                                currentFile = file;
		                                currentFileIndex++;
		                                updateProgressNotificationDetails("Processing file " + currentFileIndex + " of " + totalFiles + ": <b>" + getFolderRelativePath(currentFile) + "</b>");
		                            } else {
		                                currentFile = null;
		                                updateProgressNotificationDetails("Processing folder: <b>" + getFolderRelativePath(file) + "</b>");
		                            }
		                        }, function (evt) {
		                            if (abortingDownload) {
		                                evt.target.abort();
		                                return;
		                            }
		                            if (evt.lengthComputable && currentFile != null) {
		                                //evt.loaded the bytes browser receive
		                                //evt.total the total bytes set by the header
		                                var percentComplete = ((evt.loaded / evt.total) * 100).toFixed(2);
		                                updateProgressNotificationDetails("Processing file " + currentFileIndex + " of " + totalFiles + ": <b>" + getFolderRelativePath(currentFile) + "</b><br/>Downloading - " + percentComplete + "%");
		                            }
		                        }, function (current, total) {
		                            if (abortingDownload) {
		                                updateProgressNotificationDetails("Aborting...");
		                                zipModel.abort(handleDownloadAborted);
		                                return;
		                            }
		                            if (currentFile != null) {
		                                var zipPercentComplete = ((current / total) * 100).toFixed(2);
		                                updateProgressNotificationDetails("Processing file " + currentFileIndex + " of " + totalFiles + ": <b>" + getFolderRelativePath(currentFile) + "</b><br/>Adding to ZIP - " + zipPercentComplete + "%");
		                            }
		                        }, function () {
		                            updateProgressNotificationDetails("Finalizing ZIP file...");
		                            zipModel.getBlob(function (blob) {
		                                updateProgressNotificationDetails("Initiating download for ZIP file...");
		                                saveAs(blob, "files.zip");
		                                handleDownloadCompleted();
		                            });
		                        },
                                function () {
                                    updateProgressNotificationDetails("Aborting...");
                                    zipModel.abort(handleDownloadAborted);
                                });
                            }, handleDownloadAborted);
		            }
			        break;
		    }
		}
	};
})();

if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    OfficeDevPnP.Core.RibbonManager.mdsInit()
} else {
    OfficeDevPnP.Core.RibbonManager.init()
}

