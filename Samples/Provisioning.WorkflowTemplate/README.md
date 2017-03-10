# Workflow templates automated provisioning in on-premises or Office365 Dedicated #

### Summary ###
This sample shows how to provision workflow template (.wsp) in on-premises or in Office 365 Dedicated using CSOM. Solution gives you insight how to migrate reusable workflow across site collection and insight how to automate process of deploying and activating reusable workflow packages by investigating solution structure and SharePoint CSOM.

### Applies to ###
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
- Solutions feature must be enabled in Site Settings->Web Designer Galleries in the Site Collection.
- Keep in mind that using DesignPackage.Install will clear the composed look gallery (bug).

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.WorkflowTemplate | Ilker Karimanov (**OneBitSoftware**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 7th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Workflow Solution Settings #
In order to provision workflow templates you need to be aware of some internal artifacts. Reusable workflows can be saved as template and later distributed in another SharePoint sites. Templates are stored as regular .wsp packages which can be uniquely identified. Let's make quick overview what we need from the workflow user solution package in order automate process of provisioning workflow templates.

* Save workflow as template and download it.

* Make copy, rename the WSP file to CAB and extract the CAB file.

* Extract SolutionId from the manifest file, as it is used as globally unique identifier for your workflow template solution.

```XML
<Solution SolutionId="{305bd577-4126-4e56-8f59-031b9ac653af}" SharePointProductVersion="16.0.3819.1226" xmlns="http://schemas.microsoft.com/sharepoint/">
	<FeatureManifests>
		<FeatureManifest Location="SampleApprovalListInstances\Feature.xml" />
		<FeatureManifest Location="SampleApprovalWebEventReceivers\Feature.xml" />
	</FeatureManifests>
</Solution>
```

* Extract Workflow Feature Id located in feature xml file in [Template name] + ListInstances. For example refer to sample workflow in the Contents folder - SampleApprovalListInstances.

```XML
<Feature Id="{0897fb2d-d86c-46f0-8805-a490775839a3}" Title="Workflow template SampleApproval from web template en-US title" Hidden="FALSE" Version="1.0.0.0" Scope="Web" RequireResources="TRUE" ReceiverAssembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" ReceiverClass="Microsoft.SharePoint.Workflow.SPDeclarativeWorkflowProvisioningFeatureReceiver" xmlns="http://schemas.microsoft.com/sharepoint/">
	<ElementManifests>
		<ElementManifest Location="Elements.xml" />
		<ElementFile Location="resources\resources.en-US.resx" />
		<ElementFile Location="Files\Workflows\Schema.xml" />
		<ElementFile Location="Files\Workflows\SampleApproval\SampleApproval.xoml.wfconfig.xml" />
		<ElementFile Location="Files\Workflows\SampleApproval\SampleApproval.xoml" />
		<ElementFile Location="Files\Workflows\SampleApproval\SampleApproval.xsn" />
	</ElementManifests>
</Feature>
```

* Edit app.config file with extracted workflow template information as follows.

```XML
      <!--Workflow Template Settings-->
      <!--Path to the workflow template file(wsp)-->
      <add key="WorkflowTemplatePath" value="..\..\Contents\SampleApproval.wsp"/>
      <!--Worfklow template feature to activate solution on the Web-->
      <add key="WorkflowFeatureId" value="{0897fb2d-d86c-46f0-8805-a490775839a3}"/>
      <!--Workflow template feature to workflow exporting capabilities-->
      <add key="WorkflowEventsFeatureId" value="{7e18f05c-9e3a-4d90-abf5-bb1eb7785e57}"/>
      <!--Workflow template solution in of the template file (wsp)-->
      <add key="WorkflowSolutionId" value="{305bd577-4126-4e56-8f59-031b9ac653af}"/>
```

# Application Settings #
The solution has one Visual Studio project - Provisioning.WorkflowTemplate, which is a console application.

The Solution requires some configuration in the app.config file. The following code sample outlines the appSettings that need to be configured with values specific to your environment.
```XML
      <!-- Site Collection Url-->
      <add key="SharePointUrl" value="******" />
      <!--SharePoint credentials for provisioning workflow-->
      <!--SharePoint user name-->
      <add key="UserName" value="******"/>
      <!--SharePoint password-->
      <add key="Password" value="******"/>
      <!--SharePoint Mode-Cloud or OnPremise-->
      <add key="SharePointMode" value="Cloud"/>
```


## Provision Workflow ##
Description:
Workflow template provisioning consists of providing WorkflowTemplateInfo object with file-path to the solution file, SolutionId and FeatureId of the package. Provisioning is two step operation:

1. Deploy package file to template library. In our case it is stored in Site Assets.

2. Activate user solution in the Solutions Gallery and also activate solution feature on Web scope.

Code snippet:
```C#
//Construct object with workflow template info
WorkflowTemplateInfo solutionInfo = new WorkflowTemplateInfo();
solutionInfo.PackageFilePath = solutionPath;
//PackageName is mandatory
solutionInfo.PackageName = Path.GetFileNameWithoutExtension(solutionPath);
//Guid is automatically predefined in template file (.wsp)
solutionInfo.PackageGuid = workflowUserSolutionId;
//Workflow feature Id is need to activate workflow in the web
solutionInfo.FeatureId = workflowFeature;
//Init workflow template deployer
using (WorkflowTemplateDeployer workflowDeployer = new WorkflowTemplateDeployer(context))
{
 //Provisioning workflow resources
 workflowDeployer.DeployWorkflowSolution(solutionPath);
 //Activates workflow template
 workflowDeployer.ActivateWorkflowSolution(solutionInfo);
}
```


# Remove Workflow #
Description:
In order to uninstall workflow template mandatory info is Solution id and Package name. Removing is two step operation:

1. Deactivating user solution in the Solution Gallery.

2. Remove package file from the template library.


Code snippet:
```C#
//Construct object with workflow template info
WorkflowTemplateInfo solutionInfo = new WorkflowTemplateInfo();
//Package Guid is mandatory
solutionInfo.PackageGuid = workflowUserSolutionId;
solutionInfo.PackageName = Path.GetFileNameWithoutExtension(solutionPath);
//Init workflow template deployer
using (WorkflowTemplateDeployer workflowDeployer = new WorkflowTemplateDeployer(context))
{
  //Deactivate workflow template
  workflowDeployer.DeactivateWorkflowSolution(solutionInfo);
  //Remove workflow template files
  workflowDeployer.RemoveWorkflowSolution(Path.GetFileName(solutionPath));
}
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.WorkflowTemplate" />