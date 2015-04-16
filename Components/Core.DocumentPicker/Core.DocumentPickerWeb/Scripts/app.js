
// variable used for cross site CSOM calls
var context;

var basicDocumentPicker;
var documentPickerWithOptions;
var documentPickerWithCustomDataSource;
var defaultItem;

//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the document picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                        var appContextSite = new SP.AppContextSite(context, spHostUrl);
                       
                        //create basic documentPicker
                        CreateBasicDocumentPicker(appContextSite);
                        
                        //create documentPicker with options
                        CreateDocumentPickerWithOptions(appContextSite);
                            
                        //create documentpicker with custom (fake) datasource
                        CreateDocumentPickerWithCustomDataSource(appContextSite);
                    });

                });
        });

    //get selected values by javascript
    $("#GetValuesByJs").click(function (event) {
        event.preventDefault();

        //get values from javascript
        var documents = basicDocumentPicker.GetValues();

        var outputString = "Selected documents: \n";
        for (var i = 0; i < documents.length; i++) {
            outputString += "ItemId: " + documents[i].ItemId + "\n";
            outputString += "Path: " + documents[i].DocumentPath + "\n";
            outputString += "Url: " + documents[i].DocumentUrl + "\n";
            outputString += "---------------\n";
        }
        alert(outputString);
    });

    //set selected values by javascript
    $("#SetValuesByJs").click(function (event) {
        event.preventDefault();

        //get some values
        var docUrl = $("#defaultDocumentUrl").val();
        var docPath = $("#defaultDocumentPath").val();

        //create document array
        var documents = [];
        var doc = new CAMControl.PickedDocument();
        doc.DocumentUrl = docUrl;
        doc.DocumentPath = docPath;
        doc.ItemId = "2";
        documents.push(doc);

        basicDocumentPicker.SetValues(documents);

        //NOTE: it is also possible to add json data to the hiddenfield and call documentPicker1.Refresh
    });
});

function CreateBasicDocumentPicker(context) {
    //param1: context of the site to get lists (host or app web)
    //param2: array of library titles or ID's, to use in the control
    //param3: type of identifier passed in the above parameter (possible choises= 'TITLE' or 'ID')
    var basicDocumentPickerDatasource = new CAMControl.DocumentPickerDataSource(context, ["DocumentPickerDocLib"],"TITLE"); //pass list titles to find lists

    basicDocumentPicker = new CAMControl.DocumentPicker();
    //param1: id of div hosting this control
    //param2: id of hiddenfield to store values
    //param3: datasource to get the data (created above)
    basicDocumentPicker.Initialize("BasicDocumentPicker", "BasicDocumentPickerValue", basicDocumentPickerDatasource);
}

function CreateDocumentPickerWithOptions(context) {
    //the id of the 2 created lists that were stored in a hidden field on the page
    var documentLibrary1Id = $("#DocList1Id").val();
    var documentLibrary2Id = $("#DocList2Id").val();

    //param1: context of the site to get lists (host or app web)
    //param2: array of library titles or ID's, to use in the control
    //param3: type of identifier passed in the above parameter (possible choises= 'TITLE' or 'ID')
    var documentPickerWithOptionsDataSource = new CAMControl.DocumentPickerDataSource(context, [documentLibrary1Id, documentLibrary2Id], "ID"); //pass id's instead of titles
    documentPickerWithOptionsDataSource.AllowedFileTypes = ["docx", "xlsx"]; //only show docx and xlsx documents in picker

    documentPickerWithOptions = new CAMControl.DocumentPicker();
    documentPickerWithOptions.MaximumNumberOfFiles = 1; //only allow 1 document to be selected
    documentPickerWithOptions.ExpandFolders = false; //show the folders collapsed when dialog is opened
    documentPickerWithOptions.Language = "en-us"; //to translate the text in the control. If we change this to nl-be the "documentpickercontrol_resources.nl" resource file will be loaded. 
    //You can create a resource file in your own language if you want
    documentPickerWithOptions.ImageFolder = "../Styles/documentpicker/images/"; //by default the images for the control are stored in this folder. But you can change it.
    //param1: id of div hosting this control
    //param2: id of hiddenfield to store values
    //param3: datasource to get the data (created above)
    documentPickerWithOptions.Initialize("DocumentPickerWithOptions", "DocumentPickerWithOptionsValue", documentPickerWithOptionsDataSource);
}

function CreateDocumentPickerWithCustomDataSource(context)
{
    var customDataSource = new CAMControl.MyCustomDocumentPickerDataSource(context);
    documentPickerWithCustomDataSource = new CAMControl.DocumentPicker();
    //param1: id of div hosting this control
    //param2: id of hiddenfield to store values
    //param3: datasource to get the CUSTOM data (created above)
    documentPickerWithCustomDataSource.Initialize("DocumentPickerWithCustomDataSource", "DocumentPickerWithCustomDataSourceValue", customDataSource);

}

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}
