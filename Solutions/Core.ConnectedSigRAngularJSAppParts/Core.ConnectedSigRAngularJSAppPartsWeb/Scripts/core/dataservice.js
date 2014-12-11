(function () {
    'use strict';

    // dataservice factory
    angular
        .module('app.core')
        .factory('dataService', ['$http', '$location', '$q', function dataService($http, $location, $q) {                
            
                var isPrimed = false;
                var primePromise;

                var service = {
                    getEvents: getEvents,
                    getSessions: getSessions,
                    getSpeakers: getSpeakers,
                    addEvent: addEvent,
                    deleteEvent: deleteEvent,
                    ready: ready
                };

                return service;
                          

                function getEvents() {
                    var events = [];
                    return getCorporateEvents()
                        .then(getCorporateEventsComplete)

                    function getCorporateEventsComplete(data) {
                        events = data;
                        return $q.when(events);
                    }
                }
            
                function getCorporateEvents() {

                    //// implementation details go here
                    //var deferred = $q.defer();

                    var selectableSources = [];                    
                    var currentIndex = 0;

                    // Read from SharePoint List for migration sources
                    var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                    var appweburl =
                       decodeURIComponent(
                           getQueryStringParameter("SPAppWebUrl")
                    );

                    // resources are in URLs in the form:
                    // web_url/_layouts/15/resource
                    var scriptbase = hostweburl + "/_layouts/15/";

                    // Load the js files and continue to the successHandler
                    $.getScript(scriptbase + "SP.Runtime.js",
                        function () {
                            $.getScript(scriptbase + "SP.js",
                                function () {
                                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                                         function () {
                                             var context = new SP.ClientContext(appweburl);
                                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                             context.set_webRequestExecutorFactory(factory);
                                             var appContextSite = new SP.AppContextSite(context, hostweburl);
                                             var web = appContextSite.get_web();
                                             context.load(web);

                                             var list = web.get_lists().getByTitle('Corporate Events');
                                             //var camlQuery = SP.CamlQuery.createAllItemsQuery();
                                             var camlQuery = new SP.CamlQuery();
                                             camlQuery.set_viewXml(
                                              '<View><Query><Where><Geq><FieldRef Name="ID"/>' +
                                              '<Value Type="Number">1</Value></Geq></Where></Query>' +
                                              '<RowLimit>10</RowLimit><ViewFields>' +
                                              '<FieldRef Name="ID" />' +
                                              '<FieldRef Name="RegisteredEventID" />' +
                                              '<FieldRef Name="Title" />' +
                                              '<FieldRef Name="EventDescription" />' +
                                              '<FieldRef Name="EventCategory" />' +
                                              '<FieldRef Name="EventDate" />' +
                                              '<FieldRef Name="EventLocation" />' +
                                              '<FieldRef Name="EventContactEmail" />' +
                                              '<FieldRef Name="EventStatus" />' +
                                              '<FieldRef Name="EventImageUrl" />' +
                                              '</ViewFields></View>');

                                             this.listItems = list.getItems(camlQuery);
                                             context.load(this.listItems);


                                             context.executeQueryAsync(
                                                 Function.createDelegate(this, function () {
                                                     var ListEnumerator = this.listItems.getEnumerator();
                                                     while (ListEnumerator.moveNext()) {
                                                         var currentItem = ListEnumerator.get_current();

                                                         var spEventDate = new Date(currentItem.get_item('EventDate'));
                                                         var month = spEventDate.getMonth() + 1;                                                         
                                                         var day = spEventDate.getDate();
                                                         var year = spEventDate.getFullYear();
                                                         var shortStartDate = month + "/" + day + "/" + year;

                                                         selectableSources.push({
                                                             'id': currentItem.get_item('ID'),
                                                             'registeredeventid': currentItem.get_item('RegisteredEventID'),
                                                             'title': currentItem.get_item('Title'),
                                                             'description': currentItem.get_item('EventDescription'),
                                                             'category': currentItem.get_item('EventCategory'),
                                                             'eventdate': shortStartDate,
                                                             'location': currentItem.get_item('EventLocation'),
                                                             'contactemail': currentItem.get_item('EventContactEmail'),
                                                             'status': currentItem.get_item('EventStatus'),
                                                             'imageurl': currentItem.get_item('EventImageUrl'),
                                                             'isSelected': false
                                                         });

                                                     }

                                                     $q.all(selectableSources);

                                                 }),
                                                  Function.createDelegate(this, function (sender, args) {
                                                      //deferred.reject('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                                                  })
                                             );
                                         }
                                    );
                                }
                            );
                        }
                    );

                    return $q.when(selectableSources);
                }

                function addEvent(eventItem) {
                    var events = [];
                    return addCorporateEvents(eventItem)
                        .then(addCorporateEventsComplete)

                    function addCorporateEventsComplete(data) {
                        events = data;
                        return $q.when(events);
                    }
                }

                function addCorporateEvents(eventItem) {

                    //// implementation details go here
                    //var deferred = $q.defer();
                    
                    var updatedEvents = [];
                    var currentIndex = 0;

                    // Read from SharePoint List for migration sources
                    var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                    var appweburl =
                       decodeURIComponent(
                           getQueryStringParameter("SPAppWebUrl")
                    );

                    // resources are in URLs in the form:
                    // web_url/_layouts/15/resource
                    var scriptbase = hostweburl + "/_layouts/15/";

                    // Load the js files and continue to the successHandler
                    $.getScript(scriptbase + "SP.Runtime.js",
                        function () {
                            $.getScript(scriptbase + "SP.js",
                                function () {
                                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                                         function () {
                                             var context = new SP.ClientContext(appweburl);
                                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                             context.set_webRequestExecutorFactory(factory);
                                             var appContextSite = new SP.AppContextSite(context, hostweburl);
                                             var web = appContextSite.get_web();
                                             context.load(web);

                                             var list = web.get_lists().getByTitle('Corporate Events');
                                             
                                             // create the ListItemInformational object
                                             var listItemInfo = new SP.ListItemCreationInformation();
                                             var listItem = list.addItem(listItemInfo);
                                             listItem.set_item('Title', eventItem.title);
                                             listItem.set_item('EventDescription', eventItem.eventdesc);
                                             listItem.set_item('EventCategory', eventItem.category);
                                             listItem.set_item('EventDate', eventItem.eventdate);
                                             listItem.set_item('EventContactEmail', eventItem.contact);
                                             listItem.set_item('EventStatus', eventItem.status);
                                             listItem.set_item('RegisteredEventID', eventItem.eventid);
                                             listItem.set_item('EventImageUrl', eventItem.eventimageurl);
                                             listItem.set_item('EventLocation', eventItem.eventlocation);

                                             listItem.update();

                                             // Now retrieve all the events including the new events and populate array
                                             // Get the updated events list
                                             var camlQuery = new SP.CamlQuery();
                                             camlQuery.set_viewXml(
                                              '<View><Query><Where><Geq><FieldRef Name="ID"/>' +
                                              '<Value Type="Number">1</Value></Geq></Where></Query>' +
                                              '<RowLimit>10</RowLimit><ViewFields>' +
                                              '<FieldRef Name="ID" />' +
                                              '<FieldRef Name="RegisteredEventID" />' +
                                              '<FieldRef Name="Title" />' +
                                              '<FieldRef Name="EventDescription" />' +
                                              '<FieldRef Name="EventCategory" />' +
                                              '<FieldRef Name="EventDate" />' +
                                              '<FieldRef Name="EventLocation" />' +
                                              '<FieldRef Name="EventContactEmail" />' +
                                              '<FieldRef Name="EventStatus" />' +
                                              '<FieldRef Name="EventImageUrl" />' +
                                              '</ViewFields></View>');

                                             this.listItems = list.getItems(camlQuery);
                                             context.load(this.listItems);

                                             context.executeQueryAsync(
                                                 Function.createDelegate(this, function () {
                                                     var ListEnumerator = this.listItems.getEnumerator();
                                                     while (ListEnumerator.moveNext()) {
                                                         var currentItem = ListEnumerator.get_current();

                                                         var spEventDate = new Date(currentItem.get_item('EventDate'));
                                                         var month = spEventDate.getMonth() + 1;
                                                         var day = spEventDate.getDate();
                                                         var year = spEventDate.getFullYear();
                                                         var shortStartDate = month + "/" + day + "/" + year;

                                                         updatedEvents.push({
                                                             'id': currentItem.get_item('ID'),
                                                             'registeredeventid': currentItem.get_item('RegisteredEventID'),
                                                             'title': currentItem.get_item('Title'),
                                                             'description': currentItem.get_item('EventDescription'),
                                                             'category': currentItem.get_item('EventCategory'),
                                                             'eventdate': shortStartDate,
                                                             'location': currentItem.get_item('EventLocation'),
                                                             'contactemail': currentItem.get_item('EventContactEmail'),
                                                             'status': currentItem.get_item('EventStatus'),
                                                             'imageurl': currentItem.get_item('EventImageUrl'),
                                                             'isSelected': false
                                                         });

                                                     }

                                                     $q.all(updatedEvents);
                                            }),
                                                  Function.createDelegate(this, function (sender, args) {
                                                      //deferred.reject('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                                                  })
                                             );
                                         }
                                    );
                                }
                            );
                        }
                    );

                    return $q.when(updatedEvents);
                }

                function deleteEvent(eventItem) {
                    var events = [];
                    return deleteCorporateEvents(eventItem)
                        .then(deleteCorporateEventsComplete)

                    function deleteCorporateEventsComplete(data) {
                        events = data;
                        return $q.when(events);
                    }
                }

                function deleteCorporateEvents(eventItem) {
                  
                    var updatedEvents = [];
                    var currentIndex = 0;

                    // Read from SharePoint List 
                    var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                    var appweburl =
                       decodeURIComponent(
                           getQueryStringParameter("SPAppWebUrl")
                    );

                    // resources are in URLs in the form:
                    // web_url/_layouts/15/resource
                    var scriptbase = hostweburl + "/_layouts/15/";

                    // Load the js files and continue to the successHandler
                    $.getScript(scriptbase + "SP.Runtime.js",
                        function () {
                            $.getScript(scriptbase + "SP.js",
                                function () {
                                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                                         function () {
                                             var context = new SP.ClientContext(appweburl);
                                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                             context.set_webRequestExecutorFactory(factory);
                                             var appContextSite = new SP.AppContextSite(context, hostweburl);
                                             var web = appContextSite.get_web();
                                             context.load(web);

                                             var list = web.get_lists().getByTitle('Corporate Events');

                                             // Get the item to delete and then delete it
                                             var listItem = list.getItemById(eventItem);
                                             listItem.deleteObject();

                                             // Get the updated events list
                                             var camlQuery = new SP.CamlQuery();
                                             camlQuery.set_viewXml(
                                              '<View><Query><Where><Geq><FieldRef Name="ID"/>' +
                                              '<Value Type="Number">1</Value></Geq></Where></Query>' +
                                              '<RowLimit>10</RowLimit><ViewFields>' +
                                              '<FieldRef Name="ID" />' +
                                              '<FieldRef Name="RegisteredEventID" />' +
                                              '<FieldRef Name="Title" />' +
                                              '<FieldRef Name="EventDescription" />' +
                                              '<FieldRef Name="EventCategory" />' +
                                              '<FieldRef Name="EventDate" />' +
                                              '<FieldRef Name="EventLocation" />' +
                                              '<FieldRef Name="EventContactEmail" />' +
                                              '<FieldRef Name="EventStatus" />' +
                                              '<FieldRef Name="EventImageUrl" />' +
                                              '</ViewFields></View>');

                                             this.listItems = list.getItems(camlQuery);
                                             context.load(this.listItems);

                                             context.executeQueryAsync(
                                                 Function.createDelegate(this, function () {
                                                     var ListEnumerator = this.listItems.getEnumerator();
                                                     while (ListEnumerator.moveNext()) {
                                                         var currentItem = ListEnumerator.get_current();

                                                         var spEventDate = new Date(currentItem.get_item('EventDate'));
                                                         var month = spEventDate.getMonth() + 1;
                                                         var day = spEventDate.getDate();
                                                         var year = spEventDate.getFullYear();
                                                         var shortStartDate = month + "/" + day + "/" + year;

                                                         updatedEvents.push({
                                                             'id': currentItem.get_item('ID'),
                                                             'registeredeventid': currentItem.get_item('RegisteredEventID'),
                                                             'title': currentItem.get_item('Title'),
                                                             'description': currentItem.get_item('EventDescription'),
                                                             'category': currentItem.get_item('EventCategory'),
                                                             'eventdate': shortStartDate,
                                                             'location': currentItem.get_item('EventLocation'),
                                                             'contactemail': currentItem.get_item('EventContactEmail'),
                                                             'status': currentItem.get_item('EventStatus'),
                                                             'imageurl': currentItem.get_item('EventImageUrl'),
                                                             'isSelected': false
                                                         });

                                                     }

                                                     $q.all(updatedEvents);
                                                 }),
                                                  Function.createDelegate(this, function (sender, args) {
                                                      //deferred.reject('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                                                  })
                                             );
                                         }
                                    );
                                }
                            );
                       }
                    );

                    return $q.when(updatedEvents);
                }

                function getSessions(eventId) {
                    var sessions = [];
                    return getSessionsByEventId(eventId)
                        .then(getSessionsComplete)

                    function getSessionsComplete(data) {
                        sessions = data;
                        return $q.when(sessions);
                    }
                }

                function getSessionsByEventId(eventId) {
                    //// implementation details go here
                    //var deferred = $q.defer();

                    var selectableSessions = [];
                    var currentIndex = 0;

                    // Read from SharePoint List for migration sources
                    var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                    var appweburl =
                       decodeURIComponent(
                           getQueryStringParameter("SPAppWebUrl")
                    );

                    // resources are in URLs in the form:
                    // web_url/_layouts/15/resource
                    var scriptbase = hostweburl + "/_layouts/15/";

                    // Load the js files and continue to the successHandler
                    $.getScript(scriptbase + "SP.Runtime.js",
                        function () {
                            $.getScript(scriptbase + "SP.js",
                                function () {
                                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                                         function () {
                                             var context = new SP.ClientContext(appweburl);
                                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                             context.set_webRequestExecutorFactory(factory);
                                             var appContextSite = new SP.AppContextSite(context, hostweburl);
                                             var web = appContextSite.get_web();
                                             context.load(web);

                                             var list = web.get_lists().getByTitle('Event Sessions');
                                             //var camlQuery = SP.CamlQuery.createAllItemsQuery();
                                             var camlQuery = new SP.CamlQuery();
                                             camlQuery.set_viewXml(
                                              '<View><Query><Where><Eq><FieldRef Name="RegisteredEventID"/>' +
                                              '<Value Type="Text">' + eventId + '</Value></Eq></Where></Query>' +
                                              '<RowLimit>10</RowLimit><ViewFields>' +
                                              '<FieldRef Name="ID" />' +
                                              '<FieldRef Name="RegisteredEventID" />' +                                              
                                              '<FieldRef Name="SessionID" />' +
                                              '<FieldRef Name="Title" />' +                                  
                                              '<FieldRef Name="SessionDate" />' +
                                              '<FieldRef Name="SessionDescription" />' +
                                              '<FieldRef Name="SessionImageUrl" />' +
                                              '<FieldRef Name="SpeakerID" />' +
                                              '</ViewFields></View>');

                                             this.listItems = list.getItems(camlQuery);
                                             context.load(this.listItems);

                                             context.executeQueryAsync(
                                                 Function.createDelegate(this, function () {
                                                     var ListEnumerator = this.listItems.getEnumerator();
                                                     while (ListEnumerator.moveNext()) {
                                                         var currentItem = ListEnumerator.get_current();                                                         

                                                         var spSessionDate = new Date(currentItem.get_item('SessionDate'));
                                                         var month = spSessionDate.getMonth() + 1;
                                                         var day = spSessionDate.getDate();
                                                         var year = spSessionDate.getFullYear();
                                                         var shortSessionDate = month + "/" + day + "/" + year;

                                                         selectableSessions.push({
                                                             'id': currentItem.get_item('ID'),
                                                             'registeredeventid': currentItem.get_item('RegisteredEventID'),
                                                             'title': currentItem.get_item('Title'),
                                                             'sessionid': currentItem.get_item('SessionID'),
                                                             'description': currentItem.get_item('SessionDescription'),
                                                             'sessiondate': shortSessionDate,                                                             
                                                             'sessionimageurl': currentItem.get_item('SessionImageUrl'),
                                                             'speakerid': currentItem.get_item('SpeakerID'),
                                                             'isSelected': false
                                                         });

                                                     }

                                                     $q.all(selectableSessions);

                                                 }),
                                                  Function.createDelegate(this, function (sender, args) {
                                                      //deferred.reject('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                                                  })
                                             );
                                         }
                                    );
                                }
                            );
                        }
                    );

                    return $q.when(selectableSessions);
                }

                function getSpeakers(speakerId) {
                    var speakers = [];
                    return getSpeakersBySpeakerId(speakerId)
                        .then(getSpeakersComplete)

                    function getSpeakersComplete(data) {
                        speakers = data;
                        return $q.when(speakers);
                    }
                }

                function getSpeakersBySpeakerId(speakerId) {
                    //// implementation details go here
                    //var deferred = $q.defer();

                    var selectableSpeakers = [];
                    var currentIndex = 0;

                    // Read from SharePoint List for migration sources
                    var hostweburl =
                        decodeURIComponent(
                            getQueryStringParameter("SPHostUrl")
                    );
                    var appweburl =
                       decodeURIComponent(
                           getQueryStringParameter("SPAppWebUrl")
                    );

                    // resources are in URLs in the form:
                    // web_url/_layouts/15/resource
                    var scriptbase = hostweburl + "/_layouts/15/";

                    // Load the js files and continue to the successHandler
                    $.getScript(scriptbase + "SP.Runtime.js",
                        function () {
                            $.getScript(scriptbase + "SP.js",
                                function () {
                                    $.getScript(scriptbase + "SP.RequestExecutor.js",
                                         function () {
                                             var context = new SP.ClientContext(appweburl);
                                             var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                             context.set_webRequestExecutorFactory(factory);
                                             var appContextSite = new SP.AppContextSite(context, hostweburl);
                                             var web = appContextSite.get_web();
                                             context.load(web);

                                             var list = web.get_lists().getByTitle('Event Speakers');
                                             //var camlQuery = SP.CamlQuery.createAllItemsQuery();
                                             var camlQuery = new SP.CamlQuery();
                                             camlQuery.set_viewXml(
                                              '<View><Query><Where><Eq><FieldRef Name="SpeakerID"/>' +
                                              '<Value Type="Text">' + speakerId + '</Value></Eq></Where></Query>' +
                                              '<RowLimit>10</RowLimit><ViewFields>' +
                                              '<FieldRef Name="ID" />' +
                                              '<FieldRef Name="Title" />' +
                                              '<FieldRef Name="SpeakerID" />' +                                              
                                              '<FieldRef Name="SpeakerFirstName" />' +
                                              '<FieldRef Name="SpeakerLastName" />' +
                                              '<FieldRef Name="SpeakerEmail" />' +
                                              '</ViewFields></View>');

                                             this.listItems = list.getItems(camlQuery);
                                             context.load(this.listItems);

                                             context.executeQueryAsync(
                                                 Function.createDelegate(this, function () {
                                                     var ListEnumerator = this.listItems.getEnumerator();
                                                     while (ListEnumerator.moveNext()) {
                                                         var currentItem = ListEnumerator.get_current();

                                                         selectableSpeakers.push({
                                                             'id': currentItem.get_item('ID'),                                                             
                                                             'title': currentItem.get_item('Title'),
                                                             'speakerid': currentItem.get_item('SpeakerID'),
                                                             'speakerfirstname': currentItem.get_item('SpeakerFirstName'),
                                                             'speakerlastname': currentItem.get_item('SpeakerLastName'),
                                                             'speakeremail': currentItem.get_item('SpeakerEmail'),
                                                             'isSelected': false
                                                         });

                                                     }

                                                     $q.all(selectableSpeakers);

                                                 }),
                                                  Function.createDelegate(this, function (sender, args) {
                                                      //deferred.reject('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
                                                  })
                                             );
                                         }
                                    );
                                }
                            );
                        }
                    );

                    return $q.when(selectableSpeakers);
                }

                function prime() {
                    if (primePromise) {
                        return primePromise;
                    }

                    primePromise = $q.when(true).then(success);
                    return primePromise;

                    function success() {
                        isPrimed = true;
                        
                    }
                }

                function ready(nextPromises) {
                    var readyPromise = primePromise || prime();

                    return readyPromise
                        .then(function () {
                            return $q.all(nextPromises);
                        })
                        
                }

                function getQueryStringParameter(paramToRetrieve) {
                    var params =
                        document.URL.split("?")[1].split("&");
                    var strParams = "";
                    for (var i = 0; i < params.length; i = i + 1) {
                        var singleParam = params[i].split("=");
                        if (singleParam[0] == paramToRetrieve)
                            return singleParam[1];
                    }
                }
        }]);    
})();
