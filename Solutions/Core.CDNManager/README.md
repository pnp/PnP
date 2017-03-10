# CDN Manager #

### Summary ###
The CDN Manager is used to manage Content Delivery Network (CDN) references for JavaScript Libraries and Cascading Style Sheets (CSS) in SharePoint 2013 sites. Using the CDN Manager prevents multiple references to the same library and conflicts between libraries that may arise when such references are not managed globally.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

## To get started with the add-in:
1. Download the project
2. Change the SiteUrl property to refer to one of your sites
3. Hit F5 to start debugging

### Solution ###
Solution    | Author(s)
------------|----------
CDN Manager | Scot Hillier, Hugh Wood, Marc Anderson, David Mann

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | March 26th 2015| Support for script-on-demand, Support for Minimal Download Strategy, Support for CSS CDNs like Bootstrap, The ability to specify script dependencies between CDNs, Yahoo Query Language validation of CDN locations, Scripting site permission verification
1.0  | January 22nd 2015 | Initial release on IT Unity site

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Using CDN Manager with a Script Editor Web Part #
You can use the following code directly in a script editor web part to try out the functionality. This assumes that you have injected the jQuery library reference that is preloaded in the add-in.

Simple jQuery
  ```JavaScript

  <script>
  CDNManager.getScript('jquery-1.11.2.min.js',function() {
  jQuery('.menu-item-text').css('color','red');
  });
  </script>

  ```

If you also activate KnockoutJS, you can use this code in a script editor web part.

Knockout and jQuery loading simultaneously
  ```JavaScript

  <div>
  <div data-bind='text: firstName'></div>
  <div data-bind='text: lastName'></div>
  </div>
  <script>
  CDNManager.getScript(['jquery-1.11.2.min.js','knockout-2.2.1.js'],function() {
    var viewModel = {
    'firstName': ko.observable('Scot'),
    'lastName': ko.observable('Hillier')
    };
    ko.applyBindings(viewModel);
  });
  </script>

  ```


# Using CDN Manager with JS Link #
The CDN Manager can also be used to load references in JSLink files. If you activate the bootstrap CSS and JS that comes pre-loaded in the add-in, then the following code can be used to create a bootstrap progress bar in tasks lists.

  ```JavaScript
 
    (function () {
        // Intialize the variables for overrides objects
        var overrideCtx = {};
        overrideCtx.Templates = {};

        // Override field data
        overrideCtx.Templates.Fields = {
            // PercentComplate = internal name of the % Complete
            // View = you want to change the field rendering of a view
            // <dev ... = here we define what the output of the field will be.
            'PercentComplete': { 'View': '<div class="progress"><div class="progress-bar" role="progressbar" aria-valuenow="<#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>" aria-valuemin="0" aria-valuemax="100" style="width: <#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>%;"><#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>%</div></div>' }
        };

        // Register the override of the field
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    })();



    (function () {
        "use strict";

        if (typeof (_spBodyOnLoadCalled) === 'undefined' || _spBodyOnLoadCalled) {
            load();
        }
        else {
            _spBodyOnLoadFunctions.push(load);
        }

        function load () {
            CDNManager.getScript(['jquery-1.11.2.min.js','bootstrap.min.js'], ready);
        };

        function ready () {
            jQuery('[data-toggle="tooltip"]').tooltip();
        };

    }());


 ```

## Known issues
1. Scripting must be enabled in the tenancy to use the CDN Manager.
2. Script Editor web parts will break the Minimal Download Service.

<img  src="https://telemetry.sharepointpnp.com/pnp/solutions/Core.CDNManager" />