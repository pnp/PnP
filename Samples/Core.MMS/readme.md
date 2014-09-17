# OFFICE AMS: TAXONOMY OPERATIONS #

### Summary ###
This sample demonstrates how to perform taxonomy related operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Contoso.Core.MMS | Vesa Juvonen, Bert Jansen, Frank Marasco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 6th 2013 (to update) | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# GENERAL COMMENTS #
Developing customizations with CSOM is very similar to developing .NET server taxonomy solutions: get a reference to the TaxonomySession object and the TermStore object, Group objects, TermSet objects, and Term objects required for the session. When executing this sample, ensure that the account has the appropriate permissions to create new term set groups and terms.

# DEPENDENCIES #
-  Microsoft.SharePoint.Client.dll
-  Microsoft.SharePoint.Client.Runtime.dll
-  Microsoft.SharePoint.Client.Taxonomy.dll