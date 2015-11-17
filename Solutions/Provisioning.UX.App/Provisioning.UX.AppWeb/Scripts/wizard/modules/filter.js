(function () {
    'use strict'

    angular.module('wizard.filters', [])
        .filter('yesNo', function yesNo() {
            return function (boolValue) {
                if (boolValue === true)
                    return "Yes";
                else
                    return "No";
            }
        });
})();

