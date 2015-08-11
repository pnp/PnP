'use strict';

var spmanage = angular.module('spmanage', [
  'ngRoute',
  'spmanageFilters',
  'spmanageDirectives',
  'siteControllers'  
]);

spmanage.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/readme', {
                templateUrl: 'readme.html',
                controller: 'ReadmeCtrl'
            }).
            when('/unlock', {
                templateUrl: 'unlock.html',
                controller: 'UnlockCtrl'
            }).
            when('/siteedit/:siteUrl*', {
                templateUrl: 'siteedit.html',
                controller: 'SiteEditCtrl'
            }).
            otherwise({
                redirectTo: 'readme'
            });
    }]);