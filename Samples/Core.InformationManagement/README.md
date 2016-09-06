# Information Management #

### Summary ###
This scenario shows how you can work with information management policies.

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
Core.InformationManagment | Frank Marasco, Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | May 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
With most SharePoint implementations there are sites that need to be managed. Sites have been created by end-users for team collaboration, maybe sites used for a short period of time, or even sites that are no longer used that is just taking up space in your SharePoint on-premises or Office 365 environment. This scenario shows the pattern on how to work with Site Policies using CSOM. 

Site Policies allow you to create a retention policy that can be applied to a site. Site Policy includes the following options:
-  **Site Closure:** The date the site is put into a "Closed" state. Closed does not stop users and admins from continuing to use the site and its content, but is just meant to denote that the site is not being actively used any longer and will be moving to the next site lifecycle stage. Sites can be manually closed. Site owners can also manually reopen closed sites (Note: only site collection administrators can reopen closed site collections.)
-  **Site Deletion:** The period of time following site creation or site closure that the site should be permanently deleted. When a site is deleted all its content and sub-webs are also deleted.
-  **Postponement:** This option will allow a site owner to manually postpone site deletion for a period of time determined by the Site Policy.
-  **E-mail Notification:** Before a site is deleted, an e-mail will be sent to site owners alerting them about the pending site deletion. Site Policy determines the period of time before scheduled site deletion that the e-mail is sent as well as recurrence frequency and timing of the e-mail alert.

To get more information related to site policies visit http://technet.microsoft.com/en-us/library/jj219569.aspx

This solution can be incorporated into a custom site provisioning process and have policies applied post site provisioning or maybe executing outside perhaps in a governance solution.

# SITE POLICIES SETUP #
In order to use Site Policies within your environment you must use the Content Type Hub to publish out site policies to site collections. If you are using a SharePoint Online MT environment the content type hub is already provisioned and configured for you and is located https://TENANT/sites/contentTypeHub. You publish the Site Policies very similar to publishing content types, and in fact Site Policies is a content type under the covers. The name of the content is Project Policy and has an ID 0x010085EC78BE64F9478aAE3ED069093B9963.

### Note: ###
Creating Site Policies programmatically is not available today using add-in model. 

![Site policy list](http://i.imgur.com/njiTUNy.png)

![Creating new site policy from UI](http://i.imgur.com/DeMfdG5.png)

# SCENARIO: FETCH SITE POLICY INFORMATION #
This include sample demonstrates a pattern on how to work with the SharePoint CSOM ClientContext to retrieve site policy settings for the current site collection.

## CHECK FOR APPLIED SITE POLICY AND GET THE CLOSURE AND EXPIRATION DATE ##

```C#
if (cc.Web.HasSitePolicyApplied())
{
  lblSiteExpiration.Text = String.Format("The expiration date for the site is {0}", cc.Web.GetSiteExpirationDate());
  lblSiteClosure.Text = String.Format("The closure date for the site is {0}", cc.Web.GetSiteCloseDate());
}
```

## GET A LIST OF THE AVAILABLE SITE POLICIES, KNOW THE APPLIED POLICY ##

```C#
List<SitePolicyEntity> policies = cc.Web.GetSitePolicies();
SitePolicyEntity appliedPolicy = cc.Web.GetAppliedSitePolicy();
```

# SCENARIO: UPDATE SITE POLICY SETTINGS #
This scenario demonstrates how to work with Site Policies and use CSOM ClientContext to update site policy settings for the current site collection.

## APPLY A SITE POLICY (SITE POLICY NEEDS TO BE AVAILABLE) ##
This scenario demonstrates how to retrieve all policies that are available on the site.

```C#
cc.Web.ApplySitePolicy("policy name");
```

# REFERENCES #
-  http://technet.microsoft.com/en-us/library/jj219569.aspx
-  http://blogs.technet.com/b/tothesharepoint/archive/2013/03/28/site-policy-in-sharepoint.aspx
-  http://blogs.technet.com/b/speschka/archive/2009/10/30/publish-and-subscribe-to-content-types-in-sharepoint-2010.aspx


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.InformationManagment" />