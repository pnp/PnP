# Simple Hybrid Provisioning #

### Summary ###
This sample demonstrates simplest possible hybrid setup with Azure storage queues, WebJobs and Service Bus relay. This is a demonstration of hosting a provider add-in in the Azure web site, which can be used to provision new custom branded site collections to on-premises farm without any add-in infrastructure requirements to on-premises.


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Will require Azure setup and on-premises SharePoint farm. Includes also PS script to adjust on-premises SP farm setup for the 

You will have to also enable CSOM based site collection provisioning in on-premises using following guidance: [Provisioning site collections using SP add-in model in on-premises with just CSOM](http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx) 

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Hybrid.Simple | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 6th 2015 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Documentation #
Please see following blog posts for details

- [Hybrid site collection provisioning from Azure towards on-premises SharePoint](http://blogs.msdn.com/b/vesku/archive/2015/03/05/hybrid-site-collection-provisioning-from-azure-to-on-premises-sharepoint.aspx)

# Video #
We have also video explaining this sample at Office 365 Developer Patterns and Practices Channel 9 site. 

- [Hybrid site collection provisioning from Azure to on-premises](http://channel9.msdn.com/blogs/OfficeDevPnP/Hybrid-site-collection-provisioning-from-Azure-to-on-premises)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Hybrid.Simple" />