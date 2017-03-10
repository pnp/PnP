# Client Side rendering #

### Summary ###
This sample shows how to customize a field type with Client-Side Rendering (also called CSR, JS Link) technology in SharePoint 2013.

***Notice**. Techniques shown in this sample do require full permission to web or site collection level, so this is not a suitable model for apps designed to be distributed from the SharePoint store.*

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Branding.ClientSideRendering | Leo Qian, Tyler Lu, Cindy Yan, Todd Baginski (**Canviz LLC**), Andrei Markeev

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1 | October 4th 2014 | Made samples MDS compliant
1.0  | June 20th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: CLIENT SIDE RENDING SAMPLES #
Client Side Rendering is a new concept in SharePoint 2013. It’s provides you with a mechanism that allows you to render your own output for a set of controls that are hosted in a SharePoint page (list views, display, add and edit forms). This mechanism enables you to use well-known technologies, such as HTML and JavaScript, to define the rendering logic of custom and predefined field types.

JSLink files have the ability to quickly and easily change how a list’s views and forms are rendered. More specifically, how the fields in that list should be displayed.

The Client Side Rendering JS files in this sample were taken from an MSDN code sample by Muawiyah Shannak.  They have been included in this code sample to demonstrate how the remote provisioning pattern may be used to deploy Client Side Rendering components and associated them with views and forms in a SharePoint list.

# CONFIGURATION & DEPLOYMENT #
In order for the Client Side Rendering sample to render correctly, you must first create your SharePoint site collection.

## SHAREPOINT ##
1)  Navigate to your SharePoint tenancy and create a new site collection using the Developer Site template in the Collaboration tab.


![Creation of dev site collection](http://i.imgur.com/MDvG8Vr.png)

2)  Once the site collection is created, open the Branding.ClientSideRendering.sln file with Visual Studio 2013.

3)  If the Branding.ClientSideRendering project isn’t the StartUp project, set it as the StartUp project.

![Setting start up project in Visual Studio](http://i.imgur.com/b6whnvz.png)

4)  In the Solution Explorer, select the Branding.ClientSideRendering project.

5)  In the Properties window, set the “Site URL” property to the site collection you previously created and configured.

![Setting site URL accordingly](http://i.imgur.com/jrxn4Zv.png)

6)  Press F5 or click the Start button in Visual Studio 2013.

7)  Enter you user name and password to connect to your SharePoint site collection. 

![Signing in to your tenant](http://i.imgur.com/pzygqkL.png)

8)  After your username and password have been verified, the trust dialog is displayed. Click the “Trust It” button. 

![Trust add-in](http://i.imgur.com/p9fQUTp.png)

9)  After add-in installation, a new page will be displayed.  Click the Provision Samples button to create the list columns, lists, list views, initialize the lists with data, and upload the Client Side Rendering JavaScript and image files that support the samples.  The provisioning code registers the Client Side Rendering JavaScript files with the list forms and views via the JSLink property.

![Client Side Rendering -- JSlink page, with button for Provision Samples](http://i.imgur.com/LGC32Fc.png)

10)  After you have successfully configured your SharePoint environment and deployed the artifacts via the add-in, you can view the Client Side Rendering samples by clicking on the links in the add-in.

# DEPLOYMENT DETAILS #
The code behind in the default.aspx.cs file contains all the code used to deploy the artifacts which support this sample.  This code sample uses the remote provisioning pattern to deploy the artifacts.  The remote provisioning pattern uses the SharePoint Client Side Object Model to deploy the artifacts.  There are many other Office AMS samples which demonstrate this same approach.

## UPLOADING THE CLIENT SIDE RENDERING JAVASCRIPT FILES ##
When the Provision Sample sbutton is clicked the UploadJSFiles method is called.  The UploadJSFiles method creates a folder named JSLink-Samples in the Style Library to store the Client Side Rendering JavaScript files, then it uploads the Client Side Rendering JavaScript files.  Then, it creates a sub folder named imgs and uploads an image to the folder.  The image is used by one of the samples.

