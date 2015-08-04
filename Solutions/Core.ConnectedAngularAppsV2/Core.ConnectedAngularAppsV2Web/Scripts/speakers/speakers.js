(function () {
    //'use strict';

    angular
       .module('app.speakers')
       .controller('speakersController', ['$q', 'dataService', 'signalRservice', '$SharePointJSOMService', function ($q, dataService, signalRservice, $SharePointJSOMService) {
           
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
                                                       
           vm.speakers = [];
           vm.title = 'Speakers';
           vm.areAllSourcesSelected = false;
           vm.checkbox = [];
           vm.updateSelected;
           vm.updateSelection;

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
                   vm.corporateEventsHubProxy.server.send("Event Speaker(s)", hubId);
                   // Join the SignalR group using hub id and unique session key used for unique SignalR group
                   vm.corporateEventsHubProxy.server.joinSession($.connection.hub.id, vm.sessionKey);

               })
               .fail(function () {
                   console.log('Could not Connect!');
               });               

           }).fail(function (err) {
               console.info(JSON.stringify(err));
           });

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
                     
           function setUpdateSpeakers(speakerId) {
               activate(speakerId);
           }

           function setEventChanged(data) {
               
           }

           function setSessionChanged(data) {
               
           }

           function setEventAdded(eventId) {

           }

           function setEventCancelled(eventId) {

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