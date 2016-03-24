(function() {
    angular.module('office365app')
        .directive('focusOn', ['$timeout', focusOn]);

    function focusOn($timeout) {
        return {
            restrict: 'A',
            link: link
        };

        function link($scope, element, attrs) {
            $scope.$on(attrs.focusOn, function() {
                $timeout(function() {
                    element[0].focus();
                }, 0);
            });
        }
    }
})();
