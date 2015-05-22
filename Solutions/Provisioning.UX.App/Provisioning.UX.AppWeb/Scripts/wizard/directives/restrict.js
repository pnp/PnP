(function() {
    'use strict';

    angular
        .module('app.wizard')
        .directive('restrict', restrict);

    restrict.$inject = ['$parse'];
    
    function restrict ($parse) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, iElement, iAttrs, controller) {
                scope.$watch(iAttrs.ngModel, function (value) {
                    if (!value) {
                        return;
                    }
                    $parse(iAttrs.ngModel).assign(scope, value.toLowerCase().replace(new RegExp(iAttrs.restrict, 'g'), '').replace(/\s+/g, ''));
                });
            }
        }
    }

})();