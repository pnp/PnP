# Handle custom events in a workflow #

### Summary ###
This sample shows how to create a workflow that supports custom events.

### Applies to ###
- Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Workflow.CustomEvents | Paolo Pialorsi (**PiaSys.com**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: WAIT CUSTOM EVENT #
This SharePoint-hosted sample application for SharePoint demonstrates how to create a workflow that waits for a custom event, which will be raised by a custom Page via JavaScript and using the JavaScript Client Object Model (JSOM) for the Workflow Services Manager library.

## WORKFLOW ##
### WORKFLOW ARGUMENTS ###
The workflow does not expect any argument, it simply starts against the target item and waits for a custom event.

## WORKFLOW TARGET ITEMS LIST ###
The list of items against which the sample workflow runs.

The *WFEventFired* field is used to set the value of the custom event that has been fired using the custom Page.

### START THE WORKFLOW ###
Using the Workflows page or the ECB menu item named "Start Sample Workflow" you can start the workflow.

### FIRE CUSTOM EVENT ###
Using ECB menu item named "Fire Custom Event" you can fire the custom event.

# HOW TO TEST THE SAMPLE #
Using Visual Studio 2013, configure the target SharePoint Online site collection, and simply start (F5) the SharePoint add-in from within Visual Studio 2013


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Workflow.CustomEvents" />