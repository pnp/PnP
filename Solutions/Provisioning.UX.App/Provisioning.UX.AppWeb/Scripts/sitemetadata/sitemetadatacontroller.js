(function () {
    'use strict';
    var controllerId = 'siteclassification';   

    angular
        .module('app.wizard')
        .controller('SiteClassificationController', SiteClassificationController);

    SiteClassificationController.$inject = ['spinnerService', '$rootScope', 'common', 'config', '$scope', '$log', 'AppSettings', 'utilservice', '$translate', '$SiteMetadataEditService', 'BusinessMetadata'];

    function SiteClassificationController(spinnerService, $rootScope, common, config, $scope, $log, AppSettings, $utilservice, $translate, $SiteMetadataEditService, BusinessMetadata) {
        $scope.title = 'SiteClassificationController';       

        var vm = this;
        var logSuccess = common.logger.getLogFn(controllerId, 'success');
        var logError = common.logger.getLogFn(controllerId, 'error');
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);
        var events = config.events;
        var user = new Object();

        $rootScope.userContext = [];
        $scope.user;
        $scope.metadata = {};
        $scope.spinnerService = spinnerService;
        $scope.spinnersON;
        $scope.loading = false;
        $scope.saving = false;''
        
        
        vm.isOnPrem = "false";        
        vm.translations = {};
        vm.userHasPermissions = false;
        vm.externalSharingEnabled = false;
        vm.siteExternalSharingEnabled = false;
        vm.businessUnit;
        vm.function;
        vm.region;
        vm.division;
        vm.appSettings;
        vm.sitePolicyName;

        vm._externallySharedStatus = "Off";           

        var SITE_PROPERTY_DIVISION = "_site_props_division";
        var SITE_PROPERTY_BUSINESS = "_site_props_business";
        var SITE_PROPERTY_REGION = "_site_props_region";
        var SITE_PROPERTY_FUNCTION = "_site_props_function";
        var SITE_PROPERTY_ISONPREM = "_site_props_sponprem";
        var SITE_PROPERTY_EXTERNAL_SHARING = "_site_props_externalsharing";            

        activate();       

        function activate() {

            $log.info($scope.title + ' Activated');
            $scope.appSettings = {};
            $scope.spinnersON = true;
            $scope.loading = true;
            $scope.saving = false;
            var promises = [];

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

                                     $scope.hostUrl = $app.getUrlParamByName('SPHostUrl');
                                     //$scope.getCurrentUser();                                     
                                 }
                            );
                        }
                    );
                }
            );

            // Get app settings from appsetting.config
            getAppSettings();
            
            // Load property bag data needed for page
            LoadPropertyBagMetadata();
            
            common.activateController(promises, controllerId)
                               .then(function () {
                                   log('Site Classification enabled');                                   
                               });
        }

        $scope.cancel = function () {
            //alert($scope.hostUrl);
            window.location = $scope.hostUrl;
        };

        $scope.save = function () {
            // Activate spinners
            $scope.loading = false;
            $scope.saving = true;
            $scope.spinnersON = true;
            $scope.spinnerService.showGroup('metadata');
            var siteMetadata = $scope.metadata;           

            $.when($SiteMetadataEditService.SetSiteMetadata(siteMetadata)).done(function (metadata) {
                if (metadata != null) {
                    if (metadata.success == true) {
                        $scope.metadata = metadata;                        
                        window.location = $scope.hostUrl;
                    }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function loadSpinners() {
            $scope.spinnerService.showGroup('metadata');
        }

        function getAppSettings() {

            // Use the app settings factory to retrieve app settings data
            AppSettings.getAppSettings().then(function (settingsdata) {

                // Store settings data 
                $scope.appSettings = settingsdata;
            });
        }

        function LoadPropertyBagMetadata()
        {
            var siteMetadata = new Object();           

            siteMetadata.url = hostweburl;                       

            $.when($SiteMetadataEditService.GetSiteMetadata(siteMetadata)).done(function (metadata) {
                if (metadata != null) {
                    if (metadata.success == true) {                                               
                        $scope.metadata = metadata;

                        vm.businessUnit = metadata.businessUnit;
                        vm.region = metadata.region;
                        vm.division = metadata.division;
                        vm.function = metadata.function;
                        vm.sitePolicyName = metadata.sitePolicyName;

                        // Get reference metadata values
                        GetBusinessMetadata();         
                        
                        $scope.spinnerService.hideGroup('metadata');
                    }                    
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function GetOnPremPropertyBagValue()
        {
            var propertyBagRequest = new Object();
            propertyBagRequest.Key = SITE_PROPERTY_ISONPREM;
            $.when($SiteMetadataEditService.GetPropertyBagItem(propertyBagRequest)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        vm.isOnPrem = data.Value;
                    }
                    else { vm.isOnPrem = false; }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }
        
        function SetUXAvailableSitePolicy()
        {
            // Implement code
        }

        function DoesUserHavePermission()
        {  
            var userPermissonsCheckRequest = new Object();          
            $.when($SiteMetadataEditService.CheckUserPermissions(userPermissonsCheckRequest)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        vm.userHasPermissions = data.DoesUserHavePermissions;

                        if (vm.userHasPermissions) {
                            // Check property bag settings for OnPrem and External Sharing                            
                            GetOnPremPropertyBagValue();

                            // Check sharing capabilities
                            

                            var externalSharingRequest = new Object();
                            externalSharingRequest.tenantAdminUrl = template.tenantAdminUrl;
                            CheckTenantExternalSharing(externalSharingRequest);
                        }
                    }
                    else { vm.userHasPermissions = false; }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function CheckTenantExternalSharing(externalSharingRequest)
        {
            //get if external sharing is enabled for the tenant
            $.when($SiteMetadataEditService.isExternalSharingEnabled(externalSharingRequest)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        vm.externalSharingEnabled = data.externalSharingEnabled;

                        
                    }
                    else { vm.externalSharingEnabled = false; }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function CheckSiteExternalSharing(externalSharingRequest)
        {
            //get if external sharing is enabled for the tenant
            $.when($SiteMetadataEditService.IsSiteExternalSharingEnabled(externalSharingRequest)).done(function (data) {
                if (data != null) {
                    if (data.success == true) {
                        vm.siteExternalSharingEnabled = data.siteExternalSharingEnabled;
                    }
                    else { vm.siteExternalSharingEnabled = false; }
                }
            }).fail(function (err) {
                console.info(JSON.stringify(err));
            });
        }

        function SetExternalSharing()
        {
            // Implement code
        }
        
        function GetBusinessMetadata() {

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
                $scope.siteclassifications = [];

                // Store site classification data 
                $scope.siteclassifications = classificationdata;
            });
        }

        
       
        $(document).ready(function () {

        });

    }
})();