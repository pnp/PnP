# Fields Extensibility Provider


## Summary

- [x] Localizes a field title
- [ ] Next things to be implemented on fields goes here...
- [ ] Next things to be implemented on fields goes here...
- [ ] Next things to be implemented on fields goes here...


## Applies to

- [x] SharePoint 2013 on-premise
- [x] Office 365


## Prerequisites

None


## Sample

```xml
<pnp:Provider Enabled="true" "HandlerType="PNP.Deployer.ExtensibilityProviders.SSOM.FieldsExtensibilityProvider, PNP.Deployer.ExtensibilityProviders.SSOM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
	<pnp:Configuration>
		<ProviderConfiguration xmlns="http://PNP.Deployer/ProviderConfiguration">
			<Fields>
				<Field Name="MyField">
					<TitleResources>
						<TitleResource LCID="1033" Value="My field" />
						<TitleResource LCID="1033" Value="Ma colonne" />
					</TitleResources>
				</Field>
			</Fields>
		</ProviderConfiguration>
	</pnp:Configuration>
</pnp:Provider>
```
