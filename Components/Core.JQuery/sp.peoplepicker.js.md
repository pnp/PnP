# sp.peoplepicker.js #

### Summary ###

This extension allows for the inclusion of a client side people picker in your provider or SharePoint hosted add in.

### Relevant Files ###

The relevant files from the example solution are:

- [sp.peoplepicker.js](Core.JQueryWeb/Scripts/PnP/sp.peoplepicker.js) : the jQuery extension
- [app.js](Core.JQueryWeb/Scripts/PnP/app.js) : supporting functions
- [Index.cshtml](Core.JQueryWeb/Views/Home/Index.cshtml) : example usage
- [Site.css](Core.JQueryWeb/Content/Site.css) : control styles

### Dependencies ###

- jQuery >= 1.8
- Bootstrap >= 3

### Usage ###

The extension is used like any other jQuery extension, based initially off of a jQuery object created using the selector syntax. The method signature is **spPeoplePicker([options])**

#### Include the needed files in your page ####

```ASPX
<script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/app.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/sp.peoplepicker.js"></script>
```

#### Provide some markup for the extension to transform: ####

It is important to note that all of the jQuery people picker operations (create, get, set, clear) are done on the container element. Our examples throughout the article use a container div with id "ppDefault". All of the generated HTML will be appended to this container element.


```HTML
<div id="ppDefault"></div>
```

#### Transform Markup ####

Once the DOM loads you can transform using jQuery syntax, options are provided in a plain js object. In the provided example add in we are using a custom on start method, here we use the jQuery standard. By default no options are required, the SPHostUrl and SPAppWebUrl are taken from the query string and only used in the $app.withSPContext method.

```JavaScript
$(function() {
    $('#ppDefault').spPeoplePicker();
});
```

Once complete you should see a people picker control.

![The people picker control, which consists of a text box and a drop down. The text box contains the text patr and the text cursor. The drop down contains the name Patrick, followed by the text, Showing 1 items of 1.](http://i.imgur.com/cpeP4aS.png)

----------

### Configuration Options ###

The extension supports the following configuration options:

**Option** | **Required** | **Description** | **Default**
---- | ---- | ---- | ----
onLoaded |  | A function whose "this" will be the originally selected container element | null
minSearchTriggerLength |  | Minimum number of characters entered before a call to the server | 4
maximumEntitySuggestions |  | Maximum number of users to return from a search | 30
principalType |  | Type of object for which to search. [Enumeration Details](http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx) | 1
principalSource |  | User Search Principal Source | 15
searchPrefix |  | Prepended to all search request queries | ''
searchSuffix |  | Appended to all search request queries | ''
displayResultCount |  | Maximum number of results to show in the suggestions dropdown | 4
maxSelectedUsers |  | Maximum users allowed to be selected | 1

This example specifies all the options:

```JavaScript
$(function () {
    $('#ppDefault').spPeoplePicker({
        onLoaded: function () {
            // set the value once the control is loaded
            $(this).spPeoplePicker('set', [{ login: 'fakedomain\fakeuser', title: '', displayName: 'Fake User', email: 'fake.user@fakedomain.com' }]);
        },
        minSearchTriggerLength: 2,
        maximumEntitySuggestions: 10,
        principalType: 1,
        principalSource: 15,
        searchPrefix: '',
        searchSuffix: '',
        displayResultCount: 6,
        maxSelectedUsers: 2
    });
});
```

----------

### Get Values ###

The people picker control supports getting the selected user(s) using a command parameter. User(s) are returned in an array of plain objects with the following properties: login, title, displayName, email. Even if the control is set to only allow a single user an array is returned. Code sample and example return:

```JavaScript
var selected = $('#ppDefault').spPeoplePicker('get');
// selected == [{ login: 'fakedomain\fakeuser', title: '', displayName: 'Fake User', email: 'fake.user@fakedomain.com' }]
```

----------

### Set Values ###

Similarly you can set the control's value using a command parameter and the new values. Any values supplied will replace the values in the control. When setting the control you can supply an array or a single object.

```JavaScript
$('#ppDefault').spPeoplePicker('set', [{ login: 'fakedomain\fakeuser', title: '', displayName: 'Fake User', email: 'fake.user@fakedomain.com' }]);
```

Note: You can set multiple users in a control set to allow a single user but users will not be able to add additional users or edit existing unless the number selected is reduced to below the configured value. This is by design so that any UI loading where the max users has been reduced will not break on load and still display the set values.

----------

### Clear Values ###

The syntax to clear the control's value is below. This will clear all values from the control. You can also use set to clear the control, the three lines below accomplish the same thing. It is suggested to use the clear syntax should this behavior change in future releases.

```JavaScript
$('#ppDefault').spPeoplePicker('clear');
$('#ppDefault').spPeoplePicker('set', []);
$('#ppDefault').spPeoplePicker('set', null);
```

----------

### Styling ###

The control uses the default Bootstrap styles and functionality in association with the style found in the example [Site.css](Core.JQueryWeb/Content/Site.css). You can extend and modify these styles as needed to meet your needs.




