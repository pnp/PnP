# sp.addChrome.js #

### Summary ###

This extension allows for the inclusion of the SharePoint chrome controls in your provider hosted add in.

### Relevant Files ###

The relevant files from the example solution are:

- [sp.addChrome.js](Core.JQueryWeb/Scripts/PnP/sp.addChrome.js) : the jQuery extension
- [app.js](Core.JQueryWeb/Scripts/PnP/app.js) : supporting functions
- [Index.cshtml](Core.JQueryWeb/Views/Home/Index.cshtml) : example usage

### Dependencies ###

- jQuery >= 1.8

### Usage ###

The extension is used like any other jQuery extension, based initially off of a jQuery object created using the selector syntax. The method signature is **addSPChrome([options])**

#### Include the needed files in your page ####

```ASPX
<script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/app.js"></script>
<script type="text/javascript" src="~/Scripts/PnP/sp.addChrome.js"></script>
```

#### Provide some markup for the extension to transform: ####

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

#### Transform Markup ####

Once the DOM loads you can transform using jQuery syntax, options are provided in a plain js object. In the provided example add in we are using a custom on start method, here we use the jQuery standard.

```JavaScript
$(function() {
    $('#spChrome').addSPChrome({
        appTitle: 'Test Harness',
        appIcon: '/Images/AppIcon.png'
    });
});
```

Once complete the SharePoint chrome control and host styles should be loaded in your site

![Chrome ui the add-in](http://i.imgur.com/rlHatmn.png)

----------

### Configuration Options ###

In addition to the core options you can configure:

**Option** | **Required** | **Description**
---- | ---- | ----
appTitle | X | Title of the application, displayed in the title area
appIcon | X | Icon of the application, displayed in the title area
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
        helpUrl: $app.appendSPQueryToUrl("/Home/Index")
    });
});
```

![Settings links in the chrome](http://i.imgur.com/Q7LYZOn.png)
