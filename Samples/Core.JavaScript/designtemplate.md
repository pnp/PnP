#PnP JavaScript Core - Design Template#

### Summary ###
This sample is a design template to demonstrate their use in modifying the output of the standard List View web part using client side rendering.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.JavaScript | Patrick Rodgers (**Microsoft**) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 28th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Design Template #

The file [designtemplate.js](Core.JavaScript.CDN\js\designtemplate.js) uses Bootstrap to create an accordion view of a list. This template can be uploaded, un-associated with an add-in, to a style library and set in the JSLink property of the list view web part.

To see the example work you will need to create a list with a column BigNote, add the list view web part to a page and set the JSLink property to the location of the uploaded designtemplate.js.

```JavaScript
function EstablishDesignTemplate() {

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
            var headingId = 'heading' + ctx.CurrentItem.ID;

            // we need a unique collapse panel id
            var collapseId = 'collapse' + ctx.CurrentItem.ID;

            var html = [];
            html.push('<div class="panel panel-info">');
            html.push('<div class="panel-heading" role="tab" id="' + headingId + '">');
            html.push('<h4 class="panel-title">');
            html.push('<a class="collapsed" role="button" data-toggle="collapse" data-parent="#accordion" href="#' + collapseId + '" aria-controls="' + headingId + '" aria-expanded="false">');

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

        preRenderCallback: function (ctx) {
            // nothing here, note that we don't have any output in the DOM yet. If you need to manipulate the display use the post render
        },

        postRenderCallback: function (ctx) {
            // note that we have the DOM now. If you need to manipulate our ouput you can do so
            ExecuteOrDelayUntilBodyLoaded(function () {
                SP.SOD.registerSod('cdn-bootstrap.js', 'https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js');
                SP.SOD.executeFunc('cdn-bootstrap.js', null, function () {
                    utils.loadStyleSheet('https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/css/bootstrap.css');
                    // manually call this to try and ensure it is run on the pages with MDS enabled.
                    $('.collapsed').collapse();
                });
            });
        }
    }

    // the override is a JSON structure
    var override = {

        //	Register this Display Template against views with matching BaseViewID and ListTemplateType
        //	See http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.listtemplatetype(v=office.15).aspx for more ListTemplateTypes	
        BaseViewID: 1,
        ListTemplateType: 100,

        // this is an array of callback functions
        OnPreRender: [template.preRenderCallback],

        // this is an array of callback functions
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
};

// CSR override for MDS enabled site
RegisterModuleInit('~sitecollection/Style%20Library/Examples/designtemplate.js', EstablishDesignTemplate);

// CSR override for MDS disabled site
EstablishDesignTemplate();
```