var CAMControl;
(function (CAMControl) {
    var PickedDocument = (function () {

        // Constructor
        function PickedDocument() {
            this.DocumentUrl;
            this.DocumentPath;
            this.ListIdentifier;
            this.ItemId;
        }

        return PickedDocument;
    })();
    CAMControl.PickedDocument = PickedDocument;


    var DocumentPickerDataSource = (function () {

        // Constructor
        function DocumentPickerDataSource(clientContext, listIdentifiersArray, identifierType) {
            this.ClientContext = clientContext;
            this.ListIdentifiersArray = listIdentifiersArray;
            this.IdentifierType = identifierType;
            this.AllowedFileTypes;
        }

        DocumentPickerDataSource.prototype.GetFolderParentUrl = function (url, endPart) {
            var sitesIndex = url.indexOf("/sites/");
            var rootPart = url.substring(0, sitesIndex);
            var folderParentUrl = rootPart + endPart;
            return folderParentUrl();
        }

        DocumentPickerDataSource.prototype.GetDocumentLists = function (currentControl, callBack) {
            var documents = [];
            var processedLists = 0;

            var allowedFileTypesString = "";
            //set allowed file types
            if (this.AllowedFileTypes != null && this.AllowedFileTypes.length > 0) {
                for (var i = 0; i < this.AllowedFileTypes.length; i++) {
                    allowedFileTypesString += "*" + this.AllowedFileTypes[i].toUpperCase() + "*";
                }
            }

            var listIdentifier = this.ListIdentifiersArray.pop();
            var documents = [];
            //get data with recursive function. If the ListIdentifiersArray collection is looped, do the callback to the document picker control
            this.GetData(currentControl, callBack, this, allowedFileTypesString, documents, this.ListIdentifiersArray, listIdentifier);
        };

        DocumentPickerDataSource.prototype.GetData = function (currentControl, callBack, self, allowedFileTypesString, documents,listIdentifiersArray, currentListIdentifier) {
            
            var web = self.ClientContext.get_web();
            var docLibrary;
            if (self.IdentifierType == "ID") {
                docLibrary = web.get_lists().getById(currentListIdentifier);
            }
            else
            {
                docLibrary = web.get_lists().getByTitle(currentListIdentifier);
            }
            
            var items = docLibrary.getItems(SP.CamlQuery.createAllItemsQuery());
            context.load(items, "Include(EncodedAbsUrl, FileSystemObjectType, Id)");
            context.load(web);
            context.load(docLibrary);

            var docLibraryRootFolder = docLibrary.get_rootFolder();
            context.load(docLibraryRootFolder);
            var docLibraryParentFolder = docLibraryRootFolder.get_parentFolder();
            context.load(docLibraryParentFolder);

            context.executeQueryAsync(
                  function () {

                      if (items.get_count() > 0) {
                          var webUrl = web.get_url();

                          //get url of list parent
                          var url = webUrl;
                          var endPart = docLibraryParentFolder.get_serverRelativeUrl();
                          var sitesIndex = url.indexOf("/sites/");
                          var rootPart = url.substring(0, sitesIndex);
                          var folderParentUrl = rootPart + endPart;
                          var folderPathStartPosition = folderParentUrl.length;

                          //loop files
                          var e = items.getEnumerator();
                          while (e.moveNext()) {
                              var item = e.get_current();

                              var fileSystemObjectType = item.get_fileSystemObjectType();
                              if (fileSystemObjectType != 1) //if not a folder 
                              {
                                  var documentAbsoluteUrl = item.get_item("EncodedAbsUrl");
                                  var documentPath = documentAbsoluteUrl.substring(folderPathStartPosition).replace(/%20/g, ' ');;
                                  var fileParts = documentPath.split(".");
                                  var extension = fileParts[fileParts.length - 1];

                                  //if it is a allowed type (or if there are no allowed types specified)
                                  if (allowedFileTypesString == "" || allowedFileTypesString.indexOf("*" + extension.toUpperCase() + "*") > -1) {

                                      //create document
                                      var doc = new CAMControl.PickedDocument();
                                      doc.DocumentUrl = documentAbsoluteUrl;
                                      doc.DocumentPath = documentPath;
                                      doc.ListIdentifier = currentListIdentifier;
                                      doc.ItemId = item.get_id();

                                      documents.push(doc);
                                  }
                              }
                          }
                      }

                      if (listIdentifiersArray.length > 0) {
                          //get data for next list
                          var nextListIdentifier = listIdentifiersArray.pop();
                          self.GetData(currentControl, callBack, self, allowedFileTypesString, documents, listIdentifiersArray, nextListIdentifier);
                      }
                      else {
                          //do callback with data to document picker control
                          callBack(currentControl, documents);
                      }
                  },
                        function (sender, args) {
                            alert("Error while getting document data: " + args.get_message());
                        }
                  );
        };

        return DocumentPickerDataSource;
    })();
    CAMControl.DocumentPickerDataSource = DocumentPickerDataSource;

})(CAMControl || (CAMControl = {}));