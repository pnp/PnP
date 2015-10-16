(function () {
    //'use strict';    

    angular
       .module('app.events')
       .controller('eventsController', ['$scope', '$q', '$log', '$modal', 'dataService', 'signalRservice', '$SharePointJSOMService', function ($scope, $q, $log, $modal, dataService, signalRservice, $SharePointJSOMService) {
                      
           var vm = this;
           vm.sessionNameDefault = "Default";

           // Check for user defined property values on app part
           vm.sessionKey =
                    decodeURIComponent(
                            getQueryStringParameter("SessionKey")
                    );

           vm.combineUserIdWithSessionKey =
                    decodeURIComponent(
                            getQueryStringParameter("CombineUserIdWithSessionKey")
                    );

           vm.events = [];
           vm.deletedEvent = [];
           vm.title = 'Events';
           vm.areAllSourcesSelected = false;
           vm.selectableSources = [];
           vm.checkbox = [];
           vm.selectedId = '';
           vm.broadcastedEventId = '';
           vm.updateSelected;
           vm.updateSelection;
           vm.invokeDeleteEvent;

           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;

           // Get user information before we try to create a unique session key and join the SignalR group
           vm.userContext = [];
           $.when($SharePointJSOMService.get_userProperties()).done(function (data) {
               var json = JSON.parse(data.body);

               vm.userContext.push({
                   loginName: json.d.LoginName,
                   displayName: json.d.Title,
                   email: json.d.Email
               })

               //Creating connection and proxy objects
               vm.corporateEventsHubProxy = $.connection.corporateEventsHub;

               $.connection.hub.start()
               .done(function () {
                   if (vm.sessionKey == null || vm.sessionKey == "") {
                       // Create session key from static default since app property was not set
                       vm.sessionKey = vm.sessionNameDefault;
                   }

                   if (vm.combineUserIdWithSessionKey == "true") {
                       // Create unique session key from app property value and login id
                       vm.sessionKey = vm.sessionKey + "-" + vm.userContext[0].loginName;
                   }

                   //capture the ID of the client used by SignalR
                   var hubId = 'connection ID=' + $.connection.hub.id;
                   // Write out for logging purposes - displayed by logging app part
                   vm.corporateEventsHubProxy.server.send("Events Master", hubId);
                   // Join the SignalR group using hub id and unique session key used for unique SignalR group
                   vm.corporateEventsHubProxy.server.joinSession($.connection.hub.id, vm.sessionKey);

               })
               .fail(function () {
                   console.log('Could not Connect!');
               });

               // Activate the events retrieval once we have our SignalR activities done
               vm.activate = activate();

           }).fail(function (err) {
               console.info(JSON.stringify(err));
           });

           function activate() {
               var promises = [getEvents()];

               /**
                * Step 1
                * Ask the getEvents function for the
                * events data and wait for the promise
                */
               //return getEvents().then(function () {
               return $q.all(promises).then(function () {
                   /**
                    * Step 4
                    * Perform an action on resolve of final promise
                    */
               });
           }

           function getEvents() {
               /**
                * Step 2
                * Ask the data service for the data and wait
                * for the promise
                */
               return dataService.getEvents()
                 .then(function (data) {
                     /**
                      * Step 3
                      * set the data and resolve the promise
                      */
                     vm.events = data;
                     return vm.events;
                 });
           }

           vm.invokeDeleteEvent = function (evId) {
               var promises = [deleteEvent(evId)];

               /**
                * Step 1
                * Ask the function for the
                * events data and wait for the promise
                */

               return $q.all(promises).then(function (evId) {
                   /**
                    * Step 4
                    * Perform an action on resolve of final promise
                    */
                   // Invoke the proxy and notify SignalR
                   sigRops.eventCancel(vm.sessionKey, evId);
               });
           }

           function deleteEvent(evId) {
               /**
                * Step 2
                * Ask the data service for the data and wait
                * for the promise
                */
               return dataService.deleteEvent(evId)
                 .then(function (data) {
                     /**
                      * Step 3
                      * set the data and resolve the promise
                      */
                     vm.events = data;
                     return vm.events;
                 });
           }

           // Update UI checkboxe statuses
           vm.updateSelected = function (action, id) {
               if (action === 'add' && vm.checkbox.indexOf(id) === -1) {
                   vm.checkbox.push(id);
               }
               if (action === 'remove' && vm.checkbox.indexOf(id) !== -1) {
                   vm.checkbox.splice(vm.checkbox.indexOf(id), 1);
               }
           };

           // Perform actions based on items checked in UI
           vm.updateSelection = function ($event, id, evtId) {
               var checkbox = $event.target;
               var action = (checkbox.checked ? 'add' : 'remove');
               vm.updateSelected(action, id);

               if (action == 'add' || action === 'add') {

                   // Invoke the proxy
                   sigRops.eventChange(vm.sessionKey, evtId);

                   // Broadcast message
                   vm.corporateEventsHubProxy.server.send('Selected Event ID', evtId);
               }

               vm.selectedId = evtId;

               for (var i = 0; i < vm.checkbox.length; i++) {
                   if (i != id) {
                       vm.checkbox[i] = false;
                   }
               }
           };

           function setEventChanged(data) {
               vm.broadcastedEventId = data;
           }

           function setSessionChanged(data) {
               vm.broadcastedSessionId = data;
           }

           function setUpdateSpeakers(data) {
               vm.broadcastedSpeakerId = data;
           }

           function setEventAdded(data) {
               activate();
           }

           function setEventCancelled(data) {
               activate();
           }

           var sigRops = signalRservice();
           sigRops.setCallbacks(setEventChanged, setSessionChanged, setUpdateSpeakers, setEventAdded, setEventCancelled);
           sigRops.initializeClient();


           function getQueryStringParameter(paramToRetrieve) {
               var params =
                   document.URL.split("?")[1].split("&");
               var strParams = "";
               for (var i = 0; i < params.length; i = i + 1) {
                   var singleParam = params[i].split("=");
                   if (singleParam[0] == paramToRetrieve)
                       return singleParam[1];
               }
           }

           // Modal functionality
           $scope.animationsEnabled = true;
           $scope.selectedEvent;
           
           vm.open = function (size, $event, event) {
               $scope.eventData = [];
               $scope.eventData.push({
                   id: event.id,
                   eventid: event.registeredeventid,
                   title: event.title,
                   eventdate: event.eventdate,
                   description: event.description,
                   category: event.category,
                   location: event.location,
                   contactemail: event.contactemail,
                   status: event.status
                   })

               //alert($scope.eventData[0].description);

               var modalInstance = $modal.open({
                   animation: $scope.animationsEnabled,
                   templateUrl: 'eventInfo.html',
                   controller: 'eventInfoController',
                   size: size,
                   resolve: {
                       eventData: function () {
                           return $scope.eventData;
                       }
                   }
                   
               })

               modalInstance.result.then(function (selectedEvent) {
                   $scope.selected = selectedEvent;
               }, function () {
                   $log.info('Modal dismissed at: ' + new Date());
               })
           };

           $scope.toggleAnimation = function () {
               $scope.animationsEnabled = !$scope.animationsEnabled;
           };

       }]).controller('eventInfoController', function ($scope, $modalInstance, eventData) {
           
           $scope.eventData = eventData;
           $scope.selected = {
               eventItem: $scope.eventData
           };

           $scope.ok = function () {
               $modalInstance.close();
           };

           $scope.cancel = function () {
               $modalInstance.dismiss('cancel');
           };
        });
 })();