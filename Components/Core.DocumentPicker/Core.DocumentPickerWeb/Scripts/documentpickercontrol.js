var CAMControl;
(function (CAMControl) {
   
    var DocumentPicker = (function () {

        // Constructor
        function DocumentPicker() {
            //public properties set by initialize
            this.ControlDivId;
            this.HiddenDataField;
            this.DataSource;
            this.InstanceName;
            
            //public properties
            this.MaximumNumberOfFiles = 0;
            this.Language = "en-us";
            this.ImageFolder = "../Styles/documentpicker/images/";
            this.ExpandFolders = true;
            
            //Private variable is not really private, just a naming convention
            this._selectedDocumentsContainerId;
            this._openDocumentDialogButtonId;
            this._documentPickerDialogId;
            this._documentPickerDialogOverlayId;
            this._documentPickerCloseButtonId;
            this._treeContainerId;
            this._documentDialogLoadingDivId;
            this._rootTreeNodeId;
            this._treeDialogSelectButtonId;
            this._dialogSelectedDocumentsContainerId;
            this._documentPickerCancelId;
            this._documentPickerOkId;
            this._dialogDocumentPickerTitleId;
            this._dialogSelectedDocumentsTitleId;
            this._maximumFilesAllowedMessage = "Too much files selected";
        }

        //Events
        DocumentPicker.prototype.OpenDocumentDialogButton_OnClick = function (currentControl) {
            //copy data from selected document div to selected document div of dialog window
            $("#" + currentControl._dialogSelectedDocumentsContainerId).html($("#" + currentControl._selectedDocumentsContainerId).html());
            $("#" + currentControl._dialogSelectedDocumentsContainerId).find(".cam-documentpicker-dialog-selected-delete").click(function () { $(this).parent().remove(); });

            //show the dialog
            $("#" + currentControl._documentPickerDialogId).show();
            $("#" + currentControl._documentPickerDialogOverlayId).show();

            window.scrollTo(0, 0);
            $("body").css('overflow', 'hidden');
        };

        DocumentPicker.prototype.CloseDocumentDialogButton_OnClick = function (currentControl) {
            //hide dialog
            $("#" + currentControl._documentPickerDialogId).hide();
            $("#" + currentControl._documentPickerDialogOverlayId).hide();

            //clear previous selected items in the tree
            $("#" + currentControl._documentPickerDialogId).find(".cam-documentpicker-treenode-file").removeClass('selected');

            $("body").css('overflow', 'auto');
        };

        DocumentPicker.prototype.SelectButton_OnClick = function (currentControl) {
            var selectedNode = $("#" + currentControl._documentPickerDialogId).find(".selected");

            if (selectedNode.length > 0) {

                //see if document is already selected
                var documentId = selectedNode.attr("data-documenturl");
                var alreadySelected = $("#" + currentControl._dialogSelectedDocumentsContainerId).find("[data-documenturl='" + documentId + "']");

                if (alreadySelected.length == 0) { //if the document is not selected already
                    var parentDiv = selectedNode.parent();
                    var documentImage = parentDiv.children().eq(1).clone(); //make copy of document image
                    var span = parentDiv.children().eq(2).clone(); //make copy of span

                    //add correct styling and classes to selected document
                    span.removeClass();
                    span.addClass("cam-documentpicker-term-selected");

                    //build selected document html
                    var documentImageHtml = documentImage.wrap('<p/>').parent().html();
                    var spanHtml = span.wrap('<p/>').parent().html();

                    var selectedElementHtml = '<div class="cam-documentpicker-treenode">' +
                                              documentImageHtml +
                                              '<a class="cam-documentpicker-link" href="' + documentId + '" target="_blank">' +
                                              spanHtml +
                                              '</a>' +
                                              '<img alt="" src="' + currentControl.ImageFolder + 'Close.png" class="cam-documentpicker-dialog-selected-delete">' +
                                              '</div>';

                    //show document in selected box
                    $("#" + currentControl._dialogSelectedDocumentsContainerId).append(selectedElementHtml);
                    $("#" + currentControl._documentPickerDialogId).find(".cam-documentpicker-dialog-selected-delete").click(function () { $(this).parent().remove(); });
                }
            }
        };

        DocumentPicker.prototype.DocumentPickerOk_OnClick = function (currentControl) {
            var files = $("#" + currentControl._dialogSelectedDocumentsContainerId).find(".cam-documentpicker-term-selected");

            if (currentControl.MaximumNumberOfFiles > 0 && files.length > currentControl.MaximumNumberOfFiles) { //if a maximum of files is set
                alert(currentControl._maximumFilesAllowedMessage); //maximum number od files selected is succeeded
            }
            else {
                //copy html from selected document div of dialog to selected documents in the page
                $("#" + currentControl._selectedDocumentsContainerId).html($("#" + currentControl._dialogSelectedDocumentsContainerId).html());
                $("#" + currentControl._dialogSelectedDocumentsContainerId).html("");
                $("#" + currentControl._selectedDocumentsContainerId).find(".cam-documentpicker-dialog-selected-delete").click(function () { currentControl.DocumentPickerRemovePageDocument($(this), currentControl) });

                //hide dialog
                $("#" + currentControl._documentPickerDialogId).hide();
                $("#" + currentControl._documentPickerDialogOverlayId).hide();
                $("#" + currentControl._documentPickerDialogId).find(".cam-documentpicker-treenode-file").removeClass('selected');

                //save values to the hidden field
                currentControl.DocumentPickerSetValueField(currentControl);

                $("body").css('overflow', 'auto');
            }
        };


        //Functions

        DocumentPicker.prototype.DocumentPickerRemovePageDocument = function (selectedItem, currentControl) {
            //remove item from selected documents
            selectedItem.parent().remove();

            //save values to the hidden field
            currentControl.DocumentPickerSetValueField(currentControl);
        }

        DocumentPicker.prototype.DocumentPickerSetValueField = function(currentControl)
        {
            var selectedFiles = [];
            var pickedFiles = $("#" + currentControl._selectedDocumentsContainerId).find(".cam-documentpicker-term-selected");
            for (var i = 0; i < pickedFiles.length; i++) {
                var file = pickedFiles[i];

                //build object based on data attributes of html selected items
                var doc = new CAMControl.PickedDocument();
                doc.DocumentUrl = $(file).attr("data-documenturl");
                doc.DocumentPath = $(file).attr("data-documentpath");
                doc.ListIdentifier = $(file).attr("data-listidentifier");;
                doc.ItemId = $(file).attr("data-itemid");

                selectedFiles.push(doc);
            }

            //save values in hidden field
            var hiddenControl = $('#' + currentControl.HiddenDataField);
            hiddenControl.val(JSON.stringify(selectedFiles));
        }
        
        DocumentPicker.prototype.CreateFolder = function (folderParts, currentControl) {
            //find out index from where we need to create the structure
            var existingFolderIndex = 0;
            for (var i = folderParts.length - 1; i >= 0; i--) {
                var tmpFolderString = "";
                for (var a = 0; a < i; a++) {
                    tmpFolderString += folderParts[a] + '/';
                }

                var tmpFoundFolder = $("#" + currentControl._treeContainerId).find("[data-path='" + tmpFolderString + "']");
                existingFolderIndex = i;
                if (tmpFoundFolder.length != 0) {
                    break;
                }
            }

            //get default collapsed or not
            var expandedClass = "collapsed";
            var expandedStyle = "display: none;";
            if (currentControl.ExpandFolders) {
                var expandedClass = "expanded";
                var expandedStyle = "display: block;";
            }

            var createdFolderDataId = "";
            //create folderstructure, starting from already existing index
            for (var a = existingFolderIndex; a < folderParts.length; a++) {
                
                    //get parent folder
                    var tmpParentFolderString = "";
                    for (var b = 0; b < existingFolderIndex; b++) {
                        tmpParentFolderString += folderParts[b] + "/";
                    }

                    var parent = $("#" + currentControl._treeContainerId).find("[data-path='" + tmpParentFolderString + "']");

                    if (tmpParentFolderString == "" || parent.length == 0) {
                        parent = $("#" + currentControl._rootTreeNodeId);
                    }

                    //create div in parent
                    createdFolderDataId = tmpParentFolderString + folderParts[a] + "/";

                    var newHtml = '<li class="cam-documentpicker-treenode-li">' +
                                  '  <div class="cam-documentpicker-treenode">' +
                                  '    <div class="cam-documentpicker-expander ' + expandedClass + '"></div>' + //collapsed
                                  '    <img alt="" src="' + currentControl.ImageFolder + 'folder.gif">' +
                                  '    <span class="cam-documentpicker-treenode-title">' + folderParts[a] + '</span>' + //selected
                                  '  </div>' +
                                  '  <ul data-path="' + createdFolderDataId + '" class="cam-documentpicker-treenode-ul" style="' + expandedStyle + '">' +
                                  '  </ul>' +
                                  '</li>';

                    parent.append(newHtml);

                    existingFolderIndex++;
            }

            var returnFolder = $("#" + currentControl._treeContainerId).find("[data-path='" + createdFolderDataId + "']");
            return returnFolder;
        };

        DocumentPicker.prototype.CreateStringOfArray = function (arr, length) {
            var string = "";
            for (var i = 0; i < length; i++) {
                string += arr[i] + '/';
            }
            return string;
        };

        DocumentPicker.prototype.CreateImageIconUrl = function (imageFolder, fileName) {
            var fileNameParts = fileName.split('.');
            var extension = fileNameParts[fileNameParts.length - 1].toUpperCase();

            var image = "";
            switch (extension) {
                case "PNG":
                    image = "icpng.gif";
                    break;
                case "JPG":
                    image = "icpng.gif";
                    break;
                case "JPEG":
                    image = "icpng.gif";
                    break;
                case "GIF":
                    image = "icpng.gif";
                    break;
                case "BMP":
                    image = "icpng.gif";
                    break;
                case "TXT":
                    image = "ictxt.gif";
                    break;
                case "DOC":
                    image = "icdocx.png";
                    break;
                case "DOCX":
                    image = "icdocx.png";
                    break;
                case "ACCDB":
                    image = "icaccdb.png";
                    break;
                case "MDB":
                    image = "icaccdb.png";
                    break;
                case "XLS":
                    image = "icxlsx.png";
                    break;
                case "XLSX":
                    image = "icxlsx.png";
                    break;
                case "MPP":
                    image = "icmpp.png";
                    break;
                case "ZIP":
                    image = "iczip.gif";
                    break;
                case "RAR":
                    image = "iczip.gif";
                    break;
                case "7Z":
                    image = "iczip.gif";
                    break;
                case "PPTX":
                    image = "icpptx.png";
                    break;
                case "PPT":
                    image = "icpptx.png";
                    break;
                case "PDF":
                    image = "icpdf.png";
                    break;
                case "HTM":
                    image = "ichtm.gifg";
                    break;
                case "HTML":
                    image = "ichtm.gifg";
                    break;
                case "ONE":
                    image = "icone.png";
                    break;
                default:
                    image = "icgen.gif";
            }

            return imageFolder + "/" + image;
        };

        //function that refreshed the control based on the values in the hidden field
        DocumentPicker.prototype.Refresh = function () {
            $("#" + this._selectedDocumentsContainerId).html("");

            var dataField = $("#" + this.HiddenDataField);
            if (dataField.val() != null && dataField.val().length > 0) {
                var documentArray = JSON.parse(dataField.val());

                //for each document in the hidden field
                for (var i = 0; i < documentArray.length; i++) {
                    var document = documentArray[i];
                    var folderParts = document.DocumentPath.split("/");
                    var fileName = folderParts[folderParts.length - 1];
                    
                    var imageSrc = this.CreateImageIconUrl(this.ImageFolder, fileName);

                    //build html item for selected documents div
                    var selectedItemHtml = '<div class="cam-documentpicker-treenode">' +
                                           '<img alt="" src="' + imageSrc + '">' +
                                           '<a class="cam-documentpicker-link" href="' + document.DocumentUrl + '" target="_blank">' +
                                           '<span data-documenturl="' + document.DocumentUrl + '" data-documentpath="' + document.DocumentPath + '" data-listidentifier="' + document.ListIdentifier + '" data-itemid="' + document.ItemId + '" class="cam-documentpicker-term-selected">' + fileName + '</span>' +
                                           '</a>' + 
                                           '<img class="cam-documentpicker-dialog-selected-delete" alt="" src="' + this.ImageFolder + 'Close.png">' +
                                           '</div>';

                    $("#" + this._selectedDocumentsContainerId).append(selectedItemHtml);
                }

                var currentControl = this;
                $("#" + currentControl._selectedDocumentsContainerId).find(".cam-documentpicker-dialog-selected-delete").click(function () { currentControl.DocumentPickerRemovePageDocument($(this), currentControl) });
            }
        };

        //function that sets data in the hidden field and updates the control
        DocumentPicker.prototype.SetValues = function (documentArray) {
            var hiddenControl = $("#" + this.HiddenDataField);
            hiddenControl.val(JSON.stringify(documentArray));
            this.Refresh();
        };

        //function that gets the data from the control
        DocumentPicker.prototype.GetValues = function () {
            var hiddenControl = $("#" + this.HiddenDataField);
            var documentArray = JSON.parse(hiddenControl.val());
            return documentArray;
        };

        DocumentPicker.prototype.SortPickedDocumentsByDocumentUrl = function (a, b) {
            var aName = a.DocumentUrl.toLowerCase();
            var bName = b.DocumentUrl.toLowerCase();
            return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
        }

        DocumentPicker.prototype.ProcessDocumentsLoad = function (currentControl, documents) {
            //sort the documents in correct order
            documents = documents.sort(currentControl.SortPickedDocumentsByName);
            
            //loop array
            $.each(documents, function (index, document) {
                var folderParts = document.DocumentPath.split('/');
                var fileName = folderParts[folderParts.length - 1];
                var folder = "";

                var fileParts = fileName.split(".");
                var extension = fileParts[fileParts.length - 1];

                //build folderpath of file
                if (folderParts.length > 1) {
                    folderParts.pop();
                    folder = currentControl.CreateStringOfArray(folderParts, folderParts.length);
                }

                //create folder if it does not exist
                var foundFolder = $("#" + currentControl._treeContainerId).find("[data-path='" + folder + "']");
                if (foundFolder.length == 0) {
                    foundFolder = currentControl.CreateFolder(folderParts, currentControl);
                }
            });

             //loop array
            $.each(documents, function (index, document) {
                var folderParts = document.DocumentPath.split('/');
                var fileName = folderParts[folderParts.length - 1];
                var folder = "";

                var fileParts = fileName.split(".");
                var extension = fileParts[fileParts.length - 1];

                //build folderpath of file
                if (folderParts.length > 1) {
                    folderParts.pop();
                    folder = currentControl.CreateStringOfArray(folderParts, folderParts.length);
                }

                //find folder
                var foundFolder = $("#" + currentControl._treeContainerId).find("[data-path='" + folder + "']");

                //add file
                var imageSrc = currentControl.CreateImageIconUrl(currentControl.ImageFolder, fileName);

                var newHtml = '<li class="cam-documentpicker-treenode-li">' +
                                 '  <div class="cam-documentpicker-treenode">' +
                                 '    <div class="cam-documentpicker-expander"></div>' +
                                 '    <img alt="" src="' + imageSrc + '">' +
                                 '    <span data-documenturl="' + document.DocumentUrl + '" data-documentpath="' + document.DocumentPath + '" data-listidentifier="' + document.ListIdentifier + '" data-itemid="' + document.ItemId + '" class="cam-documentpicker-treenode-title cam-documentpicker-treenode-file">' + fileName + '</span>' +
                                 '  </div>' +
                                 '</li>';
                foundFolder.append(newHtml);
            });

            //attach events to folder expand/collapse
            $("#" + currentControl._documentPickerDialogId).find('.cam-documentpicker-expander').click(function () {
                //toggle tree node
                if ($(this).hasClass('expanded')) {
                    $(this).removeClass('expanded');
                    $(this).addClass('collapsed');
                    $(this).parent().next().hide();
                }
                else if ($(this).hasClass('collapsed')) {
                    $(this).removeClass('collapsed');
                    $(this).addClass('expanded');
                    $(this).parent().next().show();
                }
            });

            //attach file selected events
            $("#" + currentControl._documentPickerDialogId).find(".cam-documentpicker-treenode-file").click(function () {
                $("#" + currentControl._documentPickerDialogId).find(".cam-documentpicker-treenode-file").removeClass('selected');
                $(this).addClass('selected');
            });

            $("#" + currentControl._rootTreeNodeId).show();
            $("#" + currentControl._documentDialogLoadingDivId).hide();
        };

        DocumentPicker.prototype.SetTranslations = function (currentControl) {
            if (typeof DocumentPickerDialogTitle != 'undefined') {
                $("#" + currentControl._dialogDocumentPickerTitleId).html(DocumentPickerDialogTitle);
            }
            if (typeof DocumentPickerSelectButtontext != 'undefined') {
                $("#" + currentControl._treeDialogSelectButtonId).html(DocumentPickerSelectButtontext);
            }
            if (typeof DocumentPickerSelectedDocumentsTitle != 'undefined') {
                $("#" + currentControl._dialogSelectedDocumentsTitleId).html(DocumentPickerSelectedDocumentsTitle);
            }
            if (typeof DocumentPickerOkButton != 'undefined') {
                $("#" + currentControl._documentPickerOkId).html(DocumentPickerOkButton);
            }
            if (typeof DocumentPickerCancelButton != 'undefined') {
                $("#" + currentControl._documentPickerCancelId).html(DocumentPickerCancelButton);
            }
            if (typeof DocumentPickerAddDocumentsImage != 'undefined') {
                $("#" + currentControl._openDocumentDialogButtonId).attr("title", DocumentPickerAddDocumentsImage)
            }
            if (typeof DocumentPickerTooMuchFilesSelectedMessage != 'undefined') {
                currentControl._maximumFilesAllowedMessage = DocumentPickerTooMuchFilesSelectedMessage.replace("{0}", currentControl.MaximumNumberOfFiles);
            }
        }

        DocumentPicker.prototype.Initialize = function (controlDivId, hiddenDataField, dataSource) {

            var currentControl = this;
            this.HiddenDataField = hiddenDataField;
            this.ControlDivId = controlDivId;
            this.DataSource = dataSource;
            this.InstanceName = controlDivId;

            //create control names based on instance name
            this._selectedDocumentsContainerId = this.InstanceName + "SelectedDocumentsContainer";
            this._openDocumentDialogButtonId = this.InstanceName + "OpenDocumentDialogButton";
            this._documentPickerDialogId = this.InstanceName + "DocumentPickerDialog";
            this._documentPickerDialogOverlayId = this.InstanceName + "DocumentPickerDialogOverlay";
            this._documentPickerCloseButtonId = this.InstanceName + "DocumentPickerCloseButton";
            this._treeContainerId = this.InstanceName + "TreeContainer";
            this._documentDialogLoadingDivId = this.InstanceName + "DocumentDialogLoadingDiv";
            this._rootTreeNodeId = this.InstanceName + "RootTreeNode";
            this._treeDialogSelectButtonId = this.InstanceName + "TreeDialogSelectButton";
            this._dialogSelectedDocumentsContainerId = this.InstanceName + "DialogSelectedDocumentsContainer";
            this._documentPickerCancelId = this.InstanceName + "DocumentPickerCancel";
            this._documentPickerOkId = this.InstanceName + "DocumentPickerOk";
            this._dialogDocumentPickerTitleId = this.InstanceName + "DocumentPickerDialogTitle";
            this._dialogSelectedDocumentsTitleId = this.InstanceName + "DocumentPickerDialogSelectedDocuments";

            //get url of current script
            var scriptUrl = "";
            var scriptRevision = "";
            $('script').each(function (i, el) {
                if (el.src.toLowerCase().indexOf('documentpickercontrol.js') > -1) {
                    scriptUrl = el.src;
                    scriptRevision = scriptUrl.substring(scriptUrl.indexOf('.js') + 3);
                    scriptUrl = scriptUrl.substring(0, scriptUrl.indexOf('.js'));
                }
            })

            // Load translation files
            var resourcesFile = scriptUrl + "_resources." + this.Language.substring(0, 2).toLowerCase() + ".js";
            if (scriptRevision.length > 0) {
                resourcesFile += scriptRevision;
            }
            $.getScript(resourcesFile, function (data, textStatus, jqxhr) {
                currentControl.SetTranslations(currentControl);
            });



            //set control html on page
            var initialPageHtml = '<div class="cam-documentpicker-selection-editor-container">' +
                                  '  <div ID="' + this._selectedDocumentsContainerId + '" class="cam-documentpicker-selection-editor">' +
                                  '  </div>' +
                                  '  <img id="' + this._openDocumentDialogButtonId + '" title="Click here to add documents" src="' + currentControl.ImageFolder + 'icgen.gif" class="cam-documentpicker-OpenDocumentDialogButton">';
                                  '</div>';

            $("#" + this.ControlDivId).html(initialPageHtml);

            //set dialog html on page
            var initialDialogPageHtml = '<div id="' + this._documentPickerDialogId + '" class="cam-documentpicker-dialog" style="display:none;">' +
                                        '  <div id="' + this._documentPickerDialogOverlayId + '" class="cam-documentpicker-dialog-overlay" style="display:none;"></div>' +
                                        '  <div class="cam-documentpicker-dialog-content">' +
                                        '    <div class="cam-documentpicker-dialog-content-header">' +
                                        '      <div id="' + this._documentPickerCloseButtonId + '" class="cam-documentpicker-dialog-content-close"></div>' +
                                        '      <h1 id="' + this._dialogDocumentPickerTitleId + '" class="cam-documentpicker-dialog-content-header-title">Please select your documents</h1>' +
                                        '    </div>' +
                                        '    <div class="cam-documentpicker-dialog-content-body">' +
                                        '      <div ID="' + this._treeContainerId + '" class="cam-documentpicker-dialog-tree-container">' +
                                        '        <div id="' + this._documentDialogLoadingDivId + '" class="cam-documentpicker-dialog-Loading" style="display:block;">' +
                                        '          <img class="cam-documentpicker-dialog-loadingimage" alt="" src="' + currentControl.ImageFolder + 'Loading.gif" />' +
                                        '          <span class="cam-documentpicker-dialog-loading-text">Loading documents</span>' +
                                        '        </div>' +
                                        '        <ul ID="' + this._rootTreeNodeId + '" class="cam-documentpicker-treenode-ul root" style="display:none;">' +
                                        '        </ul>' +
                                        '      </div>' +
                                        '      <div class="cam-documentpicker-dialog-selection-container">' +
                                        '        <button ID="' + this._treeDialogSelectButtonId + '" class="cam-documentpicker-dialog-selectbutton">Select &gt;&gt;</button>' +
                                        '        <div id="' + this._dialogSelectedDocumentsTitleId + '" class="cam-documentpicker-dialog-selected-title">Selected documents:</div>' +
                                        '        <div ID="' + this._dialogSelectedDocumentsContainerId + '" class="cam-documentpicker-dialog-selection-editor">' +
                                        '        </div>' +
                                        '      </div>' +
                                        '      <div class="cam-documentpicker-dialog-button-container">' +
                                        '        <button ID="' + this._documentPickerCancelId + '" style="float: right;">Cancel</button>' +
                                        '        <button ID="' + this._documentPickerOkId + '" style="float: right;">Ok</button>' +
                                        '      </div>' +
                                        '    </div>' +
                                        '  </div>' +
                                        '</div>';

            $("body").append(initialDialogPageHtml);

            //attach events
            $("#" + this._openDocumentDialogButtonId).click(function () { currentControl.OpenDocumentDialogButton_OnClick(currentControl) });
            $("#" + this._documentPickerCloseButtonId).click(function () { currentControl.CloseDocumentDialogButton_OnClick(currentControl) });
            $("#" + this._treeDialogSelectButtonId).click(function () { currentControl.SelectButton_OnClick(currentControl) });
            $("#" + this._documentPickerCancelId).click(function () { currentControl.CloseDocumentDialogButton_OnClick(currentControl) });
            $("#" + this._documentPickerOkId).click(function () { currentControl.DocumentPickerOk_OnClick(currentControl) });

            //get data from datasource
            this.DataSource.GetDocumentLists(this, this.ProcessDocumentsLoad);

            //set default values based on hidden field
            this.Refresh();
        };

        return DocumentPicker;
    })();
    CAMControl.DocumentPicker = DocumentPicker;

})(CAMControl || (CAMControl = {}));