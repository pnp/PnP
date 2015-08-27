// Create unique namespace
var jslinkTemplates = window.jslinkTemplates || {};

jslinkTemplates.Announcements = {};
jslinkTemplates.Announcements.Accordion = function () {
    var _onPreRender = function (ctx) {
        // Load css file
        loadCss(ctx.HttpRoot + '/Style Library/OfficeDevPnP/Branding.JSLink/Styles/accordion.css');
    };

    var _item = function (ctx) {
        var html = '';

        html += '<li class="accordion-item">';
        html += '<a class="accordion-item-link" href="' + ctx.displayFormUrl + "&ID=" + ctx.CurrentItem.ID + '">';
        html += '<span class="accordion-item-image"></span>';
        html += '<span class="accordion-item-title">' + ctx.CurrentItem.Title + '</span>';
        html += '</a>';
        html += '<div class="accordion-item-container">';
        html += '<p>' + ctx.CurrentItem.Body + '</p>';
        html += '</div></li>';

        return html;
    };

    var _onPostRender = function (ctx) {
        $('.accordion-item-link').click(function (e) {
            // Grab the parent
            var currentAccordionItem = $(this).parent();

            // Check if it is selected or not
            if (!currentAccordionItem.hasClass('selected')) {
                // Set the item to selected
                currentAccordionItem.addClass('selected');

                // Slide the container down and set it to open
                currentAccordionItem.find('.accordion-item-container').slideDown(300).addClass('open');
            }
            else {
                // Remove selected from item
                currentAccordionItem.removeClass('selected');

                // Slide the container up and remove the open class
                currentAccordionItem.find('.accordion-item-container').slideUp(300).removeClass('open');
            }

            e.preventDefault();
        });
    };

    return {
        "item": _item,
        "onPreRender": _onPreRender,
        "onPostRender": _onPostRender
    }
}();

function loadCss(url) {
    var link = document.createElement('link');
    link.href = url;
    link.rel = 'stylesheet';
    document.getElementsByTagName('head')[0].appendChild(link);
}