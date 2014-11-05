/* =
	jquery.eventCalendar.js
	version: 0.54
	date: 18-04-2013
	author:
		Jaime Fernandez (@vissit)
	company:
		Paradigma Tecnologico (@paradigmate)
*/

;$.fn.eventCalendar = function(options){

	var eventsOpts = $.extend({}, $.fn.eventCalendar.defaults, options);

	// define global vars for the function
	var flags = {
		wrap: "",
		directionLeftMove: "300",
		eventsJson: {}
	}

	// each eventCalendar will execute this function
	this.each(function(){

		flags.wrap = $(this);
		flags.wrap.addClass('eventCalendar-wrap').append("<div class='eventsCalendar-list-wrap'><p class='eventsCalendar-subtitle'></p><span class='eventsCalendar-loading'>loading...</span><div class='eventsCalendar-list-content'><ul class='eventsCalendar-list'></ul></div></div>");

		if (eventsOpts.eventsScrollable) {
			flags.wrap.find('.eventsCalendar-list-content').addClass('scrollable');
		}

		setCalendarWidth();
		$(window).resize(function(){
			setCalendarWidth();
		});
		//flags.directionLeftMove = flags.wrap.width();

		// show current month
		dateSlider("current");

		getEvents(eventsOpts.eventsLimit,false,false,false,false);

		changeMonth();

		flags.wrap.on('click','.eventsCalendar-day a',function(e){
		//flags.wrap.find('.eventsCalendar-day a').live('click',function(e){
			e.preventDefault();
			var year = flags.wrap.attr('data-current-year'),
				month = flags.wrap.attr('data-current-month'),
				day = $(this).parent().attr('rel');

			getEvents(false, year, month,day, "day");
		});
		flags.wrap.on('click','.monthTitle', function(e){
		//flags.wrap.find('.monthTitle').live('click',function(e){
			e.preventDefault();
			var year = flags.wrap.attr('data-current-year'),
				month = flags.wrap.attr('data-current-month');

			getEvents(eventsOpts.eventsLimit, year, month,false, "month");
		})



	});

	// show event description
	flags.wrap.find('.eventsCalendar-list').on('click','.eventTitle',function(e){
	//flags.wrap.find('.eventsCalendar-list .eventTitle').live('click',function(e){
		if(!eventsOpts.showDescription) {
			e.preventDefault();
			var desc = $(this).parent().find('.eventDesc');

			if (!desc.find('a').size()) {
				var eventUrl = $(this).attr('href');
				var eventTarget = $(this).attr('target');

				// create a button to go to event url
				desc.append('<a href="' + eventUrl + '" target="'+eventTarget+'" class="bt">'+eventsOpts.txt_GoToEventUrl+'</a>')
			}

			if (desc.is(':visible')) {
				desc.slideUp();
			} else {
				if(eventsOpts.onlyOneDescription) {
					flags.wrap.find('.eventDesc').slideUp();
				}
				desc.slideDown();
			}

		}
	});

	function sortJson(a, b){
		return a.date.toLowerCase() > b.date.toLowerCase() ? 1 : -1;
	};

	function dateSlider(show, year, month) {
		var $eventsCalendarSlider = $("<div class='eventsCalendar-slider'></div>"),
			$eventsCalendarMonthWrap = $("<div class='eventsCalendar-monthWrap'></div>"),
			$eventsCalendarTitle = $("<div class='eventsCalendar-currentTitle'><a href='#' class='monthTitle'></a></div>"),
			$eventsCalendarArrows = $("<a href='#' class='arrow prev'><span>" + eventsOpts.txt_prev + "</span></a><a href='#' class='arrow next'><span>" + eventsOpts.txt_next + "</span></a>");
			$eventsCalendarDaysList = $("<ul class='eventsCalendar-daysList'></ul>"),
			date = new Date();

		if (!flags.wrap.find('.eventsCalendar-slider').size()) {
			flags.wrap.prepend($eventsCalendarSlider);
			$eventsCalendarSlider.append($eventsCalendarMonthWrap);
		} else {
			flags.wrap.find('.eventsCalendar-slider').append($eventsCalendarMonthWrap);
		}

		flags.wrap.find('.eventsCalendar-monthWrap.currentMonth').removeClass('currentMonth').addClass('oldMonth');
		$eventsCalendarMonthWrap.addClass('currentMonth').append($eventsCalendarTitle, $eventsCalendarDaysList);



		// if current show current month & day
		if (show === "current") {
			day = date.getDate();
			$eventsCalendarSlider.append($eventsCalendarArrows);

		} else {
			date = new Date(flags.wrap.attr('data-current-year'),flags.wrap.attr('data-current-month'),1,0,0,0); // current visible month
			day = 0; // not show current day in days list

			moveOfMonth = 1;
			if (show === "prev") {
				moveOfMonth = -1;
			}
			date.setMonth( date.getMonth() + moveOfMonth );

			var tmpDate = new Date();
			if (date.getMonth() === tmpDate.getMonth()) {
				day = tmpDate.getDate();
			}

		}

		// get date portions
		var year = date.getFullYear(), // year of the events
			currentYear = (new Date).getFullYear(), // current year
			month = date.getMonth(), // 0-11
			monthToShow = month + 1;

		if (show != "current") {
			// month change
			getEvents(eventsOpts.eventsLimit, year, month,false, show);
		}

		flags.wrap.attr('data-current-month',month)
			.attr('data-current-year',year);

		// add current date info
		$eventsCalendarTitle.find('.monthTitle').html(eventsOpts.monthNames[month] + " " + year);

		// print all month days
		var daysOnTheMonth = 32 - new Date(year, month, 32).getDate();
		var daysList = [];
		if (eventsOpts.showDayAsWeeks) {
			$eventsCalendarDaysList.addClass('showAsWeek');

			// show day name in top of calendar
			if (eventsOpts.showDayNameInCalendar) {
				$eventsCalendarDaysList.addClass('showDayNames');

				var i = 0;
				// if week start on monday
				if (eventsOpts.startWeekOnMonday) {
					i = 1;
				}

				for (; i < 7; i++) {
					daysList.push('<li class="eventsCalendar-day-header">'+eventsOpts.dayNamesShort[i]+'</li>');

					if (i === 6 && eventsOpts.startWeekOnMonday) {
						// print sunday header
						daysList.push('<li class="eventsCalendar-day-header">'+eventsOpts.dayNamesShort[0]+'</li>');
					}

				}
			}

			dt=new Date(year, month, 01);
			var weekDay = dt.getDay(); // day of the week where month starts

			if (eventsOpts.startWeekOnMonday) {
				weekDay = dt.getDay() - 1;
			}
			if (weekDay < 0) { weekDay = 6; } // if -1 is because day starts on sunday(0) and week starts on monday
			for (i = weekDay; i > 0; i--) {
				daysList.push('<li class="eventsCalendar-day empty"></li>');
			}
		}
		for (dayCount = 1; dayCount <= daysOnTheMonth; dayCount++) {
			var dayClass = "";

			if (day > 0 && dayCount === day && year === currentYear) {
				dayClass = "today";
			}
			daysList.push('<li id="dayList_' + dayCount + '" rel="'+dayCount+'" class="eventsCalendar-day '+dayClass+'"><a href="#">' + dayCount + '</a></li>');
		}
		$eventsCalendarDaysList.append(daysList.join(''));

		$eventsCalendarSlider.css('height',$eventsCalendarMonthWrap.height()+'px');
	}

	function num_abbrev_str(num) {
		var len = num.length, last_char = num.charAt(len - 1), abbrev
		if (len === 2 && num.charAt(0) === '1') {
			abbrev = 'th'
		} else {
			if (last_char === '1') {
				abbrev = 'st'
			} else if (last_char === '2') {
				abbrev = 'nd'
			} else if (last_char === '3') {
				abbrev = 'rd'
			} else {
				abbrev = 'th'
			}
		}
		return num + abbrev
	}

	function getEvents(limit, year, month, day, direction) {
		var limit = limit || 0;
		var year = year || '';
		var day = day || '';

		// to avoid problem with january (month = 0)

		if (typeof month != 'undefined') {
			var month = month;
		} else {
			var month = '';
		}

		//var month = month || '';
		flags.wrap.find('.eventsCalendar-loading').fadeIn();

		if (eventsOpts.jsonData) {
			// user send a json in the plugin params
			eventsOpts.cacheJson = true;

			flags.eventsJson = eventsOpts.jsonData;
			getEventsData(flags.eventsJson, limit, year, month, day, direction);

		} else if (!eventsOpts.cacheJson || !direction) {
			// first load: load json and save it to future filters
			$.getJSON(eventsOpts.eventsjson + "?limit="+limit+"&year="+year+"&month="+month+"&day="+day, function(data) {
				flags.eventsJson = data; // save data to future filters
				getEventsData(flags.eventsJson, limit, year, month, day, direction);
			}).error(function() {
				showError("error getting json: ");
			});
		} else {
			// filter previus saved json
			getEventsData(flags.eventsJson, limit, year, month, day, direction);
		}

		if (day > '') {
			flags.wrap.find('.current').removeClass('current');
			flags.wrap.find('#dayList_'+day).addClass('current');
		}
	}

	function getEventsData(data, limit, year, month, day, direction){
		directionLeftMove = "-=" + flags.directionLeftMove;
		eventContentHeight = "auto";

		subtitle = flags.wrap.find('.eventsCalendar-list-wrap .eventsCalendar-subtitle')
		if (!direction) {
			// first load
			subtitle.html(eventsOpts.txt_NextEvents);
			eventContentHeight = "auto";
			directionLeftMove = "-=0";
		} else {
			if (day != '') {
				subtitle.html(eventsOpts.txt_SpecificEvents_prev + eventsOpts.monthNames[month] + " " + num_abbrev_str(day) + " " + eventsOpts.txt_SpecificEvents_after);
			} else {
				subtitle.html(eventsOpts.txt_SpecificEvents_prev + eventsOpts.monthNames[month] + " " + eventsOpts.txt_SpecificEvents_after);
			}

			if (direction === 'prev') {
				directionLeftMove = "+=" + flags.directionLeftMove;
			} else if (direction === 'day' || direction === 'month') {
				directionLeftMove = "+=0";
				eventContentHeight = 0;
			}
		}

		flags.wrap.find('.eventsCalendar-list').animate({
			opacity: eventsOpts.moveOpacity,
			left: directionLeftMove,
			height: eventContentHeight
		}, eventsOpts.moveSpeed, function() {
			flags.wrap.find('.eventsCalendar-list').css({'left':0, 'height': 'auto'}).hide();
			//wrap.find('.eventsCalendar-list li').fadeIn();

			var events = [];

			data = $(data).sort(sortJson); // sort event by dates

			// each event
			if (data.length) {

				// show or hide event description
				var eventDescClass = '';
				if(!eventsOpts.showDescription) {
					eventDescClass = 'hidden';
				}
				var eventLinkTarget = "_self";
				if(eventsOpts.openEventInNewWindow) {
					eventLinkTarget = '_target';
				}

				var i = 0;
				$.each(data, function(key, event) {
					if (eventsOpts.jsonDateFormat == 'human') {
						var eventDateTime = event.date.split(" "),
							eventDate = eventDateTime[0].split("-"),
							eventTime = eventDateTime[1].split(":"),
							eventYear = eventDate[0],
							eventMonth = parseInt(eventDate[1]) - 1,
							eventDay = parseInt(eventDate[2]),
							//eventMonthToShow = eventMonth,
							eventMonthToShow = parseInt(eventMonth) + 1,
							eventHour = eventTime[0],
							eventMinute = eventTime[1],
							eventSeconds = eventTime[2],
							eventDate = new Date(eventYear, eventMonth, eventDay, eventHour, eventMinute, eventSeconds);
					} else {
						var eventDate = new Date(parseInt(event.date)),
							eventYear = eventDate.getFullYear(),
							eventMonth = eventDate.getMonth(),
							eventDay = eventDate.getDate(),
							eventMonthToShow = eventMonth + 1,
							eventHour = eventDate.getHours(),
							eventMinute = eventDate.getMinutes();

					}

					if (parseInt(eventMinute) <= 9) {
						eventMinute = "0" + parseInt(eventMinute);
					}


					if (limit === 0 || limit > i) {
						// if month or day exist then only show matched events

						if ((month === false || month == eventMonth)
								&& (day == '' || day == eventDay)
								&& (year == '' || year == eventYear) // get only events of current year
							) {
								// if initial load then load only future events
								if (month === false && eventDate < new Date()) {

								} else {
									//eventStringDate = eventDay + "/" + eventMonthToShow + "/" + eventYear;
									eventStringDate = moment(eventDate).format(eventsOpts.dateFormat);

									if (event.url) {
										var eventTitle = '<a href="'+event.url+'" target="' + eventLinkTarget + '" class="eventTitle">' + event.title + '</a>';
									} else {
										var eventTitle = '<span class="eventTitle">'+event.title+'</span>';
									}
									events.push('<li id="' + key + '" class="'+event.type+'"><time datetime="'+eventDate+'"><em>' + eventStringDate + '</em><small>'+eventHour+":"+eventMinute+'</small></time>'+eventTitle+'<p class="eventDesc ' + eventDescClass + '">' + event.description + '</p></li>');
									i++;
								}
						}
					}

					// add mark in the dayList to the days with events
					if (eventYear == flags.wrap.attr('data-current-year') && eventMonth == flags.wrap.attr('data-current-month')) {
						flags.wrap.find('.currentMonth .eventsCalendar-daysList #dayList_' + parseInt(eventDay)).addClass('dayWithEvents');
					}

				});
			}
			// there is no events on this period
			if (!events.length) {
				events.push('<li class="eventsCalendar-noEvents"><p>' + eventsOpts.txt_noEvents + '</p></li>');
			}
			flags.wrap.find('.eventsCalendar-loading').hide();

			flags.wrap.find('.eventsCalendar-list')
				.html(events.join(''));

			flags.wrap.find('.eventsCalendar-list').animate({
				opacity: 1,
				height: "toggle"
			}, eventsOpts.moveSpeed);


		});
		setCalendarWidth();
	}

	function changeMonth() {
		flags.wrap.find('.arrow').click(function(e){
			e.preventDefault();

			if ($(this).hasClass('next')) {
				dateSlider("next");
				var lastMonthMove = '-=' + flags.directionLeftMove;

			} else {
				dateSlider("prev");
				var lastMonthMove = '+=' + flags.directionLeftMove;
			}

			flags.wrap.find('.eventsCalendar-monthWrap.oldMonth').animate({
				opacity: eventsOpts.moveOpacity,
				left: lastMonthMove
			}, eventsOpts.moveSpeed, function() {
				flags.wrap.find('.eventsCalendar-monthWrap.oldMonth').remove();
			});
		});
	}

	function showError(msg) {
		flags.wrap.find('.eventsCalendar-list-wrap').html("<span class='eventsCalendar-loading error'>"+msg+" " +eventsOpts.eventsjson+"</span>");
	}

	function setCalendarWidth(){
		// resize calendar width on window resize
		flags.directionLeftMove = flags.wrap.width();
		flags.wrap.find('.eventsCalendar-monthWrap').width(flags.wrap.width() + 'px');

		flags.wrap.find('.eventsCalendar-list-wrap').width(flags.wrap.width() + 'px');

	}
};


