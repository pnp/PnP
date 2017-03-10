# JSOM Navigation Sample #

### Summary ###
This sample demonstrates how to make modifications to quick launch and top navigation nodes as well as setting navigation inheritance.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.Jsom.Navigation | Pete Filicetti (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 17th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Sample Add-In #
The provided SharePoint-hosted sample add-in uses JavaScript promises to control the flow of navigation updates.

![Add-in UI](https://raw.githubusercontent.com/pefilice/PnP-Support/master/Core.Jsom.Navigation.png)

## Initialize ##
Description:
The following code shows how to initialize the URLs needed for operation.  

Code snippet:

```JavaScript
var hostWebUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

coreNavigation.initialize(appWebUrl, hostWebUrl);
```

## Add a node to top navigation ##
Description:
The following code shows how to call the function to add a node to the top level navigation.

Code snippet:

```JavaScript
coreNavigation.addNavigationNode("Test", "http://www.microsoft.com", null, false)
    .done(
        function() {
            $("#statusMessage").html('Added top nav node \'Test\'');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to add top nav node \'Test\': ' + message);
    });
```

## Delete a node from top navigation ##
Description:
The following code shows how to call the function to delete a node from the top level navigation.

Code snippet:

```JavaScript
coreNavigation.deleteNavigationNode("Test", null, false)
    .done(
        function() {
            $("#statusMessage").html('Removed top nav node \'Test\'');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to remove top nav node \'Test\': ' + message);
        });
```

## Add parent and child nodes to quick launch ##
Description:
The following code shows how to call the function to add parent and child nodes to quick launch.

Code snippet:

```JavaScript
coreNavigation.addNavigationNode("Parent", "#", null, true)
    .then(
        function() {
            return coreNavigation.addNavigationNode("Child", "http://www.microsoft.com", 'Parent', true);
        })
    .done(
        function() {
            $("#statusMessage").html('Added quick launch nodes \'Parent\' and \'Child\'');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to add quick launch nodes \'Parent\' and \'Child\': ' + message);
        });
```

## Delete parent and child nodes from quick launch ##
Description:
The following code shows how to call the function to delete parent and child nodes from quick launch.

Code snippet:

```JavaScript
coreNavigation.deleteNavigationNode("Child", "Parent", true)
    .then(
        function() {
            return coreNavigation.deleteNavigationNode("Parent", null, true);
        })
    .done(
        function() {
            $("#statusMessage").html('Deleted quick launch nodes \'Parent\' and \'Child\'');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to delete quick launch nodes \'Parent\' and \'Child\': ' + message);
        });
```

## Delete all quick launch nodes ##
Description:
The following code shows how to call the function to delete all quick launch nodes.

Code snippet:

```JavaScript
coreNavigation.deleteAllQuickLaunchNodes()
    .done(
        function() {
            $("#statusMessage").html('Deleted all quick launch nodes');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to delete all quick launch nodes: ' + message);
        });
```

## Update navigation inheritance ##
Description:
The following code shows how to call the function to update navigation inheritance.  *Note: the example shows how to set inheritance to true, passing false to the function sets inheritance to false.*

Code snippet:

```JavaScript
coreNavigation.updateNavigationInheritance(true)
    .done(
        function() {
            $("#statusMessage").html('Navigation inheritance set to true');
        })
    .fail(
        function(message) {
            $("#statusMessage").html('Failed to set navigation inheritance to true: ' + message);
        });
```
<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Jsom.Navigation" />