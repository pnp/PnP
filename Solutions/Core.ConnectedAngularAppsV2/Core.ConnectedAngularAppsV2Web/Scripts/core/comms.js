(function () {
    //'use strict';

    angular
       .module('app.signalrcomms')
       .controller('communicationsController', ['$q', 'dataService', 'signalRservice', '$SharePointJSOMService', function ($q, dataService, signalRservice, $SharePointJSOMService) {

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
                      
           vm.title = 'SignalR Communications Display';           

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

               //General messaging callback
               vm.corporateEventsHubProxy.client.broadcastMessage = function (name, message) {
                   // Html encode display name and message. 
                   var encodedName = $('<div />').text(name).html();
                   var encodedMsg = $('<div />').text(message).html();
                   // Add the message to the page. 
                   $('#sigRMessages').append('<dt></dt><dt><strong>' + encodedName
                       + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</dt>');
               }

               $.connection.hub.start()
               .done(function () {

                   //capture the ID of the client used by SignalR
                   var hubId = 'connection ID=' + $.connection.hub.id;
                   var loggingScope = "public";

                   // Write out for logging purposes - displayed by logging app part
                   vm.corporateEventsHubProxy.server.send("SignalR Communications", hubId);

                   // Note: If no session key name is specified, don't try to join a session.
                   // The app part will display all messages sent via the .server.send method
                   // If this app part is set to join a group (session key specified), then it 
                   // will not display message sent by other apps using the .server.send method

                   if (vm.sessionKey != null && vm.sessionKey != "") {                       
                       loggingScope = "private";
                   }

                   if (loggingScope == "private") {                                            

                       if (vm.combineUserIdWithSessionKey == "true") {                           

                           // Create unique session key from app property value and login id
                           vm.sessionKey = vm.sessionKey + "-" + vm.userContext[0].loginName;                           
                       }

                       // Join the SignalR group using hub id and unique session key used for unique SignalR group
                       vm.corporateEventsHubProxy.server.joinSession($.connection.hub.id, vm.sessionKey);
                   }                  

               })
               .fail(function () {
                   console.log('Could not Connect!');
               });              

           }).fail(function (err) {
               console.info(JSON.stringify(err));
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