(function (angular) {

    "use strict";

    angular
        .module('indexService', [
            'common.services'
        ])

        .factory('IndexService', IndexService)
    ;

    IndexService.$inject = ['Sharepoint', '$q'];
    function IndexService(Sharepoint, $q) {
        
        function addRecurringEventData(event) {
            var event; 

            if(true === event.fRecurrence) {
                var relativeRestUrl = "/_api/" + event['odata.editLink'] + "/fieldvaluesastext";
                event = R.pCompose(createMixedEvent(event), Sharepoint.getByUrl)(relativeRestUrl);
            }
            else {
                event = createMixedEvent(event, null);
            }

            return event;
        }
    
        var addTitleAndColor = R.curry(function (listConfig, o) {
            o.title = listConfig.title;
            o.color = listConfig.color;
            return o;
        });

        function convertSpEventToDomainEvent(spEvent) {
            return {
                allDay: spEvent.fAllDayEvent,
                description: spEvent.description,
                end: spEvent.endDate,
                start: spEvent.eventDate,
                title: spEvent.title
            };
        }

        var createMixedEvent = R.curry(function (event, fieldValuesAsTextEvent) {
            return {
                originalEvent: event,
                fieldValuesAsTextEvent: fieldValuesAsTextEvent
            };
        });

        var getCalendarDataFromSharepoint   = R.pCompose($q.all, R.map(addRecurringEventData), findCalendarListItems);
        
        var convertToEventSource = R.curry(function (convertSpEventToDomainEvent, o) {
            var mainEventSource = {
                events: R.map(convertSpEventToDomainEvent)(o.events)
            };

            var recurringEventSources = R.map(function (recurringEventSource) {
                return {
                    events: recurringEventSource.generateEvents
                };
            })(o.recurringEvents);

            var calendarEventSources = [mainEventSource].concat(recurringEventSources);
            return calendarEventSources;
        });

        // ====================================================================

        var loadCalendar = function (listConfig) {
            var f = R.pCompose(R.map(addTitleAndColor(listConfig)), convertToEventSource(convertSpEventToDomainEvent), SP.Calendar.parseEvents(convertSpEventToDomainEvent), SP.Calendar.normalizeEvents, getCalendarDataFromSharepoint);
            var eventSources = f(listConfig.title);
            return eventSources;
        };

        function findCalendarListItems(listTitle) {
            var relativeRestUrl = "/_api/web/lists/getbytitle('" + encodeURIComponent(listTitle) + "')/items";
            return Sharepoint.getByUrl(relativeRestUrl)
                .then(function (response) {
                    return response.value;
                })
            ;
        }

        function loadCalendars(calendarListTitles) {
            return R.pCompose($q.all, R.map(loadCalendar))(calendarListTitles);
        }

        function getSiteCollectionUrl() {
            return Sharepoint.getHostWebUrl();
        }

        return {
            loadCalendars: loadCalendars,
            getSiteCollectionUrl: getSiteCollectionUrl
        };
    }

})(angular, deparam);