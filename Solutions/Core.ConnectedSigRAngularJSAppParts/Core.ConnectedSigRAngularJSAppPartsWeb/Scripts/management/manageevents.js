(function () {
    //'use strict';

    angular
       .module('app.manage')
       .controller('eventMgmtController', ['$q', 'dataService', 'signalRservice', function ($q, dataService, signalRservice) {

           var sigRops = signalRservice();

           /*jsHint validthis: true */
           var vm = this;
           vm.events = [];
           vm.title = 'Add Event Items';
           vm.addNewEvent;
           vm.newEvent;
           vm.addedEvent;
          
           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;                     

           vm.addNew = function (newItem) {
              
               var promises = [addCorporateEvent(newItem)];

               /**
                * Step 1
                * Ask the getEvents function for the
                * events data and wait for the promise
                */
               
               return $q.all(promises).then(function () {
                   /**
                    * Step 4
                    * Perform an action on resolve of final promise
                    */

               });
           }

           function addCorporateEvent(newItem) {
               /**
                * Step 2
                * Ask the data service for the data and wait
                * for the promise
                */
               return dataService.addEvent(newItem)
                 .then(function (data) {
                     /**
                      * Step 3
                      * set the data and resolve the promise
                      */
                     vm.events = data;

                     // Invoke the proxy
                    sigRops.eventAdd(vm.addedEvent.id);

                     return vm.events;
                 });
           };          
                             

           //Creating connection and proxy objects
           vm.corporateEventsHubProxy = $.connection.corporateEventsHub;           

           $.connection.hub.start()
           .done(function () {
               //initializeCorporateEvents();                    

               var hubId = 'connection ID=' + $.connection.hub.id;
               vm.corporateEventsHubProxy.server.send("Add Events", hubId);
               console.log('Now connected, connection ID=' + $.connection.hub.id);
           })
           .fail(function () {
               console.log('Could not Connect!');
           });

           function setEventChanged(data) {
              
           }

           function setSessionChanged(data) {
              
           }

           function setUpdateSpeakers(data) {
               
           }

           function setEventAdded(data) {
               
           }

           function setEventCancelled(data) {

           }


           sigRops.setCallbacks(setEventChanged, setSessionChanged, setUpdateSpeakers, setEventAdded, setEventCancelled);          
           sigRops.initializeClient();
       }]);
})();