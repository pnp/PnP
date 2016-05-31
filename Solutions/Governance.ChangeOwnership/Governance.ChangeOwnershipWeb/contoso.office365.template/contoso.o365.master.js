$(document).on('click', "div.ms-Icon-base", function () {
    if ($(this).hasClass('ms-Icon--waffle2')) {
        resetAllMenuButton();
        $(this).removeClass('ms-Icon--waffle2');
        $(this).addClass('ms-Icon--waffle2-selected');
        $(this).addClass('o365cs-spo-topbarMenuOpen');
    } else if ($(this).hasClass('ms-Icon--waffle2-selected')) {
        $(this).removeClass('ms-Icon--waffle2-selected');
        $(this).addClass('ms-Icon--waffle2');
        $(this).removeClass('o365cs-spo-topbarMenuOpen');
    } else if ($(this).hasClass('ms-Icon--gear')) {
        resetAllMenuButton();
        $(this).removeClass('ms-Icon--gear');
        $(this).addClass('ms-Icon--gear-selected');
        $(this).addClass('o365cs-spo-topbarMenuOpen');
        //$('#O365_MainLink_Settings_ContextMenu').show();
        showSettingContextMenu();
    } else if ($(this).hasClass('ms-Icon--gear-selected')) {
        $(this).removeClass('ms-Icon--gear-selected');
        $(this).addClass('ms-Icon--gear');
        $(this).removeClass('o365cs-spo-topbarMenuOpen');
        $('#O365_MainLink_Settings_ContextMenu').hide();
    } else if ($(this).hasClass('ms-Icon--bell')) {
        resetAllMenuButton();
        $(this).removeClass('ms-Icon--bell');
        $(this).addClass('ms-Icon--bell-selected');
        $(this).addClass('o365cs-spo-topbarMenuOpen');
    } else if ($(this).hasClass('ms-Icon--bell-selected')) {
        $(this).removeClass('ms-Icon--bell-selected');
        $(this).addClass('ms-Icon--bell');
        $(this).removeClass('o365cs-spo-topbarMenuOpen');
    } else if ($(this).hasClass('ms-Icon--question')) {
        resetAllMenuButton();
        $(this).removeClass('ms-Icon--question');
        $(this).addClass('ms-Icon--question-selected');
        $(this).addClass('o365cs-spo-topbarMenuOpen');
        $('#o365cs-flexpane-overlay').show(100);
    } else if ($(this).hasClass('ms-Icon--question-selected')) {
        $(this).removeClass('ms-Icon--question-selected');
        $(this).addClass('ms-Icon--question');
        $(this).removeClass('o365cs-spo-topbarMenuOpen');
        $('#o365cs-flexpane-overlay').hide(100);
    } else if ($(this).hasClass('ms-Icon--userprofile')) {
        resetAllMenuButton();
        $(this).removeClass('ms-Icon--userprofile');
        $(this).addClass('ms-Icon--userprofile-selected');
        $(this).addClass('o365cs-spo-topbarMenuOpen');
        //$('#O365_MainLink_CurrentUser_ContextMenu').show();
        showCurrentUserContextMenu();
    } else if ($(this).hasClass('ms-Icon--userprofile-selected')) {
        $(this).removeClass('ms-Icon--userprofile-selected');
        $(this).addClass('ms-Icon--userprofile');
        $(this).removeClass('o365cs-spo-topbarMenuOpen');
        $('#O365_MainLink_CurrentUser_ContextMenu').hide();
    }
});

function resetAllMenuButton() {

    $('.ms-Icon-base').each(function () {

        if ($(this).hasClass('ms-Icon--waffle2-selected')) {
            $(this).removeClass('ms-Icon--waffle2-selected');
            $(this).addClass('ms-Icon--waffle2');
            $(this).removeClass('o365cs-spo-topbarMenuOpen');
        } else if ($(this).hasClass('ms-Icon--gear-selected')) {
            $(this).removeClass('ms-Icon--gear-selected');
            $(this).addClass('ms-Icon--gear');
            $(this).removeClass('o365cs-spo-topbarMenuOpen');
        } else if ($(this).hasClass('ms-Icon--bell-selected')) {
            $(this).removeClass('ms-Icon--bell-selected');
            $(this).addClass('ms-Icon--bell');
            $(this).removeClass('o365cs-spo-topbarMenuOpen');
        } else if ($(this).hasClass('ms-Icon--question-selected')) {
            $(this).removeClass('ms-Icon--question-selected');
            $(this).addClass('ms-Icon--question');
            $(this).removeClass('o365cs-spo-topbarMenuOpen');
        } else if ($(this).hasClass('ms-Icon--userprofile-selected')) {
            $(this).removeClass('ms-Icon--userprofile-selected');
            $(this).addClass('ms-Icon--userprofile');
            $(this).removeClass('o365cs-spo-topbarMenuOpen');
        }
    });

    $('#o365cs-flexpane-overlay').hide();
    $('#O365_MainLink_Settings_ContextMenu').hide();
    $('#O365_MainLink_CurrentUser_ContextMenu').hide();
}

