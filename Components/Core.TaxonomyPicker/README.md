# Taxonomy picker for sharepoint app #

### Summary ###
This sample shows an implementation of a SharePoint Taxonomy Picker control that can be used on provider hosted SharePoint apps.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
It's important that the provider hosted app that's running the taxonomy picker is using the same IE security zone as the SharePoint site it's installed on. If you get "Sorry we had trouble accessing your site" errors then please check this.

### Solution ###
Solution | Author(s)
---------|----------
Contoso.Components.TaxonomyPicker | Richard diZerega (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | March 26th 2014 | Updates
1.0  | October 30th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# How to use the taxonomy picker in your provider hosted sp app #

Using the Taxonomy Picker in your provider hosted app does not require many steps ?

## Ensure you trigger the creation of an app web ##
When you build a provider hosted app it does not necessarily have an app web associated with it whereas a SharePoint hosted app always has an app web. 

Since the Taxonomy Picker control uses the CSOM object model from JavaScript it’s required to have an app web. 

To ensure you have an app web you can just add an empty module to your SharePoint app as shown below:

![Screenshot of module](http://i.imgur.com/FBh3CfY.png "Screenshot of module")

## App permissions ##
The Taxonomy Picker communicates with SharePoint’s Managed Metadata Service, which requires special permissions in the app model.  Working with Closed TermSets will require Read permission on the Taxonomy permission scope.  To enable the creation of new terms in Open TermSets, the app will require Write permission on the Taxonomy permission scope.  These permissions can be set in the AppManifest.xml as seen below:

![Screenshot of app permissions](http://i.imgur.com/MjQHxN1.png "Screenshot of app permissions")
 
## Required files ##
The Taxonomy Picker is implemented as a jQuery extension, which means it requires a reference to jQuery on and pages it will be used. In addition to jQuery, the Taxonomy Picker control requires the reference of a taxonomypicker.js and taxonomypicker.css files included in the sample solution.

![Screenshot of script tag](http://i.imgur.com/azNdlUM.png "Screenshot of script tags")
 
## Loading required scripts and establishing clientcontext ##
The Taxonomy Picker uses SharePoint’s JavaScript Client Object Model (JSOM) for communication back to SharePoint and the Managed Metadata Service.  The JavaScript below shows how to load the appropriate JSOM scripts, initialize SharePoint ClientContext, and wiring up a RequestExecutor to make cross-domain calls.  Notice the reference to sp.taxonomy.js, which is a JSOM script specific to working with taxonomies:  

```javascript
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

                    //load scripts for cross-domain calls
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                    });

                    //load scripts for calling taxonomy APIs
                    $.getScript(layoutsRoot + 'init.js',
                        function () {
                            $.getScript(layoutsRoot + 'sp.taxonomy.js',
                                function () {
                                    //READY TO INITALIZE TAXONOMY PICKERS
                                });
                        });
                });
        });
});
```

## Adding the taxonomy picker to html ##
Any hidden input element can be converted to a Taxonomy Picker.  This includes regular hidden input elements and server-side controls that render hidden inputs elements (ex: asp:HiddenField):

### Client-side example ###

```html
<input type="hidden" id="taxPickerGeography" />
```

### Server-side example ###

```c#
<asp:HiddenField runat="server" ID="taxPickerGeography" />
```

### Transforming the html into a taxonomy picker control ###
The Taxonomy Picker is implemented as a jQuery extension, which makes it extremely easy to wire-up on the hidden input element:

```javascript
$('#taxPickerGeography').taxpicker({ 
        isMulti: false,
        allowFillIn: false,
        termSetId: '1c4da890-60c8-4b91-ad3a-cf79ebe1281a' 
}, context);
```

### Parameters ###
The first parameter of the Taxonomy Picker sets the options for the control. The properties that can be set include:

| Parameter | Description |
| ----------|-------------|
| isMulti | Boolean indicating if taxonomy picker support multiple value |
| isReadOnly | Boolean indicating if the taxonomy picker is rendered in read only mode |
| allowFillIn | Boolean indicating if the control allows fill=ins (Open TermSets only) |
| termSetId | the GUID of the TermSet to bind against (available from Term Mgmt) |
| useHashtags | Boolean indicating if the default hashtags TermSet should be used |
| useKeyword | Boolean indicating if the default keywords TermSet should be used |
| maxSuggestions | integer for the max number of suggestions to list (defaults is 10) |
| lcid | the locale ID for creating terms (default is 1033) |
| language | the language code for the control (defaults to en=us) context. |

 The second parameter is an initialized SP.ClientContext object 

## Sample implementations ##

```javascript
//Single-select open termset field
$('#taxPickerOpenSingle').taxpicker({ 
 isMulti: false,
 allowFillIn: true,
 termSetId: 'ac8b3d2f-37e9-4f75-8f67-6fb8f8bfb39b' }
, context);
```

```javascript
//Multi-select closed termset field
$('#taxPickerClosedMulti').taxpicker({ 
 isMulti: true,
 allowFillIn: false,
 termSetId: '1c4da890-60c8-4b91-ad3a-cf79ebe1281a' }
, context);
```

```javascript
//Use default Hashtags termset and limit the suggestions to 5
$('#taxPickerHashtags').taxpicker({ 
 isMulti: true,
 allowFillIn: true,
 useHashtags: true,
 maxSuggestions: 5 }
, context);
```

```javascript
//Use default keywords termset with a locale id of 1031 and German
$('#taxPickerKeywords').taxpicker({ 
 isMulti: true,
 allowFillIn: true,
 useKeywords: true,
 lcid: 1031,
 language: 'de-de' }
, context);
```

## Setting values ##
The sample project includes a TaxonomyPickerExtensions.cs file, containing extension methods to help set values of a Taxonomy Picker server-side.  This includes extension methods for converting TaxonomyFieldValue and TaxonomyFieldValueCollection objects into JSON that the Taxonomy Picker script can read from the hidden fields.  Here is an example of using these methods to set the value of two Taxonomy Picker fields using C#:

```c#
//The following code shows how to set a taxonomy field server-side
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
using (var clientContext = spContext.CreateUserClientContextForSPHost())
{
    var list = clientContext.Web.Lists.GetByTitle("MyList");
    var listItem = list.GetItemById(1);

    clientContext.Load(listItem);
    clientContext.ExecuteQuery();

    taxPickerGeographySingle.Value = 
        ((TaxonomyFieldValue)listItem["SomeTaxFieldSingle"]).Serialize();
    taxPickerGeographyMulti.Value = 
        ((TaxonomyFieldValueCollection)listItem["SomeTaxFieldMulti"]).Serialize();
}
```

## Reading values ##
The Taxonomy Picker will store the selected terms in the hidden field using JSON string format.  These values can be accessed by other client-side scripts or server-side following a post.  The JSON will include the Term Name, Id, and PathOfTerm (ex: World;North America;United States).  JSON.parse can be used client-side to convert the hidden input’s value to a typed object and any number of server-side libraries can be used (ex: JSON.net)

## Language support ##
The strings displayed by the control will be loaded dynamically based on the passed language. This requires you to pass the language via taking over the SPLanguage url parameter (see sample) or by hardcoding it. If no language is passed the control assumes the language is English (en-us). 
$('#taxPickerKeywords').taxpicker({ isMulti: true, allowFillIn: true, useKeywords: true, lcid: 1031, language: 'de-de' }, context);
If you would like to add additional languages you need to create the appropriate JavaScript language resource files:

![Screenshot of js files](http://i.imgur.com/Ul6QIXU.png "Screenshot of js files")

Such a resource file is simple collection of global constants:

![Screenshot of resource files](http://i.imgur.com/pNQpQst.png "Screenshot of resource files")