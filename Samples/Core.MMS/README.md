# OFFICE PnP: TAXONOMY OPERATIONS #

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
1.0  | 5-May-2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------
# Introduction #
Developing customizations with CSOM is very similar to developing .NET server side taxonomy solutions: get a reference to the TaxonomySession object and the TermStore object, Group objects, TermSet objects, and Term objects required for the session. When executing this sample, ensure that the account has the appropriate permissions to create new term set groups and terms.

High level process as follows.

![High level process](http://i.imgur.com/LottPge.png)

1. SharePoint hosted in the Office 365 or in on-premises
2. Remote solution using CSOM to manipulate the taxonomy store based on business requirements. Taxonomy CSOM operations are identical cross different environments, only the authentication model is slightly different depending on SharePoint hosting platform.


# Retrieve MMS terms #
Below code example demonstrates how to access terms using the taxonomy CSOM. This code is loading all groups, term sets and terms from the store and is outputting the name of them to console.

```C#
    //
    // Load up the taxonomy item names.
    //
    TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(cc);
    TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
    cc.Load(termStore,
            store => store.Name,
            store => store.Groups.Include(
                group => group.Name,
                group => group.TermSets.Include(
                    termSet => termSet.Name,
                    termSet => termSet.Terms.Include(
                        term => term.Name)
                )
            )
    );
    cc.ExecuteQuery();

    //
    //Writes the taxonomy item names.
    //
    if (taxonomySession != null)
    {
        if (termStore != null)
        {
            foreach (TermGroup group in termStore.Groups)
            {
                Console.WriteLine("Group " + group.Name);

                foreach (TermSet termSet in group.TermSets)
                {
                    Console.WriteLine("TermSet " + termSet.Name);

                    foreach (Term term in termSet.Terms)
                    {
                        //Writes root-level terms only.
                        Console.WriteLine("Term " + term.Name);
                    }
                }
            }
        }
    }
```

# Create MMS terms #
Here's simple sample on how to create group, term set and some terms using the Taxonomy CSOM.

```C#
 // Get access to taxonomy CSOM
TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(cc);
cc.Load(taxonomySession);
cc.ExecuteQuery();

if (taxonomySession != null)
{
    TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
    if (termStore != null)
    {
        //
        //  Create group, termset, and terms.
        //
        TermGroup myGroup = termStore.CreateGroup("Custom", Guid.NewGuid());
        TermSet myTermSet = myGroup.CreateTermSet("Colors", Guid.NewGuid(), 1033);
        myTermSet.CreateTerm("Red", 1033, Guid.NewGuid());
        myTermSet.CreateTerm("Orange", 1033, Guid.NewGuid());
        myTermSet.CreateTerm("Yellow", 1033, Guid.NewGuid());
        myTermSet.CreateTerm("Green", 1033, Guid.NewGuid());
        myTermSet.CreateTerm("Blue", 1033, Guid.NewGuid());
        myTermSet.CreateTerm("Purple", 1033, Guid.NewGuid());

        cc.ExecuteQuery();
    }
}
```


# DEPENDENCIES #
-  Microsoft.SharePoint.Client.dll
-  Microsoft.SharePoint.Client.Runtime.dll
-  Microsoft.SharePoint.Client.Taxonomy.dll

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.MMS" />