```C#
void UploadJSFiles(Web web)
{
    //Delete the folder if it exists
    Microsoft.SharePoint.Client.List list = web.Lists.GetByTitle("Style Library");
    IEnumerable<Folder> results = web.Context.LoadQuery<Folder>(list.RootFolder.Folders.Where(folder => folder.Name == "JSLink-Samples"));
    web.Context.ExecuteQuery();
    Folder samplesJSfolder = results.FirstOrDefault();

    if (samplesJSfolder != null)
    {
        samplesJSfolder.DeleteObject();
        web.Context.ExecuteQuery();
    }

    samplesJSfolder = list.RootFolder.Folders.Add("JSLink-Samples");
    web.Context.Load(samplesJSfolder);
    web.Context.ExecuteQuery();

    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/Accordion.js"), 	samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/ConfidentialDocuments.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/DisableInput.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/HiddenField.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/HTML5NumberInput.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/PercentComplete.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/PriorityColor.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/ReadOnlySPControls.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/RegexValidator.js"), samplesJSfolder);
    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/SubstringLongText.js"), samplesJSfolder);

    Folder imgsFolder = samplesJSfolder.Folders.Add("imgs");
    web.Context.Load(imgsFolder);
    web.Context.ExecuteQuery();

    UploadFileToFolder(web, Server.MapPath("../Scripts/JSLink-Samples/imgs/Confidential.png"), imgsFolder);
}
```

## CREATING LISTS, VIEWS, ITEMS, AND REGISTERING CLIENT SIDE RENDERING JAVASCRIPT FILES WITH LIST VIEWS AND FORMS ##
After the Client Side Rendering JavaScript files and image are uploaded, the add-in provisions the list columns, lists, list views, initializes the lists with data, and registers the Client Side Rendering JavaScript files with the list views and forms via the JSLink property.

Not every sample demonstrates how to register the Client Side Rendering JavaScript files with list views and forms.  However, Sample 5 demonstrates everything.  The ProvisionSample5 method deploys Sample 5.  It illustrates all of the patterns.

```C#
void ProvisionSample5(Web web)
{
    //Delete list if it already exists
    ListCollection lists = web.Lists;
    IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == "CSR-Tasks-Percent-Complete"));
    web.Context.ExecuteQuery();
    List existingList = results.FirstOrDefault();

    if (existingList != null)
    {
        existingList.DeleteObject();
        web.Context.ExecuteQuery();
    }

    //Create list
    ListCreationInformation creationInfo = new ListCreationInformation();
    creationInfo.Title = "CSR-Tasks-Percent-Complete";
    creationInfo.TemplateType = (int)ListTemplateType.Tasks;
    List newlist = web.Lists.Add(creationInfo);

    newlist.Update();
    web.Context.Load(newlist);
    web.Context.ExecuteQuery();

    //Add items
    Microsoft.SharePoint.Client.ListItem item1 = newlist.AddItem(new ListItemCreationInformation());
    item1["Title"] = "Task 1";
    item1["StartDate"] = "2014-1-1";
    item1["DueDate"] = "2014-2-1";
    item1["PercentComplete"] = "0.59";
    item1.Update();

    Microsoft.SharePoint.Client.ListItem item2 = newlist.AddItem(new ListItemCreationInformation());
    item2["Title"] = "Task 2";
    item2["StartDate"] = "2014-1-1";
    item2["DueDate"] = "2014-2-1";
    item2["PercentComplete"] = "0.40";
    item2.Update();

    Microsoft.SharePoint.Client.ListItem item3 = newlist.AddItem(new ListItemCreationInformation());
    item3["Title"] = "Task 3";
    item3["StartDate"] = "2014-1-1";
    item3["DueDate"] = "2014-2-1";
    item3["PercentComplete"] = "1.0";
    item3.Update();

    Microsoft.SharePoint.Client.ListItem item4 = newlist.AddItem(new ListItemCreationInformation());
    item4["Title"] = "Task 4";
    item4["StartDate"] = "2014-1-1";
    item4["DueDate"] = "2014-2-1";
    item4["PercentComplete"] = "0.26";
    item4.Update();

    Microsoft.SharePoint.Client.ListItem item5 = newlist.AddItem(new ListItemCreationInformation());
    item5["Title"] = "Task 5";
    item5["StartDate"] = "2014-1-1";
    item5["DueDate"] = "2014-2-1";
    item5["PercentComplete"] = "0.50";
    item5.Update();

    //Create sample view
    ViewCreationInformation sampleViewCreateInfo = new ViewCreationInformation();
    sampleViewCreateInfo.Title = "CSR Sample View";
    sampleViewCreateInfo.ViewFields = new string[] { "DocIcon", "LinkTitle", "DueDate", "AssignedTo", "PercentComplete" };
    sampleViewCreateInfo.SetAsDefaultView = true;
    Microsoft.SharePoint.Client.View sampleView = newlist.Views.Add(sampleViewCreateInfo);
    sampleView.Update();
    web.Context.Load(newlist, l => l.DefaultViewUrl,
        l => l.DefaultDisplayFormUrl,
        l => l.DefaultEditFormUrl,
        l => l.DefaultNewFormUrl);
    web.Context.ExecuteQuery();

    //Register JS files via JSLink properties
    RegisterJStoWebPart(web, newlist.DefaultViewUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
    RegisterJStoWebPart(web, newlist.DefaultDisplayFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
    RegisterJStoWebPart(web, newlist.DefaultEditFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
    RegisterJStoWebPart(web, newlist.DefaultNewFormUrl, "~sitecollection/Style Library/JSLink-Samples/PercentComplete.js");
}
```

