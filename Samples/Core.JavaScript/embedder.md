#PnP JavaScript Core - Test Embedder#

### Summary ###
This sample is a consolidated set of JavaScript examples for use in your PnP related projects.

This sample also demonstrates how to use a loader file in conjunction with a UserCustomAction to embed JavaScript in a SharePoint site. This pattern minimizes the need to update the deployed custom action.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations in the embedder program.

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

## Embedder Console Application ##

To ease testing it can be helpful to use a console application to embed the UserCustomAction in the target site. In practice this would likely be done during provisioning of the site/web. The small block of script that is embedded allows for all updates to occur outside of SharePoint, easing the burden on administrators and developers. This console application provides an example of a broadly reusable development tool.

## Embedded JavaScript ##

The JavaScript block code is kept to a minimum and makes use of the built-in SharePoint script on demand functionality. This example makes use of the public Microsoft hosted CDN for [jQuery](http://jquery.com) - but could be updated to use any other source. There are two examples, the first works for non-MDS enabled sites, and the second for MDS enabled sites. When unsure you can always use the second.

### Non-MDS Version ###

```JavaScript
(function (loaderFile, nocache) {
    var url = loaderFile + ((nocache) ? '?' + encodeURIComponent((new Date()).getTime()) : '');
    SP.SOD.registerSod('pnp-jquery.js', 'https://localhost:44324/js/jquery.js');
    SP.SOD.registerSod('pnp-loader.js', url);
    SP.SOD.registerSodDep('pnp-loader.js', 'pnp-jquery.js');
    SP.SOD.executeFunc('pnp-loader.js', null, function () { });
})('https://localhost:44324/pnp-loader.js', true);
```

### MDS Version ###
```JavaScript
ExecuteOrDelayUntilBodyLoaded(function () {
    var url = 'https://localhost:44324/js/pnp-loaderMDS.js?' + encodeURIComponent((new Date()).getTime());
    SP.SOD.registerSod('pnp-jquery.js', 'https://localhost:44324/js/jquery.js');
    SP.SOD.registerSod('pnp-loader.js', url);
    SP.SOD.registerSodDep('pnp-loader.js', 'pnp-jquery.js');
    SP.SOD.executeFunc('pnp-loader.js', null, function () {
        if (typeof pnpLoadFiles === 'undefined') {
            RegisterModuleInit('https://localhost:44324/js/pnp-loaderMDS.js', pnpLoadFiles);
        } else {
            pnpLoadFiles();
        }
    });
});
```

In the above code we are using an anonymous function embedded in the page to first load jQuery and then load our remote loader file. Additional required files can also be loaded here, such as [Bootstrap](http://getbootstrap.com) or [Knockout](http://knockoutjs.com/). These would then be available on every page - but it may be desirable to load them in the loader.js file in case the version or references need to change. It is recommended, where possible, to host the jQuery.js file references above at a generic location with a non-versioned name (such as jQuery.js). This allows for the file to be updated while avoiding the need to update the UserCustomActions.

## pnp-loader.js ##

The [pnp-loader.js](Core.JavaScript.CDN/js/pnp-loader.js) file is the engine used to load any other required client files. This model provides a single location to update those files required by an application. Two versions are provided as examples, one that does not cache the files, for development and testing, and a [second](Core.JavaScript.CDN/js/pnp-loader-cached.js) that does cache the loaded client files for production/UAT scenarios. In production the loader file can also be minimized to reduce download times.

## pnp-loaderMDS.js ##

The [pnp-loaderMDS.js](Core.JavaScript.CDN/js/pnp-loaderMDS.js) file is the engine used to load any other required client files when using the MDS version of the embedded loader script. This model provides a single location to update those files required by an application. Two versions are provided as examples, one that does not cache the files, for development and testing, and a [second](Core.JavaScript.CDN/js/pnp-loaderMDS-cached.js) that does cache the loaded client files for production/UAT scenarios. In production the loader file can also be minimized to reduce download times.