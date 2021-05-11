# Migrate Remote Event Receivers to .NET Core #

### Summary ###
This sample shows how Remote Event Receivers can be migrated to .NET Core and PnP Framework.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
NETCore.RemoteEventReceiver | Antons Mislevics

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 7th 2021 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# Overview #
When migrating existing solutions to CSOM for .NET Standard developers must address a set of differences between the .NET Framework version and the .NET Standard version redistributable. One of such differences is a lack of support for `Microsoft.SharePoint.Client.EventReceivers` namespace, and as a result Remote Event Receivers. Developers are recommended to switch to modern eventing concepts such as Web Hooks (see [Using CSOM for .NET Standard instead of CSOM for .NET Framework](https://docs.microsoft.com/en-us/sharepoint/dev/sp-add-ins/using-csom-for-dotnet-standard)).

Since there are some differences between Remote Event Receivers and modern eventing concepts, such migration is not always easy and may require a complete redesign of existing solution. This may also bring significant challenges to the teams that have a large number of Remote Event Receivers implemented in their solution.

This sample shows how existing Remote Event Receivers can be migrated to the solution running on .NET 5.0 with PnP Framework in Docker container. Such Remote Event Receiver can later be productionalized via Microsoft Azure Web Apps for Containers.

**IMPORTANT:** Please note that this sample is not contradicting the official Microsoft recommendation in any way. You should consider switching to modern eventing concepts. Hopefully, this sample can help you to get more time to plan this switch properly, and also to ensure a gradual transition for complex solutions relying on Remote Event Receivers.

This sample consists of the following:
1. *NETCore.RemoteEventReceiver/Models* folder contains a set of models that are required to implement Remote Event Receivers and have been previously available in `Microsoft.SharePoint.Client.EventReceivers` namespace;
2. *NETCore.RemoteEventReceiver/Services/DemoEventReceiver.svc.cs* demonstrates how to migrate the code of existing Remote Event Receiver;
3. *NETCore.RemoteEventReceiver/Startup.cs* shows how to expose Remote Event Receiver as a SOAP endpoint;
4. *Dockerfile* gives an example of how to build a Docker image with Remote Event Receiver that can be deployed on Microsoft Azure Web Apps for Containers (for more info see [Migrate custom software to Azure App Service using a custom container](https://docs.microsoft.com/en-us/azure/app-service/tutorial-custom-container?pivots=container-linux)).
