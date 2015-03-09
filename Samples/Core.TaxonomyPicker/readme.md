# Cascaded Taxonomy Picker #

### Summary ###
This sample shows an implementation of a SharePoint Taxonomy Picker control that can be used on provider hosted SharePoint apps.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.TaxonomyPicker | Anand Malli (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 26th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Using the Taxonomy Picker #
This sample is suitable when cascaded taxonomy picker control is required in your SharePoint Provider Hosted App and you have Term Set structure similar to mentioned below:
![Typical Term Set](http://i.imgur.com/bQk27IP.png)

And you wanted to represent them like this with cascading filter functionality:

![Sample Picker UI](http://i.imgur.com/h2XkNXw.png)


## Ensure you trigger the creation of an App Web ##
When you build a provider hosted app it does not necessarily have an app web associated with it whereas a SharePoint hosted app always has an app web. Since the Taxonomy Picker control uses the CSOM object model from JavaScript it’s required to have an app web. To ensure you have an app web you can just add an empty module to your SharePoint app as shown below:

![AppAssets Module](http://i.imgur.com/zxkaCPW.png)

## App permissions ##
The Taxonomy Picker communicates with SharePoint’s Managed Metadata Service, which requires special permissions in the app model.  Working with Closed Term Sets will require Read permission on the Taxonomy permission scope.  To enable the creation of new terms in Open Term Sets, the app will require Write permission on the Taxonomy permission scope.  These permissions can be set in the AppManifest.xml as seen below:

![Permissions](http://i.imgur.com/sUwHGtG.png)

## Required Files ##
The Taxonomy Picker is implemented as a jQuery extension, which means it requires a reference to jQuery on and pages it will be used.  In addition to jQuery, the Taxonomy Picker control requires the reference of a taxonomypicker.js and taxonomypicker.css files included in the sample solution.

![Required Files](http://i.imgur.com/D9FNgfN.png)

## Loading required scripts and establishing ClientContext ##
The Taxonomy Picker uses SharePoint’s JavaScript Client Object Model (JSOM) for communication back to SharePoint and the Managed Metadata Service.  The JavaScript below shows how to load the appropriate JSOM scripts, initialize SharePoint ClientContext, and wiring up a RequestExecutor to make cross-domain calls.  Notice the reference to sp.taxonomy.js, which is a JSOM script specific to working with taxonomies:

```JavaScript
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

## Adding the cascaded taxonomy picker to HTML ##
Any hidden input element can be converted to a Taxonomy Picker.  This includes regular hidden input elements and server-side controls that render hidden inputs elements (ex: asp:HiddenField):

**Client-side example:**

```HTML
	<div class="ms-core-form-line" style="margin-bottom: 0px;">
	     <input type="hidden" id="taxPickerContinent" />
	</div>
```

**Server-side example:**

```ASPX
	<div class="ms-core-form-line" style="margin-bottom: 0px;">
		<asp:HiddenField runat="server" ID="taxPickerContinent" />
	</div>
```

## Transforming the HTML into a Taxonomy Picker Control ##
The Taxonomy Picker is implemented as a jQuery extension, which makes it extremely easy to wire-up on the hidden input element:

```JavaScript

$('#taxPickerContinent').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", levelToShowTerms: 1 }, context, callbackmethod);

```

OR

```JavaScript

$('#taxPickerCountry').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false }, context, initializeRegionTaxPicker);

```

### PARAMETERS ###

<table>
<tr>
<td>options</td>
<td>
The first parameter of the Taxonomy Picker sets the options for the control.  The properties that can be set include:
<ul>
<li>isMulti – Boolean indicating if taxonomy picker support multiple value</li>
<li>allowFillIn – Boolean indicating if the control allows fill-ins (Open Term Sets only)</li>
<li>termSetId – the GUID of the Term Set to bind against (available from Term Mgmt)</li>
<li>useHashtags – Boolean indicating if the default hashtags Term Set should be used</li>
<li>useKeyword – Boolean indicating if the default keywords Term Set should be used</li>
<li>maxSuggestions – integer for the max number of suggestions to list (defaults is 10)</li>
<li>lcid – the locale ID for creating terms (default is 1033)</li>
<li>language – the language code for the control (defaults to en-us)</li>
<li>filterTermId – the GUID of the term which will be used to filter terms from Term Set for cascading (it is optional parameter)</li>
<li>useTermSetasRootNode – Boolean value indicating whether to display term set as root node in the selection control (it is optional parameter)</li>
<li>levelToShowTerms – Numeric value indicating how many level of cascading controls are being displayed</li>
</td>
</tr>
<tr>
<td>options</td>
<td>The second parameter is an initialized SP.ClientContext object</td>
</tr>
<tr>
<td>callbackmethod</td>
<td>The third parameter is callback method to call whenever control value changes</td>
</tr>
</table>

### SAMPLE IMPLEMENTATIONS ###

```JavaScript
$('#taxPickerContinent').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", levelToShowTerms: 1 }, context, initializeCountryTaxPicker);

function initializeCountryTaxPicker() {
    if (this._selectedTerms.length > 0) {
        $('#taxPickerCountry').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false }, context, initializeRegionTaxPicker);
    }
}

function initializeRegionTaxPicker() {
    if (this._selectedTerms.length > 0) {
        $('#taxPickerRegion').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false }, context);
    }
}

```

## How to run this sample ##
The sample project includes app.js file, containing initialization methods to set up the cascading taxonomy picker control. 

Please ensure that you are already having a Term Set containing terms for at least 2 level.

![Need 2 level Term Set](http://i.imgur.com/eUzIS7u.png)

Find out the GUID of the Term Set to bind (using Site Settings  Term Store Management) & update below line with actual Term Set GUID.

```JavaScript
$('#taxPickerContinent').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "<<TERMSET GUID>>", levelToShowTerms: 1 }, context, initializeCountryTaxPicker);

$('#taxPickerCountry').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "<<TERMSET GUID>>", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false }, context, initializeRegionTaxPicker);

$('#taxPickerRegion').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "<<TERMSET GUID>>", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false }, context);

```

## Setting Values ##
The sample project includes a TaxonomyPickerExtensions.cs file, containing extension methods to help set values of a Taxonomy Picker server-side.  This includes extension methods for converting TaxonomyFieldValue and TaxonomyFieldValueCollection objects into JSON that the Taxonomy Picker script can read from the hidden fields.  Here is an example of using these methods to set the value of two Taxonomy Picker fields using C#:

```C#
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
## Reading Values ##
The Taxonomy Picker will store the selected terms in the hidden field using JSON string format.  These values can be accessed by other client-side scripts or server-side following a post.  The JSON will include the Term Name, Id, and PathOfTerm (ex: World;North America;United States).  JSON.parse can be used client-side to convert the hidden input’s value to a typed object and any number of server-side libraries can be used (ex: JSON.net)
# Language Support #
The strings displayed by the control will be loaded dynamically based on the passed language. This requires you to pass the language via taking over the SPLanguage url parameter (see sample) or by hardcoding it. If no language is passed the control assumes the language is English (en-us). 
```JavaScript
$('#taxPickerContinent').taxpicker({ isMulti: true, allowFillIn: true, useKeywords: true, lcid: 1031, language: 'de-de' }, context);
```
If you would like to add additional languages you need to create the appropriate JavaScript language resource files:

![Language Files](http://i.imgur.com/ffywQBF.png)

Such a resource file is simple collection of global constants:

![Glogal Constants](http://i.imgur.com/tMj6WCM.png)

