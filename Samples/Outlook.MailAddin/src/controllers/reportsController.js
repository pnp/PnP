(function () {
  'use strict';

  angular.module('appowa')
      .controller('reportsController',['$q', '$location', 'officeService', 'restService',reportsController])
      .directive('reports', reportsDirective);

  function reportsDirective(){
    return {
      restrict: 'E',
      templateUrl:'/views/partial/reports.html' 
    }
  }

  /**
   * Controller constructor
   * @param $q                Angular's $q promise service.
   * @param $location         Angular's $location service.
   * @param officeService     Custom Angular service for talking to the Office client.
   * @param restService   Custom Angular service for rest data.
   */
  function reportsController($q, $location, officeService, restService) {
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
            getReports();
          });
    }

    vm.chartConfig = {
        options: {
            chart: {
                type: 'area'
            },
            xAxis: {
                tickmarkPlacement: 'on',
                title: {
                    enabled: false
                }
            },
        },
        title: {
            text: 'Annual Reports'
        },
        credits: {
            enabled: true
        },
        loading: false,
        size: {}
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

    function getReports(){
      var deferred = $q.defer();

      restService.getReports(vm.currentMailboxItem)
          .then(function(object){
            vm.chartConfig.series = object.data;
            vm.chartConfig.options.xAxis.categories = object.data[0].years;
            deferred.resolve();
          })
          .catch(function (error) {
              deferred.reject(error);
          });

      return deferred.promise;
    }
  }
})();