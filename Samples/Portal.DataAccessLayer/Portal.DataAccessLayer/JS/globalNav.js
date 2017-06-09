'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

ns.GlobalNav = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.GlobalNav.ExpirationTimeoutInMinutes = "13";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.GlobalNav.DefaultHtml =
   "<ul class=\"ms-core-suiteLinkList\">" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 1</a></li>" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 2</a></li>" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 3</a></li>" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 4</a></li>" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 5</a></li>" +
    "<li class=\"ms-core-suiteLink\"><a class=\"ms-core-suiteLink-a\" href=\"/\">DEFAULT GLOBAL LINK 6</a></li>" +
   "</ul>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.GlobalNav.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpGlobalNav').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.GlobalNav.DefaultHtml;
    
        ////----------------------------------------------------------------------------
        //// Note: ENABLE this code block if you wish to present a progress indicator...
        ////----------------------------------------------------------------------------
        //// insert a progress indicator while we request the BDO for the control. 
        //$('#pnpGlobalNav').empty();
        //$('#pnpGlobalNav').append(
        //  "<ul class=\"ms-core-suiteLinkList\">" + 
        //    "<li class=\"ms-core-suiteLink\">" +
        //      "<a class=\"ms-core-suiteLink-a\" href=\"/\"><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</a>"); +
        //    "</li>" +
        //  "</ul>";
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. We use DurableStorage for Global Nav -- its content is not personalized/private
        ns.BusinessDataManager.GetGlobalNavData({ storageMode: ns.StorageManager.DurableStorageMode, useSlidingExpiration: false, timeout: ns.GlobalNav.ExpirationTimeoutInMinutes }).then(

            function (globalNavData)
            {
                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml = "<ul class=\"ms-core-suiteLinkList\">";

                if (globalNavData == null || globalNavData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (globalNavData.Nodes.length > 0)
                    {
                        var listItemInfo = "";
                        $.each(globalNavData.Nodes, function ()
                        {
                            var linkText = this.Title;
                            var linkUrl = this.Url;
                            listItemInfo +=
                             "<li class=\"ms-core-suiteLink\">" +
                              "<a class=\"ms-core-suiteLink-a\" href=\"" + linkUrl + "\">" + linkText + "</a>" +
                             "</li>";
                        });
                        contentHtml += listItemInfo.toString();
                        contentHtml += "</ul>";
                    }
                    else
                    {
                        // We have received an empty BDO; update the control instance with the default HTML.
                        contentHtml = ns.GlobalNav.DefaultHtml
                    }
                }

                // Update the control instance with the resulting content HTML
                $('#pnpGlobalNav').empty();
                $('#pnpGlobalNav').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpGlobalNav').empty();
                $('#pnpGlobalNav').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // The BDO request failed; update the control instance with the fallback Html.
        $('#pnpGlobalNav').empty();
        $('#pnpGlobalNav').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".GlobalNav",
{
    _create: function ()
    {
        var divElement = this.element;
        ns.GlobalNav.GetContents();
    }
});


