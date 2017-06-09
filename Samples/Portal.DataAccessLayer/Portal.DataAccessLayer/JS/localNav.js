'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

ns.LocalNav = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.LocalNav.ExpirationTimeoutInMinutes = "5";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.LocalNav.DefaultHtml =
   "<h3>Local Navigation</h3>" +
   "<ul style=\"margin:0px; padding:0px; list-style:none;\">" +
    "<li><a href=\"/\">DEFAULT LOCAL LINK 1</a></li>" +
    "<li><a href=\"/\">DEFAULT LOCAL LINK 2</a></li>" +
    "<li><a href=\"/\">DEFAULT LOCAL LINK 3</a></li>" +
    "<li><a href=\"/\">DEFAULT LOCAL LINK 4</a></li>" +
    "<li><a href=\"/\">DEFAULT LOCAL LINK 5</a></li>" +
   "</ul>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.LocalNav.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpLocalNav').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.LocalNav.DefaultHtml;
    
        //----------------------------------------------------------------------------
        // Note: ENABLE this code block if you wish to present a progress indicator...
        //----------------------------------------------------------------------------
        //// insert a progress indicator while we request the BDO for the control. 
        //$('#pnpLocalNav').empty();
        //$('#pnpLocalNav').append("<h3>Local Navigation</h3>" + "<p><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</p>");
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. We use DurableStorage for Local Nav Links -- its content is not personalized/private
        ns.BusinessDataManager.GetLocalNavData({ storageMode: ns.StorageManager.DurableStorageMode, useSlidingExpiration: false, timeout: ns.LocalNav.ExpirationTimeoutInMinutes }).then(

            function (localNavData)
            {
                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml =
                    "<h3>Local Navigation</h3>" +
                     "<ul style=\"margin:0px; padding:0px; list-style:none;\">";

                if (localNavData == null || localNavData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (localNavData.Nodes.length > 0)
                    {
                        var listItemInfo = '';
                        $.each(localNavData.Nodes, function ()
                        {
                            var linkText = this.Title;
                            var linkUrl = this.Url;
                            listItemInfo += "<li>" + "<a href=\"" + linkUrl + "\">" + linkText + "</a>" + "</li>";
                        });
                        contentHtml += listItemInfo.toString();
                        contentHtml += "</ul>";
                    }
                    else
                    {
                        // We have received an empty BDO; update the control instance with the default HTML.
                        contentHtml = ns.LocalNav.DefaultHtml;
                    }
                }

                // Update the control instance with the resulting content HTML
                $('#pnpLocalNav').empty();
                $('#pnpLocalNav').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpLocalNav').empty();
                $('#pnpLocalNav').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // The BDO request failed; update the control instance with the fallback Html.
        $('#pnpLocalNav').empty();
        $('#pnpLocalNav').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".LocalNav",
{
    _create: function ()
    {
        var divElement = this.element;
        ns.LocalNav.GetContents();
    }
});


