using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    [System.Runtime.InteropServices.GuidAttribute("8A8AEA7A-7C25-4138-9C83-2584028868C5")]
    public static partial class TaxonomyExtensions
    {
        #region Taxonomy Management
        private static Regex TrimSpacesRegex = new Regex("\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex invalidDescriptionRegex = new Regex("[\t]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex invalidNameRegex = new Regex("[;\"<>|&\\t]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public const string TaxonomyGuidLabelDelimiter = "|";


        /// <summary>
        /// Ensures the named group exists, returning a reference to the group, and creating or updating as necessary.
        /// </summary>
        /// <param name="site">Site connected to the term store to use</param>
        /// <param name="groupName">Name of the term group</param>
        /// <param name="groupId">(Optional) ID of the group; if not provided the parameter is ignored, a random GUID is used if necessary to create the group, otherwise if the ID differs a warning is logged</param>
        /// <param name="groupDescription">(Optional) Description of the term group; if null or not provided the parameter is ignored, otherwise the group is updated as necessary to match the description; passing an empty string will clear the description</param>
        /// <returns>The required term group</returns>
        public static TermGroup EnsureTermGroup(this Site site, string groupName, Guid groupId = default(Guid), string groupDescription = null)
        {
            if (string.IsNullOrEmpty(groupName)) { throw new ArgumentNullException("groupName"); }

            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            var termStore = session.GetDefaultSiteCollectionTermStore();
            site.Context.Load(termStore, s => s.Name, s => s.Id);

            bool changed = false;
            TermGroup termGroup = null;
            groupName = NormalizeName(groupName);
            ValidateName(groupName, "groupName");

            // Find or create group
            IEnumerable<TermGroup> groups = site.Context.LoadQuery(termStore.Groups.Include(g => g.Name, g => g.Id, g => g.Description));
            site.Context.ExecuteQuery();
            if (groupId != Guid.Empty)
            {
                termGroup = groups.FirstOrDefault(g => g.Id == groupId);
            }
            if (termGroup == null)
            {
                termGroup = groups.FirstOrDefault(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase));
            }

            if (termGroup == null)
            {
                if (groupId == Guid.Empty)
                {
                    groupId = Guid.NewGuid();
                }
                LoggingUtility.Internal.TraceInformation((int)EventId.CreateTermGroup, CoreResources.TaxonomyExtension_CreateTermGroup0InStore1, groupName, termStore.Name);
                termGroup = termStore.CreateGroup(groupName, groupId);
                site.Context.Load(termGroup, g => g.Name, g => g.Id, g => g.Description);
                site.Context.ExecuteQuery();
            }
            else
            {
                // Check ID (if retrieved by name and ID is different)
                if (groupId != Guid.Empty && termGroup.Id != groupId)
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.ProvisionTaxonomyIdMismatch, CoreResources.TaxonomyExtension_TermGroup0Id1DoesNotMatchSpecifiedId2, termGroup.Name, termGroup.Id, groupId);
                }
            }
            // Apply name (if retrieved by ID and name has changed)
            if (!string.Equals(termGroup.Name, groupName))
            {
                termGroup.Name = groupName;
                changed = true;
            }
            // Apply description
            if (groupDescription != null && !string.Equals(termGroup.Description, groupDescription))
            {
                try
                {
                    ValidateDescription(groupDescription, "groupDescription");
                    termGroup.Description = groupDescription;
                    changed = true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.ProvisionTaxonomyUpdateException, ex, CoreResources.TaxonomyExtension_ExceptionUpdateDescriptionGroup01, termGroup.Name, termGroup.Id);
                    //errorMessage = string.Format("Error setting description for taxonomy group '{0}': {1}", termGroup.Name, ex);
                }
            }
            if (changed)
            {
                LoggingUtility.Internal.TraceVerbose("Updating term group");
                site.Context.ExecuteQuery();
                //termStore.CommitAll();
            }
            return termGroup;
        }

        /// <summary>
        /// Ensures the named term set exists, returning a reference to the set, and creating or updating as necessary.
        /// </summary>
        /// <param name="parentGroup">Group to check or create the term set in</param>
        /// <param name="termSetName">Name of the term set</param>
        /// <param name="termSetId">(Optional) ID of the term set; if not provided the parameter is ignored, a random GUID is used if necessary to create the term set, otherwise if the ID differs a warning is logged</param>
        /// <param name="lcid">(Optional) Default language of the term set; if not provided the default of the associate term store is used</param>
        /// <param name="description">(Optional) Description of the term set; if null or not provided the parameter is ignored, otherwise the term set is updated as necessary to match the description; passing an empty string will clear the description</param>
        /// <param name="isOpen">(Optional) Whether the term store is open for new term creation or not</param>
        /// <param name="contact">(Optional)</param>
        /// <param name="owner">(Optional)</param>
        /// <returns>The required term set</returns>
        public static TermSet EnsureTermSet(this TermGroup parentGroup, string termSetName, Guid termSetId = default(Guid), int? lcid = null, string description = null, bool? isOpen = null, string termSetContact = null, string termSetOwner = null)
        {
            bool changed = false;
            TermSet termSet = null;
            termSetName = NormalizeName(termSetName);
            ValidateName(termSetName, "termSetName");

            // Find or create term set
            parentGroup.Context.Load(parentGroup, g => g.Name, g => g.Id);
            IEnumerable<TermSet> termSets = parentGroup.Context.LoadQuery(parentGroup.TermSets.Include(g => g.Name, g => g.Id, g => g.Description, g => g.IsOpenForTermCreation, g => g.Contact, g => g.Owner));
            parentGroup.Context.ExecuteQuery();
            if (termSetId != Guid.Empty)
            {
                termSet = termSets.FirstOrDefault(s => s.Id == termSetId);
            }
            if (termSet == null)
            {
                termSet = termSets.FirstOrDefault(s => string.Equals(s.Name, termSetName, StringComparison.OrdinalIgnoreCase));
            }

            if (termSet == null)
            {
                if (termSetId == Guid.Empty)
                {
                    termSetId = Guid.NewGuid();
                }
                if (lcid.HasValue)
                {
                    var termStore = parentGroup.TermStore;
                    parentGroup.Context.Load(termStore, ts => ts.Languages);
                    parentGroup.Context.ExecuteQuery();
                    if (!termStore.Languages.Contains(lcid.Value))
                    {
                        termStore.AddLanguage(lcid.Value);
                    }
                }
                else
                {
                    var termStore = parentGroup.TermStore;
                    parentGroup.Context.Load(termStore, ts => ts.DefaultLanguage);
                    parentGroup.Context.ExecuteQuery();
                    lcid = termStore.DefaultLanguage;
                }
                LoggingUtility.Internal.TraceInformation((int)EventId.CreateTermSet, CoreResources.TaxonomyExtension_CreateTermSet0InGroup1, termSetName, parentGroup.Name);
                termSet = parentGroup.CreateTermSet(termSetName, termSetId, lcid.Value);
                parentGroup.Context.Load(termSet, g => g.Name, g => g.Id, g => g.Description, g => g.IsOpenForTermCreation, g => g.Contact, g => g.Owner);
                parentGroup.Context.ExecuteQuery();
            }
            else
            {
                if (termSetId != Guid.Empty && termSet.Id != termSetId)
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.ProvisionTaxonomyIdMismatch, CoreResources.TaxonomyExtension_TermSet0Id1DoesNotMatchSpecifiedId2, termSet.Name, termSet.Id, termSetId);
                }
            }
            // Apply name (if retrieved by ID and name has changed)
            if (!string.Equals(termSet.Name, termSetName))
            {
                termSet.Name = termSetName;
                changed = true;
            }
            // Apply description
            if (description != null && (termSet.Description != description))
            {
                try
                {
                    ValidateDescription(description, "termSetDescription");
                    termSet.Description = description;
                    changed = true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.Internal.TraceWarning((int)EventId.ProvisionTaxonomyUpdateException, ex, CoreResources.TaxonomyExtension_ExceptionUpdateDescriptionSet01, termSet.Name, termSet.Id);
                }
            }
            // Other settings
            if (isOpen.HasValue && (termSet.IsOpenForTermCreation != isOpen.Value))
            {
                termSet.IsOpenForTermCreation = isOpen.Value;
                changed = true;
            }
            if (termSetContact != null && termSet.Contact != termSetContact)
            {
                termSet.Contact = termSetContact;
                changed = true;
            }
            if (termSetOwner != null && termSet.Owner != termSetOwner)
            {
                termSet.Owner = termSetOwner;
                changed = true;
            }

            // TODO: Add Stakeholders
            //if (settings.EnvironmentSettings.TermSetStakeholder) {
            //    foreach (user in settings.EnvironmentSettings.TermSetStakeholder) {
            //        Write-Host "Adding term set stakeholder 'user'."
            //        termSet.AddStakeholder(user)
            //    }
            //}

            // Update (if changed)
            if (changed)
            {
                //Diagnostics.TraceVerbose("Committing term set creation");
                LoggingUtility.Internal.TraceVerbose("Updating term set");
                parentGroup.Context.ExecuteQuery();
            }
            return termSet;
        }

        /// <summary>
        /// Private method used for resolving taxonomy term set for taxonomy field
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns></returns>
        private static TermStore GetDefaultTermStore(Web web)
        {
            TermStore termStore = null;
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            web.Context.Load(taxonomySession,
                ts => ts.TermStores.Include(
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name
                        )
                    )
                );
            web.Context.ExecuteQuery();
            if (taxonomySession != null)
            {
                termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            }

            return termStore;
        }

        public static TaxonomySession GetTaxonomySession(this Site site)
        {
            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(site.Context);
            site.Context.Load(tSession);
            site.Context.ExecuteQuery();
            return tSession;
        }

        public static TermStore GetDefaultKeywordsTermStore(this Site site)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            var termStore = session.GetDefaultKeywordsTermStore();
            site.Context.Load(termStore);
            site.Context.ExecuteQuery();

            return termStore;
        }

        public static TermStore GetDefaultSiteCollectionTermStore(this Site site)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            var termStore = session.GetDefaultSiteCollectionTermStore();
            site.Context.Load(termStore);
            site.Context.ExecuteQuery();

            return termStore;
        }


        public static TermSetCollection GetTermSetsByName(this Site site, string name, int lcid = 1033)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            TermStore store = session.GetDefaultSiteCollectionTermStore();
            var termsets = store.GetTermSetsByName(name, lcid);
            site.Context.Load(termsets);
            site.Context.ExecuteQuery();
            return termsets;
        }


        public static TermGroup GetTermGroupByName(this Site site, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            var store = session.GetDefaultSiteCollectionTermStore();
            IEnumerable<TermGroup> groups = site.Context.LoadQuery(store.Groups.Include(g => g.Name, g => g.Id, g => g.TermSets)).Where(g => g.Name == name);
            site.Context.ExecuteQuery();
            return groups.FirstOrDefault();
        }

        public static TermGroup GetTermGroupById(this Site site, Guid termGroupId)
        {
            if (termGroupId == null || termGroupId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("termGroupId");
            }

            TaxonomySession session = TaxonomySession.GetTaxonomySession(site.Context);
            var store = session.GetDefaultSiteCollectionTermStore();
            IEnumerable<TermGroup> groups = site.Context.LoadQuery(store.Groups.Include(g => g.Name, g => g.Id, g => g.TermSets)).Where(g => g.Id == termGroupId);
            site.Context.ExecuteQuery();
            return groups.FirstOrDefault();
        }

        /// <summary>
        /// Gets a Taxonomy Term by Name
        /// </summary>
        /// <param name="termSetId"></param>
        /// <param name="term"></param>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public static Term GetTermByName(this Site site, Guid termSetId, string term)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentNullException("term");

            TermCollection termMatches = null;
            ExceptionHandlingScope scope = new ExceptionHandlingScope(site.Context);

            string termId = string.Empty;
            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(site.Context);
            TermStore ts = tSession.GetDefaultSiteCollectionTermStore();
            TermSet tset = ts.GetTermSet(termSetId);

            var lmi = new LabelMatchInformation(site.Context);

            lmi.Lcid = 1033;
            lmi.TrimUnavailable = true;
            lmi.TermLabel = term;

            termMatches = tset.GetTerms(lmi);
            site.Context.Load(tSession);
            site.Context.Load(ts);
            site.Context.Load(tset);
            site.Context.Load(termMatches);

            site.Context.ExecuteQuery();

            if (termMatches.AreItemsAvailable)
            {
                return termMatches.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public static Term AddTermToTermset(this Site site, Guid termSetId, string term)
        {
            return AddTermToTermset(site, termSetId, term, Guid.NewGuid());
        }

        public static Term AddTermToTermset(this Site site, Guid termSetId, string term, Guid termId)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentNullException("term");

            Term t = null;
            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(site.Context);
            TermStore ts = tSession.GetDefaultSiteCollectionTermStore();
            TermSet tset = ts.GetTermSet(termSetId);

            t = tset.CreateTerm(term, 1033, termId);
            //site.Context.Load(tSession);
            //site.Context.Load(ts);
            //site.Context.Load(tset);
            site.Context.Load(t);

            site.Context.ExecuteQuery();

            return t;
        }

        /// <summary>
        ///  Imports an array of | delimited strings into the deafult site collection termstore. Specify strings in this format:
        ///  TermGroup|TermSet|Term
        ///  
        ///  E.g. "Locations|Nordics|Sweden"
        ///  
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termLines"></param>
        /// <param name="lcid"></param>
        /// <param name="delimiter"></param>
        /// <param name="synchronizeDeletions">Remove tags that are not present in the import</param>
        public static void ImportTerms(this Site site, string[] termLines, int lcid, string delimiter = "|", bool synchronizeDeletions = false)
        {
            termLines.ValidateNotNullOrEmpty("termLines");

            var clientContext = site.Context;

            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

            ImportTerms(site, termLines, lcid, termStore, delimiter, synchronizeDeletions);
        }

        /// <summary>
        ///  Imports an array of | delimited strings into the deafult site collection termstore. Specify strings in this format:
        ///  TermGroup|TermSet|Term
        ///  
        ///  E.g. "Locations|Nordics|Sweden"
        ///  
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termLines"></param>
        /// <param name="lcid"></param>
        /// <param name="termStore">The termstore to import the terms into</param>
        /// <param name="delimiter"></param>
        /// <param name="synchronizeDeletions">Remove tags that are not present in the import</param>
        public static void ImportTerms(this Site site, string[] termLines, int lcid, TermStore termStore, string delimiter = "|", bool synchronizeDeletions = false)
        {
            var groupDict = new Dictionary<TermGroup, List<string>>();

            var clientContext = site.Context;
            if (termStore.ServerObjectIsNull == true)
            {
                clientContext.Load(termStore);
                clientContext.ExecuteQuery();
            }
            clientContext.Load(termStore);
            clientContext.ExecuteQuery();

            foreach (var line in termLines)
            {
                // Find termgroup
                var items = line.Split(new[] { delimiter }, StringSplitOptions.None);
                if (items.Any())
                {


                    List<string> terms = null;

                    var groupItem = items[0];
                    var groupName = groupItem;
                    var groupId = Guid.Empty;
                    if (groupItem.IndexOf(";#", StringComparison.Ordinal) > -1)
                    {
                        groupName = groupItem.Split(new[] { ";#" }, StringSplitOptions.None)[0];
                        groupId = new Guid(groupItem.Split(new[] { ";#" }, StringSplitOptions.None)[1]);
                    }
                    TermGroup termGroup = null;
                    // Cached?
                    if (groupDict.Any())
                    {
                        KeyValuePair<TermGroup, List<string>> groupDictItem;
                        if (groupId != Guid.Empty)
                        {
                            groupDictItem = groupDict.FirstOrDefault(tg => tg.Key.Id == groupId);

                            termGroup = groupDictItem.Key;
                            terms = groupDictItem.Value;

                        }
                        else
                        {
                            groupDictItem = groupDict.FirstOrDefault(tg => tg.Key.Name == groupName);

                            termGroup = groupDictItem.Key;
                            terms = groupDictItem.Value;
                        }
                    }
                    if (termGroup == null)
                    {
                        if (groupId != Guid.Empty)
                        {

                            termGroup = termStore.Groups.GetById(groupId);
                        }
                        else
                        {
                            termGroup = termStore.Groups.GetByName(NormalizeName(groupName));
                        }
                        try
                        {
                            clientContext.Load(termGroup);
                            clientContext.ExecuteQuery();
                            groupDict.Add(termGroup, new List<string>());
                            terms = new List<string>();
                        }
                        catch
                        {

                        }
                    }
                    if (termGroup.ServerObjectIsNull == null)
                    {
                        if (groupId == Guid.Empty)
                        {
                            groupId = Guid.NewGuid();
                        }
                        termGroup = termStore.CreateGroup(NormalizeName(groupName), groupId);
                        terms = new List<string>();
                        clientContext.Load(termGroup);
                        clientContext.ExecuteQuery();

                        groupDict.Add(termGroup, new List<string>());

                    }
                    var sb = new StringBuilder();
                    if (items.Length > 1)
                    {
                        var termSetName = items[1];
                        termSetName = termSetName.Replace(";#", "|");
                        sb.AppendFormat("{0},,{1},True,,", termSetName, lcid);

                        // Termset = position 1
                        for (var q = 0; q < 7; q++)
                        {
                            var item = "";
                            if (items.Length > q + 2)
                            {
                                item = items[q + 2];
                                item = item.Replace(";#", "|");
                            }
                            sb.AppendFormat("{0},", item);
                        }
                        if (terms != null)
                        {
                            terms.Add(sb.ToString());

                            groupDict[termGroup] = terms;
                        }
                    }
                }
            }
            foreach (var groupDictItem in groupDict)
            {
                var memoryStream = new MemoryStream();

                var termGroup = groupDictItem.Key as TermGroup;
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    // Header
                    streamWriter.WriteLine(@"""Term Set Name"",""Term Set Description"",""LCID"",""Available for Tagging"",""Term Description"",""Level 1 Term"",""Level 2 Term"",""Level 3 Term"",""Level 4 Term"",""Level 5 Term"",""Level 6 Term"",""Level 7 Term""");

                    // Items
                    foreach (var termLine in groupDictItem.Value)
                    {
                        streamWriter.WriteLine(termLine);
                    }
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    termGroup.ImportTermSet(memoryStream, synchroniseDeletions: synchronizeDeletions);
                }

            }
        }

      

        private static Term AddTermToTerm(this Term term, int lcid, string termLabel, Guid termId)
        {
            var clientContext = term.Context;
            if (term.ServerObjectIsNull == true)
            {
                clientContext.Load(term);
                clientContext.ExecuteQuery();
            }
            Term subTerm = null;
            if (termId != Guid.Empty)
            {
                subTerm = term.Terms.GetById(termId);
            }
            else
            {
                subTerm = term.Terms.GetByName(NormalizeName(termLabel));
            }
            clientContext.Load(term);
            try
            {
                clientContext.ExecuteQuery();
            }
            catch { }

            clientContext.Load(subTerm);
            try
            {
                clientContext.ExecuteQuery();
            }
            catch { }
            if (subTerm.ServerObjectIsNull == null)
            {
                if (termId == Guid.Empty) termId = Guid.NewGuid();
                subTerm = term.CreateTerm(NormalizeName(termLabel), lcid, termId);
                clientContext.Load(subTerm);
                clientContext.ExecuteQuery();
            }
            return subTerm;
        }

        /// <summary>
        /// Imports terms from a term set file, updating with any new terms, in the same format at that used by the web interface import ability.
        /// </summary>
        /// <param name="termGroup">Group to create the term set within</param>
        /// <param name="filePath">Local path to the file to import</param>
        /// <param name="termSetId">GUID to use for the term set; if Guid.Empty is passed then a random GUID is generated and used</param>
        /// <param name="synchroniseDeletions">(Optional) Whether to also synchronise deletions; that is, remove any terms not in the import file; default is no (false)</param>
        /// <param name="termSetIsOpen">(Optional) Whether the term set should be marked open; if not passed, then the existing setting is not changed</param>
        /// <param name="termSetContact">(Optional) Contact for the term set; if not provided, the existing setting is retained</param>
        /// <param name="termSetOwner">(Optional) Owner for the term set; if not provided, the existing setting is retained</param>
        /// <returns>The created, or updated, term set</returns>
        /// <remarks>
        /// <para>
        /// The format of the file is the same as that used by the import function in the 
        /// web interface. A sample file can be obtained from the web interface.
        /// </para>
        /// <para>
        /// This is a CSV file, with the following headings:
        /// </para>
        /// <para>
        /// <code>Term Set Name,Term Set Description,LCID,Available for Tagging,Term Description,Level 1 Term,Level 2 Term,Level 3 Term,Level 4 Term,Level 5 Term,Level 6 Term,Level 7 Term</code>
        /// </para>
        /// <para>
        /// The first data row must contain the Term Set Name, Term Set Description, and LCID, and should also contain the first term. 
        /// </para>
        /// <para>
        /// It is recommended that a fixed GUID be used as the termSetId, to allow the term set to be easily updated (so do not pass Guid.Empty).
        /// </para>
        /// <para>
        /// In contrast to the web interface import, this is not a one-off import but runs synchronisation logic allowing updating of an existing Term Set.
        /// When synchronising, any existing terms are matched (with Term Description and Available for Tagging updated as necessary),
        /// any new terms are added in the correct place in the hierarchy, and (if synchroniseDeletions is set) any terms not in the imported file 
        /// are removed.
        /// </para>
        /// <para>
        /// The import file also supports an expanded syntax for the Term Set Name and term names (Level 1 Term, Level 2 Term, etc).
        /// These columns support values with the format "Name|GUID", with the name and GUID separated by a pipe character (note that the pipe character is invalid to use within a taxomony item name).
        /// This expanded syntax is not required, but can be used to ensure all terms have fixed IDs.
        /// </para>
        /// </remarks>
        public static TermSet ImportTermSet(this TermGroup termGroup, string filePath, Guid termSetId = default(Guid), bool synchroniseDeletions = false, bool? termSetIsOpen = null, string termSetContact = null, string termSetOwner = null)
        {
            if (filePath == null) { throw new ArgumentNullException("filePath"); }
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException("File path is required.", "filePath"); }

            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                return ImportTermSet(termGroup, fs, termSetId, synchroniseDeletions, termSetIsOpen, termSetContact, termSetOwner);
            }
        }

        /// <summary>
        /// Imports terms from a term set stream, updating with any new terms, in the same format at that used by the web interface import ability.
        /// </summary>
        /// <param name="termGroup">Group to create the term set within</param>
        /// <param name="termSetData">Stream containing the data to import</param>
        /// <param name="termSetId">GUID to use for the term set; if Guid.Empty is passed then a random GUID is generated and used</param>
        /// <param name="synchroniseDeletions">(Optional) Whether to also synchronise deletions; that is, remove any terms not in the import file; default is no (false)</param>
        /// <param name="termSetIsOpen">(Optional) Whether the term set should be marked open; if not passed, then the existing setting is not changed</param>
        /// <param name="termSetContact">(Optional) Contact for the term set; if not provided, the existing setting is retained</param>
        /// <param name="termSetOwner">(Optional) Owner for the term set; if not provided, the existing setting is retained</param>
        /// <returns>The created, or updated, term set</returns>
        /// <remarks>
        /// <para>
        /// The format of the file is the same as that used by the import function in the 
        /// web interface. A sample file can be obtained from the web interface.
        /// </para>
        /// <para>
        /// This is a CSV file, with the following headings:
        /// </para>
        /// <para>
        /// <code>Term Set Name,Term Set Description,LCID,Available for Tagging,Term Description,Level 1 Term,Level 2 Term,Level 3 Term,Level 4 Term,Level 5 Term,Level 6 Term,Level 7 Term</code>
        /// </para>
        /// <para>
        /// The first data row must contain the Term Set Name, Term Set Description, and LCID, and should also contain the first term. 
        /// </para>
        /// <para>
        /// It is recommended that a fixed GUID be used as the termSetId, to allow the term set to be easily updated (so do not pass Guid.Empty).
        /// </para>
        /// <para>
        /// In contrast to the web interface import, this is not a one-off import but runs synchronisation logic allowing updating of an existing Term Set.
        /// When synchronising, any existing terms are matched (with Term Description and Available for Tagging updated as necessary),
        /// any new terms are added in the correct place in the hierarchy, and (if synchroniseDeletions is set) any terms not in the imported file 
        /// are removed.
        /// </para>
        /// <para>
        /// The import file also supports an expanded syntax for the Term Set Name and term names (Level 1 Term, Level 2 Term, etc).
        /// These columns support values with the format "Name|GUID", with the name and GUID separated by a pipe character (note that the pipe character is invalid to use within a taxomony item name).
        /// This expanded syntax is not required, but can be used to ensure all terms have fixed IDs.
        /// </para>
        /// </remarks>
        public static TermSet ImportTermSet(this TermGroup termGroup, Stream termSetData, Guid termSetId = default(Guid), bool synchroniseDeletions = false, bool? termSetIsOpen = null, string termSetContact = null, string termSetOwner = null)
        {
            if (termSetData == null) { throw new ArgumentNullException("termSetData"); }

            LoggingUtility.Internal.TraceInformation((int)EventId.ImportTermSet, CoreResources.TaxonomyExtension_ImportTermSet);

            TermSet termSet = null;
            bool allTermsAdded;
            var importedTermIds = new Dictionary<Guid, object>();
            using (var reader = new StreamReader(termSetData))
            {
                termSet = ImportTermSetImplementation(termGroup, reader, termSetId, importedTermIds, termSetIsOpen, termSetContact, termSetOwner, out allTermsAdded);
                //if (!string.IsNullOrEmpty(errorMessage))
                //{
                //    //Diagnostics.ErrorEvent(EventId.ProvisionErrorImportingTermSet, "Error adding term set '{0}': {1}", TermSetName, errorMessage);
                //}
            }

            if (synchroniseDeletions)
            {
                ImportTermSetRemoveExtraTerms(termSet, importedTermIds);
            }

            //termStore.CommitAll();
            //TaxonomySession.SyncHiddenList(site);
            return termSet;
        }

        private static TermSet ImportTermSetImplementation(this TermGroup parentGroup, TextReader reader, Guid termSetId, IDictionary<Guid, object> importedTermIds, bool? termSetIsOpen, string termSetContact, string termSetOwner, out bool allTermsAdded)
        {
            if (parentGroup == null)
            {
                throw new ArgumentNullException("parentGroup");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            LoggingUtility.Internal.TraceVerbose("Begin import term set");

            TermSet termSet = null;

            int lcid = 0;

            int lineIndex = -1;
            allTermsAdded = true;
            checked
            {
                try
                {
                    string rowText;
                    while ((rowText = reader.ReadLine()) != null)
                    {
                        lineIndex++;
                        if (lineIndex == 0)
                        {
                            // Check file look vaguely like a CSV -- ensure the first line (headers) has some commas:
                            if (!rowText.Contains(","))
                            {
                                throw new ArgumentException("Invalid CSV format; was expecting a comma in the first (header) line.", "reader");
                            }
                        }
                        else
                        {
                            // Process the second line (index=1), and then all non-blank lines
                            if (lineIndex <= 1 || !string.IsNullOrEmpty(rowText.Trim()))
                            {
                                var entries = ImportTermSetLineParse(rowText);
                                //lcid = this.GetImportLcid(termStore, lcid, lineIndex, entries);
                                if (termSet == null)
                                {
                                    if (lineIndex != 1)
                                    {
                                        throw new InvalidOperationException("Term set not created on first line.");
                                    }
                                    if (entries.Count > 0)
                                    {
                                        string termSetName = entries[0];
                                        // Accept extended format of "Name|Guid", noting that | is not an allowed character in the term name
                                        if (termSetName.Contains(TaxonomyGuidLabelDelimiter))
                                        {
                                            var split = termSetName.Split(new string[] { TaxonomyGuidLabelDelimiter }, StringSplitOptions.None);
                                            termSetName = split[0];
                                            termSetId = new Guid(split[1]);
                                        }
                                        string description = null;
                                        if (entries.Count > 1)
                                        {
                                            description = entries[1];
                                        }
                                        if (entries.Count > 2)
                                        {
                                            if (!Int32.TryParse(entries[2], NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                                                NumberFormatInfo.InvariantInfo, out lcid))
                                            {
                                                var termStore = parentGroup.TermStore;
                                                parentGroup.Context.Load(termStore, ts => ts.DefaultLanguage);
                                                parentGroup.Context.ExecuteQuery();
                                                lcid = termStore.DefaultLanguage;
                                            }
                                        }
                                        termSet = parentGroup.EnsureTermSet(termSetName, termSetId, lcid, description, termSetIsOpen, termSetContact, termSetOwner);
                                        //termStore.CommitAll();
                                    }
                                }
                                var termAdded = ImportTermSetLineImport(entries, termSet, lcid, lineIndex + 1, importedTermIds);
                                if (!termAdded)
                                {
                                    allTermsAdded = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        string.Format("Exception on line {0}: {1}", lineIndex + 1, ex.Message),
                        ex);
                }
                LoggingUtility.Internal.TraceVerbose("End ImportTermSet");
                return termSet;
            }
        }

        private static bool ImportTermSetLineImport(IList<string> entries, TermSet importTermSet, int lcid, int lineNumber, IDictionary<Guid, object> importedTermIds)
        {
            TermSetItem parentTermSetItem = null;
            Term term = null;
            int num = 0;
            bool success = true;
            bool result = false;
            bool termCreated = false;
            bool changed = false;
            if (entries == null || entries.Count <= 5)
            {
                return false;
            }
            num = 0;
            checked
            {
                string termName = null;
                Guid termId = Guid.Empty;
                // Find matching existing terms
                while (num < entries.Count - 5 && success)
                {
                    string termNameEntry = entries[5 + num];
                    if (string.IsNullOrEmpty(termNameEntry))
                    {
                        if (termCreated)
                        {
                            result = true;
                        }
                        break;
                    }
                    termName = null;
                    termId = Guid.Empty;
                    // Accept extended format of "Name|Guid", noting that | is not an allowed character in the term name
                    if (termNameEntry.Contains(TaxonomyGuidLabelDelimiter))
                    {
                        var split = termNameEntry.Split(new string[] { TaxonomyGuidLabelDelimiter }, StringSplitOptions.None);
                        termName = split[0];
                        termId = new Guid(split[1]);
                    }
                    else
                    {
                        termName = termNameEntry;
                    }
                    // Process the entry
                    if (termName.Length > 255)
                    {
                        termName = termName.Substring(0, 255);
                    }
                    termName = NormalizeName(termName);
                    try
                    {
                        ValidateName(termName, "name");
                    }
                    catch (ArgumentNullException)
                    {
                        LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorName, CoreResources.TaxonomyExtension_ImportErrorName0Line1, new object[]
						{
							termName,
							lineNumber
						});
                        success = false;
                        break;
                    }
                    catch (ArgumentException)
                    {
                        LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorName, CoreResources.TaxonomyExtension_ImportErrorName0Line1, new object[]
						{
							termName,
							lineNumber
						});
                        success = false;
                        break;
                    }
                    if (term == null)
                    {
                        parentTermSetItem = importTermSet;
                    }
                    else
                    {
                        parentTermSetItem = term;
                    }
                    term = null;
                    if (!parentTermSetItem.IsObjectPropertyInstantiated("Terms"))
                    {
                        parentTermSetItem.Context.Load(parentTermSetItem, i => i.Terms.Include(t => t.Id, t => t.Name, t => t.Description, t => t.IsAvailableForTagging));
                        parentTermSetItem.Context.ExecuteQuery();
                    }
                    foreach (Term current in parentTermSetItem.Terms)
                    {
                        if (termId != Guid.Empty && current.Id == termId)
                        {
                            term = current;
                            break;
                        }
                        if (current.Name == termName)
                        {
                            term = current;
                            break;
                        }
                    }
                    if (term == null && parentTermSetItem != null)
                    {
                        if (termId == Guid.Empty)
                        {
                            termId = Guid.NewGuid();
                        }
                        LoggingUtility.Internal.TraceInformation((int)EventId.CreateTerm, CoreResources.TaxonomyExtension_CreateTerm01UnderParent2, termName, termId, parentTermSetItem.Name);
                        term = parentTermSetItem.CreateTerm(termName, lcid, termId);
                        parentTermSetItem.Context.Load(parentTermSetItem, i => i.Terms.Include(t => t.Id, t => t.Name, t => t.Description, t => t.IsAvailableForTagging));
                        parentTermSetItem.Context.Load(term, t => t.Id, t => t.Name, t => t.Description, t => t.IsAvailableForTagging);
                        parentTermSetItem.Context.ExecuteQuery();
                        termCreated = true;
                        if (num == entries.Count - 5 - 1)
                        {
                            result = true;
                        }
                    }
                    if (term != null)
                    {
                        importedTermIds[term.Id] = null;
                    }
                    num++;
                }
                if (success && term != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(entries[3]))
                        {
                            var isAvailableForTagging = bool.Parse(entries[3]);
                            if (term.IsAvailableForTagging != isAvailableForTagging)
                            {
                                LoggingUtility.Internal.TraceVerbose("Setting IsAvailableForTagging = {1} for term '{0}'.", term.Name, isAvailableForTagging);
                                term.IsAvailableForTagging = isAvailableForTagging;
                                changed = true;
                            }
                        }
                        else
                        {
                            LoggingUtility.Internal.TraceVerbose("The available for tagging entry on line {0} is null or empty.", new object[]
							{
								lineNumber
							});
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorTagging, CoreResources.TaxonomyExtension_ImportErrorTaggingLine0, new object[]
						{
							lineNumber
						});
                        success = false;
                    }
                    catch (FormatException)
                    {
                        LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorTagging, CoreResources.TaxonomyExtension_ImportErrorTaggingLine0, new object[]
						{
							lineNumber
						});
                        success = false;
                    }
                    string description = entries[4];
                    if (description.Length > 1000)
                    {
                        description = description.Substring(0, 1000);
                    }
                    if (!string.IsNullOrEmpty(description))
                    {
                        try
                        {
                            ValidateDescription(description, "description");
                            if (!(term.Description == description))
                            {
                                LoggingUtility.Internal.TraceVerbose("Updating description for term '{0}'.", term.Name);
                                term.SetDescription(description, lcid);
                                changed = true;
                            }
                        }
                        catch (ArgumentException)
                        {
                            LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorDescription, CoreResources.TaxonomyExtension_ImportErrorDescription0Line1, new object[]
							{
								description,
								lineNumber
							});
                            success = false;
                        }
                    }
                    if (!(term.Name == termName))
                    {
                        LoggingUtility.Internal.TraceVerbose("Updating name for term '{0}'.", term.Name);
                        term.Name = termName;
                        changed = true;
                    }
                    if (!success)
                    {
                        result = false;
                        Guid id = term.Id;
                        try
                        {
                            LoggingUtility.Internal.TraceVerbose("Was an issue; deleting");
                            term.DeleteObject();
                            changed = true;
                        }
                        catch (Exception ex)
                        {
                            LoggingUtility.Internal.TraceError((int)EventId.ProvisionTaxonomyImportErrorDelete, ex, CoreResources.TaxonomyExtension_ImportErrorDeleteId0Line1, new object[]
							{
								id,
								lineNumber
							});
                        }
                    }
                    if (changed)
                    {
                        LoggingUtility.Internal.TraceVerbose("Updating term {0}", term.Id);
                        parentTermSetItem.Context.ExecuteQuery();
                    }
                }
                return result || changed;
            }
        }

        private static IList<string> ImportTermSetLineParse(string line)
        {
            List<string> entries = new List<string>();
            char[] lineChars = line.ToCharArray();
            string entry = string.Empty;
            bool flagInsideQuotes = false;
            int charIndex = 0;
            checked
            {
                while (charIndex < line.Length)
                {
                    if (flagInsideQuotes || !string.IsNullOrEmpty(entry)
                        || (!char.IsWhiteSpace(lineChars[charIndex]) && lineChars[charIndex] != '"'))
                    {
                        if (flagInsideQuotes && lineChars[charIndex] == '"'
                            && (charIndex + 1 >= line.Length || lineChars[charIndex + 1] == ','))
                        {
                            // End of quotes (and either end of line or next char is comma)
                            flagInsideQuotes = false;
                        }
                        else
                        {

                            if (flagInsideQuotes && lineChars[charIndex] == '"')
                            {
                                if (lineChars[charIndex + 1] != '"')
                                {
                                    // End of quotes  and next char is not a comma!
                                    return null;
                                }
                                // Doubled (escaped) quotes
                                charIndex++;
                            }
                            if (flagInsideQuotes || lineChars[charIndex] != ',')
                            {
                                entry += lineChars[charIndex];
                            }
                            else
                            {
                                entry = entry.Trim();
                                entries.Add(entry);
                                entry = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        if (lineChars[charIndex] == '"')
                        {
                            flagInsideQuotes = true;
                        }
                    }

                    charIndex++;
                }
                entry = entry.Trim();
                entries.Add(entry);

                return entries;
            }
        }

        private static void ImportTermSetRemoveExtraTerms(TermSet termSet, IDictionary<Guid, object> importedTermIds)
        {
            LoggingUtility.Internal.TraceVerbose("Removing extra terms");
            var termsToDelete = new List<Term>();
            var allTerms = termSet.GetAllTerms();
            termSet.Context.Load(allTerms, at => at.Include(t => t.Id, t => t.Name));
            termSet.Context.ExecuteQuery();
            foreach (var term in allTerms)
            {
                if (!importedTermIds.ContainsKey(term.Id))
                {
                    termsToDelete.Add(term);
                }
            }
            foreach (var termToDelete in termsToDelete)
            {
                try
                {
                    LoggingUtility.Internal.TraceInformation((int)EventId.DeleteTerm, CoreResources.TaxonomyExtension_DeleteTerm01, termToDelete.Name, termToDelete.Id);
                    termToDelete.DeleteObject();
                    termSet.Context.ExecuteQuery();
                }
                catch (ServerException ex)
                {
                    if (ex.Message.StartsWith("Taxonomy item instantiation failed."))
                    {
                        // This is a sucky way to check if the term was already deleted
                        LoggingUtility.Internal.TraceVerbose("Term id {0} already deleted.", termToDelete.Id);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Exports the full list of terms from all termsets in all termstores.
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <param name="termSetId">The ID of the termset to export</param>
        /// <param name="includeId">if true, Ids of the the taxonomy items will be included</param>
        /// <param name="delimiter">if specified, this delimiter will be used. Notice that IDs will be delimited with ;# from the label</param>
        /// <returns></returns>
        public static List<string> ExportTermSet(this Site site, Guid termSetId, bool includeId, string delimiter = "|")
        {
            var clientContext = site.Context;
            TaxonomySession taxonomySession = taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);

            var termStore = site.GetDefaultSiteCollectionTermStore();

            return ExportTermSet(site, termSetId, includeId, termStore, delimiter);
        }

        /// <summary>
        /// Exports the full list of terms from all termsets in all termstores.
        /// </summary>
        /// <param name="site">The site to export the termsets from</param>
        /// <param name="termSetId">The ID of the termset to export</param>
        /// <param name="includeId">if true, Ids of the the taxonomy items will be included</param>
        /// <param name="termStore">The term store to export the termset from</param>
        /// <param name="delimiter">if specified, this delimiter will be used. Notice that IDs will be delimited with ;# from the label</param>
        /// <returns></returns>
        public static List<string> ExportTermSet(this Site site, Guid termSetId, bool includeId, TermStore termStore, string delimiter = "|")
        {
            var clientContext = site.Context;
            var termsString = new List<string>();
            TermCollection terms = null;

            if (termSetId != Guid.Empty)
            {
                var termSet = termStore.GetTermSet(termSetId);
                terms = termSet.Terms;
                clientContext.Load(terms, t => t.IncludeWithDefaultProperties(s => s.TermSet), t => t.IncludeWithDefaultProperties(s => s.TermSet.Group));
            }

            clientContext.ExecuteQuery();

            if (terms.Any())
            {
                foreach (var term in terms)
                {
                    var groupName = DenormalizeName(term.TermSet.Group.Name);
                    var termsetName = DenormalizeName(term.TermSet.Name);
                    var termName = DenormalizeName(term.Name);
                    clientContext.ExecuteQuery();
                    var groupPath = string.Format("{0}{1}", groupName, (includeId) ? string.Format(";#{0}", term.TermSet.Group.Id.ToString()) : "");
                    var termsetPath = string.Format("{0}{1}", termsetName, (includeId) ? string.Format(";#{0}", term.TermSet.Id.ToString()) : "");
                    var termPath = string.Format("{0}{1}", termName, (includeId) ? string.Format(";#{0}", term.Id.ToString()) : "");
                    termsString.Add(string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter));

                    if (term.TermsCount > 0)
                    {
                        var subTermPath = string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter);

                        termsString.AddRange(ParseSubTerms(subTermPath, term, includeId, delimiter, clientContext));
                    }

                    termsString.Add(string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter));
                }
            }

            return termsString.Distinct().ToList<string>();
        }

        /// <summary>
        /// Exports the full list of terms from all termsets in all termstores.
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <param name="includeId">if true, Ids of the the taxonomy items will be included</param>
        /// <param name="delimiter">if specified, this delimiter will be used. Notice that IDs will be delimited with ;# from the label</param>
        /// <returns></returns>
        public static List<string> ExportAllTerms(this Site site, bool includeId, string delimiter = "|")
        {
            var clientContext = site.Context;

            var termsString = new List<string>();

            TaxonomySession taxonomySession = taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);

            clientContext.ExecuteQuery();

            var termStores = taxonomySession.TermStores;
            clientContext.Load(termStores, t => t.IncludeWithDefaultProperties(s => s.Groups));
            clientContext.ExecuteQuery();
            foreach (var termStore in termStores)
            {
                foreach (var termGroup in termStore.Groups)
                {
                    var termSets = termGroup.TermSets;
                    clientContext.Load(termSets, t => t.IncludeWithDefaultProperties(s => s.Terms));
                    clientContext.ExecuteQuery();
                    var termGroupName = DenormalizeName(termGroup.Name);
                    var groupPath = string.Format("{0}{1}", termGroupName, (includeId) ? string.Format(";#{0}", termGroup.Id.ToString()) : "");
                    foreach (var set in termSets)
                    {
                        var setName = DenormalizeName(set.Name);
                        var termsetPath = string.Format("{0}{3}{1}{2}", groupPath, setName, (includeId) ? string.Format(";#{0}", set.Id.ToString()) : "", delimiter);
                        foreach (var term in set.Terms)
                        {
                            var termName = DenormalizeName(term.Name);
                            var termPath = string.Format("{0}{3}{1}{2}", termsetPath, termName, (includeId) ? string.Format(";#{0}", term.Id.ToString()) : "", delimiter);
                            termsString.Add(termPath);

                            if (term.TermsCount > 0)
                            {
                                var subTermPath = string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter);

                                termsString.AddRange(ParseSubTerms(subTermPath, term, includeId, delimiter, clientContext));
                            }
                        }
                    }
                }
            }

            return termsString.Distinct().ToList<string>();
        }

        private static List<string> ParseSubTerms(string subTermPath, Term term, bool includeId, string delimiter, ClientRuntimeContext clientContext)
        {
            var items = new List<string>();
            if (term.ServerObjectIsNull == null || term.ServerObjectIsNull == false)
            {
                clientContext.Load(term.Terms);
                clientContext.ExecuteQuery();
            }

            foreach (var subTerm in term.Terms)
            {
                var termName = DenormalizeName(subTerm.Name);
                var termPath = string.Format("{0}{3}{1}{2}", subTermPath, termName, (includeId) ? string.Format(";#{0}", subTerm.Id.ToString()) : "", delimiter);

                items.Add(termPath);

                if (term.TermsCount > 0)
                {
                    items.AddRange(ParseSubTerms(termPath, subTerm, includeId, delimiter, clientContext));
                }

            }
            return items;
        }

        private static string NormalizeName(string name)
        {
            if (name == null)
                return (string)null;
            else
                return TrimSpacesRegex.Replace(name, " ").Replace('&', '＆').Replace('"', '＂');
        }

        private static string DenormalizeName(string name)
        {
            if (name == null)
                return (string)null;
            else
                return TrimSpacesRegex.Replace(name, " ").Replace('＆', '&').Replace('＂', '"');
        }

        public static TaxonomyItem GetTaxonomyItemByPath(this Site site, string path, string delimiter = "|")
        {
            var context = site.Context;

            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            var pathSplit = path.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(context);
            TermStore ts = tSession.GetDefaultKeywordsTermStore();

            var groups = context.LoadQuery(ts.Groups);
            context.ExecuteQuery();

            var group = groups.FirstOrDefault(l => l.Name.Equals(pathSplit[0], StringComparison.CurrentCultureIgnoreCase));
            if (group == null) return null;
            if (pathSplit.Length == 1) return group;

            var termSets = context.LoadQuery(group.TermSets);
            context.ExecuteQuery();

            var termSet = termSets.FirstOrDefault(l => l.Name.Equals(pathSplit[1], StringComparison.CurrentCultureIgnoreCase));
            if (termSet == null) return null;
            if (pathSplit.Length == 2) return termSet;

            Term term = null;
            for (int i = 2; i < pathSplit.Length; i++)
            {
                IEnumerable<Term> termColl = context.LoadQuery(i == 2 ? termSet.Terms : term.Terms);
                context.ExecuteQuery();

                term = termColl.FirstOrDefault(l => l.Name.Equals(pathSplit[i], StringComparison.OrdinalIgnoreCase));

                if (term == null) return null;
            }

            return term;
        }

        private static void ValidateDescription(string description, string parameterName)
        {
            if (string.IsNullOrEmpty(description))
            {
                return;
            }
            if (invalidDescriptionRegex.IsMatch(description))
            {
                throw new ArgumentException(string.Format("Invalid characters in description '{0}'.", new object[]
				{
					description
				}), parameterName);
            }
            if (description.Length > 1000)
            {
                throw new ArgumentException(string.Format("Description exceeds maximum length (1000): '{0}'.", new object[]
				{
					description
				}), parameterName);
            }
        }

        private static void ValidateName(string name, string parameterName)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(parameterName); }

            if (name.Length > 255 || invalidNameRegex.IsMatch(name))
            {
                throw new ArgumentException(string.Format("Invalid taxonomy name '{0}'.", new object[]
				{
					name
				}), parameterName);
            }
        }

        #endregion

        #region Fields
        /// <summary>
        /// Sets a value in a taxonomy field
        /// </summary>
        /// <param name="item">The item to set the value to</param>
        /// <param name="TermPath">The path of the term in the shape of "TermGroupName|TermSetName|TermName"</param>
        /// <param name="fieldId">The id of the field</param>
        /// <exception cref="KeyNotFoundException"/>
        public static void SetTaxonomyFieldValueByTermPath(this ListItem item, string TermPath, Guid fieldId)
        {
            var clientContext = item.Context as ClientContext;
            TaxonomyItem taxItem = clientContext.Site.GetTaxonomyItemByPath(TermPath);
            if (taxItem != null)
            {
                item.SetTaxonomyFieldValue(fieldId, taxItem.Name, taxItem.Id);
            }
            else
            {
                throw new KeyNotFoundException("Taxonomy Term not found");
            }
        }

        public static void SetTaxonomyFieldValue(this ListItem item, Guid fieldId, string label, Guid termGuid)
        {
            ClientContext clientContext = item.Context as ClientContext;

            List list = item.ParentList;

            clientContext.Load(list);
            clientContext.ExecuteQuery();

            IEnumerable<Field> fieldQuery = clientContext.LoadQuery(
              list.Fields
              .Include(
                fieldArg => fieldArg.TypeAsString,
                fieldArg => fieldArg.Id,
                fieldArg => fieldArg.InternalName
              )
            ).Where(fieldArg => fieldArg.Id == fieldId);

            clientContext.ExecuteQuery();

            TaxonomyField taxField = fieldQuery.Cast<TaxonomyField>().FirstOrDefault();

            clientContext.Load(taxField);
            clientContext.ExecuteQuery();

            TaxonomyFieldValue fieldValue = new TaxonomyFieldValue();
            fieldValue.Label = label;
            fieldValue.TermGuid = termGuid.ToString();
            fieldValue.WssId = -1;
            taxField.SetFieldValueByValue(item, fieldValue);
            item.Update();
            clientContext.ExecuteQuery();
        }




        /// <summary>
        /// Can be used to create taxonomy field remotely to web.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="fieldCreationInformation">Creation Information of the field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this Web web, TaxonomyFieldCreationInformation fieldCreationInformation)
        {
            fieldCreationInformation.InternalName.ValidateNotNullOrEmpty("internalName");
            fieldCreationInformation.DisplayName.ValidateNotNullOrEmpty("displayName");
            fieldCreationInformation.TaxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            if (fieldCreationInformation.Id == Guid.Empty)
            {
                fieldCreationInformation.Id = Guid.NewGuid();
            }

            try
            {
                List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
                additionalAttributes.Add(new KeyValuePair<string, string>("ShowField", "Term1033"));

                var _field = web.CreateField(fieldCreationInformation);

                WireUpTaxonomyFieldInternal(_field, fieldCreationInformation.TaxonomyItem, fieldCreationInformation.MultiValue);
                _field.Update();

                web.Context.ExecuteQuery();

                return _field;
            }
            catch (Exception)
            {
                // If there is an exception the hidden field might be present
                FieldCollection _fields = web.Fields;
                web.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                web.Context.ExecuteQuery();
                var _hiddenField = fieldCreationInformation.Id.ToString().Replace("-", "");

                var _field = _fields.FirstOrDefault(f => f.InternalName == _hiddenField);
                if (_field != null)
                {
                    _field.DeleteObject();
                    web.Context.ExecuteQuery();
                }
                throw;

            }

        }





        /// <summary>
        /// Can be used to create taxonomy field remotely in a list. 
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="fieldCreationInformation">Creation information of the field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this List list, TaxonomyFieldCreationInformation fieldCreationInformation)
        {
            fieldCreationInformation.InternalName.ValidateNotNullOrEmpty("internalName");
            fieldCreationInformation.DisplayName.ValidateNotNullOrEmpty("displayName");
            fieldCreationInformation.TaxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            if (fieldCreationInformation.Id == Guid.Empty)
            {
                fieldCreationInformation.Id = Guid.NewGuid();
            }
            try
            {
                List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
                additionalAttributes.Add(new KeyValuePair<string, string>("ShowField", "Term1033"));

                var _field = list.CreateField(fieldCreationInformation);

                WireUpTaxonomyFieldInternal(_field, fieldCreationInformation.TaxonomyItem, fieldCreationInformation.MultiValue);
                _field.Update();

                list.Context.ExecuteQuery();

                return _field;
            }
            catch (Exception)
            {
                // If there is an exception the hidden field might be present
                FieldCollection _fields = list.Fields;
                list.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                list.Context.ExecuteQuery();
                var _hiddenField = fieldCreationInformation.Id.ToString().Replace("-", "");

                var _field = _fields.FirstOrDefault(f => f.InternalName == _hiddenField);
                if (_field != null)
                {
                    _field.Hidden = false; // Cannot delete a hidden column
                    _field.Update();
                    _field.DeleteObject();
                    list.Context.ExecuteQuery();
                }
                throw;
            }
        }


        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create a multivalue field</param>
        public static void WireUpTaxonomyField(this Web web, Field field, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            TermStore termStore = GetDefaultTermStore(web);

            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            if (string.IsNullOrEmpty(mmsTermSetName))
                throw new ArgumentNullException("mmsTermSetName", "The MMS term set is not specified.");

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            web.Context.Load(termStore);
            web.Context.Load(termSet);
            web.Context.ExecuteQuery();

            WireUpTaxonomyField(web, field, termSet, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="termSet">Taxonomy TermSet</param>
        /// <param name="multiValue">If true, create a multivalue field</param>
        public static void WireUpTaxonomyField(this Web web, Field field, TermSet termSet, bool multiValue = false)
        {
            WireUpTaxonomyFieldInternal(field, termSet, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="anchorTerm">Taxonomy Term</param>
        /// <param name="multiValue">If true, create a multivalue field</param>
        public static void WireUpTaxonomyField(this Web web, Field field, Term anchorTerm, bool multiValue = false)
        {
            WireUpTaxonomyFieldInternal(field, anchorTerm, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create a multivalue field</param>
        public static void WireUpTaxonomyField(this Web web, Guid id, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            var field = web.Fields.GetById(id);
            web.Context.Load(field);
            web.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="termSet">Taxonomy TermSet</param>
        /// <param name="multiValue">Term set name</param>
        public static void WireUpTaxonomyField(this List list, Field field, TermSet termSet, bool multiValue = false)
        {
            WireUpTaxonomyFieldInternal(field, termSet, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="anchorTerm">Taxonomy Term</param>
        /// <param name="multiValue">Allow multiple selection</param>
        public static void WireUpTaxonomyField(this List list, Field field, Term anchorTerm, bool multiValue = false)
        {
            WireUpTaxonomyFieldInternal(field, anchorTerm, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">Allow multiple selection</param>
        public static void WireUpTaxonomyField(this List list, Field field, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            var clientContext = list.Context as ClientContext;
            TermStore termStore = clientContext.Site.GetDefaultSiteCollectionTermStore();

            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            if (string.IsNullOrEmpty(mmsTermSetName))
                throw new ArgumentNullException("mmsTermSetName", "The MMS term set is not specified.");

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            clientContext.Load(termStore);
            clientContext.Load(termSet);
            clientContext.ExecuteQuery();

            list.WireUpTaxonomyField(field, termSet, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">Allow multiple selection</param>
        public static void WireUpTaxonomyField(this List list, Guid id, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            var clientContext = list.Context as ClientContext;
            var field = list.Fields.GetById(id);
            clientContext.Load(field);
            list.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set or term.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="taxonomyItem">Taxonomy TermSet or Term</param>
        /// <param name="multiValue">Allow multiple selection</param>
        private static void WireUpTaxonomyFieldInternal(Field field, TaxonomyItem taxonomyItem, bool multiValue)
        {
            var clientContext = field.Context as ClientContext;

            taxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            var anchorTerm = taxonomyItem as Term;

            if (anchorTerm != default(Term) && !anchorTerm.IsPropertyAvailable("TermSet"))
            {
                clientContext.Load(anchorTerm.TermSet);
                clientContext.ExecuteQuery();
            }

            var termSet = taxonomyItem is Term ? anchorTerm.TermSet : taxonomyItem as TermSet;

            if (termSet == default(TermSet))
                throw new ArgumentException("Bound TaxonomyItem must be either a TermSet or a Term");

            if (!termSet.IsPropertyAvailable("TermStore"))
            {
                clientContext.Load(termSet.TermStore);
                clientContext.ExecuteQuery();
            }

            // set the SSP ID and Term Set ID on the taxonomy field
            var taxField = clientContext.CastTo<TaxonomyField>(field);
            taxField.SspId = termSet.TermStore.Id;
            taxField.TermSetId = termSet.Id;

            if (anchorTerm != default(Term))
            {
                taxField.AnchorId = anchorTerm.Id;
            }

            taxField.AllowMultipleValues = multiValue;
            taxField.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Returns the Id for a term if present in the TaxonomyHiddenList. Otherwise returns -1;
        /// </summary>
        /// <param name="web"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static int GetWssIdForTerm(this Web web, Term term)
        {
            var clientContext = web.Context as ClientContext;
            var list = clientContext.Site.RootWeb.GetListByUrl("Lists/TaxonomyHiddenList");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"<View><Query><Where><Eq><FieldRef Name='IdForTerm' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", term.Id);

            var items = list.GetItems(camlQuery);
            web.Context.Load(items);
            web.Context.ExecuteQuery();

            if (items.Any())
            {
                return items[0].Id;
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }
}
