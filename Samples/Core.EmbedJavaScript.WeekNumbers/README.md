# Core.EmbedJavaScript.WeekNumbers #

### Summary ###
In some parts of the world it's important to have week numbers on calendar views. In SharePoint week numbers can be 
activated on the date picker. There is however no built in function for displaying week numbers in a calendar's monthly view. 
This sample embeds some JavaScript code that adds week numbers. The code works both with minimum download strategy turned on and off.

***Notice**. Techniques shown in this sample do require full permission to web or site collection level, so this is not a suitable model for apps designed to be distributed from the SharePoint store.*

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
A calendar on the host web.

### Solution ###
Solution | Author(s)
---------|----------
Core.EmbedJavaScript.WeekNumbers | Johan Skårman (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 19th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
The code basically has two main parts. First we need to intercept the client side calls that the calendar view does in SharePoint 2013. Thanks to this post for showing how to do that.

```JavaScript
    Core.EmbedJavaScript.WeekNumbers.InterceptCalendarEvent = function () {
        ExecuteOrDelayUntilScriptLoaded(function () {
            var onItemsSucceed = SP.UI.ApplicationPages.CalendarStateHandler.prototype.onItemsSucceed;
            SP.UI.ApplicationPages.CalendarStateHandler.prototype.onItemsSucceed = function ($p0, $p1) {
                onItemsSucceed.call(this, $p0, $p1);
                Core.EmbedJavaScript.WeekNumbers.AddWeekNumbers();
            };
        }, "SP.UI.ApplicationPages.Calendar.js");
    }
```

Secondly using JQuery we can find all TH elements and add week numbers.

```JavaScript
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
```

Result should look something like the image below.

![Calendar view with week numbers](http://i.imgur.com/tJNFtYL.png)

#NOTE#
A function that returns the week numbers according to the Gregorian calendar is used but that could be replaced by any other calculation. Also this example takes some dependencies on both how the HTML in a SharePoint calendar is structured and also on the calendar scripts. So updates to how SharePoint renders a calendar can affect this sample.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.EmbedJavaScript.WeekNumbers" />