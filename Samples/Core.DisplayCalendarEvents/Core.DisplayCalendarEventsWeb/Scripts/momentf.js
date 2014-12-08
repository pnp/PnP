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

    momentf.isEq = R.curry(function (property, value, moment) {
        return (value === moment.get(property));
    });

    momentf.add = R.curry(function (value, unit, moment) {
        return moment.clone().add(value, unit);
    });

    momentf.isOccerenceOfDayWithinMonth = R.curry(function (weekdayofmonth, dayIndicies, moment) {
        var startOfMonth    = moment.clone().startOf('month');
        var endOfMonth      = moment.clone().endOf('month');
        var weekDayFilter   = SP.Calendar.Constants.WeekdayToFilters[weekdayofmonth];
        var moments         = momentf.splitRangeByDay(startOfMonth, endOfMonth);

        var daysMatchingIndicies = function (m) {
            return R.contains(m.day())(dayIndicies);
        };

        var filteredMoments = R.compose(R.filter(daysMatchingIndicies), R.map(R.prop('start')))(moments);
        var days = R.filter.idx(weekDayFilter)(filteredMoments);

        // If the date we are trying to match has the same day of the month as the one we found through filtering it passes
        if((days.length === 1) && (days[0].date() === moment.date())) {
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

    momentf.isOfFrequency = R.curry(function(startMoment, interval, type, floating, testMoment) {
        // TODO: ensure interval is > 0
        // TODO: ensure floating is boolean
        // TODO: ensure type is acceptable ('days', 'weeks', 'months', etc)

        var diff = startMoment.diff(testMoment, type, floating);
        return ((diff % interval) === 0);
    });

    momentf.isDayInDays = R.curry(function(dayIndicies, testMoment) {
        return R.contains(testMoment.day())(dayIndicies);
    });

})(R, moment, this.momentf = this.momentf || {});