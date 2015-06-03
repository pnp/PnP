(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('WizardController', WizardController);

    WizardController.$inject = ['$scope', '$log', '$modal', 'AppSettings', 'utilservice', '$SharePointProvisioningService'];

    function WizardController($scope, $log, $modal, AppSettings, $utilservice, $SharePointProvisioningService) {
        $scope.title = 'WizardController';

        var vm = this;
        vm.existingRequests = [];
        
        $scope.spHostWebUrl = $utilservice.spHostUrl();
        $scope.spAppWebUrl = $utilservice.spAppWebUrl();
              
        // web_url/_layouts/15/resource
        var scriptbase = hostweburl + "/_layouts/15/";
        // Load the js files and continue to the successHandler
        $.getScript(scriptbase + "SP.Runtime.js",
            function () {
                $.getScript(scriptbase + "SP.js",
                    function () {
                        $.getScript(scriptbase + "SP.RequestExecutor.js",
                             function () {
                                 $scope.getCurrentUser();
                                 $log.info('Current user data retrieved');



                             }
                        );
                    }
                );
            }
        );

        activate();

        function activate() {

            $log.info($scope.title + ' Activated');         
            $scope.appSettings = {};

           

            getAppSettings();
            initModal();
        }

        
        function initModal() {

            // Set event handler to open the modal dialog window
            $scope.open = function () {
                
                // Set modal configuration options
                var modalInstance = $modal.open({
                    scope: $scope,
                    templateUrl: '/Pages/Wizard.modal.html',
                    controller: 'WizardModalInstanceController',
                    size: 'lg',
                    windowClass: 'modal-pnp'                                      
                });

                // Process the data returned from the modal after it is successfuly completed
                modalInstance.result.then(function (configuration) {
                    $scope.completedConfiguration = configuration;
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });

            };

        }

        function getAppSettings() {

            // Use the app settings factory to retrieve app settings data
            AppSettings.getAppSettings().then(function (settingsdata) {

                // Store settings data 
                $scope.appSettings = settingsdata;

            });

        }

        function getRequestsByOwner(request) {
            $.when($SharePointProvisioningService.getSiteRequestsByOwners(request)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        vm.existingRequests = data.requests;
                        $log.info('Site Requests Retrieved');
                    }
                    else {
                        $scope.existingRequests[0] = 'No existing site requests exist';
                        $log.info('No existing site requests');
                    }
                }

            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        $scope.getCurrentUser = function () {
            var executor = new SP.RequestExecutor($utilservice.spAppWebUrl());
            executor.executeAsync(
                   {
                       url: $utilservice.spAppWebUrl() + "/_api/web/currentuser",
                       method: "GET",
                       headers:
                       {
                           "Accept": "application/json;odata=nometadata"

                       },
                       success: function (data) {
                           var jsonResults = JSON.parse(data.body);
                           $scope.currentUserEmail = jsonResults.Email;
                           $log.info('Current user email: ' + jsonResults.Email);

                           var user = new Object();
                           user.name = $scope.currentUserEmail;

                           getRequestsByOwner(user);

                       },
                       error: function () { alert("We are having problems retrieving specific information from the server. Please try again later") }
                   }
               );
        }

        
        
    }
})();