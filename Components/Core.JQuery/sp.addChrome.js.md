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

You must provide some markup for the control to transform, something similar to the below.

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