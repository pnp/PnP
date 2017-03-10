# Site collection provisioning in on-premises or Dedicated using CSOM #

### Summary ###
This sample shows how to provision site collections in on-premises or in Office 365 Dedicated using CSOM.

**Notice** that after SP2013 April CU (2014), this capability is natively supported in on-premises. This example however does show how to also expose other relevant APIs from the server, which might not be exposed by using oob methods. Check following blog article for latest guidance.

- [Async site collection provisioning with add-in model for on-prem and Office 365 Dedicated](http://blogs.msdn.com/b/vesku/archive/2014/08/29/async-site-collection-provisioning-with-app-model-for-on-prem-and-office-365-dedicated.aspx)

### Applies to ###
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
To be able to use this sample, you will explicitly need to enable this capability in on-premises. It is automatically enabled for Office 365 Dedicated.

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.SiteCol.OnPrem | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 8th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
Required steps to enable the sample are following.

1. Install 2014 April CU to your on-premises farm
1. Rim EnableOnPremSiteCol.ps1
1. Choose one site collection to act as "Tenant ddmin site" and execute SetSiteColAdministrationSiteType.ps1
1. Ensure that you use 15 version of the Microsoft.Online.SharePoint.Client.Tenant assembly

Check detailed documentation from [http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx](http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx).

You should also have a look on how you can perform similar capability using asynchronous pattern, which is much more end user friendlier way to provide this functionality. Details on this pattern are available from following blog post: [http://blogs.msdn.com/b/vesku/archive/2014/08/29/async-site-collection-provisioning-with-app-model-for-on-prem-and-office-365-dedicated.aspx](http://blogs.msdn.com/b/vesku/archive/2014/08/29/async-site-collection-provisioning-with-app-model-for-on-prem-and-office-365-dedicated.aspx)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.SiteCol.OnPrem" />