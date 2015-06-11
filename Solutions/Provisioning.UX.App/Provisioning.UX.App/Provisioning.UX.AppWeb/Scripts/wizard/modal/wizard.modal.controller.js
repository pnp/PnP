(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('WizardModalInstanceController', WizardModalInstanceController);
        //.value('urlparams', null);

    WizardModalInstanceController.$inject = ['$scope', '$log', '$modalInstance', 'Templates', 'utilservice'];

    function WizardModalInstanceController($scope, $log, $modalInstance, Templates, $utilservice) {
        $scope.title = 'WizardModalInstanceController';

        
        var spHostWebUrl = $scope.spHostWebUrl;
        var spAppWebUrl = $scope.spAppWebUrl;       

        activate();

        $scope.siteConfiguration.spHostWebUrl = spHostWebUrl;
        $scope.siteConfiguration.spRootHostName = "Https://" + $utilservice.spRootHostName(spHostWebUrl); // still need to capture proto
        $scope.siteConfiguration.spNewSitePrefix = "Https://" + $utilservice.spRootHostName(spHostWebUrl) + "/sites/"; // still need to replace hardcoded /sites/

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.finished = function() {
            $modalInstance.close($scope.siteConfiguration);
        };

        $scope.interacted = function (field) {
            return field.$dirty;
        };


        $scope.selectTemplate = function (template) {

            // Add the selected template to the configuration object
            $scope.siteConfiguration.template = template;

        }

        //$scope.checkSiteUrl = function () {
            

        //    if ($scope.siteConfiguration.details.url == undefined || $scope.siteConfiguration.details.url == '' || $scope.siteConfiguration.details.url.indexOf('/', $scope.siteConfiguration.details.url.length - '/'.length) !== -1) {
        //        $scope.urlOK = false;
        //        $scope.siteConfiguration.details.Url = "";
        //        $("detailsUrl").val("");

        //        return;
        //    }
        //    $scope.siteTitleEnabled = false;
        //    $scope.checkingUrl = true;
            
        //    $scope.siteConfiguration.details.Url = $scope.siteConfiguration.details.Url.replace(new RegExp("[^a-z0-9\_\s]", 'g'), '').replace(/\s+/g, '');

        //    $http({
        //        method: "get",
        //        url: "/api/Sites",
        //        params: {
        //            SPHostUrl: $scope.siteConfiguration.spRootHostName,
        //            SiteUrl: $scope.siteConfiguration.spNewSitePrefix + $scope.siteConfiguration.details.url
        //        }
        //    }).success(function (data) {
        //        $timeout(function () {
        //            $scope.checkingUrl = false;
        //            $scope.siteTitleEnabled = true;
        //            if (data == "false") {
        //                $scope.urlOK = true;
        //            } else {
        //                $scope.urlOK = false;
        //            }
                    
                    
        //        });
        //    });
        //}

        function activate() {

            $log.info($scope.title + ' Activated');
            $scope.siteConfiguration = {};

            // Initialize modal dialog box information
            initModal();
            getTemplates();

           

        }

        function initModal() {

            $scope.steps = [1, 2, 3, 4, 5, 6, 7, 8];
            $scope.step = 0;
            $scope.wizard = { tacos: 2 };

            $scope.isCurrentStep = function (step) {
                return $scope.step === step;
            };

            $scope.setCurrentStep = function (step) {
                $scope.step = step -= 1;
            };

            $scope.getCurrentStep = function () {
                return $scope.steps[$scope.step];
            };

            $scope.isFirstStep = function () {
                return $scope.step === 0;
            };

            $scope.isLastStep = function () {
                return $scope.step === ($scope.steps.length - 1);
            };

            $scope.handlePrevious = function () {
                $scope.step -= ($scope.isFirstStep()) ? 0 : 1;
            };

            $scope.handleNext = function () {
                if ($scope.isLastStep()) {
                    //$modalInstance.close($scope.wizard);
                } else {
                    $scope.step += 1;
                }
            };          

        }

        function getTemplates() {

            // Use the Templates factory to retrieve an array of available site templates
            Templates.getData().then(function (data) {

                // Store returned templates 
                $scope.templates = data;

            });

        }

        
        

    }
})();
