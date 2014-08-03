(function (angular) {
    "use strict";

    angular
        .module('prettyjson.directive', [])
        .directive('prettyJson', PrettyJson);

    function PrettyJson() {

        function formatJson(json, spaces) {
            spaces = spaces || '  ';

            return JSON.stringify(json, null, spaces);
        }

        return {
            restrict: 'E',
            scope: {
                json: '='
            },
            template: '{{prettyJson}}',
            link: function ($scope, element, attrs, ngModelCtrl) {
                $scope.$watch('json', function () {
                    $scope.prettyJson = formatJson($scope.json);
                }, true);
            }
        };
    }

})(angular);
