(function (R, moment, momentf) {
    "use strict";

    momentf.isSameOrAfter  = R.curry(function(momentA, momentB) {
        return momentB.isSame(momentA) || momentB.isAfter(momentA);
    });

    momentf.isAfter = R.curry(function(momentA, momentB) {
        return momentB.isAfter(momentA);
    });

    momentf.isBefore = R.curry(function (momentA, momentB) {
        return momentB.isBefore(momentA);
    });

    momentf.isDateEq = R.curry(function (date, testMoment) {
        return (testMoment.date() === date);
    });

    momentf.add = R.curry(function (value, unit, moment) {
        return moment.clone().add(value, unit);
    });

    momentf.isOccerenceOfDayWithinMonth = R.curry(function (weekdayofmonth, dayIndex, moment) {
        var startOfMonth = moment.clone().startOf('month');
        var endOfMonth = moment.clone().endOf('month');
        var weekDayFilter = SP.Calendar.Constance.WeekdayToFilters[weekdayofmonth];
        var moments = momentf.splitRangeByDay(startOfMonth, endOfMonth);

        var daysMatchingIndicies = function (m) {
              return R.contains(m.day())(dayIndicies);
        };

        var filteredMoments = R.filter(daysMatchingIndicies)(moments);
        var day = R.find(weekDayFilter);

        // If the date we are trying to match has the same day of the month as the one we found through filtering it passes
        if(day && (day.date() === moment.date())) {
            return true;
        }

        return false;
    });

    momentf.dateOnly = function (moment) {
        var clone = moment.clone();
        clone.set('hours', 0);
        clone.set('minutes', 0);
        clone.set('seconds', 0);

        return clone;
    };

    momentf.splitRangeByDay = function (startMoment, endMoment) {
        var range = moment().range(startMoment, endMoment)
            , dateRanges = []
            , dateStart
            , dateEnd
            , dateRange
        ;

        range.by('day', function (m) {
            dateStart   = m.clone();
            dateEnd     = m.clone().add(1, 'days');
            dateRange   = moment().range(dateStart, dateEnd);
            dateRanges.push(dateRange);
        });

        return dateRanges;
    };

    momentf.isOfFrequency = R.curry(function(startMoment, interval, type, testMoment) {
        
        // TODO: ensure interval is > 0
        // TODO: ensure type is acceptable ('days', 'weeks', 'months', etc)

        var diff = startMoment.diff(testMoment, type, true);
        return ((diff % interval) === 0);
    });

    momentf.isDayInDays = R.curry(function(dayIndicies, testMoment) {
        return R.contains(dayIndicies)(testMoment.day());
    });

})(R, moment, this.momentf = this.momentf || {});