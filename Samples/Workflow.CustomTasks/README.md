# Workflow with custom forms (initiation and task) #

### Summary ###
This sample shows how to create a workflow that supports custom task forms and custom initiation forms.

### Applies to ###
- Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Workflow.CustomTasks | Paolo Pialorsi (**PiaSys.com**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 24th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: WORKFLOW WITH CUSTOM FORMS #
This SharePoint-hosted sample application for SharePoint demonstrates how to create a workflow that provides custom forms for workflow initiation and approval tasks.

Below screenshot shows the custom initiation form:

![Add-in UI with custom form](http://i.imgur.com/nx2jK1V.png)

And this is the custom approval form:
![Custom approval form](http://i.imgur.com/bQN4qiO.png)

## WORKFLOW ##
### WORKFLOW ARGUMENTS ###
The workflow expect the target approver and the due days for approval as input arguments.
These arguments are provided by the starting user through a custom Initiation Form, which is part of the sample.

## WORKFLOW TARGET LIBRARY ###
The library of documents against which the sample workflow runs.

### START THE WORKFLOW ###
Using the Workflows page start the workflow with name "Sample Approval Workflow".
Provide the input arguments (target approver and due days) and press the "Start" button.
In the target approver field, you can see an example of using the client-side PeoplePicker control for a better user experience.

### HANDLE CUSTOM TASK ###
Within the workflow status page, wait for the custom approval task to be assigned to the target approver.
Then you will be able to click on the custom task item, in order to access the custom task page.
You can also access the workflow status page by clicking on the workflow status column named "Sample Approval Workflow" available in the default view of the document library in the home page of the sample SharePoint add-in.
If you click on the custom task after you have already handled it, you will see a read-only display form, in order to avoid multiple task handlings.

# HOW TO TEST THE SAMPLE #
Using Visual Studio 2013, configure the target SharePoint Online site collection, and simply start (F5) the SharePoint add-in from within Visual Studio 2013

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Workflow.CustomTasks" />