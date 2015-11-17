(function () {
    'use strict';

    var controllerId = 'shell';
    angular.module('app').controller(controllerId,
        ['$rootScope', 'common', 'config','$translate', shell]);

    function shell($rootScope, common, config, $translate) {
        var vm = this;
        var logSuccess = common.logger.getLogFn(controllerId, 'success');
        var events = config.events;
        vm.busyMessage = 'Please wait ...';
        vm.isBusy = true;
        vm.spinnerOptions = {
            radius: 40,
            lines: 7,
            length: 0,
            width: 30,
            speed: 1.7,
            corners: 1.0,
            trail: 100,
            color: '#DC3C00'
        };

        
        vm.translations ={};
        $translate(['SOLUTION_LOADED']).then(function (translations) {
            vm.translations.HEADLINE = translations.SOLUTION_LOADED;
        }).then(activate)


        function activate() {
            logSuccess(vm.translations.HEADLINE, null, true);
            common.activateController([], controllerId);
        }

        function toggleSpinner(on) { vm.isBusy = on; }

        //$rootScope.$on('$routeChangeStart',
        //    function (event, next, current) { toggleSpinner(true); }
        //);

        $rootScope.$on(events.controllerActivateSuccess,
            function (data) { toggleSpinner(false); }
        );

        $rootScope.$on(events.spinnerToggle,
            function (data) { toggleSpinner(data.show); }
        );
    };
})();