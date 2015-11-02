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


