# PnP Provisioning engine extensions #

### Summary ###
This scenario shows a usage of exentensibiliy providers for the PnP provisioning engine. 

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Framework.Extensions | Ivan Vagunin (Knowit Oy)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 7th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Navigation #
Navigation\NavigationHandler implements extensibility provider for provisioning structural and taxonomy navigation
Navigation\NavigationProvisionSchema.xsd - xml schema for extensibility provide, use compile_xsd.bat to generate C# classes based on schema

Current version supports only top level navigation nodes. For structural navigation existing all nodes are deleted before provisioning new nodes

Structural navigation xml snippet
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.NavigationHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Navigations xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Navigation">
			<pnpx:Navigation Type="Structural" RootNodeId="1002">
				<pnpx:NavigationNode Title="Home" Url="~SiteCollection"/>
				<pnpx:NavigationNode Title="Documents" Url="~SiteCollection/Shared%20Documents"/>
			</pnpx:Navigation>
		</pnpx:Navigations>
	</pnpx:Configuration>
</pnp:Provider>

Taxonomy navigation xml snippet:
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.NavigationHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Navigations xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Navigation">
			<pnpx:Navigation Type="Taxonomy" SiteMapProvider="GlobalNavigationTaxonomyProvider">
				<pnpx:NavigationNode Title="Home" Url="/"/>
			</pnpx:Navigation>
		</pnpx:Navigations>
	</pnpx:Configuration>
</pnp:Provider>

# Site template #
sitetemplate.xml contains sample data for extensibility providers




