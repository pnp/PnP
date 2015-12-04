#PnP JavaScript Core - JS Files#

### Summary ###
This page lists the files in the consolidated set of JavaScript examples for use in your PnP related projects. Together they can be used to enable base functionality for projects without the need to reinvent the code each time.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.JavaScript | Patrick Rodgers (**Microsoft**) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 28th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

## [pnp-clientcache.js](Core.JavaScript.CDN/js/pnp-clientcache.js) ##

Provides a simple client cache implementation using localStorage. Makes use of a serialized object to manage expiration of cached data.

```JavaScript
// add the item stored using 'key'
$pnp.caching.add('key', { prop: 'value'}[, {expiration}]);

// get the item stored using 'key'
var o = $pnp.caching.get('key');

// or

// get or add and return the item stored using 'key'
var o2 = $pnp.caching.getOrAdd('key', function() {
	return { prop: 'value'};
});

// remove the item stored using 'key'
$pnp.caching.remove('key');
```
## [pnp-config.js](Core.JavaScript.CDN/js/pnp-config.js) ##

Demonstrates using a list based configuration store, caching the data locally using pnp-clientcache.js. Also of note is the calling pattern used to load the configuration making use of a ready method as seen below. This allows code that depends on the configuration data to execute once it is available as there may be a delay if it is retrieved from the server.

```JavaScript
$pnp.config.ready(function (c) { 
	var myValue = c.myValueFromServer;
	// code that requires the loaded value
	...
});
```

## [pnp-devdashboard.js](Core.JavaScript.CDN/js/pnp-devdashboard.js) ##

Provides an extensible client-side dashboard. Attaches a listener to the pnp-logging module to intercept logging data.

```JavaScript
$pnp.dashboard.ready(function (db) { 
	db.write('section title', 'logged message');
});
```

## [pnp-logging.js](Core.JavaScript.CDN/js/pnp-logging.js) ##

Provides an event driven logging interface to which any number of logging listeners can be subscribed. The events are filtered using the $pnp.settings.activeLoggingLevel value [0 = Verbose, 1 = Info, 2 = Warning, 3 = Error]. There is also an example in this file of using the [Azure App Insights client library](https://azure.microsoft.com/en-us/documentation/articles/app-insights-javascript/) which requires you to set the $pnp.settings.azureInsightsInstrumentationKey value for your environment.

The args will be a plain object with the following properties:
- correlationId
- currentUserLogin
- timestamp
- message
- level
- origin
- component

```JavaScript
// add console logging (a simple subscription example)
$pnp.logging.subscribe(function (e, args) {
    switch (args.level) {
        case 0: console.log(args.message); break;
        case 1: console.info(args.message); break;
        case 2: console.warn(args.message); break;
        case 3: console.error(args.message); break;
    }
});
```

## [pnp-settings.js](Core.JavaScript.CDN/js/pnp-settings.js) ##

Global settings file, also used to establish the top-level name space.

## [pnp-uimods.js](Core.JavaScript.CDN/js/pnp-uimods.js) ##

Simple UI modification framework provides an example of using the logging module with levels. Hides the new site link on the site contents page.