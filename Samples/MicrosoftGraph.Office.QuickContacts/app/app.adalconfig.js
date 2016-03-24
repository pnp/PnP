(function () {
  'use strict';

  var office365app = angular.module('office365app');

  office365app.config(['$httpProvider', 'adalAuthenticationServiceProvider', 'appId', 'sharePointUrl', adalConfigurator]);

  function adalConfigurator($httpProvider, adalProvider, appId, sharePointUrl) {
    var adalConfig = {
      tenant: 'common',
      clientId: appId,
      extraQueryParameter: 'nux=1',
      endpoints: {
        'https://graph.microsoft.com': 'https://graph.microsoft.com'
      }
      // cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost. 
    };
    adalConfig.endpoints[sharePointUrl + '/_api/'] = sharePointUrl;
    adalProvider.init(adalConfig, $httpProvider);
  }
})();