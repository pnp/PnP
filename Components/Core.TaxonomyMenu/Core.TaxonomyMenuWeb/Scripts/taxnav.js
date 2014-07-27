'use strict';

(function ($, undefined) {
    var html = "",

    getNavItems = function () {

        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            'use strict';
            SP.SOD.executeFunc('SP.js', 'SP.ClientContext');
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                SP.SOD.registerSod('sp.taxonomy.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.taxonomy.js'));
                SP.SOD.executeFunc('sp.taxonomy.js', false, function () {
                    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                        SP.SOD.registerSod('sp.userprofiles.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.userprofiles.js'));
                        SP.SOD.executeFunc('sp.userprofiles.js', false, Function.createDelegate(this, getUserLanguage));
                    }, 'sp.js');
                });
            }, 'sp.js');
        }, 'core.js');
    },

    getUserLanguage = function () {

        var targetUser = "i:0#.f|membership|" + _spPageContextInfo.userLoginName;
        var context = new SP.ClientContext.get_current();
        var peopleManager = new SP.UserProfiles.PeopleManager(context);
        var userProperty = peopleManager.getUserProfilePropertyFor(targetUser, "SPS-MUILanguages");

        context.executeQueryAsync(function () {
            var lcid = 1033;
            var lang = userProperty.m_value.split(',')[0].split('-')[0];
            var lcids = {
                "en": 1033,
                "de": 1031,
                "fr": 1036,
                "sv": 1053
            };

            if (lcids[lang.toLowerCase()] != undefined) {
                lcid = lcids[lang.toLowerCase()];
            }

            getTaxonomy(lcid);
        });
    },

    getTaxonomy = function (userLCID) {
        var nid = SP.UI.Notify.addNotification("<img src='/_layouts/15/images/loadingcirclests16.gif?rev=23' style='vertical-align:bottom; display:inline-block; margin-" + (document.documentElement.dir == "rtl" ? "left" : "right") + ":2px;' />&nbsp;<span style='vertical-align:top;'>Loading navigation...</span>", false);

        var context = SP.ClientContext.get_current();
        var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
        var termStores = taxSession.get_termStores();
        var termStore = taxSession.getDefaultSiteCollectionTermStore();
        var termSet = termStore.getTermSet("56ca0eea-635e-4cc1-ac35-fc2040f4cfe5");

        context.load(termStore);
        context.executeQueryAsync(function () {

            if (termSet.get_serverObjectIsNull()) {
                $('#DeltaTopNavigation').html('<span style="color:red">Taxonomy Navigation: term set missing in term store</span>');
                SP.UI.Notify.removeNotification(nid);
            }
            else {

                var lcid = 1033;
                if (termStore.get_languages().indexOf(userLCID) > -1) {
                    lcid = userLCID;
                }

                var terms = termSet.getAllTerms();
                context.load(terms);
                context.executeQueryAsync(function () {

                    var termItems = [];
                    var termLabels = [];
                    var termEnumerator = terms.getEnumerator();
                    while (termEnumerator.moveNext()) {
                        var currentTerm = termEnumerator.get_current();
                        var label = currentTerm.getDefaultLabel(lcid);

                        termItems.push(currentTerm);
                        termLabels.push(label);
                        context.load(currentTerm);
                    }

                    context.executeQueryAsync(function () {

                        html += "<ul style='margin-top: 0px; margin-bottom: 0px;'>"
                        for (var i in termItems) {
                            var term = termItems[i];
                            var termLabel = termLabels[i];
                            var linkName = termLabel.get_value() != 0 ? termLabel.get_value() : term.get_name();
                            var linkUrl = term.get_localCustomProperties()['_Sys_Nav_SimpleLinkUrl'];

                            html += "<li style='display: inline;list-style-type: none; padding-right: 20px;'><a href='" + linkUrl + "'>" + linkName + "</a></li>";
                        }
                        html += "</ul>";

                        $('#DeltaTopNavigation').html(html);
                        SP.UI.Notify.removeNotification(nid);

                    }, function (sender, args) {
                        alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                    });
                }, function (sender, args) {
                    alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                });
            }
        }, function (sender, args) {
            alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        });
    },

    onReady = function () {
        getNavItems();
    };

    $(document).on({
        ready: onReady
    });
})(jQuery);