# sp.documentpicker.js #

### Summary ###

This extension allows for the inclusion of a client side document picker in your provider or SharePoint hosted add in.

### Relevant Files ###

The relevant files from the example solution are:

- [sp.documentpicker.js](Core.JQueryWeb/Scripts/PnP/sp.documentpicker.js) : the jQuery extensionDc
- [app.js](Core.JQueryWeb/Scripts/PnP/app.js) : supporting functions
- [Index.cshtml](Core.JQueryWeb/Views/Home/Index.cshtml) : example usage
- [Site.css](Core.JQueryWeb/Content/Site.css) : control styles

### Dependencies ###

- jQuery >= 1.8
- Bootstrap >= 3

### Usage ###

The extension is used like any other jQuery extension, based initially off of a jQuery object created using the selector syntax. The method signature is **spDocumentPicker([options])**

#### Include the needed files in your page ####

```ASPX
<script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/app.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/sp.documentpicker.js"></script>
```

#### Provide some markup for the extension to transform: ####

It is important to note that all of the jQuery document picker operations (create, get, set, clear) are done on the container element. Our examples throughout the article use a container div with id "dpDefault". All of the generated HTML will be appended to this container element.

```HTML
<div id="dpDefault"></div>
```

#### Transform Markup ####

Once the DOM loads you can transform using jQuery syntax, options are provided in a plain js object. In the provided example add in we are using a custom on start method, here we use the jQuery standard. By default no options are required, the SPHostUrl and SPAppWebUrl are taken from the query string and only used in the $app.withSPContext method.

```JavaScript
$(function() {
    $('#dpDefault').spDocumentPicker();
});
```

Once complete you should see a document picker control.

![Document picker UI](http://i.imgur.com/eBFmjwq.png)

----------

### Configuration Options ###

The extension supports the following configuration options:

**Option** | **Required** | **Description** | **Default**
---- | ---- | ---- | ----
onLoaded |  | A function whose "this" will be the originally selected container element | null
hostUrl |  | The SPHostUrl | SPHostUrl from QueryString
allowMultiple |  | Allow multiple documents to be selected | false
showSubSites |  | Show sites below the current root web | false
nodeFilter |  | function taking a single argument, a "node". returning true shows the node, false will hide the node. See notes below on nodes | null
__nodeFilter |  | The master node filter, should you need to completely override the behavior. Use nodeFilter if at all possible. | [internal code]

This example specifies all the options:

```JavaScript
$('#dpDefault').spDocumentPicker({
    showSubSites: true,
    hostUrl: $app.getUrlParamByName('SPHostUrl'),
    onLoaded: function () {
        // set value after load
        $(this).spDocumentPicker('set', [{ path: '/sites/fake/server/relative/path/file.txt', name: 'File Name' }])
    },
    nodeFilter: function (node) {
        // filter any nodes whose text starts with [a|A]
        return !/^a/i.test(node.text());
    },
    allowMultiple: false
});
```

----------

### Get Values ###

The document picker control supports getting the selected document(s) using a command parameter. Document(s) are returned in an array of plain objects with the following properties: path and name. Even if the control is set to only allow a single document an array is returned. Code sample and example return:

```JavaScript
var selected = $('#dpDefault').spDocumentPicker('get');
// selected == [{ path: '/sites/fake/server/relative/path/file.txt', name: 'File Name' }]
```

Note: You can set multiple documents in a control set to allow a single document but users will not be able to add additional documents or edit existing unless the number selected is reduced to below the configured value. This is by design so that any UI loading where the max users has been reduced will not break on load and still display the set values.

----------

### Set Values ###

Similarly you can set the control's value using a command parameter and the new values. Any values supplied will replace the values in the control. When setting the control you can supply an array or a single object.

```JavaScript
$('#dpDefault').spDocumentPicker('set', [{ path: '/sites/fake/server/relative/path/file.txt', name: 'File Name' }]);
```

----------

### Clear Values ###

The syntax to clear the control's value is below. This will clear all values from the control. You can also use set to clear the control, the three lines below accomplish the same thing. It is suggested to use the clear syntax should this behavior change in future releases.

```JavaScript
$('#dpDefault').spDocumentPicker('clear');
$('#dpDefault').spDocumentPicker('set', []);
$('#dpDefault').spDocumentPicker('set', null);
```

----------

### A note on nodes ###

The document picker nodeFilter function relies on "nodes" representing the various parts of the tree. Each node is a jQuery object representing an li tag with custom attribute. All nodes have the "sp-docpicker-nodeType" attribute. These nodes are what are supplied to the node filter and any logic can then be applied to the jQuery object. The list of valid node types are:

- web
- list
- folder
- file
- info
- error

----------

### Styling ###

The control uses the default Bootstrap styles and functionality in association with the style found in the example [Site.css](Core.JQueryWeb/Content/Site.css). You can extend and modify these styles as needed to meet your needs.


----------

### Add In Web Requirements ###

You must ensure creation of an Add In web for the extension to work correctly. This can be done by creating an empty list in your Add-In project. Also, to recurse the site hierarchy when showing sub-sites you must request SiteCollection : Manage permissions:

```XML
<?xml version="1.0" encoding="utf-8" ?>
<App  ...>
  ...
  <AppPermissionRequests>
    <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="Manage" />
  </AppPermissionRequests>
</App>
```