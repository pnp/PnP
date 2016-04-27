(function(){
  'use strict';

  // create
  var office365app = angular.module('office365app', [
    'ngRoute',
    'AdalAngular',
    'ngTouch'
  ]);

  // configure
  office365app.config(['$logProvider', '$compileProvider', function($logProvider, $compileProvider){
    // set debug logging to on
    if ($logProvider.debugEnabled) {
      $logProvider.debugEnabled(true);
    }
    
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|mailto|tel|sip):/);
  }]);
})();