$(document).ready(function () {

    $('#s4-workspace').css({ 'height': ($(window).height() - 85) + 'px', 'width': $(window).width() + 'px' });

    //$('#O365_MainLink_Settings_ContextMenu').css({ left: $('#O365_MainLink_Help').position().left - $('#O365_MainLink_Settings_ContextMenu').width() });

    //$('#O365_MainLink_CurrentUser_ContextMenu').css({ left: $(window).width() - $('#O365_MainLink_CurrentUser_ContextMenu').width() });

    setMasterPageLinks();

});

$(window).resize(function () {

    $('#s4-workspace').css({ 'height': ($(window).height() - 85) + 'px', 'width': $(window).width() + 'px' });
    $('#O365_MainLink_Settings_ContextMenu').css({ left: $('#O365_MainLink_Help').position().left - $('#O365_MainLink_Settings_ContextMenu').width() });
    $('#O365_MainLink_CurrentUser_ContextMenu').css({ left: $(window).width() - $('#O365_MainLink_CurrentUser_ContextMenu').width() });
});

function showSettingContextMenu() {

    $('#O365_MainLink_Settings_ContextMenu').css({ left: $('#O365_MainLink_Help').position().left - $('#O365_MainLink_Settings_ContextMenu').width() });
    $('#O365_MainLink_Settings_ContextMenu').show();
}

function showCurrentUserContextMenu() {
    $('#O365_MainLink_CurrentUser_ContextMenu').css({ left: $(window).width() - $('#O365_MainLink_CurrentUser_ContextMenu').width() });
    $('#O365_MainLink_CurrentUser_ContextMenu').show();
}

function setMasterPageLinks() {

    var spoSiteUrl = $("input[id$=HiddenField_Master_CurrentSiteUrl]").val();
    if (spoSiteUrl == null || spoSiteUrl == '') spoSiteUrl = 'undefined';

    var currentUserName = $("input[id$=HiddenField_Master_CurrentUserName]").val();
    var currentUserEmail = $("input[id$=HiddenField_Master_CurrentUserEmail]").val();
    var spoSiteTitle = $("input[id$=HiddenField_Master_CurrentSiteTitle]").val();
    var spoAppTitle = $("input[id$=HiddenField_Master_PageTitle]").val();
    var spoShortAppTitle = $("input[id$=HiddenField_Master_ShortPageTitle]").val();

    $('#O365_MainLink_NavMenu').click(function () {
        window.location.replace("https://contosonterprise-my.sharepoint.com/_layouts/15/MySite.aspx?MySiteRedirect=AllSites");
        //if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl);
    });

    $('#LeftNavBar_QuickLaunchNavigationManager_Home_Link').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl);
    });

    $('#LeftNavBar_QuickLaunchNavigationManager_Notebook_Link').click(function () {
    });

    $('#LeftNavBar_QuickLaunchNavigationManager_Document_Link').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/Shared%20Documents/Forms/AllItems.aspx');
    });

    $('#LeftNavBar_QuickLaunchNavigationManager_SiteConetent_Link').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/viewlsts.aspx');
    });

    $('#LeftNavBar_QuickLaunchNavigationManager_RecycleBin_Link').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/AdminRecycleBin.aspx?ql=1');
    });

    $('#siteicon_onetidProjectPropertyTitleGraphic').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl);
    });

    $('#O365_SubLink_SuiteMenu_zz7_MenuItem_Create').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/addanapp.aspx');
    });

    $('#O365_SubLink_SuiteMenu_zz8_MenuItem_ViewAllSiteContents').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/viewlsts.aspx');
    });

    $('#O365_SubLink_SuiteMenu_zz9_MenuItem_ChangeTheLook').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/designgallery.aspx');
    });

    $('#O365_SubLink_SuiteMenu_zz10_MenuItem_Settings').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/settings.aspx');
    });

    $('#O365_SubLink_SuiteMenu_ctl00_SiteActionsMenuMainData_ctl00_ctl04').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/GettingStarted.aspx');
    });

    $('#DeltaPlaceHolderPageTitleInTitleArea_SiteSettings_Link').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/start.aspx#/_layouts/15/settings.aspx');
    });

    $('#O365_SubLink_ShellSignout').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl + '/_layouts/15/SignOut.aspx?signoutlive=1');
    });

    $('#o365_current_user').text(currentUserName);
    $('#o365_current_user_name').text(currentUserName);
    $('#o365_current_user_email').text(currentUserEmail);
    $('#hori_nav_app_title').text(spoAppTitle);
    $('#vert_nav_app_title').text(spoShortAppTitle);

    $('#TopNavigationMenuV4_NavMenu_RootSite').text(spoSiteTitle);
    $('#TopNavigationMenuV4_NavMenu_RootSite').click(function () {
        if (spoSiteUrl != 'undefined') window.location.replace(spoSiteUrl);
    });
}