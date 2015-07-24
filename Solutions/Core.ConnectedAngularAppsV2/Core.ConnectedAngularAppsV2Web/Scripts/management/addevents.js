(function () {
    //'use strict';

    angular
       .module('app.addevents')
       .controller('eventMgmtController', ['$q', 'dataService', 'signalRservice', '$SharePointJSOMService', function ($q, dataService, signalRservice, $SharePointJSOMService) {
           
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
           vm.title = 'Add Event Items';
           vm.addNewEvent;
           vm.newEvent;
           vm.addedEvent;
          
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
                       // Create unique session key from static default since app property was not set
                       vm.sessionKey = vm.sessionNameDefault;
                   }

                   if (vm.combineUserIdWithSessionKey == "true") {
                       // Create unique session key from app property value and login id
                       vm.sessionKey = vm.sessionKey + "-" + vm.userContext[0].loginName;
                   }

                   //capture the ID of the client used by SignalR
                   var hubId = 'connection ID=' + $.connection.hub.id;
                   // Write out for logging purposes - displayed by logging app part
                   vm.corporateEventsHubProxy.server.send("Add Events", hubId);
                   // Join the SignalR group using hub id and unique session key used for unique SignalR group
                   vm.corporateEventsHubProxy.server.joinSession($.connection.hub.id, vm.sessionKey);

               })
               .fail(function () {
                   console.log('Could not Connect!');
               });               

           }).fail(function (err) {
               console.info(JSON.stringify(err));
           });

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
                             

           function setEventChanged(data) {
              
           }

           function setSessionChanged(data) {
              
           }

           function setUpdateSpeakers(data) {
               
           }

           function setEventAdded(data) {
               alert("Event Added");
           }

           function setEventCancelled(data) {

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

          

       }]);
})();