# Generic sample of building an Office 365 Connector #

### Summary ###
This is generic sample of a custom Office 365 Connector built using Microsoft .NET.

See following PnP Web Cast for additional details and live demo around this sample
- [PnP Web Cast - PnP Web Cast - Introducing Office 365 Connectors](https://dev.office.com/blogs/introducing-office-365-connectors)

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
App configuration in the Azure AD - Client ID and Client Secret

### Solution ###
Solution | Author(s)
---------|----------
Office365.Connectors | Paolo Pialorsi

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 1st 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Setup guidance #
High level configuration details as follows:

- Register client id and secret in the Azure Active Directory, or update the authentication settings using Visual Studio 2015 wizard
- Configure web.config file accordingly with registered app information targeting the ida: app settings

```xml
  <appSettings>
    <add key="ida:ClientId" value="[Your Client ID]" />
    <add key="ida:AADInstance" value="https://login.microsoftonline.com/" />
    <add key="ida:Domain" value="[Your tenant name].onmicrosoft.com" />
    <add key="ida:TenantId" value="[Your tenant ID]" />
    <add key="ida:PostLogoutRedirectUri" value="https://localhost:44361/" />
  </appSettings> 
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Office365.Connectors" />