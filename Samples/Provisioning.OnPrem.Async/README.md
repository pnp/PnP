# Provisioning.OnPrem.Async #

### Summary ###
Asynchronous provisioning of site collections in on-premises using CSOM. Process and video available from following blog post
- [Async site collection provisioning with add-in model for on-prem and Office 365 Dedicated](http://blogs.msdn.com/b/vesku/archive/2014/08/29/async-site-collection-provisioning-with-app-model-for-on-prem-and-office-365-dedicated.aspx "Async site collection provisioning with app model for on-prem and Office 365 Dedicated")

### Applies to ###

-  SharePoint 2013 on-premises


### Prerequisites ###
Check the needed prerequisites from following blog post
- [Provisioning site collections using SP add-in model in on-premises with just CSOM](http://blogs.msdn.com/b/vesku/archive/2014/06/09/provisioning-site-collections-using-sp-app-model-in-on-premises-with-just-csom.aspx "Provisioning site collections using SP App model in on-premises with just CSOM")

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.OnPrem.Async | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 11th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Building a async provisioning model #
Since provisioning of site collections can take a quite a long time, you really don't want end user to be waiting this time while gif animation is running in browser. Much better approach is to use async patter and notify the requestor whenever the provisioning is completed.

![Process picture with 5 steps](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/6644.image_5F00_505B73EC.png)

1. When new site collection is needed, users can go to self service site collection form for filling the needed metadata for the upcoming site collection. 
1. Request of the new site collection is stored to specific list located for example in root site collection of the web application or tenant (if in Office365) 
1. You can associate separate approval workflow to the submissions if that’s needed simply by using out of the box approval workflow model or just as well you could create new workflow for more complex scenarios using SharePoint designer or any other means 
1. Approved requests are processed by remote timer job, which is configured to check the requests from the specific list. There are multiple ways to schedule the needed code for this, like using worker process or web job in Microsoft Azure. In on-premises you could run this as scheduled windows task or execute it from PowerShell as needed. ◦ Note. for high availability purposes you’d simply have “in progress” stamp in the list for marking the items which are taken into processes by scheduled tasks from different servers running the remote timer job. This way you have high availability for request processing even in on-premises with two servers running the remote timer job.
1. Actual provisioning of the site collections is performed based on the stored data in the list using CSOM. 
1. When site provisioning is completed, notification is sent for the requestor using email. You can also for example show the latest newly created site collections in front page of the intranet, if that’s what business is looking for as a additional capability with the email… or push notifications to social feeds. Whatever is needed.

 
# Structure #
This sample contains two solutions. One for web UI, which is optional, but does ask the needed permissions which we can then also use for the actual "remote timer job" process.

Second solution is the actual "remote timer job", which can be scheduled for example using windows scheduler and could be running in the on-premises provider hosted add-in environment servers.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.OnPrem.Async" />