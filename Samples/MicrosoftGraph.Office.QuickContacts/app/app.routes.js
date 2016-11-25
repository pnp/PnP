(function(){
  'use strict';

  var office365app = angular.module('office365app');

  // load routes
  office365app.config(['$routeProvider', routeConfigurator]);

  function routeConfigurator($routeProvider){
    $routeProvider
      .when('/', {
        templateUrl: 'app/home/home.html',
        controller: 'homeController',
        controllerAs: 'vm',
        requireADLogin: true
      });

    $routeProvider.otherwise({redirectTo: '/'});
  }

})();
