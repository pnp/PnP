'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

//Featured links
ns.UserInfo = function () { };

// How long shall we cache the HTML response? (specify a Prime Number)
ns.UserInfo.ExpirationTimeoutInMinutes = "17";

// Default HTML for the control
// - the HTML output is suitable for direct insertion into the control
ns.UserInfo.DefaultHtml = "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">Hello User !!</a></li>";

// Constructs the entire HTML rendering of the Control and injects the HTML into the control
ns.UserInfo.GetContents = function ()
{
    // Save the current control content as the fallback content in case we encounter any errors during processing; we will simply re-render the current content.
    var currentHtml = $('#pnpUserInfoMenu').html();
    var fallbackHtml = currentHtml;

    // If current control content is not present, use default Html; optionally, insert a progress indicator while we request the BDO for the control. 
    if (currentHtml == "")
    {
        // Since current control content is not present, use the Default Html as the fallback content.
        fallbackHtml = ns.UserInfo.DefaultHtml;

        //----------------------------------------------------------------------------
        // Note: DISABLE this code block if you do not wish to present a progress indicator...
        //----------------------------------------------------------------------------
        // insert a progress indicator while we request the BDO for the control. 
        $('#pnpUserInfoMenu').empty();
        $('#pnpUserInfoMenu').append(
            "<li class=\"ms-core-defaultFont\" role=\"presentation\">" +
              "<a tabindex=\"-1\" role=\"menuitem\" href=\"#\"><img src=\"" + ns.Configuration.PortalCdnUrl + "/images/loading.gif\" alt=\"loading...\"/>&nbsp;Working on it...</a>" +
            "</li>"
            );
        //----------------------------------------------------------------------------
    }

    try
    {
        // Request the BDO for the control. Note that we use SessionStorage for UserInfo -- its content is personalized/private
        ns.BusinessDataManager.GetUserInfoData({ storageMode: ns.StorageManager.SessionStorageMode, useSlidingExpiration: false, timeout: ns.UserInfo.ExpirationTimeoutInMinutes }).then(

            function (userInfoData)
            {
                //  construct the content HTML for the control using the data returned in the BDO
                var contentHtml = "";

                if (userInfoData == null || userInfoData.Type == ns.BusinessDataManager.ErrorDataType)
                {
                    // For some reason, a valid BDO was not returned; update the control instance with the fallback Html.
                    contentHtml = fallbackHtml;
                }
                else
                {
                    // We have received a valid BDO; update the control instance with the BDO data.
                    if (userInfoData.Name != null && userInfoData.Name.length > 0)
                    {
                        var welcomeName = (userInfoData.First != null && userInfoData.First.length > 0) ? userInfoData.First : userInfoData.Name;
                        contentHtml = "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">Hello " + welcomeName + " !!</a></li>";

                        contentHtml += "<li class=\"divider\" style=\"margin:5px 0;\" role=\"presentation\"></li>";

                        if (userInfoData.Name != null && userInfoData.Name.length > 0) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">" + userInfoData.Name + "</a></li>";
                        }
                        if (userInfoData.Title != null && userInfoData.Title.length > 0) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">" + userInfoData.Title + "</a></li>";
                        }
                        if (userInfoData.Dept != null && userInfoData.Dept.length > 0) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">" + userInfoData.Dept + "</a></li>";
                        }
                        if (userInfoData.Email != null && userInfoData.Email.length > 0) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">" + userInfoData.Email + "</a></li>";
                        }
                        if (userInfoData.Phone != null && userInfoData.Phone.length > 0) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"#\">" + userInfoData.Phone + "</a></li>";
                        }
                        if (userInfoData.ProfileUrl != null)
                        {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"" + userInfoData.ProfileUrl + "\">My Profile</a></li>";
                        }
                        if (userInfoData.OneDriveUrl != null) {
                            contentHtml += "<li class=\"ms-core-defaultFont\" role=\"presentation\"><a tabindex=\"-1\" role=\"menuitem\" href=\"" + userInfoData.OneDriveUrl + "\">My OneDrive</a></li>";
                        }
                    }
                    else
                    {
                        // We have received an empty BDO; update the control instance with the default HTML.
                        contentHtml = ns.UserInfo.DefaultHtml;
                    }
                }

                // Update the control instance with the resulting HTML
                $('#pnpUserInfoMenu').empty();
                $('#pnpUserInfoMenu').append(contentHtml);
            },

            function (sender, args)
            {
                ns.LogError(args.get_message());

                // The BDO request failed; update the control instance with the fallback Html.
                $('#pnpUserInfoMenu').empty();
                $('#pnpUserInfoMenu').append(fallbackHtml);
            }
        );
    }
    catch (ex)
    {
        // The BDO request failed; update the control instance with the fallback Html.
        $('#pnpUserInfoMenu').empty();
        $('#pnpUserInfoMenu').append(fallbackHtml);
    }
}

// Create an instance of the class and render the contents of the associated control
$.widget(ns + ".UserInfo",
{
    _create: function () {
        // We do nothing when the control is created; we instead await the onClick() event...
    }
});


