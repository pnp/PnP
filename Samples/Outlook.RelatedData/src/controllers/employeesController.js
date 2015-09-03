(function () {
  'use strict';

  angular.module('appowa')
      .controller('employeesController', ['$q', '$location', 'officeService', 'restService', employeesController])
      .directive('employees', employeesDirective);

  function employeesDirective(){
  	return {
  		restriction: 'E',
  		templateUrl: '/views/partial/employees.html'
  	}
  }
  /**
   * Controller constructor
   * @param $q                Angular's $q promise service.
   * @param $location         Angular's $location service.
   * @param officeService     Custom Angular service for talking to the Office client.
   * @param restService   Custom Angular service for rest data.
   */
  function employeesController($q, $location, officeService, restService) {
    var vm = this;

	Office.initialize = function () {
      	console.log(">>> Office.initialize()");
      	init();
    };
    init();

    vm.status = {
      isFirstOpen: true,
      isFirstDisabled: false
    };

    /**
     * Initialize the controller
     */
    function init() {
      getCurrentMailboxItem()
          .then(function(){
            return getCompany();
          });
    }

    function getCurrentMailboxItem(){
      var deferred = $q.defer();

      officeService.getCurrentMailboxItem()
          .then(function(mailbox){

            vm.currentMailboxItem = mailbox;
            deferred.resolve();
          })
          .catch(function (error) {
              deferred.reject(error);
          });

      return deferred.promise;
    }

    function getCompany(){
      var deferred = $q.defer();

      restService.getCompany(vm.currentMailboxItem)
          .then(function(companies){

          	console.log("employeesController");
        	console.log(companies);

            vm.companies = companies;
            vm.numEmployees = companies.length > 0 ? companies[0].Employees.length : 0;
            deferred.resolve();

          })
          .catch(function (error) {
              deferred.reject(error);
          });

      return deferred.promise;
    }
  }
})();