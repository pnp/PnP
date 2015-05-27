# Contoso Intranet #
Real life full trust solution mock-up for FTC to add-in model transition testing and verification. Based on the guidance from MCM/MCSM related on the FTC implementation. Mimics typical set of functionalities.

- Content Types and site column
- Web templates (team site and publishing site)
- Custom master page
- Custom page layout
- Web controls in page layouts and master pages
- Custom list template
- Custom web parts

### Creation of site collections ###

```C#
New-SPSite -url http://dev.contoso.com/sites/test011 -owneralias contoso\administrator -template "{53a719c7-6766-4bba-bf89-a5789ea2360a}#WTContoso" -name Test

New-SPSite -url http://dev.contoso.com/sites/test017 -owneralias contoso\administrator -template "{53a719c7-6766-4bba-bf89-a5789ea2360a}#WTContosoPublishing" -name Test

```
