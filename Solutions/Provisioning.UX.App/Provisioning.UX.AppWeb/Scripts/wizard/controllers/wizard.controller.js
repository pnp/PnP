(function () {
    'use strict';
    var controllerId = 'dashboard';

    //if (!window.location.origin) { // Some browsers (mainly IE) does not have this property, so we need to build it manually...
    //    window.location.origin = window.location.protocol + '//' + window.location.hostname + (window.location.port ? (':' + window.location.port) : '');
    //}

    angular
        .module('app.wizard')
        .controller('WizardController', WizardController);

    WizardController.$inject = ['spinnerService', '$rootScope', 'common', 'config', '$scope', '$log', '$modal', 'AppSettings', 'utilservice', '$SharePointProvisioningService'];

    function WizardController(spinnerService, $rootScope, common, config, $scope, $log, $modal, AppSettings, $utilservice, $SharePointProvisioningService) {
        $scope.title = 'WizardController';
        var vm = this;        
        var logSuccess = common.logger.getLogFn(controllerId, 'success');
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);
        var user = new Object();

        $rootScope.userContext = [];
        $scope.user;
        $scope.spinnerService = spinnerService;
        $scope.loading = false;                     

        activate();

        function activate() {

            $log.info($scope.title + ' Activated');         
            $scope.appSettings = {};
            $scope.loading = true;

            // web_url/_layouts/15/resource
            var scriptbase = hostweburl + "/_layouts/15/";
            // Load the js files and continue to the successHandler
            $.getScript(scriptbase + "SP.Runtime.js",
                function () {
                    $.getScript(scriptbase + "SP.js",
                        function () {
                            $.getScript(scriptbase + "SP.RequestExecutor.js",
                                 function () {
                                     $scope.spHostWebUrl = $utilservice.spHostUrl();
                                     $scope.spAppWebUrl = $utilservice.spAppWebUrl();
                                     $scope.getCurrentUser();
                                 }
                            );
                        }
                    );
                }
            );

            getAppSettings();
            initModal();            
            var promises = [];
            common.activateController(promises, controllerId)
                               .then(function () {
                                   log('Activated Dashboard View');
                                   log('Retrieving request history from source');
                               });
        }

        $scope.cancel = function () {
            //alert($scope.hostUrl);
            window.location = $scope.spHostWebUrl;
        };

        function loadSpinners() {
            $scope.spinnerService.showGroup('requests');
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
                    getRequestsByOwner(user);
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                    getRequestsByOwner(user);
                });
            };
        }
               
        $scope.getCurrentUser = function () {
            var executor = new SP.RequestExecutor($scope.spAppWebUrl);
            executor.executeAsync(
                   {
                       url: $scope.spAppWebUrl + "/_api/SP.AppContextSite(@t)/web/currentUser?@t='" + $scope.spHostWebUrl + "'",
                       method: "GET",
                       headers:
                       {
                           "Accept": "application/json;odata=nometadata"

                       },
                       success: function (data) {
                           var jsonResults = JSON.parse(data.body);
                           
                           $log.info('Current user email: ' + jsonResults.Email);                           
                           user.name = jsonResults.Email;
                           getRequestsByOwner(user);                          

                       },
                       error: function () { alert("We are having problems retrieving specific information from the server. Please try again later"); }
                   }
               );
        }

        function getRequestsByOwner(request) {
            if (request.name == 'undefined' || request.name == "") {
                log('Attempting to retrieve user data...');
                $scope.getCurrentUser();
            }
            else {
                $.when($SharePointProvisioningService.getSiteRequestsByOwners(request)).done(function (data) {
                    if(data != null ){
                        vm.existingRequests = data;
                        $scope.spinnerService.hideGroup('requests');
                        logSuccess('Retrieved user request history');
                    }
                }).fail(function (err) {
                    console.info(JSON.stringify(err));
                });
            }
        }

        function getAppSettings() {

            // Use the app settings factory to retrieve app settings data
            AppSettings.getAppSettings().then(function (settingsdata) {

                // Store settings data 
                $scope.appSettings = settingsdata;
            });
        }

        $(document).ready(function () {

        });
        
    }
})();