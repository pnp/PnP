# Display (recurring) calendar events #

### Summary ###
Sample that shows how to use REST API to retrieve calendar events and process them in order to display them using a third party component which has features SharePoint does not such as tooltip information on when hovering over an event, or multiple calendar overlays with custom styling based on event properties, etc... Encapsulates all the logic specific to sharepoint events into file: SP.Calendar.js which other people could use in their projects to achieve similar results using their preferred framework.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
-  Calendar named Holidays in the host web with at least 1 event in it
-  Calendar named Meetings in the host web with at least 1 event in it

### Solution ###
Solution | Author(s)
---------|----------
Core.DisplayCalendarEvents | Matt Mazzola (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 8th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


# Introduction #
![UI  with custom recurrent entries](http://i.imgur.com/yHSpcgb.png)

Imagine you are tasked with creating an add-in that displays calendars from SharePoint.
At first this add-in seems rather straight forward.  As with most apps the logic would be similar to the following:

1.	Request calendar list items 
2.	Convert list items to event objects for UI component
3.	Bind event objects to calendar component which will handle rendering etc.

As it turns out Steps 1 and 2 have significant challenges to overcome and the design of the add-in quickly becomes so complex you would be inclined to give up. However, if SharePoint has taught us anything, it's that broken API's and inconsistent behavior can be overcome, sometimes we just need a bigger hammer.

## Step 1 Problem ##
The task for step 1 is to retrieve all the data needed for the add-in from SharePoint. After opening up Fiddler and testing a few requests you would notice we are missing the data required to show repeating events.

Sample Url: `https://https://<subdomain>.sharepoint.com/site/<sitename>/_api/web/lists/getbytitle(‘Calendar’)/items`

Sample List Item Response:
```JavaScript
{
	"Attachments": false,
    "AuthorId": 22,
    "Category": null,
    "ContentTypeId": "0x0102004DF11EA88989F040B9E108189FD3B934",
    "Created": "2014-10-13T20:16:27Z",
    "Description": null,
    "EditorId": 22,
    "EndDate": "2014-10-13T21:00:00Z",
    "EventDate": "2014-10-13T20:00:00Z",
    "FileSystemObjectType": 0,
    "FreeBusy": null,
    "GUID": "075ecd3e-217c-4a46-be9f-2a0c030f50f4",
    "ID": 1,
    "Id": 1,
    "Location": null,
    "Modified": "2014-10-13T20:16:27Z",
    "OData__UIVersionString": "1.0",
    "Overbook": null,
    "ParticipantsPickerId": null,
    "Title": "Test Event 01",
    "fAllDayEvent": false,
    "fRecurrence": false,
    "odata.editLink": "Web/Lists(guid'59c97b32-0d66-48ae-b65d-3931d3c40b46')/Items(1)",
    "odata.etag": "\"1\"",
    "odata.id": "01568583-c22e-4c78-93b5-0b2a0b9c6b94",
    "odata.type": "SP.Data.Cash_x0020_FlowListItem"
}
```

It's surprising that the default response is missing so many properties. We would need the following to process the event correctly `Duration, EventType, RecurrenceData, RecurrenceID, TimeZone, UID, XMLTZone`

There is even an open issue on UserVoice which was supposed to been responded to on 8/1 but was forgotten about:
https://officespdev.uservoice.com/forums/224641-general/suggestions/5928804-provide-csom-and-rest-api-for-recurring-calendar-e

## Step 1 Solution ##
To get around this issue we can use the special property `fieldvaluesastext` on the list item which for some undocumented/magic reason, will show the properties we couldn't see before instead of meerly transforming them to text as the name suggests.  Semantics comes in the next version ;) Anyways enough joking. Unfortunately, because we're retrieving them as a string, they are not typed properly. Nevertheless as least we have the data.

Sample Url: `https://https://<subdomain>.sharepoint.com/site/<sitename>/_api/web/lists/getbytitle(‘Calendar’)/items(1)/fieldvaluesastext`

Sample Response (lots of other properties removed):

```JavaScript
{
	...
    "Duration": "3,600",
    ...
    "EventType": "1",
    ...
    "MasterSeriesItemID": "",
    ...
    "RecurrenceData": "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><daily dayFrequency=\"4\" /></repeat><repeatInstances>10</repeatInstances></rule></recurrence>",
    "RecurrenceID": "",
    "TimeZone": "10",
    ...
    "UID": "8dd2f14c-32c1-4068-8fdd-0eb9a9f1533c",
    "UniqueId": "9e85032f-833f-43f3-930c-fe215829cf0e",
    "XMLTZone": "<timeZoneRule><standardBias>300</standardBias><additionalDaylightBias>-60</additionalDaylightBias><standardDate><transitionRule  month='11' day='su' weekdayOfMonth='first' /><transitionTime>2:0:0</transitionTime></standardDate><daylightDate><transitionRule  month='3' day='su' weekdayOfMonth='second' /><transitionTime>2:0:0</transitionTime></daylightDate></timeZoneRule>",
    ...
}
```

There is no way to get the `/fieldvaluesastext` response for all items in one request which means we have to make 1 extra request per list item that has (fRecurrence === true) which can be very slow depending on the amount of repeating events.
(Update: With the new batching API perhaps there is a way to solve this, but I have not tested)

Remember, the goal at the end of this step is to have one list item object with all of the necessary data to continue, by combining the original event data with the fieldvaluesastext event data and coercing some values we have a solution.

In order do this I first make a request for all the list items, then iterate through all the items and transform them into a mixedEvent object which contains both values. The fieldValuesAsText property is set to a promise which will either resolve as null in the case of a normal event or resolve as the fieldValuesAsText response.  At this point we have a list of mixedEventObjects. Next, pass those through a function which combines them.

```JavaScript
var mixedObject = {
	originalEvent: { ... },
	fieldValuesAsTextEvent: { ... }
};
```

```JavaScript
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
```

Notice how we have to correct the eventDate and endDate for all day events. As far as I know this is a bug in the API which incorrectly reports the datetime value in local time but includes the Z in the string which forces it to be parsed as UTC. **Example**: Assume your computer timezone is set to Central Time which is UTC-6:00 or 360. You create an All Day event for December 25th to represent Christmas. In local time the ISO8601 string value for the start time would be "2014-12-25T00:00:00" and the end time would be "2014-12-25T23:59:59"; However because these are local times, they would need to have the timezone offset applied in order to be represented in UTC which is what the JSON value always is. This would make them to make them "2014-12-25T06:00:00Z" and "2014-12-26T05:59:59Z".  This is why you can enter moment().toJSON() in the console and it's always offset by your current timezone.  

SharePoint would incorrectly return "2014-12-25T00:00:00Z" which actually means "2014-12-24T18:00:00"! This is a big deal because when the component interprets the date it would be rendered as starting 6 hours ahead which would be the previous day and be greatly misinforming the user. It seems ridiculous right? Hopefully someone else can verify this is an error and maybe we can complain on UserVoice.


## Step 2 Problems ##
### Step 2 Problem 1: How to parse recurrenceData ###
There is a bug with the API which doesn't follow proper [Content Negotiation](http://en.wikipedia.org/wiki/Content_negotiation "Content Negotiation") rules and returns recurrenceData and timezone fields as XML even though we asked for JSON. We need to be able to read this data in order to understand the recurrence rule such as 'repeat every three days' or 'repeat every 3rd wednesday'.  In order to make this easy to do in JavaScript the best format is JSON.

### Step 2 Solution 1: Use JXON to convert XML to JSON ###
As you will find there is a standard for converting XMl to JSON, called JXON.  I used an implementation available from Mozilla which you can find here: [https://developer.mozilla.org/en-US/docs/JXON](https://developer.mozilla.org/en-US/docs/JXON)

The code to use this script looks like the following:

```JavaScript
var xmlDoc = $.parseXML(event.recurrenceData);
var jsonRecurrenceData = JXON.build(xmlDoc.documentElement);
event.jsonRecurrenceData = jsonRecurrenceData;
```

At this point we have the JSON representation of the Recurrence data on the event object which we can further process.

### Step 2 Problem 2: Grouping Events ###
We've been making a lot of fuss over how to parse this recurrence data, and you might have started to wonder how calendars actually store recurring events in a scalable way. If an event repeated every day for 10 years, surely there is a better way than storing ~3650 individual events, What if the event repeats forever? Well you are correct there is a better way, and that's why there are different event types.

### Step 2 Solution 2: Grouping Events by EventType ###

There are 4 different kinds of events: Single, Recurring, Exception, Deletion. The first is the normal event, and the last three are all related to recurring events. The names are fairly descriptive but I'll explain them below:

**Single:**		(EventType: 0) These are the normal single instance of an event.

**Recurring:**	(EventType: 1) These are the main instance of the recurring event which contains the repeating rule, full ranged start/end dates, etc.

**Exception:** 	(EventType: 4) These are events which override instances of a recurring event. E.g. you have an event that repeats every Thursday at 3pm, but you can create an exception to the rule so that the Thursday on Thanksgiving occurs at 10am.

**Deletion:** (EventType: 3) Similar to exception, but instead of override, this is removal of a specific instance.

> What about EvenType: 2? I have no idea either...

Source: [https://fatalfrenchy.wordpress.com/2010/07/16/sharepoint-recurrence-data-schema/](https://fatalfrenchy.wordpress.com/2010/07/16/sharepoint-recurrence-data-schema/)

Now back to the topic.  Remember, this is an intermediate step in the process of converting the raw event data from step 1 into something usable by a calendar component/widget and we're trying to group the events so they can be processed as a whole.

Grouping process:

1. Separate recurring events from single events; Filter on event type == 0;
	
		{
			events: [...],
			recurringEvents: [...]
		}

2. Group the related recurring events; Group by UID

		{
			events: [...],
			recurringEvents: [[..],[..]]
		}

3. Group related recurring events by type
	
		{
			events: [...],
			recurringEvents: [
				{
		            event: {},
		            exceptions: [...],
		            deletions: [...]
	        	},
				{
		            event: {},
		            exceptions: [...],
		            deletions: [...]
	        	}
			]
		} 

### Step 2 Problem 3: Build a date generator function from the grouped recurring events ###
In the previous problem we noted that it wasn't scalable to store all the instances of a recurring event, and learned how SharePoint stores recurring events as rules in XML, combined with exceptions and deletion instances. This explains the storage but how would one render these events?  

Look at the information and skills we already have.  We know how to display explicit events, and we know the rules for the recurrence event. Using this, we realize we can transform those declarative recurrence rules from SharePoint into a function that takes a date range (such as the current month) and gives back a list of events that would occur based on the recurring event rules, which we can then display as we normally would.

This task is by far the hardest of the problems. Up until now we have solved problems by adding extra REST calls and adding some extra functions to transform the data into a better structure which are annoying, but conceptually very easy to understand once the sequence of events is explained. Let's look at our goal and break down the problem into smaller pieces to solve it more easily.

### Step 2 Solution 3 ###
Our goal here is to write a function which takes a recurring event object (which we generated from the steps above) and transforms it into another function which will generate dates based on the objects. The particular widget i'm using actually passes as callback into the function which you pass event instances in order to render them.

```JavaScript
function (recurringEventObject) {
	...
	//setup
	...
	return function (startDatetime, endDatetime, callback) {
		...
		var domainInstances = [{...}, {...}]; // list of events occured within range
		callback(domainInstances)
	}
}
```
Through the next sections we will walk through the setup and generation logic.

The first thing we need in the generation logic is another sub function which answers a simpler question. **Given a set of recurrence rules and a day does the day occur within the rules? (Example: If the rules are "occurs every Thursday" If the day is Thursday return true otherwise return false.)** Obviously these become very complex as the rules become more complex like "the last Thursday in November" or "the 25th of December" etc. We will call this 
function `match` and we will add it to the recurringEvent object so it can be asked if a date matches the rules.

To create this match function we are just turning the rules into a set of asserts which take a date and return true or false. If all of them are true, then the date is accepted.

The match function would look like this: 

```JavaScript
SP.Calendar.Event.convertSpRuleToMatchFunction = function(seriesStartMoment, seriesEndMoment, spRule) {

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

                    rules.push(momentf.isOfFrequency(firstOfMoment, frequency, measure));
                }

				...
				other rules
				...

			}
		}
	}

	return function match(moment) {
        return rules.every(function (f) {
            return f(moment);
        });
    };

};
```

Now this match function only tells us whether a specific date is accepted or not. We must still create a copy of the recurring event's data such as title, location, etc to generate a eventInstance for that particular date.
We will call this function `SP.Calendar.Event.createEventInstanceFromRecurringEvent` and then from this this our overall generation function would look something like this:

```JavaScript
SP.Calendar.Event.createEventGenerator = function (recurringEventObject) {

	return function (startDatetime, endDatetime) {

		...
		setup logic
		...

		var dateRanges = momentf.splitRangeByDay(viewStart, viewEnd);

        var sparseEventsInstances = R.map(SP.Calendar.Event.createEventInstanceFromRecurringEvent(recurringEventObject))(dateRanges);
        var eventInstances = R.filter(R.identity)(sparseEventsInstances);
        var domainInstances = R.map(convertSpEventToDomainEvent)(eventInstances);
        // execute callback from fullcalendar widget
        callback(domainInstances)

        return domainInstances;
	}
}
```

This would work, but it doesn't account for deletions or exceptions.  Event though the day matches the rules, there maybe a deletion event which means we should not push an instance or exception event overriding properties on the instance. To account for these we would intro some extra logic which checks all the eventInstances and replaces it with the exception or removes it if there is a deletion.

There lots of details that aren't covered here such as details the match generation function, and how to generate event instances with the correct times, but those can be seen in the source code.


## Step 3 ##
If you made it this far congratulations. Luckily there are no inherent problems with step 3, if you use a proper component you should just be able to pass it the eventSource which is a combination of single events and recurring event functions.

In my example I am using an Angular component built on the full calendar widget. 

- [https://github.com/angular-ui/ui-calendar](https://github.com/angular-ui/ui-calendar)
- [http://fullcalendar.io/](http://fullcalendar.io/)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.DisplayCalendarEvents" />