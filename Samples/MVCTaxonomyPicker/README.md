# MVC Taxonomy picker for sharepoint add-in #

### Summary ###
This sample shows an implementation of a SharePoint Taxonomy Picker control that can be used on MVC provider hosted SharePoint apps.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
- It's important that the provider hosted add-in that's running the taxonomy picker is using the same IE security zone as the SharePoint site it's installed on. If you get "Sorry we had trouble accessing your site" errors then please check this.
- You have to set the Options 'This service application is the default storage location for Keywords.' and 'This service application is the default storage location for column specific term sets.' on one of the Managed Metadata Service Application(s) Proxy Properties. If you get "Loading TermSet failed. Please refresh your browser and try again." errors then please check this.

### Solution ###
Solution | Author(s)
---------|----------
MVCTaxonomyPicker | Alexander von Malachowski (**Nilsong Group AB**)

### Version history ###
Version  | Date | Comments
---------| -----| --------

1.0  | April 5th 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# How to use the taxonomy picker in your MVC provider hosted SharePoint add-in #

Using the Taxonomy Picker in your MVC provider hosted add-in does not require many steps?

## No need to ensure you trigger the creation of an add-in web ##
When you build a provider hosted add-in it does not necessarily have an add-in web associated with it whereas a SharePoint hosted add-in always has an add-in web. 

This TaxonomyPicker control uses server-side CSOM object model thus it does not require an add-in web.

## Add-In permissions ##
The Taxonomy Picker communicates with SharePoint's Managed Metadata Service, which requires special permissions in the add-in model.  Working with Closed TermSets will require Read permission on the Taxonomy permission scope.  To enable the creation of new terms in Open TermSets, the add-in will require Write permission on the Taxonomy permission scope.  These permissions can be set in the AppManifest.xml as seen below:

