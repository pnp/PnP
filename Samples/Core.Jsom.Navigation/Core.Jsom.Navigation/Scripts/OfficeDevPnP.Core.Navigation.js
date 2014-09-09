'use strict';

var OfficeDevPnP = OfficeDevPnP || {};
OfficeDevPnP.Core = OfficeDevPnP.Core || {};

OfficeDevPnP.Core.Navigation = function() {

    return {
        initialize: function(appWebUrl, targetWebUrl) {
            this.appWebUrl = appWebUrl;
            this.targetWebUrl = targetWebUrl;
        },

        addNavigationNode: function(nodeTitle, nodeUrl, parentNodeTitle, isQuickLaunch) {
            var d = $.Deferred();

            var node = new SP.NavigationNodeCreationInformation();
            node.set_title(nodeTitle);
            node.set_url(nodeUrl);
            node.set_asLastNode(true);

            var context = new SP.ClientContext(this.appWebUrl);
            var factory = new SP.ProxyWebRequestExecutorFactory(this.appWebUrl);
            context.set_webRequestExecutorFactory(factory);
            var appContextSite = new SP.AppContextSite(context, this.targetWebUrl);

            var web = appContextSite.get_web();

            if (isQuickLaunch) {
                var quickLaunchNodeCollection = web.get_navigation().get_quickLaunch();

                context.load(quickLaunchNodeCollection);

                context.executeQueryAsync(
                    function () {
                        if (parentNodeTitle) {
                            var quickLaunchEnumerator = quickLaunchNodeCollection.getEnumerator();

                            while (quickLaunchEnumerator.moveNext()) {
                                var navEntry = quickLaunchEnumerator.get_current();

                                if (navEntry.get_title() == parentNodeTitle) {
                                    navEntry.get_children().add(node);
                                    break;
                                }
                            }
                        } else {
                            quickLaunchNodeCollection.add(node);
                        }

                        context.executeQueryAsync(
                           function () {
                               d.resolve();
                           },
                           function (sender, args) {
                               console.log('Failed to create quick launch entry: ' + args.get_message());
                               d.reject(args.get_message());
                           });
                    },
                    function(sender, args) {
                        console.log('Failed to get quick launch entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            } else {
                var topNavigationBarNodeCollection = web.get_navigation().get_topNavigationBar();

                context.load(topNavigationBarNodeCollection);

                context.executeQueryAsync(
                    function() {
                        topNavigationBarNodeCollection.add(node);

                        context.executeQueryAsync(
                            function () {
                                d.resolve();
                            },
                            function(sender, args) {
                                console.log('Failed to create top navigation bar entry: ' + args.get_message());
                                d.reject(args.get_message());
                            });
                    },
                    function(sender, args) {
                        console.log('Failed to get top navigation bar entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            }

            return d.promise();
        },
        deleteNavigationNode: function(nodeTitle, parentNodeTitle, isQuickLaunch) {

        },
        deleteAllQuickLaunchNodes: function() {

        },
        updateNavigationInheritance: function(inheritNavigation) {

        }
    };
};