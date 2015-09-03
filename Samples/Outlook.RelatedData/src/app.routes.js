
var appConf = {
  tenantName: "agile9"
};

var routeConf = {};
    routeConf['tenant']         = appConf.tenantName + '.onmicrosoft.com';
    routeConf['clientId']       = '9e03550c-1678-4093-9b12-05946b4df46b';
    routeConf['cacheLocation']  = 'localStorage';
    routeConf["endpoints"] = {};
    routeConf.endpoints['https://' + appConf.tenantName + '.sharepoint.com/_api/'] = 'https://' + appConf.tenantName + '.sharepoint.com';
    routeConf.endpoints['https://' + appConf.tenantName + '-my.sharepoint.com/_api/v1.0/me'] = 'https://' + appConf.tenantName + '-my.sharepoint.com';
    routeConf.endpoints['https://outlook.office365.com/api/v1.0/me'] = 'https://outlook.office365.com';

(function () {
  'use strict';

  var outlookApp = angular.module('appowa');

  // load routes
  outlookApp.config(['$routeProvider', '$httpProvider', 'adalAuthenticationServiceProvider', routeConfigurator]);

  function routeConfigurator($routeProvider, $httpProvider, adalProvider) {

    //Initialize ADAL
    adalProvider.init(routeConf, $httpProvider);
    
    $routeProvider
        .when('/', {
          templateUrl: '/views/home-view.html',
          controller: 'homeController',
          requireADLogin: true
        })
        .when('/files', {
          templateUrl: '/views/files-view.html',
          controller: 'homeController',
          controllerAs: 'vm',
          requireADLogin: true
        })
        .when('/mails', {
          templateUrl: '/views/mails-view.html',
          requireADLogin: true
        })
        .when('/employees', {
          templateUrl: '/views/employees-view.html',
          requireADLogin: true
        })
        .when('/reports', {
          templateUrl: '/views/reports-view.html',
          requireADLogin: true
        });
    $routeProvider.otherwise({redirectTo: '/'});
  }
})();