![Screenshot of add-in permissions](http://i.imgur.com/ULMbalg.png "Screenshot of add-in permissions")
 
## Required files ##
The Taxonomy Picker is implemented as a jQuery extension, which means it requires a reference to jQuery on and pages it will be used. In addition to jQuery, the Taxonomy Picker control requires the reference of a taxonomypicker.js and taxonomypicker.css files included in the sample solution.

![Screenshot of script tag](http://i.imgur.com/McOXD0Y.png "Screenshot of BundleConfig tags")
 
## Loading required scripts ##
The JavaScript below shows how to initialize the TaxonomyPicker, note that there is no JSOM required, this is taken from the App.js file in the scripts folder:  

```javascript
//Wait for the page to load
var taxPickerIndex = {};
(function ($) {
    $(document).ready(function () {
        //Initiate taxpicker 
        $('#DemoControl').taxpicker({ isMulti: false, allowFillIn: true, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', termSetImageUrl: '/Content/Images'});

        $('#Demo1Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', levelToShowTerms: 1, termSetImageUrl: '/Content/Images' }, function () {
            $('#Demo2Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' }, function () {
                $('#Demo3Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' });
            });
        });
        taxPickerIndex["#Demo1Control"] = 0;
        taxPickerIndex["#Demo2Control"] = 4;
        taxPickerIndex["#Demo3Control"] = 5;
    });

}(jQuery));


//function to get a parameter value by a specific key, used by taxonomypickercontrol.js
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}
```

## Using the TaxonomyPicker in your ViewModel ##
Included in this sample there is a custom EditorTemplate for the Taxonomypicker, To be able to use this just add a UIHint to your property in your ViewModel

### C# Example ###

```c#	
	[UIHint("TaxonomyPicker")]
	public List<PickerTermModel> Demo { get; set; }
```

## Adding the taxonomy picker to html ##
Now you can simply render it in your .cshtml with an EditorFor

### Client-side example ###

```html
 @Html.EditorFor(model => model.Demo)
```
### Wire up the TaxonomyPicker control ###
The Taxonomy Picker is implemented as a jQuery extension, which makes it extremely easy to wire-up on the control (by default the EditorTemplate builds around the PropertyName and the adds the other components with id's based on that name).
So if the property is named Demo the hidden input will have the id Demo and the control will have the id DemoControl,
Oposite to the Core.TaxonomyPicker we will initialize the surronding countrol inst of the hidden input, thus:

```javascript
  $('#DemoControl').taxpicker({ isMulti: false, allowFillIn: true, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', termSetImageUrl: '/Content/Images'});
```

### Parameters ###
The first parameter of the Taxonomy Picker sets the options for the control. The properties that can be set include (not changed from the original):

| Parameter | Description |
| ----------|-------------|
| isMulti | Boolean indicating if taxonomy picker support multiple value |
| isReadOnly | Boolean indicating if the taxonomy picker is rendered in read only mode |
| allowFillIn | Boolean indicating if the control allows fill=ins (Open TermSets only) |
| enterFillIn | Boolean indicating if the control allows fill=ins using enter or tab instead the button (Open TermSets only) |
| termSetId | the GUID of the TermSet to bind against (available from Term Mgmt) |
| useHashtags | Boolean indicating if the default hashtags TermSet should be used |
| useKeyword | Boolean indicating if the default keywords TermSet should be used |
| maxSuggestions | integer for the max number of suggestions to list (defaults is 10) |
| lcid | the locale ID for creating terms (default is 1033) |
| language | the language code for the control (defaults to en=us) context. |
| useContainsSuggestions | optional boolean indicating if the suggestions should search with "contains" matching (default pattern is "starts with") |


## Sample implementations ##

```javascript
//Single-select open termset field
$('#taxPickerOpenSingle').taxpicker({ 
 isMulti: false,
 allowFillIn: true,
 termSetId: 'ac8b3d2f-37e9-4f75-8f67-6fb8f8bfb39b' });
```

```javascript
//Multi-select closed termset field
$('#taxPickerClosedMulti').taxpicker({ 
 isMulti: true,
 allowFillIn: false,
 termSetId: '1c4da890-60c8-4b91-ad3a-cf79ebe1281a' });
```

```javascript
//Use default Hashtags termset and limit the suggestions to 5
$('#taxPickerHashtags').taxpicker({ 
 isMulti: true,
 allowFillIn: true,
 useHashtags: true,
 maxSuggestions: 5 });
```

```javascript
//Use default keywords termset with a locale id of 1031 and German
$('#taxPickerKeywords').taxpicker({ 
 isMulti: true,
 allowFillIn: true,
 useKeywords: true,
 lcid: 1031,
 language: 'de-de' });
```

## Working with the Controller ##
The sample project includes a TaxonomyPickerService.cs file, containing methods to help retriving TermSet and Terms, adding and deleting terms(deleting is not actually implemented yet in the taxonomypickercontrol.js). 
In the HomeController you can see these methods implemented, for the TaxonomyPicker to work these ActionResults method names must correspond with the methods being called via jQuery.ajax in the taxonomypickercontrol.js (i.e copy paste ftw):

```javascript#
//The following code shows how to call the GetTaxonomyPickerData method from inside taxonomypickercontrol.js
var parent = this;
$.ajax({
    url: '/Home/GetTaxonomyPickerData?SPHostUrl=' + decodeURIComponent(getQueryStringParameter('SPHostUrl')),
    type: 'POST',
    data:{
        Id: encodeURIComponent(this.Id),
        Name :this.Name,
        UseKeywords: this.UseHashtags != null ? this.UseHashtags : false,
        UseHashtags: this.UseKeywords != null ? this.UseKeywords : false,
        LCID: this.LCID
    },
    success: function (msg) {
        parent.RawTermSet = JSON.parse(msg);
        parent.termsLoadedSuccess();
    },
    error: function (textStatus, errorThrown) {
        parent.termsLoadedFailed(textStatus);
    }
});
```
As you may notice you are creating a data object that represents the TermQueryModel wich the method GetTaxonomyPickerData takes in as a parameter (Razor will hook that up for you transforming you object into your model):

```c#
public class TermQueryModel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string TermSetId { get; set; }     
    public string ParentTermId { get; set; }
    public int LCID { get; set; }
}
```

## Reading values ##
The Taxonomy Picker will store the selected terms in the hidden field using JSON string format. Access this data just use jQuery to geth the value and create a new object representing your ViewModel and convert the value from the hidden field into JSON and it will be translated into a List``<PickerTermModel>``() in your Controller:

```javascript#
//The following code shows how to call the GetTaxonomyPickerData method from inside taxonomypickercontrol.js
var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));

 $('#btnSubmit').on('click', function () {            
    $.ajax({
        url: '/Home/GetTaxonomyPickerHiddenValue?SPHostUrl=' + spHostUrl,
        type: 'POST',
        data: {                   
            Demo: JSON.parse($('#Demo').val()),
            Demo1: JSON.parse($('#Demo1').val()),
            Demo2: JSON.parse($('#Demo2').val()),
            Demo3: JSON.parse($('#Demo3').val())
        },
        success: function (msg) {
            console.log(msg)
        },
        error: function (textStatus, errorThrown) {
            console.log(textStatus)
        }
    });
});
	
![Screenshot of ActionResults model](http://i.imgur.com/eEocfbV.png "Screenshot of ActionResults model")

## Language support ##
Same as Core.TaxonomyPicker

# Appendix A: Using the taxonomypicker on hierarchical termsets #

Below you'll find the App.js file, containing initialization methods to set up the cascading taxonomy picker control. 

Please ensure that you are already having a Term Set containing terms for at least 2 level.

Find out the GUID of the Term Set to bind (using Site Settings --> Term Store Management) & update below line with actual Term Set GUID.

$('#Demo1Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', levelToShowTerms: 1, termSetImageUrl: '/Content/Images' }, function () {
    $('#Demo2Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' }, function () {
        $('#Demo3Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' });
    });
});
taxPickerIndex["#Demo1Control"] = 0;
taxPickerIndex["#Demo2Control"] = 4;
taxPickerIndex["#Demo3Control"] = 5;
```

And properly define them in the aspx page:
```cshtml
@model Core.ProviderHostedTaxonomyPickerWeb.ViewModels.DemoViewModel

@{
    ViewBag.Title = "TaxonomyPickerDemo";
}

<div class="ms-Grid-row">
    <div class="ms-Grid-col ms-u-sm12 ms-u-md6 ms-u-lg6 ms-u-xl4">
        <div class="ms-Table">
            <div class="ms-Table-row">
                <span class="ms-Table-cell">
                    <label class="ms-Label is-required">
                        @Html.DisplayNameFor(model => model.Demo)
                    </label>
                    @Html.EditorFor(model => model.Demo)
                    @Html.ValidationMessageFor(model => model.Demo)
                </span>
            </div>   
            <div class="ms-Table-row">
                <span class="ms-Table-cell">
                    <label class="ms-Label is-required">
                        @Html.DisplayNameFor(model => model.Demo1)
                    </label>
                    @Html.EditorFor(model => model.Demo1)
                    @Html.ValidationMessageFor(model => model.Demo1)
                </span>
            </div>  
            <div class="ms-Table-row">
                <span class="ms-Table-cell">
                    <label class="ms-Label is-required">
                        @Html.DisplayNameFor(model => model.Demo2)
                    </label>
                    @Html.EditorFor(model => model.Demo2)
                    @Html.ValidationMessageFor(model => model.Demo2)
                </span>
            </div>  
            <div class="ms-Table-row">
                <span class="ms-Table-cell">
                    <label class="ms-Label is-required">
                        @Html.DisplayNameFor(model => model.Demo3)
                    </label>
                    @Html.EditorFor(model => model.Demo3)
                    @Html.ValidationMessageFor(model => model.Demo3)
                </span>
            </div>    
            <div class="ms-Table-row">
                <span class="ms-Table-cell">
                    <button id="btnSubmit" class="ms-Button--primary ms-Button" tabindex="0">
                        <span class="ms-Button-label">Submit</span>
                    </button>
                    <button id="btnCancel" class="ms-Button--default ms-Button" tabindex="0">
                        <span class="ms-Button-label">Cancel</span>
                    </button>
                </span>
            </div>
        </div>
    </div>    
</div>
@Scripts.Render("~/bundles/taxpicker")
```
