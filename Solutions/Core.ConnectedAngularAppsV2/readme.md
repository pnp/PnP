# Connected App Parts with SignalR and Angular Version 2 #

### Summary ###
As Vesa Juvonen mentions in his blog on [Connected App Parts with SignalR](http://blogs.msdn.com/b/vesku/archive/2014/05/14/connected-sharepoint-app-parts-with-signalr.aspx), SharePoint app parts and their capability to connect between each other  is one of the classic questions and has been since the beginning of SharePoint. Vesa gives a very good overview of the architecture for the model.

This solution takes the concept of master/detail app part communication and makes it real by using SignalR and AngularJS. The sample solution uses a Corporate Events, Sessions and Speakers scenario. Each are individual app parts and utilize associated SharePoint lists. The concept was to drop multiple app parts on a page and show a sense of communication. So for this sample, You drop the Corporate Events app part, then the Event Sessions app part, then the Speakers app part on a new page. The Corporate Events app part loads events, the user selects an event, and the Event Sessions app part renders all the sessions for the selected event. The user then selects a session, and the Speakers app part show the speaker(s) for the selected session. This solution only shows read operations, but you can implement all CRUD operations as well.

These app parts contain two properties that can be set in edit mode and will allow for flexible configuration of app part communication scenarios.

![](http://i.imgur.com/gg6fQGq.jpg)

The session key allows other app parts to participate in that session and is used for a SignalR group identifier. Combining session key with the user id will make the SignalR group identifier specific to the logged in user and the specified session name. If no session name is specified, a session with the name of "Default" will be used.

When adding app parts, be sure to set the properties according to your scenario. If you set the properties on Corporate Events, and forget to set them on another such as Sessions, no session data the a selected event will be shown due to the Sessions app part not being a member of the SignalR group. Once the app parts are part of the group, messages broadcast to callbacks can be picked up and acted upon.


We'll get into how this works below.


For an overview of SignalR, you can read up on it [here](http://www.asp.net/signalr/overview/getting-started/introduction-to-signalr). Also, to learn more about AngularJS visit [angularjs.org](http://www.angularjs.org) and [docs.angularjs.org](http://docs.angularjs.org) to get up to speed on the API.

The solution includes two projects:

- **Core.ConnectedAngularAppsV2** contains multiple client app parts
- **Core.ConnectedAngularAppsV2Web** web project that contains the pages, angularjs scripts and SignalR code, as well as classes to setup all the necessities such as content types, site columns, lists and sample data that you can use.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D) (2014 April CU)
-  SharePoint 2013 on-premises (2014 April CU)

### Solution ###
Solution | Author(s)
---------|----------
Core.ConnectedAngularAppsV2.Solution | Brian Michely (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 20th 2014 | Initial release
2.0  | July 20th 2015  | Unique sessions release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Setup and Configuration ##
This is a semi-complex solution with a number of moving parts. This section explains how to get it running fast, and what is created from the configuration process. The default page of web application will present you with instructions and "Configure" button as shown below which will set up the site columns, content types and lists. It will then inject sample data into the lists.
![](http://i.imgur.com/ONIhhoW.jpg)

The configuration will create a Corporate Events list with data, an Event Sessions list with data, and a Speakers list with data. Once done, create a new page, add an html table to position app parts, then add the Corporate Events, Event Sessions and Speakers app parts to the page. Once you have added the app parts, set your desired app part properties to your desired session scenario. Below is an example showing the app parts layout. You may have to adjust the layout, app part height and width to your liking.
![](http://i.imgur.com/xNHjUgc.jpg)

Once configured, your page would look something like this with your app parts using SignalR to communicate with each other:
![](http://i.imgur.com/jxWAc86.jpg)

## How it all works ##
This section will describe the components of the solution and give a high level of how it works.

### SignalR ###
There is a SignalR hub which is the server-side code, and this concept is described in Vesa's blog post referenced above. In order to call the CorporateEventsHub from AngularJS, the JavaScript can reference the hub, and a proxy will be generated automatically, however, issues with that were encountered, so there is a SignalR utility that will generate a proxy based on the Hub. You can then save this file and add it to your solution, then reference it instead. This is the approach taken for this sample solution. The name of the file is highlighted in the image shown below.

![](http://i.imgur.com/7xzdI3F.png)

A SignalR Group is created based upon your session configuration in your app parts. 

If you did not specify a session name, one will be created as "Default". Any app part using SignalR and a session of Default will be able to be seen by anyone, and can communicate with others if code was written for that scenario.

If you only specify a unique session name and do not combine it with your userid, then any app parts using that session name can communicate with each other and can be see by any user.

If you specify to combine userid with session name, then the any app parts' content with these settings can be seen only by the logged in user, basically creating a session for that user.

### AngularJS ###
With the exception of the Hub server-side code, all the code in the solution is AngularJS. I am not an AngularJS expert, but I did spent quite a bit of time trying to find a good structure that was easily scalable, and easy to debug. Looking at the structure in the image above, you will see there are folders for different areas of functionality. The scripts are defined as controllers, factories and services. The top level script is **app.module.js**. This script declares all the modules being used and follows the simple setter syntax.

    (function () {
    	'use strict';
    
    	angular.module('app', [
    		'app.core',
    		'app.events',
    		'app.sessions',
    		'app.signalrcomms',
    		'app.speakers',
    		'app.manage'
    	]);
    })();
    
Under scripts, there is a sub-folder for each module and controller. The module is simple and is defined as:
        
    angular.module('app.events', []);
    
Each controller injects the DataService and  SignalR factories and the controllers are used in the .aspx pages. The controllers are defined as:

    angular
       .module('app.events')
       .controller('eventsController', ['$q', 'dataService', 'signalRservice', function ($q, dataService, signalRservice) {
    	...
    }

The angular code uses callbacks and does not use $scope or $rootscope. 

### Putting it all together ###

![](http://i.imgur.com/htNrj4J.png)

1. App action invokes controller
2. If data action, set promises, invoke data service
3. Retrieve events from SharePoint events list
4. Invoke SignalR proxy method to perform some broadcast
5. js SignalR proxy invokes Hub method
6. Hub sends communicated data back to all necessary registered clients
7. SignalR sets callbacks, controllers act on returned data
8. User selects event checkbox, controller invokes proxy, selected event is sent to Hub, Hub broadcasts selected event id.
9. SignalR sets callbacks, controllers act on returned data. In this case, the sessions app part says “Hey, an event was selected, I need to get the sessions for it”.
10. The sessions controller calls the data service and gets related sessions.
11. User selects a session, controller invokes proxy, selected event is sent to Hub, Hub broadcasts selected session id.
12. SignalR sets callbacks, controllers act on returned data. In this case, the speakers app part says “Hey, a session was selected, I need to get the speaker(s) for it”.

The Events app part also has the capability to delete events and it basically follows the same process as adding a new event as far as notifications, callbacks and and acting on callbacks.

### Final Notes ###
Obviously not too much effort was put into jazzing up the UI, but can be done later.
Not all the CRUD operations are there for everything, but all the methods are there in the Hub to support adding more operations.

