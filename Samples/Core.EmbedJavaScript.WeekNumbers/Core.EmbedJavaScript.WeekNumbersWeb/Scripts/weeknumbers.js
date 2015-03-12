Date.prototype.getWeek = function () {
    var year = this.getFullYear();
    var month = this.getMonth() + 1; //use 1-12
    var day = this.getDate();

    var a = Math.floor((14 - (month)) / 12);
    var y = year + 4800 - a;
    var m = (month) + (12 * a) - 3;
    var jd = day + Math.floor(((153 * m) + 2) / 5) +
                 (365 * y) + Math.floor(y / 4) - Math.floor(y / 100) +
                 Math.floor(y / 400) - 32045;      // (gregorian calendar)

    var d4 = (jd + 31741 - (jd % 7)) % 146097 % 36524 % 1461;
    var L = Math.floor(d4 / 1460);
    var d1 = ((d4 - L) % 365) + L;
    NumberOfWeek = Math.floor(d1 / 7) + 1;
    return NumberOfWeek;
};

ExecuteOrDelayUntilScriptLoaded(function () {

    Type.registerNamespace('Core.EmbedJavaScript.WeekNumbers');

    Core.EmbedJavaScript.WeekNumbers.AddWeekNumbers = function () {
        $(".ms-acal-month > TBODY > TR > TH[evtid='week']").each(function () {
            var firstDay = new Date($(this).attr("date"));
            if (firstDay.toString() != "NaN" && firstDay.toString() != "Invalid Date") {
                var week = firstDay.getWeek(firstDay.getDay());
                week = (week.toString().length == 1) ? '0' + week.toString() : week.toString();
                $(this).html("<div class='ms-picker-weekbox'><acronym title='Week number " + week + "'>" + week + "</acronym></div>");
                $(this).attr("class", "ms-picker-week");
                $(this).css("vertical-align", "middle");
            }
        });
    }

    Core.EmbedJavaScript.WeekNumbers.InterceptCalendarEvent = function () {
        ExecuteOrDelayUntilScriptLoaded(function () {
            var onItemsSucceed = SP.UI.ApplicationPages.CalendarStateHandler.prototype.onItemsSucceed;
            SP.UI.ApplicationPages.CalendarStateHandler.prototype.onItemsSucceed = function ($p0, $p1) {
                onItemsSucceed.call(this, $p0, $p1);
                Core.EmbedJavaScript.WeekNumbers.AddWeekNumbers();
            };
        }, "SP.UI.ApplicationPages.Calendar.js");
    }

    if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
        var url = _spPageContextInfo.siteServerRelativeUrl + "/SiteAssets/weeknumbers.js";
        RegisterModuleInit(url, Core.EmbedJavaScript.WeekNumbers.InterceptCalendarEvent);
    }

    Core.EmbedJavaScript.WeekNumbers.InterceptCalendarEvent();
}, "SP.js");