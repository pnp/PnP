(function () {
    //'use strict';

    angular
       .module('app.speakers')
       .controller('speakersController', ['$q', 'dataService', 'signalRservice', function ($q, dataService, signalRservice) {

           var sigRops = signalRservice();
                                 
           /*jsHint validthis: true */
           var vm = this;
           vm.speakers = [];
           vm.title = 'Speakers';

           vm.areAllSourcesSelected = false;
           vm.checkbox = [];
           vm.updateSelected;
           vm.updateSelection;

           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;
           
           //vm.activate = activate();

           function activate(speakerId) {
               var promises = [getSpeakers(speakerId)];

               /**
                * Step 1
                * Ask the getSpeakers function for the
                * sessions data and wait for the promise
                */
               //return getSpeakers().then(function () {
               return $q.all(promises).then(function () {
                   /**
                    * Step 4
                    * Perform an action on resolve of final promise
                    */
               });
           }

           function getSpeakers(speakerId) {
               /**
                * Step 2
                * Ask the data service for the data and wait
                * for the promise
                */
               return dataService.getSpeakers(speakerId)
                 .then(function (data) {
                     /**
                      * Step 3
                      * set the data and resolve the promise
                      */
                     vm.speakers = data;
                     return vm.speakers;
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

           vm.updateSelection = function ($event, id) {
               var checkbox = $event.target;
               var action = (checkbox.checked ? 'add' : 'remove');
               vm.updateSelected(action, id);

               if (action == 'add' || action === 'add') {

                   // There will most likely be no need for another app part to be listening for this
                   // so this is rerally just for showing some action happening in the logging app part
                   vm.corporateEventsHubProxy.server.send('Selected Speaker ID', id);
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
               vm.corporateEventsHubProxy.server.send("Speakers", hubId);
               console.log('Now connected, connection ID=' + $.connection.hub.id);


           })
           .fail(function () {
               console.log('Could not Connect!');
           });

                     
           function setUpdateSpeakers(speakerId) {
               activate(speakerId);
           }

           function setEventChanged(data) {
               vm.broadcastedEventId = data;
           }

           function setSessionChanged(data) {
               vm.broadcastedSessionId = data;
           }

           function setEventAdded(eventId) {

           }

           function setEventCancelled(eventId) {

           }
                                 
           sigRops.setCallbacks(setEventChanged, setSessionChanged, setUpdateSpeakers, setEventAdded, setEventCancelled);
           sigRops.initializeClient();
           
       }]);

})();