(function ($) {

    $.fn.addSPChrome = function (options) {

        //setup default settings
        var settings = $.extend({
            hostUrl: $app.getUrlParamByName('SPHostUrl'),
            helpUrl: '',
            appTitle: 'My App',
            appIcon: '',
            settingsLinks: []
        }, options);

        return this.each(function () {

            var target = $(this);

            var scriptbase = settings.hostUrl + '/_layouts/15/';

            $.getScript(scriptbase + 'SP.UI.Controls.js', function () {

                var options = {
                    "appIconUrl": settings.appIcon,
                    "appTitle": settings.appTitle,
                    "appHelpPageUrl": settings.helpUrl,
                    "settingsLinks": settings.settingsLinks
                };

                var nav = new SP.UI.Controls.Navigation(target.attr('id'), options);

                nav.setVisible(true);
            });
        });
    };
})(jQuery);