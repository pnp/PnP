# Framework.Provisioning #

### Summary ###
 
This is the initial release of a reference architecture that demonstrates how to provision sites collections in SharePoint Online and SharePoint 2013 on-premises based on a custom XML Template. You're probably familiar with the default site templates, such as Team Site, Project Site, and Communities Site. SharePoint site templates are prebuilt definitions designed around a particular business need. This reference sample take your site provisioning to the next level.  You can use these XML templates to create your own SharePoint site, that defines Fields, Content Types, libraries, lists, views, branding via Compose looks, logos, and other elements that you require for your business needs needs. This XML template servers as blueprint for site which always you to quickly apply to other SharePoint environments and even use the template as the basis for a business solution. 

We are currently working on better documentation which will include logical components, extensibility, XML Schema Definition for the XML Template, a more pleasant User Interface for your users and many more enhancements.  

### Applies to ###
- Office 365 Dedicated (D)
- Office 365 Multi-Tenant (MT)
- SharePoint 2013 On-Premises

### Prerequisites ###
- Azure Subscription
- SharePoint 2013 On-premises with an ACS trust if your hosting on-premises
- SharePoint Online MT 

### Solution ###
Solution | Author(s)
---------|----------
Framework.Provisioning | Frank Marasco, Brian Michely, Suman Chakrabarti, Bert Jansen, Vesa Juvonen


### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0  | 3-5-2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Solution structure #

![](http://i.imgur.com/fbkfFYS.png)

## Framework.Provisioning.Job ##
Primary Remote Timer Job that creates the site collection. This solution processes incoming messages from the Azure Queue. 
## Framework.Provisioning.SiteRequest.Job ##
This project is used to read from a the Site Request list that resides in your site that hosts the Provisioning App and is responsible for processing Site Requests and adding messages to the Azure Queues.

## Framework.Provisioning.SPApp ##
SharePoint App Project

## Framework.Provisioning.SPAppWeb ##
This is the actual provider hosted app which is hosted in Microsoft Azure. In this release we provide a simple User interface to test the solution.

## Framework.Provisioning.Azure ##
Helper component for working with Azure Queues

## Framework.Provisioning.Core ##
Primary Engine Component that implements the custom XML Template

## Framework.Provisioning.Extensiblity.Designer ##
Sample Provider that is used to demonstrate the extensibility of the engine by implementing a provider call out. This is just another way to introduce customization post processing of the site collection request.

## OfficeDevPnP.Core ##
Office 365 Developer PnP Core Component is extension component which encapsulates commonly used remote CSOM/REST operations as reusable extension methods towards out of the box CSOM objects 

## Framework.ProvisionTests ##
Integrate Tests which helps us test the Framework.Provisioning.Core component
