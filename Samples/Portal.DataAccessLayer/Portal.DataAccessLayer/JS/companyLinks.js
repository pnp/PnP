'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

ns.CompanyLinks = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.CompanyLinks.ExpirationTimeoutInMinutes = "7";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.CompanyLinks.DefaultHtml =
   "<h3>Company Links</h3>" +
   "<ul style=\"margin:0px; padding:0px; list-style:none;\">" +
    "<li><a href=\"/\">DEFAULT COMPANY LINK 1</a></li>" +
    "<li><a href=\"/\">DEFAULT COMPANY LINK 2</a></li>" +
    "<li><a href=\"/\">DEFAULT COMPANY LINK 3</a></li>" +
    "<li><a href=\"/\">DEFAULT COMPANY LINK 4</a></li>" +
    "<li><a href=\"/\">DEFAULT COMPANY LINK 5</a></li>" +
   "</ul>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.CompanyLinks.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpCompanyLinks').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.CompanyLinks.DefaultHtml;
    
        //----------------------------------------------------------------------------
        // Note: ENABLE this code block if you wish to present a progress indicator...
        //----------------------------------------------------------------------------
        //// insert a progress indicator while we request the BDO for the control. 
        //$('#pnpCompanyLinks').empty();
        //$('#pnpCompanyLinks').append("<h3>Company Quick Links</h3>" + "<p><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</p>");
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. We use DurableStorage for Company Links -- its content is not personalized/private
        ns.BusinessDataManager.GetCompanyLinksData({ storageMode: ns.StorageManager.DurableStorageMode, useSlidingExpiration: false, timeout: ns.CompanyLinks.ExpirationTimeoutInMinutes }).then(

            function (companyLinksData)
            {
                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml =
                    "<h3>Company Links</h3>" +
                     "<ul style=\"margin:0px; padding:0px; list-style:none;\">";

                if (companyLinksData == null || companyLinksData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (companyLinksData.Nodes.length > 0)
                    {
                        var listItemInfo = '';
                        $.each(companyLinksData.Nodes, function ()
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
                        contentHtml = ns.CompanyLinks.DefaultHtml;
                    }
                }

                // Update the control instance with the resulting content HTML
                $('#pnpCompanyLinks').empty();
                $('#pnpCompanyLinks').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpCompanyLinks').empty();
                $('#pnpCompanyLinks').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // The BDO request failed; update the control instance with the fallback Html.
        $('#pnpCompanyLinks').empty();
        $('#pnpCompanyLinks').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".CompanyLinks",
{
    _create: function ()
    {
        var divElement = this.element;
        ns.CompanyLinks.GetContents();
    }
});


