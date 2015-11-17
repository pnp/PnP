(function () {
    'use strict';

    // signalRservice factory
    angular
        .module('app.core')
        .factory('signalRservice', function () {

            function signalRops() {

                //Objects needed for SignalR
                var connection;
                var corporateEventsHubProxy;                                              

                // To set values to fields in the controller
                // Commented out lines left for later use
                
                var setEventChanged;
                var setSessionChanged;
                var setUpdateSpeakers;
                var setEventAdded;
                var setEventCancelled;              
                //var setSessionCancelled;
                //var setSessionAdded;
                //var updateEventStatus;
                //var updateSessionStatus;                

                var setCallbacks = function (
                    setEventChangedCallback,                    
                    setSessionChangedCallback,
                    setUpdateSpeakersCallback,
                    setEventAddedCallback,
                    setEventCancelledCallback) {
                    setEventChanged = setEventChangedCallback;                    
                    setSessionChanged = setSessionChangedCallback;
                    setUpdateSpeakers = setUpdateSpeakersCallback;
                    setEventAdded = setEventAddedCallback;
                    setEventCancelled = setEventCancelledCallback;
                };               

                var initializeClient = function () {                    

                    //Creating connection and proxy objects
                    corporateEventsHubProxy = $.connection.corporateEventsHub;

                    configureProxyClientFunctions();

                    $.connection.hub.start()
                        .done(function () {                            
                            
                        })
                        .fail(function () {

                        });
                };

                var configureProxyClientFunctions = function () {                                

                    //corporateEventsHubProxy.on('sessionCancel', function (eventData) {
                    //    //set notification that a session was cancelled
                    //    setSessionCancelled(eventData);
                    //});

                    //corporateEventsHubProxy.on('sessionAdded', function (sessionData) {
                    //    //set notification that a session was added
                    //    setSessionAdded(sessionData);
                    //});

                    //corporateEventsHubProxy.on('updatedEventStatus', function (eventData) {
                    //    //set notification that an event status changed
                    //    updateEventStatus(eventData);
                    //});

                    //corporateEventsHubProxy.on('updatedSessionStatus', function (sessionData) {
                    //    //set notification that a session status changed
                    //    updateSessionStatus(sessionData);
                    //});

                    corporateEventsHubProxy.on('eventCancel', function (eventData) {
                        //set notification that an event was cancelled
                        setEventCancelled(eventData);
                    });

                    corporateEventsHubProxy.on('eventAdded', function (eventData) {
                        //set notification that an event was added
                        setEventAdded(eventData);
                    });

                    corporateEventsHubProxy.on('eventChanged', function (eventData) {
                        //set notification that the selected event changed
                        setEventChanged(eventData);
                    });

                    corporateEventsHubProxy.on('sessionChanged', function (sessionData) {
                        setSessionChanged(sessionData)
                    });

                    corporateEventsHubProxy.on('updateSpeakers', function (sessionData) {
                        //set notification that the selected session changed
                        setUpdateSpeakers(sessionData);
                        
                    });
                };

                               
                var sessionChange = function (sessionName, data) {
                    corporateEventsHubProxy.invoke('selectedSessionChanged', sessionName, data);
                };
                var eventChange = function (sessionName, data) {
                    corporateEventsHubProxy.invoke('selectedEventChanged', sessionName, data);
                };
                var speakerUpdate = function (sessionName, data) {
                    corporateEventsHubProxy.invoke('updateSpeakers', sessionName, data);
                };
                var eventAdd = function (sessionName, data) {
                    corporateEventsHubProxy.invoke('eventAddition', sessionName, data);
                };
                var eventCancel = function (sessionName, data) {
                    corporateEventsHubProxy.invoke('eventCancellation', sessionName, data);
                };

                //var sessionCancel = function (data) {
                //    corporateEventsHubProxy.invoke('sessionCancellation', data);
                //};
                //var sessionAdd = function (data) {
                //    corporateEventsHubProxy.invoke('sessionAddition', data);
                //};
                //var broadcastSelectedEvent = function (data) {
                //    corporateEventsHubProxy.invoke('eventChanged', data);
                //};
                //var updatedEventStatus = function (data) {
                //    corporateEventsHubProxy.invoke('eventStatus', data);
                //};
                //var updatedSessionStatus = function (data) {
                //    corporateEventsHubProxy.invoke('sessionStatus', data);
                //};
                

                //return {
                //    initializeClient: initializeClient,                    
                //    eventChange: eventChange,
                //    eventCancel: eventCancel,
                //    eventAdd: eventAdd,
                //    sessionChange: sessionChange,
                //    sessionCancel: sessionCancel,
                //    sessionAdd: sessionAdd,                    
                //    broadcastSelectedEvent: broadcastSelectedEvent,
                //    updatedEventStatus: updatedEventStatus,
                //    updatedSessionStatus: updatedSessionStatus,
                //    speakerUpdate: speakerUpdate,                    
                //    setCallbacks: setCallbacks                                        
                //};

                return {
                    initializeClient: initializeClient,
                    eventChange: eventChange,                    
                    sessionChange: sessionChange,                    
                    speakerUpdate: speakerUpdate,
                    eventAdd: eventAdd,
                    eventCancel: eventCancel,
                    setCallbacks: setCallbacks
                };
            };

            return signalRops;
        });    
})();