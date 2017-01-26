# Getting started with the PnP Provisioning engine #

### Summary ###
This scenario shows a basic usage of the PnP provisioning engine. 

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Framework.Console | Vesa Juvonen, Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 30th 2015 | Initial release
1.1  | July 29th 2015 | Rewrite for better structure

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Get a provisioning template from a given site #
The provisioning framework provides an easy to use option to transform an existing site into a provisioning template as shown in below code snippet:

```C#
// Get template from existing site
ProvisioningTemplate template = ctx.Web.GetProvisioningTemplate();
```

When the provisioning framework creates this template it tries to filter *out the out of the box* elements by comparing the acquired template with a clean base template. If you see a huge amount of site columns, content types and lists then this means that the system was not able to find the correct base template.


## Persist, load and enumerate provisioning templates ##
Once you've used the above method to create a provisioning template or when you've manually created one you can use one of our providers to persist the template. The provisioning framework contains today an XML provider that's responsible for the translation of the provisioning framework model to and from XML. Next to a provider the provisioning framework also contains connectors that are used to read and write files to common locations such as the file system, Azure blob storage or SharePoint document libraries. When you instantiate a provider you can select an XMLFileSystemTemplateProvider, XMLAzureStorageTemplateProvider...which then ensures the right connector is hooked up. Below code shows an XMLFileSystemTemplateProvider in action:

```C#
// Save template using XML provider
XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(@"c:\temp\pnpprovisioningdemo", "");
string templateName = "template.xml";
provider.SaveAs(template, templateName);

// Load the saved model again
ProvisioningTemplate p2 = provider.GetTemplate(templateName);

// Get the available, valid templates
var templates = provider.GetTemplates();
foreach(var template1 in templates)
{
    Console.WriteLine("Found template with ID {0}", template1.ID);
}
```

## Apply a provisioning template to a site ## 
Applying a provisioning template on top of an existing site is done via single line of code:

```C#
// Apply template to existing site
ctx.Web.ApplyProvisioningTemplate(template);
```

This will apply the template to the site provided via the clientcontext object. When a template is applied the provisioning framework does verify if an object (field, list, content type,...) already exists and if not it will be created. 

> **Note:**
> The current implementation (v1) is not doing a detailed delta apply, meaning if for example the description of the field has changed this will not be applied. 


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Framework.Console" />