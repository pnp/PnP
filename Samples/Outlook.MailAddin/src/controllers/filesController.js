(function () {
  'use strict';

  angular.module('appowa')
      .controller('filesController', ['$q', '$location', 'officeService', 'restService', filesController])
      .directive('files', filesDirective);

  function filesDirective(){
  	return {
  		restriction: 'E',
  		templateUrl: '/views/partial/files.html'
  	}
  }
  /**
   * Controller constructor
   * @param $q                Angular's $q promise service.
   * @param $location         Angular's $location service.
   * @param officeService     Custom Angular service for talking to the Office client.
   * @param restService   Custom Angular service for rest data.
   */
  function filesController($q, $location, officeService, restService) {
    var vm = this;

    vm.status = {
      isFirstOpen: true,
      isFirstDisabled: false
    };

    init();

    /**
     * Initialize the controller
     */
    function init() {
      getCurrentMailboxItem()
          .then(function(){
            return getFiles();
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

    function getFiles(){
      var deferred = $q.defer();

      restService.getFiles(vm.currentMailboxItem)
          .then(function(files){
    	      vm.count = files.length || 0;
            vm.files = files;
            deferred.resolve();
          })
          .catch(function (error) {
              deferred.reject(error);
          });

      return deferred.promise;
    }
  }
})();