/* global SP */

'use strict';

var OfficeDevPnP = OfficeDevPnP || {};
OfficeDevPnP.Core = OfficeDevPnP.Core || {};

OfficeDevPnP.Core.Navigation = function () {

    return {
        initialize: function (appWebUrl, targetWebUrl) {
            this.appWebUrl = appWebUrl;
            this.targetWebUrl = targetWebUrl;
        },

        addNavigationNode: function (nodeTitle, nodeUrl, parentNodeTitle, isQuickLaunch) {
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

                                if (navEntry.get_title() === parentNodeTitle) {
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
                    function (sender, args) {
                        console.log('Failed to get quick launch entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            } else {
                var topNavigationBarNodeCollection = web.get_navigation().get_topNavigationBar();

                context.load(topNavigationBarNodeCollection);

                context.executeQueryAsync(
                    function () {
                        topNavigationBarNodeCollection.add(node);

                        context.executeQueryAsync(
                            function () {
                                d.resolve();
                            },
                            function (sender, args) {
                                console.log('Failed to create top navigation bar entry: ' + args.get_message());
                                d.reject(args.get_message());
                            });
                    },
                    function (sender, args) {
                        console.log('Failed to get top navigation bar entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            }

            return d.promise();
        },

        deleteNavigationNode: function (nodeTitle, parentNodeTitle, isQuickLaunch) {
            var d = $.Deferred();

            var nodeDeleted = false;

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
                        var quickLaunchEnumerator = quickLaunchNodeCollection.getEnumerator();
                        var quickLaunchEntry;
                        var parentNodeFound = false;
                        
                        if (parentNodeTitle) {

                            while (quickLaunchEnumerator.moveNext()) {
                                quickLaunchEntry = quickLaunchEnumerator.get_current();

                                if (quickLaunchEntry.get_title() === parentNodeTitle) {
                                    parentNodeFound = true;
                                    break;
                                }
                            }

                            if (parentNodeFound && quickLaunchEntry !== undefined) {
                                var childNodeCollection = quickLaunchEntry.get_children();
                                context.load(childNodeCollection);

                                context.executeQueryAsync(
                                    function() {
                                        var childNodeEnumerator = childNodeCollection.getEnumerator();
                                        var childNode;

                                        while (childNodeEnumerator.moveNext()) {
                                            childNode = childNodeEnumerator.get_current();

                                            if (childNode.get_title() === nodeTitle) {
                                                childNode.deleteObject();
                                                nodeDeleted = true;
                                                break;
                                            }
                                        }

                                        if (nodeDeleted) {
                                            context.executeQueryAsync(
                                                function() {
                                                    d.resolve();
                                                },
                                                function(sender, args) {
                                                    console.log('Failed to delete quick launch entry: ' + args.get_message());
                                                    d.reject(args.get_message());
                                                });
                                        } else {
                                            if (parentNodeTitle) {
                                                console.log('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                                d.reject('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                            } else {
                                                console.log('Node not found.  Node title: ' + nodeTitle);
                                                d.reject('Node not found.  Node title: ' + nodeTitle);
                                            }
                                        }
                                    },
                                    function(sender, args) {
                                        console.log('Failed to get child nodes for quick launch entry ' + parentNodeTitle + ': ' + args.get_message());
                                        d.reject(args.get_message());
                                    });
                            } else {
                                if (parentNodeTitle) {
                                    console.log('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                    d.reject('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                } else {
                                    console.log('Node not found.  Node title: ' + nodeTitle);
                                    d.reject('Node not found.  Node title: ' + nodeTitle);
                                }
                            }
                        } else {
                            while (quickLaunchEnumerator.moveNext()) {
                                quickLaunchEntry = quickLaunchEnumerator.get_current();

                                if (quickLaunchEntry.get_title() === nodeTitle) {
                                    quickLaunchEntry.deleteObject();
                                    nodeDeleted = true;
                                    break;
                                }
                            }

                            if (nodeDeleted) {
                                context.executeQueryAsync(
                                    function () {
                                        d.resolve();
                                    },
                                    function (sender, args) {
                                        console.log('Failed to delete quick launch entry: ' + args.get_message());
                                        d.reject(args.get_message());
                                    });
                            } else {
                                if (parentNodeTitle) {
                                    console.log('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                    d.reject('Node not found.  Parent node title: ' + parentNodeTitle + ', Child node title: ' + nodeTitle);
                                } else {
                                    console.log('Node not found.  Node title: ' + nodeTitle);
                                    d.reject('Node not found.  Node title: ' + nodeTitle);
                                }
                            }
                        }
                    },
                    function (sender, args) {
                        console.log('Failed to get quick launch entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            } else {
                var topNavigationBarNodeCollection = web.get_navigation().get_topNavigationBar();

                context.load(topNavigationBarNodeCollection);

                context.executeQueryAsync(
                    function () {
                        var topNavEnumerator = topNavigationBarNodeCollection.getEnumerator();

                        while (topNavEnumerator.moveNext()) {
                            var navEntry = topNavEnumerator.get_current();

                            if (navEntry.get_title() === nodeTitle) {
                                navEntry.deleteObject();
                                nodeDeleted = true;
                                break;
                            }
                        }

                        if (nodeDeleted) {
                            context.executeQueryAsync(
                                function() {
                                    d.resolve();
                                },
                                function(sender, args) {
                                    console.log('Failed to delete top navigation bar entry: ' + args.get_message());
                                    d.reject(args.get_message());
                                });
                        } else {
                            console.log('Node not found.  Node title: ' + nodeTitle);
                            d.reject('Node not found.  Node title: ' + nodeTitle);
                        }
                    },
                    function (sender, args) {
                        console.log('Failed to get top navigation bar entries: ' + args.get_message());
                        d.reject(args.get_message());
                    });
            }

            return d.promise();
        },

        deleteAllQuickLaunchNodes: function () {
            var d = $.Deferred();

            var context = new SP.ClientContext(this.appWebUrl);
            var factory = new SP.ProxyWebRequestExecutorFactory(this.appWebUrl);
            context.set_webRequestExecutorFactory(factory);
            var appContextSite = new SP.AppContextSite(context, this.targetWebUrl);

            var web = appContextSite.get_web();

            var quickLaunchNodeCollection = web.get_navigation().get_quickLaunch();

            context.load(quickLaunchNodeCollection);

            context.executeQueryAsync(
                function () {
                    var oneOrMoreNodesDeleted = false;
                    var nodeCollection = [];
                    var nodeEnumerator = quickLaunchNodeCollection.getEnumerator();

                    while (nodeEnumerator.moveNext()) {
                        nodeCollection.push(nodeEnumerator.get_current());
                    }

                    for (var collectionIndex = 0; collectionIndex < nodeCollection.length; collectionIndex++) {
                        var node = nodeCollection[collectionIndex];
                        node.deleteObject();
                        oneOrMoreNodesDeleted = true;
                    }

                    if (oneOrMoreNodesDeleted) {
                        context.executeQueryAsync(
                            function() {
                                d.resolve();
                            },
                            function(sender, args) {
                                console.log('Failed to delete all quick launch entries: ' + args.get_message());
                                d.reject(args.get_message());
                            });
                    } else {
                        console.log('No nodes were found for deletion');
                        d.reject('No nodes were found for deletion');
                    }
                },
                function (sender, args) {
                    console.log('Failed to get quick launch entries: ' + args.get_message());
                    d.reject(args.get_message());
                });

            return d.promise();
        },

        updateNavigationInheritance: function (inheritNavigation) {
            var d = $.Deferred();

            var context = new SP.ClientContext(this.appWebUrl);
            var factory = new SP.ProxyWebRequestExecutorFactory(this.appWebUrl);
            context.set_webRequestExecutorFactory(factory);
            var appContextSite = new SP.AppContextSite(context, this.targetWebUrl);

            var web = appContextSite.get_web();

            web.get_navigation().set_useShared(inheritNavigation);
            web.update();

            context.executeQueryAsync(
                function () {
                    d.resolve();
                },
                function (sender, args) {
                    console.log('Failed to update navigation inheritance: ' + args.get_message());
                    d.reject(args.get_message());
                });

            return d.promise();
        }
    };
};