# CLIENT SIDE RENDERING JAVASCRIPT DETAILS #
The following section describes the Client Side Rendering samples and the JavaScript code used to implement them.

## Minimal Download Strategy (MDS) ##
The below JavaScript templates will alter the on screen rendering. Given that SharePoint uses MDS to cache rendered HTML fragments it's important that the MDS engine is aware of the custom rendering we're about to use. To let SharePoint know the custom JavaScript files have to be registered with the MDS engine as is shown below:

```JavaScript
if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterFilenameFiledContext();
}

function RegisterInMDS() {
    // RegisterFilenameFiledContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/ConfidentialDocuments.js", RegisterFilenameFiledContext);
    //RegisterFilenameFiledContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterFilenameFiledContext();
}
```

## SAMPLE 1 – TASK PRIORITY COLOR ##
This sample demonstrates how to apply formatting to a list column based on the column value.  In this sample, list items which have a Priority column value of (1) High are indicated in red, items which have a Priority column value of (2) Normal are indicated in orange, and items which have a Priority column value of (3) Low are indicated in yellow.

![Colored column values](http://i.imgur.com/p7v03KF.png)

The following code illustrates how the Priority column is formatted based on the column’s value.

```JavaScript
function RegisterPriorityFiledContext () {

    // Create object that has the context information about the field that we want to render differently
    var priorityFiledContext = {};
    priorityFiledContext.Templates = {};
    priorityFiledContext.Templates.Fields = {
        // Apply the new rendering for Priority field in List View
        "Priority": { "View": priorityFiledTemplate }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(priorityFiledContext);

}

// This function provides the rendering logic for list view
function priorityFiledTemplate(ctx) {

    var priority = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];

    // Return html element with appropriate color based on the Priority column’s value
    switch (priority) {
        case "(1) High":
            return "<span style='color :#f00'>" + priority + "</span>";
            break;
        case "(2) Normal":
            return "<span style='color :#ff6a00'>" + priority + "</span>";
            break;
        case "(3) Low":
            return "<span style='color :#cab023'>" + priority + "</span>";
    }
}
```

## SAMPLE 2 – SUBSTRING LONG TEXT ##
This sample demonstrates how to make the Announcements list’s Body column text display shorter in the All Items view than it does out of the box.  It also demonstrates how to display the entire Body column’s text as a tooltip for each list item.  In the screenshot below you can see the Body column has been truncated so it only displays a subset of the text in the Body column. You can also see the tooltip which displays the entire Body column text for the first announcement in the list.  This tooltip appears when you mouse over items in the list.

![Substring presentation of string field value](http://i.imgur.com/rOrEg8C.png)

The following code illustrates how the Body column is truncated and how the tooltip is added via the title attribute.

```JavaScript
function RegisterBodyFiledContext() {

    // Create object that has the context information about the field that we want to render differently 
    var bodyFiledContext = {};
    bodyFiledContext.Templates = {};
    bodyFiledContext.Templates.Fields = {
        // Apply the new rendering for Body field in list view
        "Body": { "View": bodyFiledTemplate }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(bodyFiledContext);

}

// This function provides the rendering logic
function bodyFiledTemplate(ctx) {

    var bodyValue = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];

    //This regex expression is used to delete html tags from the Body field
    var regex = /(<([^>]+)>)/ig;

    bodyValue = bodyValue.replace(regex, "");

    var newBodyValue = bodyValue;

    if (bodyValue && bodyValue.length >= 100)
    {
        newBodyValue = bodyValue.substring(0, 100) + " ...";
    }

    return "<span title='" + bodyValue + "'>" + newBodyValue + "</span>";
       
}
```

## SAMPLE 3 – CONFIDENTIAL DOCUMENTS ##
This sample demonstrates how to display an image next to a document’s Name in a document library based on a field value associated with the document.  In the screenshot below you can see the red badge image displayed next to each document whose Confidential column value is Yes.

![Additional icon presentation](http://i.imgur.com/j81TpVS.png)

The following code illustrates how the Confidential column is evaluated to see if the document is marked as confidential.  It also demonstrates how the Title column is modified to display the red badge image accordingly.

```JavaScript
function RegisterFilenameFiledContext() {

    // Create object that has the context information about the field that we want to render differently
    var linkFilenameFiledContext = {};
    linkFilenameFiledContext.Templates = {};
    linkFilenameFiledContext.Templates.Fields = {
        // Apply the new rendering for LinkFilename field in list view
        "LinkFilename": { "View": linkFilenameFiledTemplate }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(linkFilenameFiledContext);
}

// This function provides the rendering logic
function linkFilenameFiledTemplate(ctx) {

    var confidential = ctx.CurrentItem["Confidential"];
    var title = ctx.CurrentItem["FileLeafRef"];

    // This Regex expression use to delete extension (.docx, .pdf ...) form the file name
    title = title.replace(/\.[^/.]+$/, "")

    // Check confidential field value
    if (confidential && confidential.toLowerCase() == 'yes') {
        // Render HTML that contains the file name and the confidential icon
        return title + "&nbsp;<img src= '" + _spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/imgs/Confidential.png' alt='Confidential Document' title='Confidential Document'/>";
    }
    else
    {
        return title;
    }
}
```

## SAMPLE 4 – PERCENT COMPLETE ##
This sample demonstrates how to display a bar chart in the % Complete column in a Task list.  In the screenshots below you can see the blue bar charts which indicate the percent each task is complete based on the value in the % Complete column.

This is how the column appears in the list view. (View)

![View mode of percent presentation in list](http://i.imgur.com/CAlaZ4H.png)

This is how the column appears when viewing a list item. (DisplayForm)

![View mode of percent presentation in item](http://i.imgur.com/80N3WG3.png)

This is how the column appears when editing a list item. (EditForm)  The red arrows indicate how you can slide the black box to select the value and the tool tip which appears over the currently selected value.

![Edit experience with custom JS editor](http://i.imgur.com/PG5WWJy.png)

This is how the column appears when creating a new list item. (NewForm)  The red arrows indicate how you can slide the black box to select the value and the tool tip which appears over the currently selected value.

![New form editor](http://i.imgur.com/9SblMgn.png)

The following code illustrates how the bar charts are created in the % Complete column and registered with the View and DisplayForms.  It also demonstrates how the input controls for the % Complete column and created and registered with the New and Edit forms.

```JavaScript
function RegisterPercentCompleteFiledContext () {

    // Create object that has the context information about the field that we want to render differently
    var percentCompleteFiledContext = {};
    percentCompleteFiledContext.Templates = {};
    percentCompleteFiledContext.Templates.Fields = {
        // Apply the new rendering for PercentComplete field in List View, Display, New and Edit forms
        "PercentComplete": { 
            "View": percentCompleteViewFiledTemplate,
            "DisplayForm": percentCompleteViewFiledTemplate,
            "NewForm": percentCompleteEditFiledTemplate,
            "EditForm": percentCompleteEditFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(percentCompleteFiledContext);

}

// This function provides the rendering logic for View and Display forms
function percentCompleteViewFiledTemplate(ctx) {

    var percentComplete = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    return "<div style='background-color: #e5e5e5; width: 100px;  display:inline-block;'> \
            <div style='width: " + percentComplete.replace(/\s+/g, '') + "; background-color: #0094ff;'> \
            &nbsp;</div></div>&nbsp;" + percentComplete;

}

// This function provides the rendering logic for New and Edit forms
function percentCompleteEditFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // Register a callback just before submit.
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return document.getElementById('inpPercentComplete').value;
    });

    return "<input type='range' id='inpPercentComplete' name='inpPercentComplete' min='0' max='100' \
            oninput='outPercentComplete.value=inpPercentComplete.value' value='" + formCtx.fieldValue + "' /> \
            <output name='outPercentComplete' for='inpPercentComplete' >" + formCtx.fieldValue + "</output>%";
}
```

## SAMPLE 5 – ACCORDION ##
This sample demonstrates how to change the rendering template for an entire list view.  In the screenshot below you can see list items when they are collapsed and how they appear once expanded.  Clicking on a list item expands and collapses it.

![Accordion presentation of field value](http://i.imgur.com/Joqqub3.png)

The following code illustrates how the accordion functionality is implemented and registered with the list Template.  The header, footer, and item properties are set to define the overall layout of the items in the list.  The onPostRender property registers the JavaScript function to execute when the list is rendered.  This function hooks up the click events and the CSS code necessary to implement the expand and collapse functionality.

```JavaScript
function RegisterAccordionContext() {

    // jQuery library is required in this sample
    // Fallback to loading jQuery from a CDN path if the local is unavailable
    (window.jQuery || document.write('<script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.0.min.js"><\/script>'));

    // Create object that has the context information about the field that we want to render differently 
    var accordionContext = {};
    accordionContext.Templates = {};

    // Be careful when adding the header for the template, because it will break the default list view render
    accordionContext.Templates.Header = "<div class='accordion'>";
    accordionContext.Templates.Footer = "</div>";

    // Add OnPostRender event handler to add accordion click events and style
    accordionContext.OnPostRender = accordionOnPostRender;

    // This line of code tells the TemplateManager that we want to change all the HTML for item row rendering
    accordionContext.Templates.Item = accordionTemplate;

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(accordionContext);

}

// This function provides the rendering logic
function accordionTemplate(ctx) {
    var title = ctx.CurrentItem["Title"];
    var description = ctx.CurrentItem["Description"];

    // Return whole item html
    return "<h2>" + title + "</h2><p>" + description + "</p><br/>";
}

function accordionOnPostRender() {

    // Register event to collapse and expand when clicking on accordion header
    $('.accordion h2').click(function () {
        $(this).next().slideToggle();
    }).next().hide();

    $('.accordion h2').css('cursor', 'pointer');
}
```

## SAMPLE 6 – EMAIL REGEX VALIDATOR ##
This sample demonstrates how to use regular expressions to validate column input values.  In the screenshot below notice a red error message appears when an invalid email address is entered into the Email column input textbox.

This is how the column appears when editing a list item (EditForm) or creating a new list item (NewForm).  Notice a red error message appears when an invalid email address is entered into the Email column input textbox.

![Validation entry when email is not valid](http://i.imgur.com/AHlTAsg.png)

The following code illustrates how the email validation for the Email column is implemented and registered with the New and Edit forms.  The registerGetCallback function registers a call back function the form will fire before submittal, in this scenario the callback function returns the value in the input control for the Email column.  The code also registers a validator with the form (emailValidator) as well as the callback method to handle validation errors.  The validator function uses a regular expression to validate the email format.  If the email format is invalid it returns the error message.  The HTML in the field template contains a placeholder <span id='spnError' class='ms-formvalidation ms-csrformvalidation'></span> to display the error message.


```JavaScript
function RegisterEmailFiledContext() {

    // Create object that has the context information about the field that we want to render differently
    var emailFiledContext = {};
    emailFiledContext.Templates = {};
    emailFiledContext.Templates.Fields = {
        // Apply the new rendering for Email field on New and Edit Forms
        "Email": {
            "NewForm": emailFiledTemplate,
            "EditForm": emailFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(emailFiledContext);

}

// This function provides the rendering logic
function emailFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // Register a callback just before submit.
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return document.getElementById('inpEmail').value;
    });

    //Create container for various validations
    var validators = new SPClientForms.ClientValidation.ValidatorSet();
    validators.RegisterValidator(new emailValidator());

    // Validation failure handler.
    formCtx.registerValidationErrorCallback(formCtx.fieldName, emailOnError);

    formCtx.registerClientValidator(formCtx.fieldName, validators);

    return "<span dir='none'><input type='text' value='" + formCtx.fieldValue + "'  maxlength='255' id='inpEmail' class='ms-long'> \
            <br><span id='spnError' class='ms-formvalidation ms-csrformvalidation'></span></span>";
}

// Custom validation object to validate email format
emailValidator = function () {
    emailValidator.prototype.Validate = function (value) {
        var isError = false;
        var errorMessage = "";

        if (!validateEmail(value)) {
            isError = true;
            errorMessage = "Invalid email address";
        }

        //Send error message to error callback function (emailOnError)
        return new SPClientForms.ClientValidation.ValidationResult(isError, errorMessage);
    };
};

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}
// Add error message to spnError element under the input field element
function emailOnError(error) {
    document.getElementById("spnError").innerHTML = "<span role='alert'>" + error.errorMessage + "</span>";
}
```

## SAMPLE 7 – READ ONLY SP CONTROLS ##
This sample demonstrates how to make some fields read only in SharePoint list item Edit forms.

This is how the form appears when editing a list item (EditForm).  The red arrows indicate the read only columns in the form.

![Read-only presentation](http://i.imgur.com/wc1wBcd.png)

The following code illustrates how the Title, AssignedTo, and Priority columns have their field templates modified to display just the field value instead of the out of the box input controls.  Notice how different types of parsing must occur to extract a field’s value and display it depending on what type of field it is.

```JavaScript
function RegisterReadonlyFiledContext () {

    // Create object that has the context information about the field that we want to render differently
    var readonlyFiledContext = {};
    readonlyFiledContext.Templates = {};
    readonlyFiledContext.Templates.Fields = {
        // Apply the new rendering for Title, AssignedTo, and Priority fields on Edit forms
        "Title": {
            "EditForm": readonlyFieldTemplate
        },
        "AssignedTo": {
            "EditForm": readonlyFieldTemplate
        },
        "Priority": {
            "EditForm": readonlyFieldTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(readonlyFiledContext);

}

// This function provides the rendering logic
function readonlyFieldTemplate(ctx) {

    //Reuse SharePoint JavaScript libraries
    switch (ctx.CurrentFieldSchema.FieldType) {
        case "Text":
        case "Number":
        case "Integer":
        case "Currency":
        case "Choice":
        case "Computed":
            return SPField_FormDisplay_Default(ctx);

        case "MultiChoice":
            prepareMultiChoiceFieldValue(ctx);
            return SPField_FormDisplay_Default(ctx);

        case "Boolean":
            return SPField_FormDisplay_DefaultNoEncode(ctx);

        case "Note":
            prepareNoteFieldValue(ctx);
            return SPFieldNote_Display(ctx);

        case "File":
            return SPFieldFile_Display(ctx);

        case "Lookup":
        case "LookupMulti":
                return SPFieldLookup_Display(ctx);           

        case "URL":
            return RenderFieldValueDefault(ctx);

        case "User":
            prepareUserFieldValue(ctx);
            return SPFieldUser_Display(ctx);

        case "UserMulti":
            prepareUserFieldValue(ctx);
            return SPFieldUserMulti_Display(ctx);

        case "DateTime":
            return SPFieldDateTime_Display(ctx);

        case "Attachments":
            return SPFieldAttachments_Default(ctx);

        case "TaxonomyFieldType":
            //Re-use JavaScript from the sp.ui.taxonomy.js SharePoint JavaScript library
            return SP.UI.Taxonomy.TaxonomyFieldTemplate.renderDisplayControl(ctx);
    }
}

//User control need specific formatted value to render content correctly
function prepareUserFieldValue(ctx) {
    var item = ctx['CurrentItem'];
    var userField = item[ctx.CurrentFieldSchema.Name];
    var fieldValue = "";

    for (var i = 0; i < userField.length; i++) {
        fieldValue += userField[i].EntityData.SPUserID + SPClientTemplates.Utility.UserLookupDelimitString + userField[i].DisplayText;

        if ((i + 1) != userField.length) {
            fieldValue += SPClientTemplates.Utility.UserLookupDelimitString
        }
    }

    ctx["CurrentFieldValue"] = fieldValue;
}

//Choice control need specific formatted value to render content correctly
function prepareMultiChoiceFieldValue(ctx) {

    if (ctx["CurrentFieldValue"]) {
        var fieldValue = ctx["CurrentFieldValue"];

        var find = ';#';
        var regExpObj = new RegExp(find, 'g');

        fieldValue = fieldValue.replace(regExpObj, '; ');
        fieldValue = fieldValue.replace(/^; /g, '');
        fieldValue = fieldValue.replace(/; $/g, '');

        ctx["CurrentFieldValue"] = fieldValue;
    }
}

//Note control need specific formatted value to render content correctly
function prepareNoteFieldValue(ctx) {

    if (ctx["CurrentFieldValue"]) {
        var fieldValue = ctx["CurrentFieldValue"];
        fieldValue = "<div>" + fieldValue.replace(/\n/g, '<br />'); + "</div>";

        ctx["CurrentFieldValue"] = fieldValue;
    }
} 
```

## SAMPLE 8 – HIDDEN FIELD ##
This sample demonstrates how to make some fields hidden in SharePoint list item New and Edit forms.
This is screenshots below indicate how the form appears when editing a list item (EditForm).  The view on the right is the out of the box Edit form for the Tasks list, it includes the Predecessors column.  The view on the left is the customized Edit form for the Tasks list, it hides the Predecessors column.

![Hiding field in edit form](http://i.imgur.com/ltiFSjc.png)

When the sample is deployed it shows the customized Edit form.  To see the default Edit form follow these steps:

1) Navigate to the **CSR-Hide-Controls list**.

2) In the Ribbon, click the **LIST tab**.

3) In the Ribbon, click **Form Web Parts** and select **Default Edit Form**.

![Ribbon button for Default Edit Form](http://i.imgur.com/4xeNW0q.png)

4) In the Ribbon, click **Web Part Properties**.

![Ribbon button for web part properties](http://i.imgur.com/WHMA5DC.png)

5) In the CSR-Hide-Controls Web Part Toolpane, expand the **Miscellaneous** section and delete the text in the **JS Link** textbox.

![JS Link property in web part properties](http://i.imgur.com/ypyOI6g.png)

6) Click OK.

7) In the Ribbon, click Stop Editing.

![Ribbon button for Stop Editing](http://i.imgur.com/ZngPaL1.png)

8) Edit an existing list item.

**Note:** 
These same steps may also be applied to the New form.

The following code illustrates how the Predecessors column is hidden in the New and Edit forms. The OnPostRender property registers a JavaScript function which locates and hides the Predecessor field.

**Note: **
This client side method does not remove the column from the HTML.  If you inspect the HTML you will see the Predecessors control is still part of the DOM, although it is not visible in the web browser.

![HTML structure with display:none entry for tr element](http://i.imgur.com/c8FF9zv.png)

```JavaScript
function RegisterHiddenFiledContext () {

    // jQuery library is required in this sample
    // Fallback to loading jQuery from a CDN path if the local is unavailable
    (window.jQuery || document.write('<script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.0.min.js"><\/script>'));

    // Create object that has the context information about the field that we want to render differently
    var hiddenFiledContext = {};
    hiddenFiledContext.Templates = {}; 
    hiddenFiledContext.Templates.OnPostRender = hiddenFiledOnPreRender;
    hiddenFiledContext.Templates.Fields = {
        // Apply the new rendering for Predecessors field in New and Edit forms
        "Predecessors": {
            "NewForm": hiddenFiledTemplate,
            "EditForm": hiddenFiledTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(hiddenFiledContext);

}

// This function provides the rendering logic
function hiddenFiledTemplate() {
    return "<span class='csrHiddenField'></span>";
}

// This function provides the rendering logic
function hiddenFiledOnPreRender(ctx) {
    jQuery(".csrHiddenField").closest("tr").hide();
}
```

## SAMPLE 9 – DEPENDENT FIELDS ##
This sample demonstrates how to make some fields dependent from each other in SharePoint list item New and Edit forms. So e.g. if you change a value in one field, another field changes it's appearance or number of variants, etc.

Screenshot below indicates, that edit control for Color field is initially empty:

![Color field not visible since car is not selected](http://i.imgur.com/LpcTcDX.png)

However, if we select a Car, then we will see that now Color field provides some variants:

![Color field visible when car is selected](http://i.imgur.com/uEICdMV.png)

Better yet, if we select a different car, available Color variants will be different:

![Different color options based on selected car](http://i.imgur.com/DWKHn0V.png)

So as you can see, it is possible to create a dependency between fields based on a custom logic. This custom logic might even include asynchronous ajax calls.

Here is the code with explanations:

```JavaScript
function RegisterDependentFieldsContext() {

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides({

        Templates: {
            OnPostRender: function (ctx) {
                var colorField = window[ctx.FormUniqueId + "FormCtx"].ListSchema["Color"];
                var colorFieldControlId = colorField.Name + "_" + colorField.Id + "_$RadioButton" + colorField.FieldType + "Field";

                var f = ctx.ListSchema.Field[0];
                if (f.Name == "Car") {
                    var fieldControl = $get(f.Name + "_" + f.Id + "_$" + f.FieldType + "Field");

                    $addHandler(fieldControl, "change", function (e) {
                        // first, let's hide all the colors - while the information is loading
                        for (var i = 0; i < 5; i++)
                            $get(colorFieldControlId + i).parentNode.style.display = "none";

                        var newValue = fieldControl.value;
                        var newText = fieldControl[fieldControl.selectedIndex].text;

                        var context = SP.ClientContext.get_current();
                        // here add logic for fetching information from an external list
                        // based on newText and newValue
                        context.executeQueryAsync(function () {
                            // fill this array according to the results of the async request
                            var showColors = [];
                            if (newText == "Kia Soul") showColors = [0, 2, 3];
                            if (newText == "Fiat 500L") showColors = [1, 4];
                            if (newText == "BMW X5") showColors = [0, 1, 2, 3, 4];

                            // now, display the relevant ones
                            for (var i = 0; i < showColors.length; i++)
                                $get(colorFieldControlId + showColors[i]).parentNode.style.display = "";
                        },
                        function (sender, args) {
                            alert("Error! " + args.get_message());
                        });

                    });
                } else if (f.Name == "Color") {
                    // initialization: hiding all the choices. first user must select a car
                    for (var i = 0; i < 5; i++)
                        $get(colorFieldControlId + i).parentNode.style.display = "none";

                }
            }
        }

    });
}
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.ClientSideRendering" />