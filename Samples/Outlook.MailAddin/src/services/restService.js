(function () {
  'use strict';

  angular.module('appowa')
      .service('restService', ['$q', '$http', restService]);

  /**
   * Custom Angular service that talks to a static JSON file simulating a REST API.
   */
  function restService($q, $http) {
    // public signature of the service
    return {
      getFiles: getFiles,
      getEmails: getEmails,
      getCompany: getCompany,
      getReports: getReports
    };

    function getCompany(mailbox){
      var deferred = $q.defer();
      var restQueryUrl = "https://localhost:44301/api/companies?$filter=substringof(Email,'" + mailbox.from.emailAddress + "')";

      $http({
        method: 'GET',
        url: restQueryUrl,
        headers: {
            "accept": "application/json; odata=verbose",
        }
      }).success(function (data) {
        deferred.resolve(data);
      }).error(function (error) {
        deferred.reject(error);
      });

      return deferred.promise;
    }

    function getFiles(mailbox) {
      var deferred = $q.defer();
      var restQueryUrl = "https://" + appConf.tenantName + ".sharepoint.com/_api/search/query?querytext='" + mailbox.from.emailAddress + "'";

      $http({
        method: 'GET',
        url: restQueryUrl,
        headers: {
            "accept": "application/json; odata=verbose",
        }
      }).success(function (data) {
        var result = {};

        // find the matching customer
        result = $.map(data.d.query.PrimaryQueryResult.RelevantResults.Table.Rows.results, function (item) {
                return getFields(item.Cells.results);
            });

        deferred.resolve(result);
      }).error(function (error) {
        deferred.reject(error);
      });

      return deferred.promise;
    }

    function getEmails(mailbox) {
        var deferred = $q.defer();
        var restQueryUrl = "https://outlook.office365.com/api/v1.0/me/messages?$filter=From/EmailAddress/Address eq '" + mailbox.from.emailAddress + "'&$top=5";

        return $http({
            url: restQueryUrl,
            method: "GET",
            headers: {
                "accept": "application/json",
            }
        }).success(function (data) {
          deferred.resolve(data);
        }).error(function (error) {
          deferred.reject(error);
        });

        return deferred.promise;
    }

    function getReports(mailbox) {
        var deferred = $q.defer();
        var restQueryUrl = "https://localhost:44301/api/reports?mail=" + mailbox.from.emailAddress;

        return $http({
            url: restQueryUrl,
            method: "GET",
            headers: {
                "accept": "application/json",
            }
        }).success(function (data) {
          deferred.resolve(data);
        }).error(function (error) {
          deferred.reject(error);
        });

        return deferred.promise;
    }
  }
})();

//helper function for rest-search result formating
function getFields(results) {
    r = {};
    for (var i = 0; i < results.length; i++) {
        if (results[i] != undefined && results[i].Key != undefined) {
            r[results[i].Key] = results[i].Value;
        }
    }
    return r;
}
