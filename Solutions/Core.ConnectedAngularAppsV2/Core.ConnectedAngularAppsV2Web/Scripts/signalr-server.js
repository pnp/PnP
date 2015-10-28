/*!
 * ASP.NET SignalR JavaScript Library v2.1.2
 * http://signalr.net/
 *
 * Copyright Microsoft Open Technologies, Inc. All rights reserved.
 * Licensed under the Apache 2.0
 * https://github.com/SignalR/SignalR/blob/master/LICENSE.md
 *
 */

/// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
/// <reference path="jquery.signalR.js" />
(function ($, window, undefined) {
    /// <param name="$" type="jQuery" />
    "use strict";

    if (typeof ($.signalR) !== "function") {
        throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
    }

    var signalR = $.signalR;

    function makeProxyCallback(hub, callback) {
        return function () {
            // Call the client hub method
            callback.apply(hub, $.makeArray(arguments));
        };
    }

    function registerHubProxies(instance, shouldSubscribe) {
        var key, hub, memberKey, memberValue, subscriptionMethod;

        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!(hub.hubName)) {
                    // Not a client hub
                    continue;
                }

                if (shouldSubscribe) {
                    // We want to subscribe to the hub events
                    subscriptionMethod = hub.on;
                } else {
                    // We want to unsubscribe from the hub events
                    subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                    if (hub.client.hasOwnProperty(memberKey)) {
                        memberValue = hub.client[memberKey];

                        if (!$.isFunction(memberValue)) {
                            // Not a client hub function
                            continue;
                        }

                        subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                    }
                }
            }
        }
    }

    $.hubConnection.prototype.createHubProxies = function () {
        var proxies = {};
        this.starting(function () {
            // Register the hub proxies as subscribed
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, true);

            this._registerSubscribedHubs();
        }).disconnected(function () {
            // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, false);
        });

        proxies['corporateEventsHub'] = this.createHubProxy('corporateEventsHub'); 
        proxies['corporateEventsHub'].client = { };
        proxies['corporateEventsHub'].server = {
            joinSession: function (connectionId, sessionName) {
                /// <summary>Calls the JoinSession method on the server-side corporateEventsHub hub.</summary>
                /// <param name=\"connectionId\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["JoinSession"], $.makeArray(arguments)));
            },

            leaveSession: function (connectionId, sessionName) {
                /// <summary>Calls the LeaveSession method on the server-side corporateEventsHub hub.</summary>
                /// <param name=\"connectionId\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["LeaveSession"], $.makeArray(arguments)));
            },

            eventAddition: function (sessionName, eventId) {
                /// <summary>Calls the EventAddition method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"eventId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["EventAddition"], $.makeArray(arguments)));
             },

            eventCancellation: function (sessionName, data) {
                /// <summary>Calls the EventCancellation method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"eventId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["EventCancellation"], $.makeArray(arguments)));
             },

            eventStatus: function (sessionName, data) {
                /// <summary>Calls the EventStatus method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"eventId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["EventStatus"], $.makeArray(arguments)));
             },

            refresh: function () {
            /// <summary>Calls the Refresh method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["Refresh"], $.makeArray(arguments)));
             },

            selectedEventChanged: function (sessionName, data) {
                /// <summary>Calls the SelectedEventChanged method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"eventId\" type=\"String\">Server side type is System.String</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["SelectedEventChanged"], $.makeArray(arguments)));
             },

            selectedSessionChanged: function (sessionName, data) {
                /// <summary>Calls the SelectedSessionChanged method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionId\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"speakerId\" type=\"String\">Server side type is System.String</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["SelectedSessionChanged"], $.makeArray(arguments)));
             },

            send: function (name, message) {
                /// <summary>Calls the Send method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"name\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"message\" type=\"String\">Server side type is System.String</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["Send"], $.makeArray(arguments)));
             },

            sessionAddition: function (sessionName, data) {
                /// <summary>Calls the SessionAddition method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["SessionAddition"], $.makeArray(arguments)));
             },

            sessionCancellation: function (sessionName, data) {
                /// <summary>Calls the SessionCancellation method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["SessionCancellation"], $.makeArray(arguments)));
             },

            sessionStatus: function (sessionName, data) {
                /// <summary>Calls the SessionStatus method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionId\" type=\"Number\">Server side type is System.Int32</param>
                return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["SessionStatus"], $.makeArray(arguments)));
            },

            updateSpeakers: function (sessionName, data) {
                /// <summary>Calls the SessionStatus method on the server-side corporateEventsHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                /// <param name=\"sessionName\" type=\"String\">Server side type is System.String</param>
                /// <param name=\"sessionId\" type=\"Number\">Server side type is System.Int32</param>
            return proxies['corporateEventsHub'].invoke.apply(proxies['corporateEventsHub'], $.merge(["UpdateSpeakers"], $.makeArray(arguments)));
        }
        };

        return proxies;
    };

    signalR.hub = $.hubConnection("/signalr", { useDefaultPath: false });
    $.extend(signalR, signalR.hub.createHubProxies());

}(window.jQuery, window));