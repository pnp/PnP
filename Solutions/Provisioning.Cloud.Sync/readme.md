# Site provisioning with custom UI modules #

### Summary ###
This sample shows how to customize the site and site collection provisioning forms with custom UI modules.  The sample has two sample modules:

- Yammer Integration Module allows Yammer groups to be provisioned with sites and displayed on the site landing page (via Yammer embed)
- Business Impact Module allows a user to mark a site with a sensitivity attribute that is set in the sites property bag after provisioning


### Applies to ###
-  Office 365 Multi Tenant (MT)


Similar model can be used with Office 365 Dedicated or on-premises, but this code is using Multi-Tenant specific site collection creation API, so the sample does not work as such in other environments.

### Prerequisites ###
Yammer integration requires existing Yammer feed and user to be logged on that network

### Solution ###
Solution | Author(s)
---------| ----------
Provisioning.Cloud.Sync | Richard diZerega (Microsoft), Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
3.0  | May 5th 2014 | Few old versions combined to one sample
2.0  | October 14th 2013 | Yammer integration included
1.0  | August 6th 2013 | Initial release with xml based configuration and basic structure

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
This solution shows the model for providing synchronous site collection or sub site (!) creation expirience to introduce model for site templates without using actual sandbox solutions or stp files. Usage of web templates (wsp) or site templates (stp) will have big impact on the evergreen model of the sites and will cause significant additional costs for long run. 

Recommended option for site provisioning is to use CSOM APIs. This model is demonstrated in this example with additional capabilities on configuring different kind of site configurations for end users to choose from.


# Usage of provisioning modules #
Provisioning Modules allow user controls to be dynamically displayed in a site creation form depending on the selected template configuration.  The provisioning module user controls need to inherit from the BaseProvisioningModule.cs file which has a single virtual “Provision” method as seen below.  This is called after the site provisioning has completed and all other configurations have been applied for the template.

```C#
public class BaseProvisioningModule : UserControl
{
    public virtual void Provision(ClientContext context, Web web)
    {
    }
}
```

## Sample provisioning module ##
The following is a sample provisioning module for setting a metadata property in the new sites property bag.  Note the inheritance of *BaseProvisioningModule* and implementation of the Provision method below:

```C#
namespace Contoso.SPOSiteProvisioningWeb.Modules
{
    public partial class BusinessImpactProvisioningModule : BaseProvisioningModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (cboSensitivity.Items.Count == 0)
            {
                cboSensitivity.Items.Add(new ListItem("Low Business Impact (LBI)"));
                cboSensitivity.Items.Add(new ListItem("Medium Business Impact"));
                cboSensitivity.Items.Add(new ListItem("High Business Impact (HBI)"));
            }
        }

        public override void Provision(ClientContext context, Web web)
        {
            //get the web's property bag
            var props = web.AllProperties;
            context.Load(props);
            context.ExecuteQuery();

            //set the ContosoBusinessImpact property and update
            props["ContosoBusinessImpact"] = cboSensitivity.SelectedValue;
            web.Update();
            context.ExecuteQuery();

            //call the base
            base.Provision(context, web);
        }
    }
}
```

# Template configuration #
Individual templates are defined in custom xml schema whcih was created specifically for this sample. This XML structure can be used to define the branding elements, lists, navigation and module configurations for the selected template in the UI. Here's an example of one template configuration, which has also two modules associated to it.

```XML
<Template Name="ContosoTeam" Title="Contoso Team" Enabled="Yes" RootTemplate="STS#0" Description="Super Team Site" RootWebOnly="false" SubWebOnly="false" ManagedPath="teams" StorageMaximumLevel="100" UserCodeMaximumLevel="100">
  <Modules>
    <Module CtrlSrc="~/Modules/BusinessImpactProvisioningModule.ascx" />
    <Module CtrlSrc="~/Modules/YammerProvisioningModule.ascx" />
  </Modules>
```

You can have a look on the sample configurations from the solution.

#Site collection provisioning#
Solution supports the creation of site collection, but currently only for Multi-Tenant SharePoint Online.  There is also a [reference solution](https://github.com/OfficeDev/PnP/tree/master/Samples/Provisioning.OnPrem.Async) for the on-premises or Dedicated environments in the PnP solution, which is using more locked down configuration.

## Configuration options ##
The Site Collection Provisioning component of this solutions leverages five  attributes on the Template element of the Configuration.xml.  

These are described below:

- **RootWebOnly**: true/false to make the template available as site collections only
- **SubWebOnly**: true/false to make the template available as subsites only
- **ManagedPath**: managed path to create the site collection under (ex: teams or sites)
- **StorageMaximumLevel**: default storage quota in MB
- **UserCodeMaximumLevel**: default user code quota

Here is a sample configurations:

```XML
<Template Name="ContosoTeam" Title="Contoso Team" Enabled="Yes" RootTemplate="STS#0" Description="Super Team Site" RootWebOnly="false" SubWebOnly="false" ManagedPath="teams" StorageMaximumLevel="100" UserCodeMaximumLevel="100">
  <Modules>
    <Module CtrlSrc="~/Modules/BusinessImpactProvisioningModule.ascx" />
    <Module CtrlSrc="~/Modules/YammerProvisioningModule.ascx" />
  </Modules>
```

# Wiring into “New Site” link #
The SPOSiteProvisioning solution is configured to create site collections when it is launched from the “New Site” link in the “Sites” area of SharePoint.  The default form can be overwritten inside the SharePoint admin portal.  First, you should launch the creation form from the host site to capture full URL (including the standard token url parameters).  

Next, paste the site creation form URL into notepad and remove all the URL parameters except the following:

-	SPHostUrl
-	SPAppWebUrl
-	IsDlg (change this value from 0 to 1)

You should end up with a url similar to the following (replace elements in brackets with the values from your deployment:

- *https://{hostdomain}/Pages/Default.aspx?SPHostUrl={HostWebUrl}&SPAppWebUrl={AppWebUrl}&IsDlg=1*

 
![](http://i.imgur.com/kQj6Gky.png)

Custom Provisioning Form launched from “new site” link for self-service site collections:

![](http://i.imgur.com/7rttP7Y.png)