(function (angular) {

    "use strict";

    angular
        .module('index.controller', [
            'index.services',
            'ui.calendar',
            'ui.bootstrap'
        ])

        .controller('IndexController', IndexController)
    ;

    IndexController.$inject = ['$scope', '$compile', 'uiCalendarConfig', 'IndexService'];
    function IndexController($scope, $compile, uiCalendarConfig, IndexService) {

        $scope.lists = [
            {
                title: 'Holidays',
                color: '#337ab7',
                checked: true
            },
            {
                title: 'Meetings',
                color: '#7f3c92',
                checked: true
            }
        ];

        $scope.allEventSources = [];

        $scope.toggleVisibility = function (listConfig) {
            listConfig.checked = !listConfig.checked;

            var eventSourcesToDisplay = R.filter(isCalendarEnabled($scope.lists))($scope.allEventSources);
            updateEventSources(eventSourcesToDisplay);
            uiCalendarConfig.calendars.fullCallendarInstance.fullCalendar('refetchEvents');
        };

        /* alert on eventClick */
        $scope.alertOnEventClick = function (date, jsEvent, view) {
            console.log(date.title + ' was clicked ');
        };
        /* alert on Drop */
        $scope.alertOnDrop = function (event, delta, revertFunc, jsEvent, ui, view) {
            console.log('Event Droped to make dayDelta ' + delta);
        };
        /* alert on Resize */
        $scope.alertOnResize = function (event, delta, revertFunc, jsEvent, ui, view) {
            console.log('Event Resized to make dayDelta ' + delta);
        };

        /* Render Tooltip */
        $scope.eventRender = function (event, element, view) {

            var tooltip = (moment(event.start).format("H:mma") + ' - ' + moment(event.end).format("H:mma") + ' ' + event.title);

            element.attr({
                'tooltip': tooltip,
                'tooltip-append-to-body': true
            });
            $compile(element)($scope);
        };

        $scope.eventSources = [];//[$scope.events];

        $scope.uiConfig = {
            calendar: {
                height: 450,
                editable: true,
                header: {
                    left: 'title',
                    center: '',
                    right: 'today prev,next'
                },
                timezone: 'local',
                eventClick: $scope.alertOnEventClick,
                eventDrop: $scope.alertOnDrop,
                eventResize: $scope.alertOnResize,
                eventRender: $scope.eventRender
            }
        };

        var isCalendarEnabled = R.curry(function (lists, eventSource) {
            var listConfig = R.find(R.propEq('title', eventSource.title))(lists);
            return (listConfig && (true === listConfig.checked));
        });

        function updateEventSources(eventSources) {
            // Use splice so as to preserve original array object reference to widget
            var parameters = [0, $scope.eventSources.length].concat(eventSources);
            Array.prototype.splice.apply($scope.eventSources, parameters);
        };

        (function () {
            IndexService.loadCalendars($scope.lists)
                .then(function (calendars) {
                    var eventSources = R.flatten(calendars);
                    $scope.allEventSources = eventSources;
                    var eventSourcesToDisplay = R.filter(isCalendarEnabled($scope.lists))(eventSources);
                    updateEventSources(eventSourcesToDisplay);
                    uiCalendarConfig.calendars.fullCallendarInstance.fullCalendar('refetchEvents');
                    return eventSourcesToDisplay;
                })
            ;
        })();
    }

})(angular, deparam);