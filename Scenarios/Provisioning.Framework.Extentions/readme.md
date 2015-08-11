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
Provisioning.Framework.Extensions | Ivan Vagunin (Knowit Oy), Alisher Abdurakhmanov (Datium Oy)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 7th 2015 | Initial release
1.1  | August 10th 2015 | Security extensions added

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Navigation #
Navigation\NavigationHandler implements extensibility provider for provisioning structural and taxonomy navigation
Navigation\NavigationProvisionSchema.xsd - xml schema for extensibility provider, use compile_xsd.bat to generate C# classes based on schema

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

# Security #
Security\SecurityHandler implements extensibility provider for security related objects
Security\SecurityProvisionSchema.xsd - xml schema for extensibility provider, use compile_xsd.bat to generate C# classes based on schema

Permission level xml snippet:
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.SecurityHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Security xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Security">
			<pnpx:PermissionLevels>
				<pnpx:PermissionLevel Title="Restricted Read" Description="Can view pages and documents. No access to system pages">
					<pnpx:Permissions>
						<pnpx:BasePermission>ViewPages</pnpx:BasePermission>
						<pnpx:BasePermission>Open</pnpx:BasePermission>
						<pnpx:BasePermission>ViewListItems</pnpx:BasePermission>
						<pnpx:BasePermission>OpenItems</pnpx:BasePermission>
					</pnpx:Permissions>
                </pnpx:PermissionLevel>
            </pnpx:PermissionLevels>
		</pnpx:Security>
	</pnpx:Configuration>
</pnp:Provider>

Security group snippet:
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.SecurityHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Security xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Security">
			<pnpx:Groups>
                <pnpx:Group Title="Finance Department" PermissionLevel="Restricted Read" Description="Members of this group have special permissions">
					<pnpx:Members>
						<pnpx:Member>user1@mytentancy.onmicrosoft.com</pnpx:Member>
						<pnpx:Member>user2@mytentancy.onmicrosoft.com</pnpx:Member>
					</pnpx:Members>
                </pnpx:Group>
            </pnpx:Groups>
		</pnpx:Security>
	</pnpx:Configuration>
</pnp:Provider>

List and folders level permissions:
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.SecurityHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Security xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Security">
			<pnpx:PermissionScopes>
				<pnpx:ListScope Title="Documents" CopyRoleAssignments="false" ClearSubscopes="false">
					<pnpx:Permissions>
						<pnpx:Permission Principal="Site Owners" PermissionLevel="FullControl"/>
						<pnpx:Permission Principal="Finance Department" PermissionLevel="Read"/>
					</pnpx:Permissions>
					<pnpx:FolderScope RelativeUrl="FinancialDocuments" CopyRoleAssignments="false">
						<pnpx:Permissions>
							<pnpx:Permission PermissionLevel="FullControl" Principal="Finance Department"/>
						</pnpx:Permissions>
					</pnpx:FolderScope>
                </pnpx:ListScope>
            </pnpx:PermissionScopes>
		</pnpx:Security>
	</pnpx:Configuration>
</pnp:Provider>

# List folders #
Folders\FolderHandler implements extensibility provider for creating list folders
Security\FolderProvisionSchema.xsd - xml schema for extensibility provider, use compile_xsd.bat to generate C# classes based on schema

Folder creation xml snippet:
<pnp:Provider Enabled="true" HandlerType="Provisioning.Framework.Extensions.FolderHandler, Provisioning.Framework.Extensions">
	<pnp:Configuration>
		<pnpx:Folders xmlns:pnpx="http://schemas.dev.office.com/PnP/2015/05/ProvisioningSchema/Extentions/Folder">
			<pnpx:Folder List="Documents" Path="FinancialDocuments"/>
		</pnpx:Folders>
    </pnp:Configuration>
</pnp:Provider>

# Site template #
sitetemplate.xml contains sample data for extensibility providers
