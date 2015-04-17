(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('WizardController', WizardController);

    WizardController.$inject = ['$scope', '$log', '$modal', 'utilservice'];

    function WizardController($scope, $log, $modal, $utilservice) {
        $scope.title = 'WizardController';
        
        $scope.spHostWebUrl = $utilservice.spHostUrl();
        $scope.spAppWebUrl = $utilservice.spAppWebUrl();       

        activate();

        function activate() {

            $log.info($scope.title + ' Activated');         

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

    }
})();