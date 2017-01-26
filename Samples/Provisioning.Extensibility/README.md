# PnP-Site-Core Provisioning Extensibility provider to provision Publishing pages #

### Summary ###
This solution shows how to use the Provisioning extensibility framework to create a custom provider that provisions Publishing Pages using the PnP templating engine.

*Notice*: 
At the moment of the creation of this project, publishing pages provisioning are not supported by PnP-Site-Core framework. The File node almost do the trick, but it can't associate the Page Layout.
Surely, support for Publishing pages will be added to PnP-Site-Core in the future release.
This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core)
This Provider is just a sample showing how to build providers. It does not cover all the Publishing pages scenarios and is not multi-lingual

### Applies to ###
- Office 365 Multi-Tenant (MT)
- Office 365 Dedicated (D)
- SharePoint 2013 on-premises

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Extensibility | Luis Ma�ez

### Configuration ###
The provider is configured using the Providers Node in the XML template:

```xml
				<pnp:Provider Enabled="true" 
						HandlerType="Provisioning.Extensibility.Providers.PublishingPageProvisioningExtensibilityProvider, Provisioning.Extensibility.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
					<pnp:Configuration>
						<PublishingPageProvider 
							id="PublishingPageProviderConfig" 
							xmlns="http://schemas.somecompany.com/PublishingPageProvisioningExtensibilityProviderConfiguration">
							
							<Page Overwrite="true"
								  FileName="default"
								  Publish="true"
								  Title="PnP Extesions Test"
								  Layout="BlankWebPartPageCopy"
								  WelcomePage="true" >
								
								<Properties>
									<Property Name="PublishingPageContent" 
											  Value="&lt;b&gt;Welcome to PnP&lt;/b&gt;. This is an Extensions sample." />
								</Properties>
								
								<WebParts>
									
									<WebPart Zone="CenterColumn" Order="0" Title="LinksList Custom JSLink" DefaultViewDisplayName="All Links">
										<Contents>
											<![CDATA[
											]]>
										</Contents>
									</WebPart>
								</WebParts>
							</Page>
						</PublishingPageProvider>
					</pnp:Configuration>
				</pnp:Provider>
```

#### Page ####
Attibute|Description
--------|-----------
Overwrite|Overwrite file if already exists
FileName|Name of the file without the extension (.aspx)
Publish|Specifies if the page is published after creation
Title|Title of the page
Layout|Publishing page layout associated to the Page
WelcomePage|Set if page is set as welcome page

##### Properties #####
List of Property nodes, where each Property is a property to set to the page, like PublishingPageContent or any other field in the Pages library

##### WebParts #####
List of WebPart nodes, where:

Attibute|Description
--------|-----------
Zone|Page layout zone where the webpart will be provisioned
Order|Webpart order in page
Title|Webpart title
DefaultViewDisplayName|Display name of the List View if we are provisioning a ListViewWebPart. Leave it empty if it is not a LVWP. *See comments in Source code to get more info about it*
Contents|Webpart content. See Sample template to view some examples.

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0  | Dec 6th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Extensibility" />