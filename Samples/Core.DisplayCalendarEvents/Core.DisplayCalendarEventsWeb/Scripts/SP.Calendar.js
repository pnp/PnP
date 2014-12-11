(function (JXON, moment, momentf, R, SP) {

    SP.Calendar = SP.Calendar || {};

    
    SP.Calendar.Constants = SP.Calendar.Constants || {};
    SP.Calendar.Constants.EventTypeMap = {
        'SINGLE': 0,
        'RECURRING': 1,
        'EXCEPTION': 4,
        'DELETION': 3
    };
    SP.Calendar.Constants.DefaultAllDayDurationSeconds = 86340;

    SP.Calendar.Constants.Days = [
        {
            isWeekday: false,
            isWeekendday: true,
            name: 'Sunday',
            weekIndex: 0,
            xmlProperty: '@su',
        },
        {
            isWeekday: true,
            isWeekendday: false,
            name: 'Monday',
            weekIndex: 1,
            xmlProperty: '@mo'
        },
        {
            isWeekday: true,
            isWeekendday: false,
            name: 'Tuesday',
            weekIndex: 2,
            xmlProperty: '@tu'
        },
        {
            isWeekday: true,
            isWeekendday: false,
            name: 'Wednesday',
            weekIndex: 3,
            xmlProperty: '@we'
        },
        {
            isWeekday: true,
            isWeekendday: false,
            name: 'Thursday',
            weekIndex: 4,
            xmlProperty: '@th'
        },
        {
            isWeekday: true,
            isWeekendday: false,
            name: 'Friday',
            weekIndex: 5,
            xmlProperty: '@fr',
        },
        {
            isWeekday: false,
            isWeekendday: true,
            name: 'Saturday',
            weekIndex: 6,
            xmlProperty: '@sa',
        }
    ];

    SP.Calendar.Constants.WeekdayToFilters = {
        'first' : function (x, i, xs) { return (0 === i); },
        'second': function (x, i, xs) { return (1 === i); },
        'third' : function (x, i, xs) { return (2 === i); },
        'fourth': function (x, i, xs) { return (3 === i); },
        'last'  : function (x, i, xs) { return ((xs.length - 1) === i); }
    };

    SP.Calendar.Constants.XmlToDayFilters = {
        '@day'          : function (day) { return true; },
        '@weekday'      : function (day) { return day.isWeekday; },
        '@weekend_day'  : function (day) { return day.isWeekendday; }
    };

    SP.Calendar.Constants.DayAttributes = (function () {
        var o = {};

        SP.Calendar.Constants.Days.forEach(function (day) {
            o[day.xmlProperty] = day.name;
        });

        return o;
    })();
  
    SP.Calendar.Utility = SP.Calendar.Utility || {};

    SP.Calendar.Utility.covertObjectToArray = function (o) {
        return Object.keys(o).map(function (key) {
            return o[key];
        });
    };

    SP.Calendar.Utility.getDayIndex = function(dayName) {
        return moment().set('day', dayName).get('day');
    };

    SP.Calendar.Utility.groupBy = R.compose(SP.Calendar.Utility.covertObjectToArray, R.groupBy);

    SP.Calendar.Utility.extractIndividualDays = function (dayAbbreviationToFullnameMap, o) {
        var days = [];

        Object.keys(o).forEach(function (key) {
            if (dayAbbreviationToFullnameMap.hasOwnProperty(key)) {
                days.push(dayAbbreviationToFullnameMap[key]);
            }
        });

        return days;
    };

    SP.Calendar.Utility.extractDays = function (o) {

        var filterfunc;
        var days;
        var sameXmlProp = function (a, b) {
            return (a.xmlProperty === b);
        };

        if (o['@day']) {
            days = SP.Calendar.Constants.Days;
        }
        else if (o['@weekend_day']) {
            filterfunc = SP.Calendar.Constants.XmlToDayFilters['@weekend_day'];
            days = SP.Calendar.Constants.Days.filter(filterfunc);
        }
        else if (o['@weekday']) {
            filterfunc = SP.Calendar.Constants.XmlToDayFilters['@weekday'];
            days = SP.Calendar.Constants.Days.filter(filterfunc);
        }
        else {
            days = R.intersectionWith(sameXmlProp, SP.Calendar.Constants.Days, Object.keys(o));
        }

        return days;
    };

    SP.Calendar.applyToRecurringEvents = function (f) {
        return function (calendar) {
            calendar.recurringEvents = f.call(null, calendar.recurringEvents);
            return calendar;
        };
    };

    SP.Calendar.separateRecurringFromNonRecurringEvents = function (events) {
        var byEventType = function (event) {
            var groupKey = 'recurringEvents';

            if(SP.Calendar.Constants.EventTypeMap.SINGLE === event.eventType) {
                groupKey = 'events';
            }
            
            return groupKey;
        };

        var groupedEvents = R.groupBy(byEventType, events);

        var separatedEvents = {
            events: [],
            recurringEvents: []
        };

        $.extend(separatedEvents, groupedEvents);

        return separatedEvents;
    };

    SP.Calendar.groupRelatedRecurringEvents = function (recurringEvents) {
        return SP.Calendar.Utility.groupBy(R.prop('uid'), recurringEvents);
    };

    

    SP.Calendar.separateRelatedRecurringEvents = function (relatedRecurringEvents) {
        var byEventType = function (event) {
            if(SP.Calendar.Constants.EventTypeMap.RECURRING === event.eventType) {
                return 'events';
            }
            else if(SP.Calendar.Constants.EventTypeMap.EXCEPTION === event.eventType) {
                return 'exceptions';
            }
            else if(SP.Calendar.Constants.EventTypeMap.DELETION === event.eventType) {
                return 'deletions';
            }

            throw Error("Error while attemping to separate recurring events by type. Could not handle unknown event.eventType: ", event.eventType);
        };

        var groupedEvents = R.groupBy(byEventType, relatedRecurringEvents);

        if (groupedEvents.events.length !== 1) {
            throw new Error("There must be exactly 1 main recurring event per related recurring event group. It is likely that the array of events passed are not all related. Check that the array passed to this all have the same uid.");
        }

        var recurringEventObject = {
            event: R.head(groupedEvents.events),
            exceptions: groupedEvents.exceptions,
            deletions: groupedEvents.deletions
        };

        return recurringEventObject;
    };

    SP.Calendar.Event = SP.Calendar.Event || {};

    SP.Calendar.Event.combineRawEventAndFieldValuesData = function (mixedEvent) {
        var durationString
            , duration
        ;

        // Set properties for normal single events and use default values for 
        var event = {
            description             : mixedEvent.originalEvent.Description,
            duration                : 0, // Will be overwritten later
            endDate                 : mixedEvent.originalEvent.EndDate,
            eventDate               : mixedEvent.originalEvent.EventDate,
            fAllDayEvent            : mixedEvent.originalEvent.fAllDayEvent,
            fRecurrence             : mixedEvent.originalEvent.fRecurrence,
            id                      : mixedEvent.originalEvent.Id,
            location                : mixedEvent.originalEvent.Location,
            title                   : mixedEvent.originalEvent.Title,

            // defaults for properties related to recurring event processing
            eventType               : 0,
            masterSeriesItemID      : null,
            recurrenceData          : null,
            recurrenceID            : null,
            timezone                : null,
            uid                     : null
        };

        // If this is an all day event the eventDate and endDate are incorrectly having a value related to local time but using Z for UTC zone
        if(mixedEvent.originalEvent.fAllDayEvent) {
            event.eventDate = moment(event.eventDate).add(moment().zone(), 'minutes').toJSON();
            event.endDate   = moment(event.endDate).add(moment().zone(), 'minutes').toJSON();
        }

        // If this is a recurrence event, we also have retrieved the extra properties
        if(
            (null !== mixedEvent.fieldValuesAsTextEvent)
            && ('object' === typeof mixedEvent.fieldValuesAsTextEvent)
        ) {
            event.eventType             = parseInt(mixedEvent.fieldValuesAsTextEvent.EventType, 10);
            event.masterSeriesItemID    = mixedEvent.fieldValuesAsTextEvent.MasterSeriesItemID;
            event.recurrenceData        = mixedEvent.fieldValuesAsTextEvent.RecurrenceData;
            event.recurrenceID          = mixedEvent.fieldValuesAsTextEvent.RecurrenceID;
            event.uid                   = mixedEvent.fieldValuesAsTextEvent.UID;

            // Parse duration string: '3,600' -> 3600
            durationString = mixedEvent.fieldValuesAsTextEvent.Duration.replace(/[^\d]/g, '');
            duration = parseInt(durationString, 10);
            event.duration = duration;
        }
        else {
            duration = moment.duration(moment(event.endDate) - moment(event.eventDate)).asSeconds();
            event.duration = duration;
        }
            
        return event;
    };
    
    SP.Calendar.Event.addGenerateEventsFunctionToRecurringEventObject = function (convertSpEventToDomainEvent) {
        return function(recurringEventObject) {
            recurringEventObject.generateEvents = SP.Calendar.Event.createEventGenerator(convertSpEventToDomainEvent, recurringEventObject);
            return recurringEventObject;
        }
    };

    SP.Calendar.Event.addMatchFunctionToRecurringEventObject = function(recurringEventObject) {
        var seriesStartDate = R.compose(momentf.dateOnly, moment)(recurringEventObject.event.eventDate);
        // add 1 day to end date to ensure date maximum
        // TODO: Verify if this is needed since in start date is used in later calculations...
        var seriesEndDate   = R.compose(momentf.dateOnly, momentf.add(1, 'days'), moment)(recurringEventObject.event.endDate);

        recurringEventObject.match = SP.Calendar.Event.convertSpRuleToMatchFunction(seriesStartDate, seriesEndDate, recurringEventObject.event.jsonRecurrenceData.rule);

        return recurringEventObject;
    };

    SP.Calendar.Event.addRecurrenceDataAsJson = function (event) {
        // TODO: Find better test to determine if recurrenceData is
        // holding XML or a string, perhaps use an isXml function?
        if (
            (typeof event.recurrenceData === 'string')
            && (event.recurrenceData.length > 0)
            && (event.recurrenceData[0] === '<')
        ) {
            var xmlDoc = $.parseXML(event.recurrenceData);
            var jsonRecurrenceData = JXON.build(xmlDoc.documentElement);
            event.jsonRecurrenceData = jsonRecurrenceData;
        }

        return event;
    };

    SP.Calendar.Event.calculateRecurringEventInstanceEndDate = function (seriesStartDate, seriesEndDate) {
        var momentSeriesStartDate = moment(seriesStartDate);
        var momentSeriesEndDate = moment(seriesEndDate);

        var momentAdjustedEndDate = momentSeriesStartDate.clone();
        momentAdjustedEndDate.hours(momentSeriesEndDate.hours());
        momentAdjustedEndDate.minutes(momentSeriesEndDate.minutes());
        momentAdjustedEndDate.seconds(momentSeriesEndDate.seconds());

        return momentAdjustedEndDate;
    };

    SP.Calendar.Event.convertSpRuleToMatchFunction = function(seriesStartMoment, seriesEndMoment, spRule) {
        var getDayIndicies = R.map(R.compose(SP.Calendar.Utility.getDayIndex, R.prop('name')));
        var rules = []
            , frequency
            , measure
            , rule
            , date
            , days
            , dayIndicies
            , firstOfMoment
        ;

        // Add Rule ensuring event is after start date.
        rules.push(momentf.isSameOrAfter(seriesStartMoment));

            // Add Rule ensuring event is before end date.
        rules.push(momentf.isBefore(seriesEndMoment));

        if (spRule) {
            if (spRule.repeat) {
                if (spRule.repeat.daily) {
                    // Example: every '2' days
                    if (spRule.repeat.daily['@dayfrequency']) {
                        frequency       = spRule.repeat.daily['@dayfrequency'];
                        measure         = 'days';
                        firstOfMoment   = seriesStartMoment.clone().startOf('day');

                        rules.push(momentf.isOfFrequency(firstOfMoment, frequency, measure, true));
                    }
                    // Example: every weekday aka. mo, tu, we, th, fr every week.
                    else if (spRule.repeat.daily['@weekday']) {
                        var weekdayIndicies = R.compose(R.map(R.compose(SP.Calendar.Utility.getDayIndex, R.prop('name'))), R.filter(R.propEq('isWeekday', true)))(SP.Calendar.Constants.Days);

                        rules.push(momentf.isDayInDays(weekdayIndicies));
                    }
                }
                    // Example: On [mo,tu,we,th,fr,sa,su] every 2 weeks
                else if (spRule.repeat.weekly) {
                    frequency       = spRule.repeat.weekly['@weekfrequency'];
                    measure         = 'week';
                    days            = SP.Calendar.Utility.extractDays(spRule.repeat.weekly);
                    dayIndicies     = getDayIndicies(days);
                    firstOfMoment   = seriesStartMoment.clone().startOf('week');

                    // Add rule to ensure correct day of week
                    rules.push(momentf.isDayInDays(dayIndicies));

                    // Add rule to ensure correct interval of week
                    rules.push(momentf.isOfFrequency(firstOfMoment, frequency, measure, false));
                }
                    // Example: The 18th day of every 2 months
                else if (spRule.repeat.monthly) {
                    frequency   = spRule.repeat.monthly['@monthfrequency'];
                    measure     = 'months';
                    date    = spRule.repeat.monthly['@day'];
                    firstOfMoment   = seriesStartMoment.clone().startOf('months');

                    // Add rule to ensure correct day of month, not sure if this is needed since interval check would overlap
                    rules.push(momentf.isEq('date', date));

                    // Add rule to ensure correct interval of month
                    rules.push(momentf.isOfFrequency(firstOfMoment, frequency, measure, false));
                }
                    // Example: on the 'second' 'thursday' of every 3 months
                else if (spRule.repeat.monthlybyday) {
                    frequency       = spRule.repeat.monthlybyday['@monthfrequency'];
                    measure         = 'months';
                    weekdayOfMonth  = spRule.repeat.monthlybyday['@weekdayofmonth'];

                    days            = SP.Calendar.Utility.extractDays(spRule.repeat.monthlybyday);
                    dayIndicies     = getDayIndicies(days);
                    firstOfMoment   = seriesStartMoment.clone().startOf('months');

                    // Add rule to ensure correct interval of month (Every 3 months)
                    rules.push(momentf.isOfFrequency(firstOfMoment, frequency, measure, false));

                    // Add rule to ensure correct occurance of day within month (Second Thursday)
                    rules.push(momentf.isOccerenceOfDayWithinMonth(weekdayOfMonth, dayIndicies));
                }
                else if (spRule.repeat.yearly) {
                    date    = spRule.repeat.yearly['@day'];
                    // months in sharepoint start at 1, but start at 0 for moment-recur, subtract 1 to convert
                    month   = spRule.repeat.yearly['@month'] - 1;

                    // Add rule to ensure correct month of year (December)
                    rules.push(momentf.isEq('month', month));

                    // Add rule to ensure correct date of month (25th)
                    rules.push(momentf.isEq('date', date));
                }
                    // Example: Repeat on the Second Tuesday of September
                else if (spRule.repeat.yearlybyday) {
                    frequency       = spRule.repeat.yearlybyday['@yearfrequency'];
                    measure         = 'years';
                    month           = spRule.repeat.yearlybyday['@month'] - 1;
                    weekdayOfMonth  = spRule.repeat.yearlybyday['@weekdayofmonth'];
                    days            = SP.Calendar.Utility.extractDays(spRule.repeat.yearlybyday);
                    dayIndicies     = getDayIndicies(days);

                    // Add rule to ensure correct month of year (December)
                    rules.push(momentf.isEq('month', month));

                    // Add rule to ensure correct occurance of day within month (Third Weekday)
                    rules.push(momentf.isOccerenceOfDayWithinMonth(weekdayOfMonth, dayIndicies));
                }
            }
        }

        return function match(moment) {
            return rules.every(function (f) {
                return f(moment);
            });
        };
    };

    SP.Calendar.Event.createEventInstanceFromRecurringEvent = function(recurringEventObject) {
        var mainEventStartMoment
            , mainEventEndMoment
        ;

        mainEventStartMoment = moment(recurringEventObject.event.eventDate);
        mainEventEndMoment   = mainEventStartMoment.clone().add(recurringEventObject.event.duration, 'seconds');

        return function (dateRange) {
            var eventInstanceClone
                , eventInstanceStart
                , eventInstanceEnd
            ;

            if( recurringEventObject.match(dateRange.start) ) {
                // get start and end moments of main recurring event
                eventInstanceStart = dateRange.start.clone();
                eventInstanceStart.hour(mainEventStartMoment.hour());
                eventInstanceStart.minute(mainEventStartMoment.minute());
                eventInstanceStart.seconds(mainEventStartMoment.seconds());

                eventInstanceEnd = eventInstanceStart.clone();
                eventInstanceEnd.hour(mainEventEndMoment.hour());
                eventInstanceEnd.minute(mainEventEndMoment.minute());
                eventInstanceEnd.seconds(mainEventEndMoment.seconds());

                eventInstanceClone = $.extend(true, {}, recurringEventObject.event);
                eventInstanceClone.eventDate = eventInstanceStart.toJSON();
                eventInstanceClone.endDate = eventInstanceEnd.toJSON();

                return eventInstanceClone;
            }
        }
    };

    SP.Calendar.Event.createEventGenerator = function(convertSpEventToDomainEvent, recurringEventObject) {
        return function (start, end, timezone, callback) {
            // support breakin change between calendar library versions
            if (callback === undefined && typeof timezone === 'function') {
                callback = timezone;
            }
                
            // Hack to get times adjust to client timezone
            var timezoneStart = moment();
            timezoneStart.startOf('day');
            timezoneStart.year(start.year());
            timezoneStart.month(start.month());
            timezoneStart.date(start.date());

            var timezoneEnd = moment();
            timezoneEnd.endOf('day');
            timezoneEnd.year(end.year());
            timezoneEnd.month(end.month());
            timezoneEnd.date(end.date());

            var dateRanges = momentf.splitRangeByDay(timezoneStart, timezoneEnd);

            var sparseEventsInstances = R.map(SP.Calendar.Event.createEventInstanceFromRecurringEvent(recurringEventObject))(dateRanges);
            var eventInstances = R.filter(R.identity)(sparseEventsInstances);
            var domainInstances = R.map(convertSpEventToDomainEvent)(eventInstances);
            // execute callback from fullcalendar widget
            callback(domainInstances)

            return domainInstances;
        }
    };

    // Seems like this has to go down there due to some things not being defined at higher positions, there is probably a beter way to structure this file

    SP.Calendar.normalizeEvents         = R.map(R.compose(SP.Calendar.Event.addRecurrenceDataAsJson, SP.Calendar.Event.combineRawEventAndFieldValuesData));
    SP.Calendar.parseRecurringEvents    = function (convertSpEventToDomainEvent) {
        return R.compose(R.map(R.compose(SP.Calendar.Event.addGenerateEventsFunctionToRecurringEventObject(convertSpEventToDomainEvent), SP.Calendar.Event.addMatchFunctionToRecurringEventObject, SP.Calendar.separateRelatedRecurringEvents)), SP.Calendar.groupRelatedRecurringEvents);
    };
    SP.Calendar.parseEvents             =  function (convertSpEventToDomainEvent) {
        return R.compose(SP.Calendar.applyToRecurringEvents(SP.Calendar.parseRecurringEvents(convertSpEventToDomainEvent)), SP.Calendar.separateRecurringFromNonRecurringEvents);
    };


})(JXON, moment, momentf, R, this.SP = this.SP || {});