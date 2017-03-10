# Document picker for provider hosted apps #

### Summary ###
This control is used to browse documents that are stored in document libaries in SharePoint. It can show multiple document libraries in 1 tree structure. If you click on one of the selected documents, it will open. It is possible to configure filters on document extensions and specify the number of selected files. There is support for a custom datasource that you can implement yourself to get data from sources that the control itself does not support.

### Prerequisites ###
It's important that the provider hosted add-in that's running the document picker is using the same IE security zone as the SharePoint site it's installed on. If you get "Sorry we had trouble accessing your site" errors then please check this.

### Solution ###
Solution | Author(s)
---------|----------
Core.DocumentPicker | Stijn Neirinckx 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW DOES THE DOCUMENT PICKER WORK? #
The document picker control makes it possible to browse one or more document libraries and select documents. An examle would be a provider hosted application in which create PDF's from existing documents: you could then use this document picker control to select the needed documents. Below picture shows the control on the page:

![Document picker UI](http://i.imgur.com/OmGyuNE.png)

One you click on the *document* icon right to the control a dialog opens in which you select documents:

![Selection of documents from library](http://i.imgur.com/gvVLyJj.png)

# HOW TO USE THE DOCUMENT PICKER IN YOUR PROVIDER HOSTED SP ADD-IN? #
Below you can find the steps needed to get the control working

## ENSURE YOU TRIGGER THE CREATION OF AN ADD-In WEB ##
When you build a provider hosted add-in it does not necessarily have an add-in web associated with it whereas a SharePoint hosted add-in always has an add-in web. Since the document picker control uses the CSOM object model from JavaScript it’s required to have an add-in web. To ensure you have an add-in web you can just add a empty element to your SharePoint add-in as shown below:

![Visual Studio project UI with an arrow pointing to element.xml](http://i.imgur.com/DYnXn5E.png)

## SETTING UP THE HTML ##
Add refences to javascript and css files to make the control work. The app.js file will be used to write our app specific code.

```ASPX
<link href="../Styles/documentpicker/documentpickercontrol.css" rel="stylesheet" />
<script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
<script type="text/javascript" src="../Scripts/MyCustomDocumentPickerDataSource.js"></script>
<script type="text/javascript" src="../Scripts/documentpickerdatasource.js"></script>
<script type="text/javascript" src="../Scripts/documentpickercontrol.js"></script>
<script type="text/javascript" src="../Scripts/app.js"></script>
```

We also need to add html on the page where we want the control to render. The control will be rendered inside the div. The heigth and width styling on the div determines the size of the control. The hiddenfield is used to get or set selected documents from the server. 

```ASPX
<div id="BasicDocumentPicker" style="width:350px;height:100px;float:right"></div>  
<asp:HiddenField runat="server" ID="BasicDocumentPickerValue" />
```

## CREATE THE CLIENTCONTEXT OBJECT ##
Below code shows how to load the relevant SP js files and how to create the cliencontext object. The clientcontext object is created is such a way (see the ProxyWebRequestExecutorFactory that's being hooked up) that it can be used in cross domain scenarios which will be the case when you’re integrating your provider hosted add-in via a dialog in SharePoint. The appContextSite is used to communicate to the hostweb (if your lists are located there). You need to add this code to your app.js file.

```JavaScript
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
                    //Load the SP.UI.Controls.js file to render the add-in Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the document picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                        var appContextSite = new SP.AppContextSite(context, spHostUrl);
                    });

                });
        });
});
```

## TRANSFORM THE HTML INTO A DOCUMENTPICKER CONTROL ##
The final step is to transform the HTML inserted in the previous step into a document picker control. This is done by creating an instance of the documentpicker JavaScript class and providing it a reference to the HTML elements. The control gets data using a datasource. With the default datasource you can get data using a array of list titles or list id's.

```JavaScript
//param1: context of the site to get lists (host or add-in web)
//param2: array of library titles or ID's, to use in the control
//param3: type of identifier passed in the above parameter (possible choises= 'TITLE' or 'ID')
var basicDocumentPickerDatasource = new CAMControl.DocumentPickerDataSource(context, ["DocumentPickerDocLib"],"TITLE"); //pass list titles to find lists

basicDocumentPicker = new CAMControl.DocumentPicker();
//param1: id of div hosting this control
//param2: id of hiddenfield to store values
//param3: datasource to get the data (created above)
basicDocumentPicker.Initialize("BasicDocumentPicker", "BasicDocumentPickerValue", basicDocumentPickerDatasource);
```

# DOCUMENTPICKER CONFIGURATION OPTIONS #
The document picker control does have some configuration options which are explained below.

## LANGUAGE ##
The strings displayed by the control will be loaded dynamically based on the passed language. This requires you to pass the language via taking over the SPLanguage url parameter (see sample) or by hardcoding it. If no language is passed the control assumes the language is English. 

```JavaScript
documentPickerWithOptions.Language = "en-us";
```

If you would like to add additional languages you need to create the appropriate JavaScript language resource files:

![Resource JS list](http://i.imgur.com/umTeI0h.png)

Such a resource file is simple collection of global variables:

![English variables](http://i.imgur.com/rLG8HbO.png)

![Dutch variables](http://i.imgur.com/SVQmC4f.png)

## SHOW MULTIPLE DOCUMENT LIBRARIES IN ONE TREE ##
The control is able to show multiple document libraries in the same treeview. Just pass a array of the list titles or id's to the documentpickerdatasource. Be sure to specify in the last parameter that you are passing ID's or Titles.

```JavaScript

//param1: context of the site to get lists (host or add-in web)
//param2: array of library titles or ID's, to use in the control
//param3: type of identifier passed in the above parameter (possible choises= 'TITLE' or 'ID')
var documentPickerWithOptionsDataSource = new CAMControl.DocumentPickerDataSource(context, ["DocumentPickerDocLib", "DocumentPickerDocLibExtra"], "TITLE"); //pass id's instead of titles
```

## MAXIMUM NUMBER OF FILES ##
This property sets the maximum number of files that can be selected in the control.

```JavaScript
documentPickerWithOptions.MaximumNumberOfFiles = 2; //only allow 2 documents to be selected
```

## FILTER ON DOCUMENT EXTENSION ##
If you set this property, only the documents matching that extension are shown in the control. This property is set on the datasource.

```JavaScript
documentPickerWithOptionsDataSource.AllowedFileTypes = ["docx", "xlsx"]; //only show docx and xlsx documents in picker
```

## EXPAND FOLDERS ##
If this setting is set to true, the folders will be expanded by default. If set to false, they will be collapsed.

```JavaScript
documentPickerWithOptions.ExpandFolders = false; //show the folders collapsed when dialog is opened
```

<img  src="https://telemetry.sharepointpnp.com/pnp/components/Core.DocumentPicker" />