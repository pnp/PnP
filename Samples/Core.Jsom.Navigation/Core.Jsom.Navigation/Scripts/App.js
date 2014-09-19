'use strict';

var OfficeDevPnP = OfficeDevPnP || {};
OfficeDevPnP.Core = OfficeDevPnP.Core || {};

OfficeDevPnP.Core.NavApp = function() {
    var coreNavigation = new OfficeDevPnP.Core.Navigation();

    return {
        initialize:
            function() {
                var hostWebUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
                var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

                coreNavigation.initialize(appWebUrl, hostWebUrl);
            },

        addTopNavNode:
            function() {
                coreNavigation.addNavigationNode("Test", "http://www.microsoft.com", null, false)
                    .done(
                        function() {
                            $("#statusMessage").html('Added top nav node \'Test\'');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to add top nav node \'Test\': ' + message);
                        });

            },

        deleteTopNavNode:
            function() {
                coreNavigation.deleteNavigationNode("Test", null, false)
                    .done(
                        function() {
                            $("#statusMessage").html('Removed top nav node \'Test\'');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to remove top nav node \'Test\': ' + message);
                        });

            },

        addQuickLaunchNodes:
            function() {
                coreNavigation.addNavigationNode("Parent", "#", null, true)
                    .then(
                        function() {
                            return coreNavigation.addNavigationNode("Child", "http://www.microsoft.com", 'Parent', true);
                        })
                    .done(
                        function() {
                            $("#statusMessage").html('Added quick launch nodes \'Parent\' and \'Child\'');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to add quick launch nodes \'Parent\' and \'Child\': ' + message);
                        });
            },

        deleteQuickLaunchNodes:
            function() {
                coreNavigation.deleteNavigationNode("Child", "Parent", true)
                    .then(
                        function() {
                            return coreNavigation.deleteNavigationNode("Parent", null, true);
                        })
                    .done(
                        function() {
                            $("#statusMessage").html('Deleted quick launch nodes \'Parent\' and \'Child\'');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to delete quick launch nodes \'Parent\' and \'Child\': ' + message);
                        });
            },

        deleteAllQuickLaunchNodes:
            function() {
                coreNavigation.deleteAllQuickLaunchNodes()
                    .done(
                        function() {
                            $("#statusMessage").html('Deleted all quick launch nodes');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to delete all quick launch nodes: ' + message);
                        });
            },

        updateNavigationInheritanceTrue:
            function() {
                coreNavigation.updateNavigationInheritance(true)
                    .done(
                        function() {
                            $("#statusMessage").html('Navigation inheritance set to true');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to set navigation inheritance to true: ' + message);
                        });
            },

        updateNavigationInheritanceFalse:
            function() {
                coreNavigation.updateNavigationInheritance(false)
                    .done(
                        function() {
                            $("#statusMessage").html('Navigation inheritance set to false');
                        })
                    .fail(
                        function(message) {
                            $("#statusMessage").html('Failed to set navigation inheritance to false: ' + message);
                        });
            }
    };
};
var navApp;

$(document).ready(function() {
    navApp = new OfficeDevPnP.Core.NavApp();
    navApp.initialize();

    $("#statusMessage").html('Ready');
    $('#aspnetForm').attr('onsubmit', 'javascript: return false;');
});