#JavaScript - Loader Pattern#

### Summary ###
This sample demonstrates how to use a loader file in conjunction with a UserCustomAction to embed JavaScript in a SharePoint site. This pattern minimizes the need to update the deployed custom action.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations in the embedder program.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
JavaScript.LoaderPattern | Patrick Rodgers (**Microsoft**) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 29th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Embedder Console Application #

To ease testing it can be helpful to use a console application to embed the UserCustomAction in the target site. In practice this would likely be done during provisioning of the site/web. The small block of script that is embedded allows for all updates to occur outside of SharePoint, easing the burden on administrators and developers. This console application provides an example of a broadly reusable development tool.

# Embedded JavaScript #

The JavaScript block code is kept to a minimum and makes use of the built-in SharePoint script on demand functionality. This example makes use of the public Microsoft hosted CDN for [jQuery](http://jquery.com) - but could be updated to use any other source.

```JavaScript
(function (loaderFile, nocache) {
    var url = loaderFile + ((nocache) ? '?' + encodeURIComponent((new Date()).getTime()) : '');
    SP.SOD.registerSod('cdn-jquery.js', 'https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.4.js');
    SP.SOD.registerSod('cdn-loader.js', url);
    SP.SOD.registerSodDep('cdn-loader.js', 'cdn-jquery.js');
    SP.SOD.executeFunc('cdn-loader.js', null, function () { });
})('https://path/to/my/loader.js', true);
```

In the above code we are using an anonymous function embedded in the page to first load jQuery and then load our remote loader file. Additional required files can also be loaded here, such as [Bootstrap](http://getbootstrap.com) or [Knockout](http://knockoutjs.com/). These would then be available on every page - but it may be desirable to load them in the loader.js file in case the version or references need to change. It is recommended, where possible, to host the jQuery.js file references above at a generic location with a non-versioned name (such as jQuery.js). This allows for the file to be updated while avoiding the need to update the UserCustomActions.

# Loader.js #

The [Loader.js](CDN/Loader.js) file is the engine used to load any other required client files. This model provides a single location to update those files required by an application. Two versions are provided as examples, one that does not cache the files, for development and testing, and a [second](CDN/LoaderCached.js) that does cache the loaded client files for production/UAT scenarios. In production the loader file can also be minimized to reduce download times.

```JavaScript
(function (files) {

    // create a promise
    var promise = $.Deferred();

    // this function will be used to recursively load all the files
    var engine = function () {

        // maintain context
        var self = this;

        // get the next file to load
        var file = self.files.shift();

        // load the remote script file
        $.getScript(file).done(function () {
            if (self.files.length > 0) {
                engine.call(self);
            }
            else {
                self.promise.resolve();
            }
        }).fail(self.promise.reject);
    };

    // create our "this" we will apply to the engine function
    var ctx = {
        files: files,
        promise: promise
    };

    // call the engine with our context
    engine.call(ctx);

    // give back the promise
    return promise.promise();

})(['https://localhost:44323/corefunctions.js', 'https://localhost:44323/uimodifications.js']).done(function () {
    /* all scripts are loaded and I could take actions here */
}).fail(function () {
    /* something failed, take some action here if needed */
});
```

# Design Template #

The other example included in this package is a JavaScript design template demonstrating how to update the view of a list view web part. The file [designtemplate.js](CDN\designtemplate.js) uses Bootstrap to create an accordian view of a list. This template can be uploaded, unassociated with an add-in, to a style library and set in the JSLink property of the list view web part.

To see the example work you will need to create a list with a column BigNote, add the list view web part to a page and set the JSLink property to the location of the uploaded designtemplate.js.

```JavaScript
(function () {

    var utils = {

        loadStyleSheet: function (path) {
            $('<link>').appendTo('head').attr({ type: 'text/css', rel: 'stylesheet' }).attr('href', path);
        }
    };

    var template = {

        renderHeader: function (ctx) {
            var html = [];
            html.push('<div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">');
            return html.join('');
        },

        renderItem: function (ctx) {

            // we need a unique heading id
            var headingId = 'heading_' + ctx.CurrentItem.ID;

            // we need a unique collapse panel id
            var collapseId = 'collapse_' + ctx.CurrentItem.ID;

            var html = [];
            html.push('<div class="panel panel-default">');
            html.push('<div class="panel-heading" role="tab" id="' + headingId + '">');
            html.push('<h4 class="panel-title">');
            html.push('<a role="button" data-toggle="collapse" data-parent="#accordion" href="#' + collapseId + '" aria-expanded="true" aria-controls="' + headingId + '">');

            html.push(ctx.CurrentItem.Title);

            html.push('</a>');
            html.push('</h4>');
            html.push('</div>');
            html.push('<div id="' + collapseId + '" class="panel-collapse collapse" role="tabpanel" aria-labelledby="' + headingId + '">');
            html.push('<div class="panel-body">');

            html.push(ctx.CurrentItem.BigNote);

            html.push('</div>');
            html.push('</div>');
            html.push('</div>');

            return html.join('');
        },

        renderFooter: function (ctx) {

            // Define any footer content here.
            var footerHTML = "</div>"; // close the main container div

            // Now begin the paging control
            var firstRow = ctx.ListData.FirstRow;
            var lastRow = ctx.ListData.LastRow;
            var prevPage = ctx.ListData.PrevHref;
            var nextPage = ctx.ListData.NextHref;

            var pagingCtrl = "<div class='paging'>";

            pagingCtrl += prevPage ? "<a class='ms-commandLink ms-promlink-button ms-promlink-button-enabled' href='" +
                prevPage + "'><span class='ms-promlink-button-image'><img class='ms-promlink-button-left'" +
                 " src='/_layouts/15/images/spcommon.png?rev=23' /></span></a>" : "";


            pagingCtrl += prevPage || nextPage ? "<span class='ms-paging'><span class='First'>" + firstRow +
                "</span> - <span class='Last'>" + lastRow + "</span></span>" : "";


            pagingCtrl += nextPage ? "<a class='ms-commandLink ms-promlink-button ms-promlink-button-enabled' href='" +
                nextPage + "'><span class='ms-promlink-button-image'><img class='ms-promlink-button-right'" +
                " src='/_layouts/15/images/spcommon.png?rev=23'/></span></a>" : "";

            pagingCtrl += '</div>';

            return footerHTML + pagingCtrl;
        },

        postRenderCallback: function (ctx) {
            // no reason these files can't also be loaded by the loader and made available to every page to be used across tempates
            SP.SOD.registerSod('cdn-bootstrap.js', 'https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js');
            SP.SOD.executeFunc('cdn-bootstrap.js', null, function () {
                utils.loadStyleSheet('https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/css/bootstrap.css');
            });
        },

        registerTemplateOverride: function () {

            // the override is a JSON structure
            var override = {

                //	Register this Display Template against views with matching BaseViewID and ListTemplateType
                //	See http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.listtemplatetype(v=office.15).aspx for more ListTemplateTypes	
                BaseViewID: 1,
                ListTemplateType: 100,

                // this is an array
                OnPostRender: [template.postRenderCallback],

                // this child object defines what methods we are calling for our header, footer and item
                Templates: {
                    Header: template.renderHeader,
                    Item: template.renderItem,
                    Footer: template.renderFooter
                }
            };

            //  Register the template overrides with SharePoint
            SPClientTemplates.TemplateManager.RegisterTemplateOverrides(override);
        },

        Init: function () {

            // CSR override for MDS enabled site
            RegisterModuleInit('~sitecollection/Style%20Library/Examples/designtemplate.js', template.registerTemplateOverride);

            // CSR override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
            template.registerTemplateOverride();
        }
    };

    template.Init();

})();
```