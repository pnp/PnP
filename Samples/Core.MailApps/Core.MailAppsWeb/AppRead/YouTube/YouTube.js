/// <reference path="../App.js" />

(function () {
    "use strict";

    // The Office initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();
            displayVideoPanel();
        });
    };

    // Displays the "Subject" and "From" fields, based on the current mail item
    function displayVideoPanel() {
        var item = Office.context.mailbox.item;

        var entities = item.getEntities();
        if (entities.urls == null || entities.urls.length == 0)
            return;

        var pattern = /v\=(\w+)/i;

        for (var i = 0; i < entities.urls.length; i++) {
            var url = entities.urls[i].toString();
            var matches = pattern.exec(url);
            if (matches != null) {
                var videoId = matches[1];
                $('#content-tabs').append('<div class="content-tab" data-videoId="' + videoId + '">Video ' + (i + 1) + '</div>');
            }
        }

        $('.content-tab').click(function () {
            var videoId = $(this).data('videoid');
            $('#content-tabs .selected').removeClass('selected');
            $(this).addClass('selected');
            displayVideo(videoId);
        });
        $('.content-tab:first').click();
    }

    function displayVideo(videoId) {
        $('#videoPlayer').attr('src','//www.youtube.com/embed/' + videoId);
        $('#videoLink').attr('href', '//www.youtube.com/watch?v=' + videoId);
    }
})();