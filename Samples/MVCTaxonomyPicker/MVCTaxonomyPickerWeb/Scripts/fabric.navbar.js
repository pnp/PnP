// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE in the project root for license information.

/**
 * Nav Bar Plugin
 */
$(document).ready(function () {
    var $navBar = $(".ms-NavBar");

    // Open the nav bar on mobile.
    $navBar.on('click', '.js-openMenu', function (event) {
        event.stopPropagation();
        $navBar.toggleClass('is-open');
    });

    // Close the nav bar on mobile.
    $navBar.click(function () {
        if ($navBar.hasClass('is-open')) {
            $navBar.removeClass('is-open');
        }
    });

    // Configure open/close menus.
    $navBar.on('click', '.ms-NavBar-item:not(.is-disabled)', function (event) {
        event.stopPropagation();

        // Prevent default actions from firing if links are not found.
        if ($(this).children('.ms-NavBar-link').length === 0) {
            event.preventDefault();
        }

        // Does the selected item have a menu?
        if ($(this).hasClass('ms-NavBar-item--hasMenu')) {

            // Toggle 'is-open' to open or close it.
            $(this).children('.ms-ContextualMenu:first').toggleClass('is-open');

            // Toggle 'is-selected' to indicate whether it is active.
            $(this).toggleClass('is-selected');
        } else {
            // Close the submenu and any open contextual menus.
            $navBar.removeClass('is-open').find('.ms-ContextualMenu').removeClass('is-open');
        }
    });

    // Prevent contextual menus from being hidden when clicking on them.
    $navBar.on('click', '.ms-NavBar-item .ms-ContextualMenu', function (event) {
        event.stopPropagation();

        // Collapse the mobile "panel" for nav items.
        $(this).removeClass('is-open');
        $navBar.removeClass('is-open').find('.ms-NavBar-item--hasMenu').removeClass('is-selected');
    });

    // Hide any menus and close the search box when clicking anywhere in the document.
    $(document).on('click', 'html', function (event) {
        $navBar.find('.ms-NavBar-item').removeClass('is-selected').find('.ms-ContextualMenu').removeClass('is-open');
    });
});