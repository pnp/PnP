angular.module('angularSpinners', [])
  .factory('spinnerService', function () {
      var spinners = {};
      return {
          _register: function (data) {
              if (!data.hasOwnProperty('name')) {
                  throw new Error("Spinner must specify a name when registering with the spinner service.");
              }
              if (spinners.hasOwnProperty(data.name)) {
                  throw new Error("A spinner with the name '" + data.name + "' has already been registered.");
              }
              spinners[data.name] = data;
          },
          _unregister: function (name) {
              if (spinners.hasOwnProperty(name)) {
                  delete spinners[name];
              }
          },
          _unregisterGroup: function (group) {
              for (var name in spinners) {
                  if (spinners[name].group === group) {
                      delete spinners[name];
                  }
              }
          },
          _unregisterAll: function () {
              for (var name in spinners) {
                  delete spinners[name];
              }
          },
          show: function (name) {
              var spinner = spinners[name];
              if (!spinner) {
                  throw new Error("No spinner named '" + name + "' is registered.");
              }
              spinner.show();
          },
          hide: function (name) {
              var spinner = spinners[name];
              if (!spinner) {
                  throw new Error("No spinner named '" + name + "' is registered.");
              }
              spinner.hide();
          },
          showGroup: function (group) {
              var groupExists = false;
              for (var name in spinners) {
                  var spinner = spinners[name];
                  if (spinner.group === group) {
                      spinner.show();
                      groupExists = true;
                  }
              }
              if (!groupExists) {
                  throw new Error("No spinners found with group '" + group + "'.")
              }
          },
          hideGroup: function (group) {
              var groupExists = false;
              for (var name in spinners) {
                  var spinner = spinners[name];
                  if (spinner.group === group) {
                      spinner.hide();
                      groupExists = true;
                  }
              }
              if (!groupExists) {
                  throw new Error("No spinners found with group '" + group + "'.")
              }
          },
          showAll: function () {
              for (var name in spinners) {
                  spinners[name].show();
              }
          },
          hideAll: function () {
              for (var name in spinners) {
                  spinners[name].hide();
              }
          }
      };
  });

angular.module('angularSpinners')
  .directive('spinner', function () {
      return {
          restrict: 'EA',
          replace: true,
          transclude: true,
          scope: {
              name: '@?',
              group: '@?',
              show: '=?',
              imgSrc: '@?',
              register: '@?',
              onLoaded: '&?',
              onShow: '&?',
              onHide: '&?'
          },
          template: [
            '<span ng-show="show">',
            '  <img ng-show="imgSrc" ng-src="{{imgSrc}}" />',
            '  <span ng-transclude></span>',
            '</span>'
          ].join(''),
          controller: ["$scope", "spinnerService", function ($scope, spinnerService) {

              // register should be true by default if not specified.
              if (!$scope.hasOwnProperty('register')) {
                  $scope.register = true;
              }

              // Declare a mini-API to hand off to our service so the service
              // doesn't have a direct reference to this directive's scope.
              var api = {
                  name: $scope.name,
                  group: $scope.group,
                  show: function () {
                      $scope.show = true;
                  },
                  hide: function () {
                      $scope.show = false;
                  },
                  toggle: function () {
                      $scope.show = !$scope.show;
                  }
              };

              // Register this spinner with the spinner service.
              if ($scope.register === true) {
                  spinnerService._register(api);
              }

              // If an onShow or onHide expression was provided, register a watcher
              // that will fire the relevant expression when show's value changes.
              if ($scope.onShow || $scope.onHide) {
                  $scope.$watch('show', function (show) {
                      if (show && $scope.onShow) {
                          $scope.onShow({ spinnerService: spinnerService, spinnerApi: api });
                      } else if (!show && $scope.onHide) {
                          $scope.onHide({ spinnerService: spinnerService, spinnerApi: api });
                      }
                  });
              }

              // This spinner is good to go. Fire the onLoaded expression.
              if ($scope.onLoaded) {
                  $scope.onLoaded({ spinnerService: spinnerService, spinnerApi: api });
              }
          }]
      };
  });