//javascript file you can create yourself. With this class you can write custom code to get data for the document picker

var CAMControl;
(function (CAMControl) {
   
    var MyCustomDocumentPickerDataSource = (function () {

        // Constructor
        function MyCustomDocumentPickerDataSource(clientContext) {
            this.ClientContext = clientContext;
        }

        //this method is required. The documentpicker will call it.
        //currenControl is the document picker control
        //callback is the processdocument method in the document picker
        //in this method you can write custom code to get data
        //when you have the data, call the callback function, and the documentpicker will render
        MyCustomDocumentPickerDataSource.prototype.GetDocumentLists = function (currentControl, callBack) {
            var documents = [];
            
            //build document array
            //here we use fake documents to show the concept
            //in real life, here you can write logic to get data from different subsites, do a ajax call to the server to get data by C# ...
            var doc = new CAMControl.PickedDocument();
            doc.DocumentUrl = "http://somesharepointsite.sharepoint.com/testsite/SomeList/SomeFolder/worddocument.docx";
            doc.DocumentPath = "SomeList/SomeFolder/worddocument.docx";
            doc.ListName = "SomeList";
            doc.ItemId = "1";
            documents.push(doc);

            var doc = new CAMControl.PickedDocument();
            doc.DocumentUrl = "http://somesharepointsite.sharepoint.com/testsite/SomeList/SomeFolder/exceldocument.xlsx";
            doc.DocumentPath = "SomeList/SomeFolder/exceldocument.xlsx";
            doc.ListName = "SomeList";
            doc.ItemId = "1";
            documents.push(doc);

            //do callback with the constructed data back to the document picker control
            callBack(currentControl, documents);
        };

        return MyCustomDocumentPickerDataSource;
    })();
    CAMControl.MyCustomDocumentPickerDataSource = MyCustomDocumentPickerDataSource;

})(CAMControl || (CAMControl = {}));