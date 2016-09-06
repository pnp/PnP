# Create custom activities for workflow #

### Summary ###
This sample shows how to create custom activities for developing workflows.

### Applies to ###
- Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)| Twitter
---------|----------|--------
Workflow.Activities | Paolo Pialorsi (**PiaSys.com**) | @PaoloPia

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 26th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: CUSTOM ACTIVITIES #
This SharePoint-hosted sample application for SharePoint demonstrates how to leverage custom activities for workflow. In particular you can see how to leverage the following activities:

Activity | Description
---------|------------
RetrieveFormDigestActivity | Retrieves the FormDigest value for subsequent HTTP(S) requests
MoveFileActivity | Moves a file from a source library to a destination library
BreakItemRoleInheritanceActivity | Allows to break roles inheritance on a specified list item
ResetItemPermissionsActivity | Allows to reset roles, inheriting from parent, for a specified list item
SetItemPermissionsActivity | Allows to define a specific set of permissions on a specified list item
SetTaskPredecessorsActivity | Alllows to configure predecessors tasks for a specified task item
SetWorkflowGlobalStatusActivity | Allows to set the workflow user status and the workflow status of a running workflow with the same value

# HOW TO TEST THE SAMPLE #
Using Visual Studio 2013, configure the target SharePoint Online site collection, and simply start (F5) the SharePoint add-in from within Visual Studio 2013.
In the home page of the SharePoint-hosted add-in you can add a file to the *Source Library*. Then, using the ECB menu of the document, you can start a new instance of the workflow named "Sample Move File" in order to test the MoveFileActivity activity.
Moreover, selecting a document in the *Target Library* you can leverage the ECB menu to start a new instance of the workflow named "Sample Security Activities". That workflow will ask you to provide a "Target Principal UPN" value and a "Target Role", which will be used to configure a custom set of permissions on the target item. After that, the workflow will assign you a task, to check the permissions on the target item. After completing the task, the permissions on the target item will be resetted back, in order to inherit them from the parent container .
Still on documents in the *Target Library* you can see the SetTaskPredecessorsActivity activity in action, by executing the "SampleTasksWithPredecessors" workflow. Through this workflow you will see three tasks assigned to you, where the third one has the previous two as predecessors.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Workflow.Activities" />