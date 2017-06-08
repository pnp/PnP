'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

//Featured links
ns.Footer = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.Footer.ExpirationTimeoutInMinutes = "11";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.Footer.DefaultHtml =
    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\">" +
     "<tbody style=\"float:right;\">" +
      "<tr>" +
       "<td align=\"center\" style=\"\">" +
        "<a href=\"/\" style=\"color: white\">DEFAULT FOOTER LINK 1</a> | " +
        "<a href=\"/\" style=\"color: white\">DEFAULT FOOTER LINK 2</a> | " +
        "<a href=\"/\" style=\"color: white\">DEFAULT FOOTER LINK 3</a> | " +
        "<a href=\"/\" style=\"color: white\">DEFAULT FOOTER LINK 4</a> | " +
        "<a href=\"/\" style=\"color: white\">DEFAULT FOOTER LINK 5</a> " +
       "</td>" +
      "</tr>" +
      "<tr>" +
       "<td align=\"center\" style=\"\">" +
        "2017 Contoso, Inc. All Rights Reserved." +
       "</td>" +
      "</tr>" +
     "</tbody>" +
    "</table>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.Footer.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpFooter').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.Footer.DefaultHtml;
    
        //----------------------------------------------------------------------------
        // Note: ENABLE this code block if you wish to present a progress indicator...
        //----------------------------------------------------------------------------
        //// insert a progress indicator while we request the BDO for the control. 
        //$('#pnpFooter').empty();
        //$('#pnpFooter').append("<p><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</p>");
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. We use DurableStorage for Footer Nav -- its content is not personalized/private
        ns.BusinessDataManager.GetFooterData({ storageMode: ns.StorageManager.DurableStorageMode, useSlidingExpiration: false, timeout: ns.Footer.ExpirationTimeoutInMinutes }).then(

            function (footerData)
            {

                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml = "";

                if (footerData == null || footerData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (footerData.Html != null && footerData.Html.length > 0)
                    {
                        contentHtml = footerData.Html;
                    }
                    else
                    {
                        // We have received an empty BDO; update the control instance with the default HTML.
                        contentHtml = ns.Footer.DefaultHtml;
                    }
                }

                // Update the control instance with the resulting HTML
                $('#pnpFooter').empty();
                $('#pnpFooter').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpFooter').empty();
                $('#pnpFooter').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // The BDO request failed; update the control instance with the fallback Html.
        $('#pnpFooter').empty();
        $('#pnpFooter').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".Footer",
{
    _create: function ()
    {
        ns.Footer.GetContents();
    }
});


