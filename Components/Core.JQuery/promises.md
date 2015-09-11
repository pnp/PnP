# jQuery Promises Examples #

### Summary ###
Provides examples of using the jQuery Deferred object to manage calls with executeQueryAsyc.

### Relevant Files ###

The relevant files from the example solution are:

- [AddIn.js](Core.JQuery.Promises/Scripts/AddIn.js) : the Add-In specific functions
- [officepnp.js](Core.JQuery.Promises/Scripts/officepnp.js) : the example library code (the good stuff)
- [arrayextensions.js](Core.JQuery.Promises/Scripts/arrayextensions.js) : extension methods for base Array object
- [Default.aspx](Core.JQuery.Promises/Pages/Default.aspx) : example page

### Dependencies ###

- jQuery >= 1.5

### Basic Pattern ###

The basic pattern to follow when using Deferred with executeQueryAsync is below. It involves creating a Deferred object, executing the query, and then either resolving or rejecting the Deferred. By supplying this and arguments as the resolution parameters we are able to pass back exactly what executeQueryAsync returned. This helps ensure we don't need to change any existing event handlers to transition to this pattern.

```JavaScript
function (/*SP.ClientContext*/ context) {

	// create a deferred
	var def = $.Deferred();
	
	// resolve/reject with whatever would have been supplied by executeQueryAsync
	context.executeQueryAsync(function () { 
		def.resolveWith(this, arguments); 
	}, function () { 
		def.rejectWith(this, arguments); 
	});
	
	// return a promise
	return def.promise();
}
```
### Extending SP.ClientContext ###

In the samples you will see that SP.ClientContext.prototype has been extended directly. There are a lot of options for approaching this as outlined in the code comments. Please choose the method with which you are most comfortable (and add other examples to the source!!).

### Query with Retry ###

If you have used the PnP CSOM framework before you are familiar with the [ExecuteQueryRetry](https://github.com/OfficeDev/PnP-Sites-Core/blob/master/Core/OfficeDevPnP.Core/AppModelExtensions/ClientContextExtensions.cs) method. The question has been asked if a similar capability is available in the JSOM framework. As an exercise, this is possible and you can see the method ext_executeQueryRetry in the [officepnp.js](Core.JQuery.Promises/Scripts/officepnp.js) file.

**An IMPORTANT note...depending on the error all the pending actions in the ClientContext may be flushed and a retry may not be possible. This will result in a failure followed by a success produced by an empty query.**