# sp.addChrome.js #

### Summary ###

This extension allows for the easy inclusion of the SharePoint chrome controls in your provider hosted add in.

### Relevant Files ###

The relevant files from the example solution are:

- [sp.addChrome.js](Core.JQueryWeb/Scripts/PnP/sp.addChrome.js) : the jQuery extension
- [app.js](Core.JQueryWeb/Scripts/PnP/app.js) : supporting functions
- [Index.cshtml](Core.JQueryWeb/Views/Home/Index.cshtml) : example usage

### Usage ###

The extension is used like any other jQuery extension, based initially off of a jQuery object created using the selector syntax. The method signature is **addSPChrome([options])**

#### Page Markup ####

You must provide some markup for the control to transform:

```HTML
<div class="container-fluid">
    <div class="row">
        <header class="col-md-12 hidden-xs">
            <div id="spChrome"></div>
        </header>
    </div>
	...
</div>
```

Then once the page loads you can transform using jQuery syntax. In the provided example we are using a custom on start method, here we use the jQuery standard. Also, we are providing the two base parameters, appTitle and appIcon.

```JavaScript
$(function() {
    $('#spChrome').addSPChrome({
        appTitle: 'Test Harness',
        appIcon: '/Images/AppIcon.png'
    });
});
```

Once complete the SharePoint chrome control and host styles should be loaded in your site

![](http://i.imgur.com/rlHatmn.png)


----

### All Options ###

In addition to the core options you can configure:

**Option** | **Required** | **Description**
---- | ---- | ----
appTitle | X | The title of the application, displayed in the title area
appIcon | X | The title of the application, displayed in the title area
hostUrl |  | Set automatically from the query string you can override the value in the settings
helpUrl |  | A url to a help page for you provider hosted add-in.
settingsLinks |  | An array of one or more links to be included in the settings menu "gear" icon

This example specifies all the options:

```JavaScript
$(function () {
    $('#spChrome').addSPChrome({
        appTitle: 'Test Harness',
        appIcon: '/Images/AppIcon.png',
        settingsLinks: [
                    {
                        "linkUrl": $app.appendSPQueryToUrl("Setup.aspx"),
                        "displayName": "Setup"
                    },
                    {
                        "linkUrl": $app.appendSPQueryToUrl("AnotherLink.aspx"),
                        "displayName": "Another Link"
                    }
        ],
        hostUrl: $app.getUrlParamByName('SPHostUrl'),
        helpUrl: "/Home/Index"
    });
});
```

