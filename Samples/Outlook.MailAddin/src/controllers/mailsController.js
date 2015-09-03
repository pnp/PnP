(function () {
  'use strict';

  angular.module('appowa')
      .controller('mailsController', ['$q', '$location', 'officeService', 'restService',mailsController])
      .directive('mails', mailsDirective);


  // helper function | filter trustAsHtml
  angular.module('appowa').filter('to_trusted', ['$sce', function ($sce) {
      return function (text) {
          return $sce.trustAsHtml(text);
      };
  }]);


  function mailsDirective(){
    return {
      restrict: 'E',
      templateUrl:'/views/partial/mails.html' 
    }
  }


  /**
   * Controller constructor
   * @param $q                Angular's $q promise service.
   * @param $location         Angular's $location service.
   * @param officeService     Custom Angular service for talking to the Office client.
   * @param restService   Custom Angular service for rest data.
   */
  function mailsController($q, $location, officeService, restService) {
    var vm = this;

    /** *********************************************************** */

    Office.initialize = function () {
      console.log(">>> Office.initialize()");
      init();
    };
    init();

    /**
     * Initialize the controller
     */
    function init() {
      getCurrentMailboxItem()
          .then(function(){
            return getEmails();
          });
    }

    vm.status = {
      isFirstOpen: true,
      isFirstDisabled: false
    };

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

    function getEmails(){
      var deferred = $q.defer();

      restService.getEmails(vm.currentMailboxItem)
          .then(function(emails){

            vm.emails = emails.data.value;
            deferred.resolve();

          })
          .catch(function (error) {
              deferred.reject(error);
          });

      return deferred.promise;
    }
  }
})();