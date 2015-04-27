(function () {
    //'use strict';

    angular
       .module('app.sessions')
       .controller('sessionsController', ['$q', 'dataService', 'signalRservice', function ($q, dataService, signalRservice) {

           var sigRops = signalRservice();                     

           /*jsHint validthis: true */
           var vm = this;
           vm.sessions = [];
           vm.title = 'Sessions';

           vm.areAllSourcesSelected = false;           
           vm.checkbox = [];                           
           vm.updateSelected;
           vm.invokeSignalR;

           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;
           
           //vm.activate = activate();

           function activate(eventId) {
               var promises = [getSessions(eventId)];

               /**
                * Step 1
                * Ask the getSessions function for the
                * sessions data and wait for the promise
                */
               //return getEvents().then(function () {
               return $q.all(promises).then(function () {
                   /**
                    * Step 4
                    * Perform an action on resolve of final promise
                    */

               });
           }
                      

           function getSessions(eventId) {
               /**
                * Step 2
                * Ask the data service for the data and wait
                * for the promise
                */
               return dataService.getSessions(eventId)
                 .then(function (data) {
                     /**
                      * Step 3
                      * set the data and resolve the promise
                      */
                     vm.sessions = data;
                     return vm.sessions;
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

           vm.invokeSignalR = function ($event, id, speakerId) {
               var checkbox = $event.target;
               var action = (checkbox.checked ? 'add' : 'remove');
               vm.updateSelected(action, id);

               if (action == 'add' || action === 'add') {

                   // Invoke the proxy
                   sigRops.sessionChange(speakerId);                  
                   
                   // Broadcast session id and speaker id
                   vm.corporateEventsHubProxy.server.send('Selected Session ID', id);
                   vm.corporateEventsHubProxy.server.send('Session Speaker ID', speakerId);
               }

               vm.selectedId = id;

               for (var i = 0; i < vm.checkbox.length; i++) {
                   if (i != id) {
                       vm.checkbox[i] = false;
                   }
               }
           };

           //Creating connection and proxy objects
           vm.corporateEventsHubProxy = $.connection.corporateEventsHub;

           //General messaging callback
           //vm.corporateEventsHubProxy.client.broadcastMessage = function (name, message) {
           //    // Html encode display name and message. 
           //    var encodedName = $('<div />').text(name).html();
           //    var encodedMsg = $('<div />').text(message).html();
           //    // Add the message to the page. 
           //    $('#sessionsmessages').append('<dt></dt><dt><strong>' + encodedName
           //        + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</dt>');
           //}

           $.connection.hub.start()
           .done(function () {
               //initializeCorporateEvents();                    

               var hubId = 'connection ID=' + $.connection.hub.id;
               vm.corporateEventsHubProxy.server.send("Event Sessions", hubId);
               console.log('Now connected, connection ID=' + $.connection.hub.id);


           })
           .fail(function () {
               console.log('Could not Connect!');
           });


           function setEventChanged(eventId) {
               activate(eventId);
           }

           function setUpdateSpeakers(data) {
               vm.broadcastSpeakerId = data;
           }

           function setSessionChanged(data) {
               vm.broadcastSpeakerId = data;
           }                    

           function setEventAdded(eventId) {
               
           }

           function setEventCancelled(eventId) {

           }
                     
         
           sigRops.setCallbacks(setEventChanged, setSessionChanged, setUpdateSpeakers, setEventAdded, setEventCancelled);
           sigRops.initializeClient();

       }]);

    })();