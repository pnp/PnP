'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

ns.StockTicker = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.StockTicker.ExpirationTimeoutInMinutes = "3";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.StockTicker.DefaultHtml =
   "<h3>Stocks</h3>" +
   "<ul style=\"margin:0px; padding:0px; list-style:none;\">" +
    "<li>CONTOSO: $56.78 +0.56</li>" +
   "</ul>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.StockTicker.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpStockTicker').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.StockTicker.DefaultHtml;

        //----------------------------------------------------------------------------
        // Note: DISABLE this code block if you do not wish to present a progress indicator...
        //----------------------------------------------------------------------------
        // insert a progress indicator while we request the BDO for the control. 
        $('#pnpStockTicker').empty();
        $('#pnpStockTicker').append("<h3>Stocks</h3>" + "<p><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</p>");
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. We use DurableStorage for the Stock Ticker -- its content is not personalized/private
        // NOTE: for DEMO purposes, we want to show the use of a Sliding expiration policy
        ns.BusinessDataManager.GetStockTickerData({ storageMode: ns.StorageManager.DurableStorageMode, useSlidingExpiration: true, timeout: ns.StockTicker.ExpirationTimeoutInMinutes }).then(

            function (stockTickerData)
            {
                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml =
                    "<h3>Stocks</h3>" +
                     "<ul style=\"margin:0px; padding:0px; list-style:none;\">";

                if (stockTickerData == null || stockTickerData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (stockTickerData.Quotes.length > 0)
                    {
                        var listItemInfo = "";
                        $.each(stockTickerData.Quotes, function ()
                        {
                            var symbol = this.Symbol;
                            var price = this.Price;
                            var change = this.Change;
                            listItemInfo += "<li>" + symbol + ":&nbsp;" + price + "&nbsp;" + change + "</li>";
                        });
                        contentHtml += listItemInfo.toString();
                        contentHtml += "</ul>";
                    }
                    else
                    {
                        // We have received an empty BDO; update the control instance with the default HTML.
                        contentHtml = ns.StockTicker.DefaultHtml;
                    }
                }

                // Update the control instance with the resulting content HTML
                $('#pnpStockTicker').empty();
                $('#pnpStockTicker').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpStockTicker').empty();
                $('#pnpStockTicker').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // An unexpected error has occurred; update the control instance with the fallback Html.
        $('#pnpStockTicker').empty();
        $('#pnpStockTicker').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".StockTicker",
{
    _create: function ()
    {
        var divElement = this.element;
        ns.StockTicker.GetContents();
    }
});


