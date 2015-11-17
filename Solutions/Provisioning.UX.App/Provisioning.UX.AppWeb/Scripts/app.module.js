(function () {
    'use strict';

    angular.module('app', [

        // Angular Modules 
        'ngAnimate',
        'ngMessages',

        // Vendor Modules
        'ui.bootstrap',
        
        // Angular translate
        'pascalprecht.translate',

        // Custom Modules
        'app.data',
        'app.wizard',
        'common'


    ]);

})();