// define the parameters with the default values of the function
$.fn.eventCalendar.defaults = {
    eventsjson: 'js/events.json',
	eventsLimit: 4,
	monthNames: [ "January", "February", "March", "April", "May", "June",
		"July", "August", "September", "October", "November", "December" ],
	dayNames: [ 'Sunday','Monday','Tuesday','Wednesday',
		'Thursday','Friday','Saturday' ],
	dayNamesShort: [ 'Sun','Mon','Tue','Wed', 'Thu','Fri','Sat' ],
	txt_noEvents: "There are no events in this period",
	txt_SpecificEvents_prev: "",
	txt_SpecificEvents_after: "events:",
	txt_next: "next",
	txt_prev: "prev",
	txt_NextEvents: "Next events:",
	txt_GoToEventUrl: "See the event",
	showDayAsWeeks: true,
	startWeekOnMonday: true,
	showDayNameInCalendar: true,
	showDescription: false,
	onlyOneDescription: true,
	openEventInNewWindow: false,
	eventsScrollable: false,
	dateFormat: "D/MM/YYYY",
	jsonDateFormat: 'timestamp', // you can use also "human" 'YYYY-MM-DD HH:MM:SS'
	moveSpeed: 500,	// speed of month move when you clic on a new date
	moveOpacity: 0.15, // month and events fadeOut to this opacity
	jsonData: "", 	// to load and inline json (not ajax calls)
	cacheJson: true	// if true plugin get a json only first time and after plugin filter events
					// if false plugin get a new json on each date change
};