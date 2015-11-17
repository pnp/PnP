(function () {
    //'use strict';

    angular
       .module('app.sessions')
       .controller('sessionsController', ['$scope', '$q', '$log', '$modal', 'dataService', 'signalRservice', '$SharePointJSOMService', function ($scope, $q, $log, $modal, dataService, signalRservice, $SharePointJSOMService) {

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
                                 
           vm.sessions = [];
           vm.title = 'Sessions';
           vm.areAllSourcesSelected = false;
           vm.checkbox = [];
           vm.updateSelected;
           vm.invokeSignalR;

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
                   vm.corporateEventsHubProxy.server.send("Event Sessions", hubId);
                   // Join the SignalR group using hub id and unique session key used for unique SignalR group
                   vm.corporateEventsHubProxy.server.joinSession($.connection.hub.id, vm.sessionKey);
                   
               })
               .fail(function () {
                   console.log('Could not Connect!');
               });

           }).fail(function (err) {
               console.info(JSON.stringify(err));
           });

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
                   sigRops.sessionChange(vm.sessionKey, speakerId);

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
           $scope.selectedSession;

           vm.open = function (size, $event, session) {
               $scope.sessionData = [];
               $scope.sessionData.push({
                   sessionid: session.sessionid,
                   eventid: session.registeredeventid,
                   title: session.title,
                   sessiondate: session.sessiondate,
                   description: session.description,
                   
               })

               //alert($scope.eventData[0].description);

               var modalInstance = $modal.open({
                   animation: $scope.animationsEnabled,
                   templateUrl: 'sessionInfo.html',
                   controller: 'sessionInfoController',
                   size: size,
                   resolve: {
                       sessionData: function () {
                           return $scope.sessionData;
                       }
                   }

               })

               modalInstance.result.then(function (selectedSession) {
                   $scope.selected = selectedSession;
               }, function () {
                   $log.info('Modal dismissed at: ' + new Date());
               })
           };

           $scope.toggleAnimation = function () {
               $scope.animationsEnabled = !$scope.animationsEnabled;
           };


       }]).controller('sessionInfoController', function ($scope, $modalInstance, sessionData) {

           $scope.sessionData = sessionData;
           $scope.selected = {
               sessionItem: $scope.sessionData
           };

           $scope.ok = function () {
               $modalInstance.close();
           };

           $scope.cancel = function () {
               $modalInstance.dismiss('cancel');
           };
       });
})();