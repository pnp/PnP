# Simple Hybrid Provisioning #

### Summary ###
This sample demonstrates simplest possible hybrid setup with Azure storage queues, WebJobs and Service Bus relay. This is a demonstration of hosting a provider app in the Azure web site, whcih can be used to provision new custom branded site collections to on-premises farm without any app infrastructure requirements to on-premises.

Documentation and detailed setup instructions fill follow soon

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Will require Azure setup and on-premises SharePoint farm. Includes also PS script to adjust on-premises SP farm setup for the 

You will have to also enable CSOM based site collection provisioning in on-premises using following guidance: [Provisioning site collections using SP App model in on-premises with just CSOM](http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx) 

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Hybrid.Simple | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 6th 2015 | Initial version without proper documentation (sorry)

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Documentation #
Fill be updated soon. If you have previous Azure experience, you can setup this pretty easily. Do check the app.config and web.config files for needed changes related on the storage account and service bus keys.