# This sample demonstrates how to Synchronize Terms across multiple term stores #

### Summary ###
This sample demonstrates how to Synchronize Terms across multiple term stores.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Core.MMSSync | Kimmo Forss, Frank Marasco, Bert Jansen (**Microsoft**)

### Version history ###
Version | Date | Comments
---------| -----| --------
1.0 | May 5th 2014 | Initial release
2.0 | December 2nd 2014 | Major rewrite of the sync manager, now supports all change events for groups, term sets and terms, hierarchical term sets, reused terms, multiple languages, more robust operations, logging,...

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Taxonomy Synchronization Scenarios #
Have you ever wanted to move Taxonomy items from one term store to another? With on-premises implementations you can move your MMS database, but this takes some work. What about SharePoint Online? Obviously, we cannot move our database to Office 365. There is already an PNP (Core.MMS) sample that demonstrates how to perform basic operations with the term store. What if, you have use case where you only want to synchronize changes of a specific Term set? This sample takes the Core.MMS sample a step further, by using the ChangeInformation class in the Microsoft.SharePoint.Client.Taxonomy assembly.

Here's the high level process for the MMS synchronization tooling.

![Process picture with 4 steps](http://i.imgur.com/CpWRWOL.png)

1. One farm has to act as the primary farm, which is used as the location where changes are applied. 
2. MMS synchronization tooling will access the primary MMS/taxonomy store and will query changes applied to taxonomy after certain time period. Tool will synchronize or repeat the operations also against the other SharePoint services, like Office 365
3. MMS is kept on sync between the environments, so that when end users will apply metadata to documents, same terminology is available and if the documents are moved cross environments, metadata is properly stored and kept in the document level
4. End users can access any environment and they will see same taxonomy terms

See also referenced blog post for some additional context.

[http://blogs.msdn.com/b/frank_marasco/archive/2014/06/29/synchronize-term-sets-with-the-term-store-csom.aspx](http://blogs.msdn.com/b/frank_marasco/archive/2014/06/29/synchronize-term-sets-with-the-term-store-csom.aspx)

## SCENARIO 1 ##
The first scenario, will take a Source Context and Term Group name and create a new Term Group in the target.

Code snippet:
```C#
private void CreateNewTargetTermGroup(ClientContext sourceClientContext, ClientContext targetClientContext, TermGroup sourceTermGroup, TermStore targetTermStore, List<int> languagesToProcess)
{
    TermGroup destinationTermGroup = targetTermStore.CreateGroup(sourceTermGroup.Name, sourceTermGroup.Id);
    if (!string.IsNullOrEmpty(sourceTermGroup.Description))
    {
        destinationTermGroup.Description = sourceTermGroup.Description;
    }

    TermSetCollection sourceTermSetCollection = sourceTermGroup.TermSets;
    if (sourceTermSetCollection.Count > 0)
    {
        foreach (TermSet sourceTermSet in sourceTermSetCollection)
        {
            sourceClientContext.Load(sourceTermSet,
                                      set => set.Name,
                                      set => set.Description,
                                      set => set.Id,
                                      set => set.Contact,
                                      set => set.CustomProperties,
                                      set => set.IsAvailableForTagging,
                                      set => set.IsOpenForTermCreation,
                                      set => set.CustomSortOrder,
                                      set => set.CustomProperties,
                                      set => set.Terms.Include(
                                                term => term.Name,
                                                term => term.Description,
                                                term => term.Id,
                                                term => term.IsAvailableForTagging,
                                                term => term.LocalCustomProperties,
                                                term => term.CustomProperties,
                                                term => term.IsDeprecated,
                                                term => term.Labels.Include(label => label.Value, label => label.Language, label => label.IsDefaultForLanguage)));

            sourceClientContext.ExecuteQuery();

            TermSet targetTermSet = destinationTermGroup.CreateTermSet(sourceTermSet.Name, sourceTermSet.Id, targetTermStore.DefaultLanguage);
            targetClientContext.Load(targetTermSet, set => set.CustomProperties);
            targetClientContext.ExecuteQuery();
            UpdateTermSet(sourceClientContext, targetClientContext, sourceTermSet, targetTermSet);

            foreach (Term sourceTerm in sourceTermSet.Terms)
            {
                Term reusedTerm = targetTermStore.GetTerm(sourceTerm.Id);
                targetClientContext.Load(reusedTerm);
                targetClientContext.ExecuteQuery();

                Term targetTerm;
                if (reusedTerm.ServerObjectIsNull.Value)
                {
                    try
                    {
                        targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, sourceTerm.Id);
                                targetClientContext.Load(targetTerm, term => term.IsDeprecated,
                                                                     term => term.CustomProperties,
                                                                     term => term.LocalCustomProperties);
                                targetClientContext.ExecuteQuery();
                                UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, targetTerm, languagesToProcess);
                    }
                    catch (ServerException ex)
                    {
                        if (ex.Message.IndexOf("Failed to read from or write to database. Refresh and try again.") > -1)
                        {
                            // This exception was due to caching issues and generally is thrown when there's term reuse accross groups
                            targetTerm = targetTermSet.ReuseTerm(reusedTerm, false);
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    targetTerm = targetTermSet.ReuseTerm(reusedTerm, false);
                }

                targetClientContext.Load(targetTerm);
                targetClientContext.ExecuteQuery();

                targetTermStore.UpdateCache();

                //Refresh session and termstore references to force reload of the term just added. This is 
                //needed cause there can be a update change event following next and without this trick
                //the newly created termset cannot be obtained from the server
                targetTermStore = GetTermStoreObject(targetClientContext);

                //recursively add the other terms
                ProcessSubTerms(sourceClientContext, targetClientContext, targetTermSet, targetTerm, sourceTerm, languagesToProcess, targetTermStore.DefaultLanguage);
            }
        }
    }
    targetClientContext.ExecuteQuery();
}
```

## SCENARIO 2 ##
This scenario will use the ChangeInformation class to process all changes in the source Term store to return all the changes that has occurred. First, create a TaxonomySession object. 

```C#
DateTime _startFrom = DateTime.Now.AddYears(-1);
Console.WriteLine("Opening the taxonomy session");
TaxonomySession _sourceTaxonomySession =
	TaxonomySession.GetTaxonomySession(sourceClientContext);
TermStore sourceTermStore =
	_sourceTaxonomySession.GetDefaultKeywordsTermStore();

sourceClientContext.Load(sourceTermStore);
sourceClientContext.ExecuteQuery();
```

Once you have created the TaxonomySession object we need to get the changes, we get the changes by creating a new Instance of ChangeInformation and set the start date. In this case, I’m getting all the changes from 1 year ago. I’m going to call the term store GetChanges method, which will return all the changes.

```C#
Console.WriteLine("Reading the changes");
ChangeInformation _ci = new ChangeInformation(sourceClientContext);
_ci.StartTime = _startFrom;
ChangedItemCollection _cic = sourceTermStore.GetChanges(_ci);

sourceClientContext.Load(_cic);
sourceClientContext.ExecuteQuery();
```

Once we invoke the GetChanges member this will return a ChangeItemCollection that  can be used to enumerate all the changes that have occurred in term store like we do below and take action based on the type of change that has occurred.

```C#
foreach (ChangedItem _changeItem in _cic) {
	///ENUMERATE YOU’RE CHANGES
	if (_changeItem.ItemType == ChangedItemType.Group) {
		///PROCESS YOU’RE CHANGES
	}
}
```

## Running the Sample ##
![Running the Sample](http://i.imgur.com/96Ub5Ht.png)

Ensure, that the user has the appropriate permissions to the term store in both the source and target term stores, or you will get an exception.


## Appendix A: So you want to automatically keep termstores in sync? ##
The MMSSyncManager class of this sample can be used to easily build a full fledged managed metadata sync tool. Below steps describe the high level tasks that you would need to deal with:
* Create a console application
* Define all configuration data (urls, users, encrypted passwords, settings) in app.config 
* Use the **CopyNewTermGroups** method to perform the initial sync
* Use the **ProcessChanges** method to get the changelog between the last sync and now
* Store the timestamp of the last sync as we're only interested in changes as of that moment
* Schedule this exe as an Azure Web Job or as a scheduled task on a Windows Server

Below sample code is an implementation of above high level steps that synchronizes managed metadata from a SharePoint Online environment to an on-premises SharePoint web application that's secured via SAML + ADFS. Note that this code uses the OfficeDevPnP.Core library to deal with the authentication needs.

```C#
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core;
using System.Threading;
using System.Configuration;
using System.Diagnostics;

namespace SharePoint.MMSSync
{
    class Program
    {
        static void Main(string[] args)
        {
            bool syncWasDone = false;
            DateTime newGetChangesAsOf;

            try
            {                
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //Read the configuration data
                string thumbPrint = ConfigurationManager.AppSettings["ThumbPrint"];
                string sourceUrl = ConfigurationManager.AppSettings["Source.Url"];
                string sourceUser = ConfigurationManager.AppSettings["Source.User"];
                string sourcePassword = ConfigurationManager.AppSettings["Source.Password"];
                string targetUrl = ConfigurationManager.AppSettings["Target.Url"];
                string targetUser = ConfigurationManager.AppSettings["Target.User"];
                string targetDomain = ConfigurationManager.AppSettings["Target.Domain"];
                string targetPassword = ConfigurationManager.AppSettings["Target.Password"];
                string targetADFSServer = ConfigurationManager.AppSettings["Target.ADFS.Server"];
                string targetADFSUrn = ConfigurationManager.AppSettings["Target.ADFS.Urn"];
                string termGroupExclusions = ConfigurationManager.AppSettings["TermGroup.Exclusions"];
                string changelogTimezoneDeltaInMinutes = ConfigurationManager.AppSettings["Changelog.TimezoneDeltaInMinutes"];
                string changelogSchedule = ConfigurationManager.AppSettings["Changelog.Schedule"];
                string syncInitializionDone = ConfigurationManager.AppSettings["Sync.InitializionDone"];
                string syncLastCompleteSyncDateTime = ConfigurationManager.AppSettings["Sync.LastCompleteSyncDateTime"];
                string loggingLogFile = ConfigurationManager.AppSettings["Logging.LogFile"];
                string loggingLevel = ConfigurationManager.AppSettings["Logging.Level"];

#if DEBUG
                if (String.IsNullOrEmpty(sourcePassword))
                {
                    sourcePassword = GetPassWord();
                    targetPassword = sourcePassword;
                }
#endif

                if (!String.IsNullOrEmpty(thumbPrint))
                {
                    sourcePassword = OfficeDevPnP.Core.Utilities.EncryptionUtility.Decrypt(sourcePassword, thumbPrint);
                    targetPassword = OfficeDevPnP.Core.Utilities.EncryptionUtility.Decrypt(targetPassword, thumbPrint);
                }

                AuthenticationManager amSource = new AuthenticationManager();
                ClientContext sourceContext = amSource.GetSharePointOnlineAuthenticatedContextTenant(sourceUrl, sourceUser, sourcePassword);
                sourceContext.RequestTimeout = Timeout.Infinite;

                AuthenticationManager amTarget = new AuthenticationManager();
                ClientContext targetContext = amTarget.GetADFSUserNameMixedAuthenticatedContext(targetUrl, targetUser, targetPassword, targetDomain, targetADFSServer, targetADFSUrn);
                targetContext.RequestTimeout = Timeout.Infinite;

                if (string.IsNullOrEmpty(loggingLogFile))
                {
                    loggingLogFile = "mmssync.log";
                }

                Log.Internal.Source.Listeners.Clear();
                Log.Internal.Source.Listeners.Add(new ConsoleTraceListener() { Name = "Console" });
                Log.Internal.Source.Listeners.Add(new DefaultTraceListener() { Name = "Default" });
                Log.Internal.Source.Listeners.Add(new TextWriterTraceListener(loggingLogFile) { Name = "File", TraceOutputOptions = TraceOptions.DateTime });

                SourceLevels level = SourceLevels.Information;
                if (!string.IsNullOrEmpty(loggingLevel))
                {
                    Enum.TryParse<SourceLevels>(loggingLevel, out level);
                }
                Log.Internal.Source.Switch.Level = level;

                if (!String.IsNullOrEmpty(syncInitializionDone))
                {
                    bool.TryParse(syncInitializionDone, out syncWasDone);
                }

                List<string> termGroupExclusionsList = new List<string>();
                if (!String.IsNullOrEmpty(termGroupExclusions))
                {
                    String[] groupsToExclude = termGroupExclusions.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    termGroupExclusionsList.AddRange(groupsToExclude);
                }

                int timeZoneAddMinutes = 0;
                if (!String.IsNullOrEmpty(changelogTimezoneDeltaInMinutes))
                {
                    int.TryParse(changelogTimezoneDeltaInMinutes, out timeZoneAddMinutes);
                }

                MMSSyncManager ms = new MMSSyncManager();

                if (!syncWasDone)
                {
                    if (ms.CopyNewTermGroups(sourceContext, targetContext, termGroupExclusionsList))
                    {
                        syncWasDone = true;

                        config.AppSettings.Settings["Sync.InitializionDone"].Value = true.ToString();
                        config.Save(ConfigurationSaveMode.Modified);
                        Log.Internal.TraceInformation((int)EventId.InitializationDone, "Sync engine initialized");
                    }
                }

                DateTime getChangesAsOf = DateTime.Now.AddMinutes(-1 * (timeZoneAddMinutes + 10));
                if (!String.IsNullOrEmpty(syncLastCompleteSyncDateTime))
                {
                    if (!DateTime.TryParse(syncLastCompleteSyncDateTime, out getChangesAsOf))
                    {
                        getChangesAsOf = DateTime.Now.AddMinutes(-1 * (timeZoneAddMinutes + 10));
                    }
                }

                Log.Internal.TraceInformation((int)EventId.GetChangesFrom, "Process changes as from {0}", getChangesAsOf.ToString());
                newGetChangesAsOf = DateTime.Now;
                if (ms.ProcessChanges(sourceContext, targetContext, getChangesAsOf, termGroupExclusionsList))
                {
                    config.AppSettings.Settings["Sync.LastCompleteSyncDateTime"].Value = newGetChangesAsOf.ToString();
                    config.Save(ConfigurationSaveMode.Modified);
                    Log.Internal.TraceInformation((int)EventId.ChangeProcessingDone, "Processing changes done");
                }
            }
            catch (Exception ex)
            {
                Log.Internal.TraceError((int)EventId.SyncError, ex, "Sync engine error");
            }
            finally
            {
                Log.Internal.Source.Flush();
            }
        }

        #if DEBUG
        private static string GetPassWord()
        {
            Console.Write("SharePoint Password : ");

            string strPwd = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (strPwd.Length > 0)
                    {
                        strPwd = strPwd.Remove(strPwd.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    Console.Write("*");
                    strPwd += keyInfo.KeyChar;

                }

            }
            Console.WriteLine("");

            return strPwd;
        }
        #endif

    }
}
```

The configuration file that belongs to this is the following:

```XML
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <appSettings>
    <!--Password decryption certificate-->
    <add key="ThumbPrint" value="xxxxxxxxxxxxxxxxxxxxx"/>
    <!--Information that describes the source of the managed metadata -->
    <add key="Source.Url" value="https://tenant.sharepoint.com/sites/dev"/>
    <add key="Source.User" value="user@tenant.onmicrosoft.com"/>
    <add key="Source.Password" value="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=="/>
    <!--Information that describes the target of the managed metadata. Given
        this is on-premises secured by ADFS the ADFS info needs to be provided -->
    <add key="Target.Url" value="https://saml.mydomain.com/Sites/test"/>
    <add key="Target.User" value="administrator"/>
    <add key="Target.Domain" value="MyDomain"/>
    <add key="Target.Password" value="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=="/>
    <add key="Target.ADFS.Server" value="sts.mydomain.com"/>
    <add key="Target.ADFS.Urn" value="urn:sharepoint:saml"/>
    <!-- Configure logging-->
    <add key="Logging.LogFile" value="c:\temp\mmssync.log"/>
    <!--Possible values: Off, Critical, Error, Warning, Information, Verbose-->
    <add key="Logging.Level" value="Verbose"/>
    <!-- The below list of termgroups are never synced -->
    <add key="TermGroup.Exclusions" value="local,People,Search Dictionaries,Taxonomy Navigation"/>
    <!-- The changelog entries have changedate that's based on the server's timezone. To correctly
         deal with this you can define the timezone delta in minutes -->
    <add key="Changelog.TimezoneDeltaInMinutes" value="60"/>
    <!-- Information about the previous sync run-->
    <add key="Sync.InitializionDone" value="false"/>
    <add key="Sync.LastCompleteSyncDateTime" value=""/>
  </appSettings>
</configuration>
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.MMSSync" />