(function () {
    'use strict';
    var controllerId = 'wizard';

    angular
        .module('app.wizard')
        .controller('WizardModalInstanceController', WizardModalInstanceController);
        //.value('urlparams', null);

    WizardModalInstanceController.$inject = ['$rootScope', 'common', 'config', '$scope', '$log', '$modalInstance', 'Templates', 'BusinessMetadata', 'utilservice', '$SharePointProvisioningService'];

    function WizardModalInstanceController($rootScope, common, config, $scope, $log, $modalInstance, Templates, BusinessMetadata, $utilservice, $SharePointProvisioningService) {
        $scope.title = 'WizardModalInstanceController';

        var logSuccess = common.logger.getLogFn(controllerId, 'success');
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);
        
        var spHostWebUrl = $scope.spHostWebUrl;
        var spAppWebUrl = $scope.spAppWebUrl;       

        activate();

        // Set language and time zone defaults
        $scope.siteConfiguration.language = $scope.appSettings[0].value;
        $scope.siteConfiguration.timezone = $scope.appSettings[1].value;
        
        $scope.siteConfiguration.spHostWebUrl = spHostWebUrl;
        $scope.siteConfiguration.spRootHostName = "Https://" + $utilservice.spRootHostName(spHostWebUrl); // still need to capture proto
        //remove hard coded path now we get from template object when selected
        //$scope.siteConfiguration.spNewSitePrefix = "Https://" + $utilservice.spRootHostName(spHostWebUrl) + "/sites/"; // still need to replace hardcoded /sites/        

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.finished = function () {
            //  save the site request when the wizard is complete

            var siteRequest = new Object();
            siteRequest.title = $scope.siteConfiguration.details.title;
            siteRequest.url = $scope.siteConfiguration.spNewSitePrefix + $scope.siteConfiguration.details.url;
            siteRequest.description = $scope.siteConfiguration.details.description;
            siteRequest.lcid = $scope.siteConfiguration.language;
            siteRequest.timeZoneId = $scope.siteConfiguration.timezone;
            siteRequest.primaryOwner = $scope.siteConfiguration.primaryOwner;
            siteRequest.additionalAdministrators = $scope.siteConfiguration.secondaryOwners;
            siteRequest.sharePointOnPremises = $scope.siteConfiguration.spOnPrem;
            siteRequest.template = $scope.siteConfiguration.template.title;
            siteRequest.sitePolicy = $scope.siteConfiguration.privacy.classification;
            siteRequest.businessCase = $scope.siteConfiguration.purpose.description;
            siteRequest.enableExternalSharing = $scope.siteConfiguration.externalSharing
    
            //property bag entries will enumerate all properties defined in siteConfiguration.properties
            var props = {};
            angular.forEach($scope.siteConfiguration.properties, function (value, key) {
                props["_site_props_" + key] = value;
            });
            //set the properties object
            siteRequest.properties = props;
            
            saveSiteRequest(siteRequest);
            logSuccess('Sweet! Your request has been submitted');

            $modalInstance.close($scope.siteConfiguration);
        };

        $scope.interacted = function (field) {
            return field.$dirty;
        };               

        $scope.selectTemplate = function (template) {

            // Add the selected template to the configuration object
            $scope.siteConfiguration.template = template;
            // Add the Path to the configuration object to store the url
            $scope.siteConfiguration.spNewSitePrefix = template.hostPath;
            $scope.siteConfiguration.spOnPrem = template.sharePointOnPremises;
            $scope.siteConfiguration.tenantAdminUrl = template.tenantAdminUrl;

            //ExternalSharing Request to determine if External Sharing is enabled in the tenant
            var externalSharingRequest = new Object();
            externalSharingRequest.tenantAdminUrl = template.tenantAdminUrl;
            isExternalSharingEnabled(externalSharingRequest);
        }

        function activate() {

            $log.info($scope.title + ' Activated');
            $scope.siteConfiguration = {};

            // Initialize modal dialog box information
            initModal();
            getTemplates();
            getBusinessMetadata();

            var promises = [];
            common.activateController(promises, controllerId)
                               .then(function () {
                                   logSuccess('Wizard Activated');
                               });

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
            //get the site templates
            $.when($SharePointProvisioningService.getSiteTemplates($scope)).done(function (jsonObject) {
                if (jsonObject != null) {
                    // Store returned templates 
                    $scope.templates = jsonObject;
                }

            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function isExternalSharingEnabled(request) {
            //get if external sharing is enabled for the tenant
            $.when($SharePointProvisioningService.isExternalSharingEnabled(request)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        $scope.siteConfiguration.externalSharingEnabled = data.externalSharingEnabled;
                    }
                    else { $scope.siteConfiguration.externalSharingEnabled = false; }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        

        function getBusinessMetadata() {

            // Use the metadata factory to retrieve a list of regions
            BusinessMetadata.getRegions().then(function (regionsdata) {

                // Store region data 
                $scope.regions = regionsdata;
            });

            // Use the metadata factory to retrieve a list of functions
            BusinessMetadata.getFunctions().then(function (functionsdata) {

                // Store functions data 
                $scope.functions = functionsdata;
            });

            // Use the metadata factory to retrieve a list of divisions
            BusinessMetadata.getDivisions().then(function (divisionsdata) {

                // Store divisions data 
                $scope.divisions = divisionsdata;
            });

            // Use the metadata factory to retrieve a list of languages
            BusinessMetadata.getLanguages().then(function (languagesdata) {

                // Store langauges data 
                $scope.languages = languagesdata;
            });

            // Use the metadata factory to retrieve a list of timezones
            BusinessMetadata.getTimeZones().then(function (timezonesdata) {

                // Store time zones data 
                $scope.timezones = timezonesdata;
            });

            // Use the metadata factory to retrieve a list of site classifications
            BusinessMetadata.getSiteClassifications().then(function (classificationdata) {

                // Store site classification data 
                $scope.siteclassifications = classificationdata;
            });
        }

        function saveSiteRequest(request) {
            $.when($SharePointProvisioningService.saveRequest(request)).done(function (data) {
                if (data != null) {
                    if(data.success != true)
                    {
                        //There was an issue posting to the service
                    }
                }
            }).fail(function (err) {
                console.log(err);
            });
            console.log(request);
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
                           $scope.siteConfiguration.primaryOwner = jsonResults.Email;

                       },
                       error: function () { alert("We are having problems retrieving specific information from the server. Please try again later") }
                   }
               );
        }
        
        $scope.getCurrentUser();

    }
})();
