(function (window) {

    $.extend(window.officepnp, {

        // creates our development dashboard for the client side usage:
        // $pnp.dashboard.ready(function(db) { db.write('my section', 'my message'); });
        dashboard: {

            _dashboardReady: null,

            _sectionMap: {},

            ready: function (action) {

                var self = this;

                if (self._dashboardReady == null) {

                    // ensure we have the DOM then process the dashboard
                    self._dashboardReady = $.Deferred(function (def) {

                        $(function () {

                            var dashboard = $('#pnp-devdashboard');

                            if (dashboard.length > 0) {

                                // we already have one, probably from MDS - clear it.
                                dashboard.find('#pnp-devdashboard-contentLeft').empty();
                                dashboard.find('#pnp-devdashboard-contentRight').empty();
                                def.resolveWith(self, [self]);

                            } else {

                                // ensure we have the config and then add our stylesheet
                                $pnp.config.ready(function (c) {

                                    $pnp.core.loadStylesheet(c.ClientCDNUrlBase + '/css/pnpdevdashboard.css', true);

                                    $.get(c.ClientCDNUrlBase + '/static/pnpdevdashboard.html').done(function (response) {

                                        // add the markup to the body
                                        $('body').prepend($(response));

                                        // section navigation
                                        $('#pnp-devdashboard-contentLeft').on('click', 'div.pnp-devdashboard-sectionNav', function (e) {
                                            e.stopPropagation();
                                            $('#pnp-devdashboard-contentLeft').find('.pnp-devdashboard-sectionNav').removeClass('pnp-devdashboard-sectionNav-active');
                                            $('#pnp-devdashboard-contentRight').find('.pnp-devdashboard-sectionContent').hide();
                                            var source = $(this);
                                            source.addClass('pnp-devdashboard-sectionNav-active');
                                            $('#' + source.attr('pnp-sectionId')).show();
                                        });

                                        // need to bind up some event handlers to the button for displaying the dashboard
                                        $('#pnp-devdashboard-wrapper').hide();
                                        $('#pnp-devdashboard-launcher').show().on('click', function (e) {
                                            e.stopPropagation();
                                            var source = $(this);
                                            $('#pnp-devdashboard-wrapper').toggle('fast');
                                        });

                                        def.resolveWith(self, [self]);

                                    }).fail(function () {

                                        def.reject();
                                    });
                                });
                            }

                            // subscribe to the logging event for the dashboard if we are loading this module
                            if ($pnp.logging !== 'undefined') {
                                // add custom dev dashboard logging (another simple subscription example)
                                $pnp.logging.subscribe(function (e, args) {
                                    // and we will, if available, output to the dev dashboard
                                    if ($pnp.dashboard && $.isFunction($pnp.dashboard.ready)) {
                                        $pnp.dashboard.ready(function (db) {
                                            db.write(args.component, args.message);
                                        });
                                    }
                                });
                            }
                        });

                    }).promise();
                }

                if ($.isFunction(action)) {
                    self._dashboardReady.done(action);
                }

                return self._dashboardReady;
            },

            addSection: function (/*string*/ title) {

                var self = this;

                // we already have this section
                if (typeof self._sectionMap[title] !== 'undefined') {
                    return;
                }

                // create a random id for the sections to use when show/hide
                var sharedId = $pnp.core.getRandomString(6);

                // record our mapping
                self._sectionMap[title] = sharedId;

                // append nav to left side
                var newNavDiv = $('<div />').attr({ 'pnp-sectionId': sharedId }).addClass('pnp-devdashboard-sectionNav').text(title).appendTo($('#pnp-devdashboard-contentLeft'));

                // append content container to right panel
                var right = $('#pnp-devdashboard-contentRight');

                // figure out if we have other section containers yet
                var count = right.find('.pnp-devdashboard-sectionContent').length;

                // create and append the new section container
                var newDiv = $('<div />').attr({ id: sharedId, 'pnp-sectionTitle': title }).addClass('pnp-devdashboard-sectionContent').appendTo(right);

                // only show this one if it is the first one being added, otherwise hide it as we have no idea what state the UI is in
                if (count > 0) {
                    newDiv.hide();
                } else {
                    newNavDiv.addClass('pnp-devdashboard-sectionNav-active');
                }
            },

            // writes a new entry to the dashboard, adding the section if it does not exist
            write: function (/*string*/ sectionTitle, /*string (html allowed) */ message) {

                var self = this;

                var section = $('#' + self._sectionMap[sectionTitle]);

                // go ahead and add this section if it doesn't exist
                if (section.length < 1) {
                    self.addSection(sectionTitle);
                    section = $('#' + self._sectionMap[sectionTitle]);
                }

                $('<div />').attr({ 'pnp-timestamp': (new Date()).toISOString() }).addClass('pnp-devdashboard-sectionEntry').html(message).appendTo(section);
            },

            builtin: {

                // dumps the _spPageContextInfo object and current $pnp.correlationId
                pageInfo: function () {

                    $pnp.dashboard.ready(function (db) {

                        var section = 'Page Information';
                        db.addSection(section);
                        db.write(section, 'Correlation Id: ' + $pnp.correlationId);

                        var list = [];
                        list.push('<ul><li style="font-weight:bold;">_spPageContextInfo<ul>');

                        for (var p in _spPageContextInfo) {
                            if (!$.isFunction(_spPageContextInfo[p])) {
                                list.push('<li>' + p + ': ' + _spPageContextInfo[p] + '</li>');
                            }
                        }

                        list.push('</ul></li></ul>');
                        db.write(section, list.join(''));
                    });
                },

                // dumps all the values from localStorage
                localStorageDump: function () {

                    $pnp.dashboard.ready(function (db) {

                        var section = 'Local Storage';
                        db.addSection(section);

                        if ($pnp.caching.enabled) {

                            for (var i = 0; i < localStorage.length; i++) {
                                var key = localStorage.key(i);
                                var o = localStorage.getItem(key);
                                db.write(section, '<span style="font-weight: bold;">' + key + '</span>:<br />' + o);
                            }

                        } else {

                            db.write(section, 'No local storage support.');
                        }
                    });
                },

                // dumps all the values from localStorage
                sessionStorageDump: function () {

                    $pnp.dashboard.ready(function (db) {

                        var section = 'Session Storage';
                        db.addSection(section);

                        if ($pnp.session.enabled) {

                            for (var i = 0; i < sessionStorage.length; i++) {
                                var key = sessionStorage.key(i);
                                var o = sessionStorage.getItem(key);
                                db.write(section, '<span style="font-weight: bold;">' + key + '</span>:<br />' + o);
                            }

                        } else {

                            db.write(section, 'No session storage support.');
                        }
                    });
                }
            }
        }
    });

    // include the standard information
    $pnp.dashboard.builtin.pageInfo();

    // dump the local storage
    $pnp.dashboard.builtin.localStorageDump();

    // dump the session storage
    $pnp.dashboard.builtin.sessionStorageDump();

})(window);