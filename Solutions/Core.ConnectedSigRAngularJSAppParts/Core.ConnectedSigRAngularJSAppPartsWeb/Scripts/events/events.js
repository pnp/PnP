(function () {
    //'use strict';

    angular
       .module('app.events')
       .controller('eventsController', ['$q', 'dataService', 'signalRservice', function ($q, dataService, signalRservice) {


           /*jsHint validthis: true */
           var vm = this;
           vm.events = [];
           vm.deletedEvent = [];
           vm.title = 'Events';

           vm.areAllSourcesSelected = false;
           vm.selectableSources = [];
           vm.checkbox = [];
           vm.stringsArray = [];
           vm.checkbox = [];
           vm.selectedId = '';
           vm.broadcastedEventId = '';
           vm.updateSelected;
           vm.updateSelection;
           vm.invokeDeleteEvent;

           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;
           
           vm.activate = activate();          

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
                  sigRops.eventCancel(evId);
                                      
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

           vm.updateSelected = function (action, id) {
               if (action === 'add' && vm.checkbox.indexOf(id) === -1) {
                   vm.checkbox.push(id);
               }
               if (action === 'remove' && vm.checkbox.indexOf(id) !== -1) {
                   vm.checkbox.splice(vm.checkbox.indexOf(id), 1);
               }
           };

           vm.updateSelection = function ($event, id, evtId) {
               var checkbox = $event.target;
               var action = (checkbox.checked ? 'add' : 'remove');
               vm.updateSelected(action, id);

               if (action == 'add' || action === 'add') {

                   // Invoke the proxy
                   sigRops.eventChange(evtId);

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

           //Creating connection and proxy objects
           vm.corporateEventsHubProxy = $.connection.corporateEventsHub;

           $.connection.hub.start()
           .done(function () {                                 

               var hubId = 'connection ID=' + $.connection.hub.id;
               vm.corporateEventsHubProxy.server.send("Events Master", hubId);
               console.log('Now connected, connection ID=' + $.connection.hub.id);

           })
           .fail(function () {
               console.log('Could not Connect!');
           });


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



       }]);

    })();