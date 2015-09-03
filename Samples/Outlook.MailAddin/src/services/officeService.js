(function () {
  'use strict';

  angular.module('appowa')
      .service('officeService', ['$q', officeService]);

  /**
   * Custom Angular service that works with the host Office client.
   *
   * @returns {{getWordCandidatesFromEmail: getWordCandidatesFromEmail}}
   */
  function officeService($q) {

    // public signature of the service.
    return {
      getCurrentMailboxItem: getCurrentMailboxItem
    };

    /** *********************************************************** */

    function getCurrentMailboxItem(){
      var deferred = $q.defer();

      try {
        var currentEmail = Office.cast.item.toItemRead(Office.context.mailbox.item);
        deferred.resolve(currentEmail);
      } catch (error) {
        deferred.reject(error);
      }

      return deferred.promise;

    }
  }

})();