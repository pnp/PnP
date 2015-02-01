(function () {
    //'use strict';

    angular
       .module('app.signalrcomms')
       .controller('communicationsController', ['$q', 'dataService', 'signalRservice', function communicationsController($q, dataService, signalRservice) {


           /*jsHint validthis: true */
           var vm = this;
           
           vm.title = 'SignalR Communications Display';           

           //Objects needed for SignalR
           vm.connection = '';
           vm.corporateEventsHubProxy;
           
           //vm.activate = activate();           

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
               //initializeCorporateEvents();                    

               var hubId = 'connection ID=' + $.connection.hub.id;
               vm.corporateEventsHubProxy.server.send("SignalR Comms", hubId);
               console.log('Now connected, connection ID=' + $.connection.hub.id);


           })
           .fail(function () {
               console.log('Could not Connect!');
           });

           

           //var sigRops = signalRservice();
           //sigRops.setCallbacks(setEventChanged, setUpdateSpeakers);
           //sigRops.initializeClient();


       }]);

    })();