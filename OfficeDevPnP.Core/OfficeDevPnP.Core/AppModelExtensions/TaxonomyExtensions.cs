using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
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
    public static class TaxonomyExtensions
    {
        private static Regex invalidDescriptionRegex = new Regex("[\t]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex invalidNameRegex = new Regex("[;\"<>|&\\t]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public const string TaxonomyGuidLabelDelimiter = "|";

        private static Regex TrimSpacesRegex = new Regex("\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #region Taxonomy Management

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

        private static string DenormalizeName(string name)
        {
            if (name == null)
                return (string)null;
            else
                return TrimSpacesRegex.Replace(name, " ").Replace('＆', '&').Replace('＂', '"');
        }

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
        /// Exports the full list of terms from all termsets in all termstores.
        /// </summary>
        /// <param name="includeId">if true, Ids of the the taxonomy items will be included</param>
        /// <param name="clientContext"></param>
        /// <param name="delimiter">if specified, this delimiter will be used. Notice that IDs will be delimited with ;# from the label</param>
        /// <returns></returns>
        public static List<string> ExportAllTerms(this Site site, bool includeId, string delimiter = "|")
        {
            var clientContext = site.Context;

            List<string> termsString = new List<string>();

            TaxonomySession taxonomySession = taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);

            clientContext.ExecuteQuery();

            TermStoreCollection termStores = taxonomySession.TermStores;
            clientContext.Load(termStores, t => t.IncludeWithDefaultProperties(s => s.Groups));
            clientContext.ExecuteQuery();
            foreach (TermStore termStore in termStores)
            {
                foreach (TermGroup termGroup in termStore.Groups)
                {
                    TermSetCollection termSets = termGroup.TermSets;
                    clientContext.Load(termSets, t => t.IncludeWithDefaultProperties(s => s.Terms));
                    clientContext.ExecuteQuery();
                    string termGroupName = DenormalizeName(termGroup.Name);
                    string groupPath = string.Format("{0}{1}", termGroupName, (includeId) ? string.Format(";#{0}", termGroup.Id.ToString()) : "");
                    foreach (TermSet set in termSets)
                    {
                        string setName = DenormalizeName(set.Name);
                        string termsetPath = string.Format("{0}{3}{1}{2}", groupPath, setName, (includeId) ? string.Format(";#{0}", set.Id.ToString()) : "", delimiter);
                        foreach (Term term in set.Terms)
                        {
                            string termName = DenormalizeName(term.Name);
                            string termPath = string.Format("{0}{3}{1}{2}", termsetPath, termName, (includeId) ? string.Format(";#{0}", term.Id.ToString()) : "", delimiter);
                            termsString.Add(termPath);

                            if (term.TermsCount > 0)
                            {
                                string subTermPath = string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termName, delimiter);

                                termsString.AddRange(ParseSubTerms(subTermPath, term, includeId, delimiter, clientContext));
                            }
                        }
                    }
                }
            }

            return termsString.Distinct().ToList<string>();
        }

        /// <summary>
        /// Exports the full list of terms from all termsets in all termstores.
        /// </summary>
        /// <param name="termSetId">The ID of the termset to export</param>
        /// <param name="includeId">if true, Ids of the the taxonomy items will be included</param>
        /// <param name="clientContext"></param>
        /// <param name="delimiter">if specified, this delimiter will be used. Notice that IDs will be delimited with ;# from the label</param>
        /// <returns></returns>
        public static List<string> ExportTermSet(this Site site, Guid termSetId, bool includeId, string delimiter = "|")
        {
            var clientContext = site.Context;
            List<string> termsString = new List<string>();
            TermCollection terms = null;
            TaxonomySession taxonomySession = taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);

            if (termSetId != Guid.Empty)
            {
                TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

                TermSet termSet = termStore.GetTermSet(termSetId);
                terms = termSet.Terms;
                clientContext.Load(terms, t => t.IncludeWithDefaultProperties(s => s.TermSet));
                clientContext.Load(terms, t => t.IncludeWithDefaultProperties(s => s.TermSet.Group));
            }

            clientContext.ExecuteQuery();

            if (terms != null)
            {
                foreach (Term term in terms)
                {
                    string groupName = DenormalizeName(term.TermSet.Group.Name);
                    string termsetName = DenormalizeName(term.TermSet.Name);
                    string termName = DenormalizeName(term.Name);
                    clientContext.ExecuteQuery();
                    string groupPath = string.Format("{0}{1}", groupName, (includeId) ? string.Format(";#{0}", term.TermSet.Group.Id.ToString()) : "");
                    string termsetPath = string.Format("{0}{1}", termsetName, (includeId) ? string.Format(";#{0}", term.TermSet.Id.ToString()) : "");
                    string termPath = string.Format("{0}{1}", termName, (includeId) ? string.Format(";#{0}", term.Id.ToString()) : "");
                    termsString.Add(string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter));

                    if (term.TermsCount > 0)
                    {
                        string subTermPath = string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termName, delimiter);

                        termsString.AddRange(ParseSubTerms(subTermPath, term, includeId, delimiter, clientContext));
                    }

                    termsString.Add(string.Format("{0}{3}{1}{3}{2}", groupPath, termsetPath, termPath, delimiter));
                }
            }

            return termsString.Distinct().ToList<string>();
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

        public static TaxonomySession GetTaxonomySession(this Site site)
        {
            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(site.Context);
            site.Context.Load(tSession);
            site.Context.ExecuteQuery();
            return tSession;
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

        public static void ImportTerms(this Site site, string[] termLines, int lcid, string delimiter)
        {
            var clientContext = site.Context;
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            clientContext.Load(termStore);
            clientContext.ExecuteQuery();
            foreach (string line in termLines)
            {
                // split up
                string[] items = line.Split(new string[] { delimiter }, StringSplitOptions.None);
                if (items.Count() > 0)
                {
                    string groupItem = items[0];
                    string groupName = groupItem;
                    Guid groupId = Guid.Empty;
                    if (groupItem.IndexOf(";#") > -1)
                    {
                        groupName = groupItem.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                        groupId = new Guid(groupItem.Split(new string[] { ";#" }, StringSplitOptions.None)[1]);
                    }
                    TermGroup termGroup = null;
                    if (groupId != Guid.Empty)
                    {
                        termGroup = termStore.GetGroup(groupId);
                    }
                    else
                    {
                        termGroup = termStore.Groups.GetByName(NormalizeName(groupName));
                    }
                    try
                    {
                        clientContext.Load(termGroup);
                        clientContext.ExecuteQuery();
                    }
                    catch
                    {

                    }
                    if (termGroup.ServerObjectIsNull == null)
                    {
                        groupId = Guid.NewGuid();
                        termGroup = termStore.CreateGroup(NormalizeName(groupName), groupId);
                        clientContext.Load(termGroup);
                        clientContext.ExecuteQuery();
                    }
                    if (items.Count() > 1)
                    {
                        // TermSet
                        if (termGroup.ServerObjectIsNull == false)
                        {
                            string termsetItem = items[1];
                            string termsetName = termsetItem;
                            Guid termsetId = Guid.Empty;
                            if (termsetItem.IndexOf(";#") > -1)
                            {
                                termsetName = termsetItem.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                                termsetId = new Guid(termsetItem.Split(new string[] { ";#" }, StringSplitOptions.None)[1]);
                            }
                            TermSet termSet = null;
                            if (termsetId != Guid.Empty)
                            {
                                termSet = termGroup.TermSets.GetById(termsetId);
                            }
                            else
                            {
                                termSet = termGroup.TermSets.GetByName(NormalizeName(termsetName));
                            }
                            clientContext.Load(termSet);
                            try
                            {
                                clientContext.ExecuteQuery();
                            }
                            catch { }
                            if (termSet.ServerObjectIsNull == null)
                            {
                                termsetId = Guid.NewGuid();
                                termSet = termGroup.CreateTermSet(NormalizeName(termsetName), termsetId, lcid);
                                clientContext.Load(termSet);
                                clientContext.ExecuteQuery();
                            }
                            if (items.Count() > 2)
                            {
                                // Term(s)

                                if (termSet.ServerObjectIsNull == false)
                                {
                                    string termItem = items[2];
                                    string termName = termItem;
                                    Guid termId = Guid.Empty;
                                    if (termItem.IndexOf(";#") > -1)
                                    {
                                        termName = termItem.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                                        termId = new Guid(termItem.Split(new string[] { ";#" }, StringSplitOptions.None)[1]);
                                    }
                                    Term term = null;
                                    if (termId != Guid.Empty)
                                    {
                                        term = termSet.Terms.GetById(termId);
                                    }
                                    else
                                    {
                                        term = termSet.Terms.GetByName(NormalizeName(termName));
                                    }
                                    clientContext.Load(term);
                                    try
                                    {
                                        clientContext.ExecuteQuery();
                                    }
                                    catch { }
                                    if (term.ServerObjectIsNull == null)
                                    {
                                        termId = Guid.NewGuid();
                                        term = termSet.CreateTerm(NormalizeName(termName), lcid, termId);
                                        clientContext.ExecuteQuery();
                                    }

                                    if (items.Count() > 3)
                                    {
                                        clientContext.Load(term);
                                        clientContext.ExecuteQuery();
                                        if (term.ServerObjectIsNull == false)
                                        {
                                            for (int q = 3; q < items.Count(); q++)
                                            {
                                                termName = items[q];
                                                termId = Guid.Empty;
                                                if (termItem.IndexOf(";#") > -1)
                                                {
                                                    termName = termItem.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                                                    termId = new Guid(termItem.Split(new string[] { ";#" }, StringSplitOptions.None)[1]);
                                                }
                                                term = term.AddTermToTerm(lcid, termName, termId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
        public static TermSet ImportTermSet(this TermGroup termGroup, string filePath, Guid termSetId, bool synchroniseDeletions = false, bool? termSetIsOpen = null, string termSetContact = null, string termSetOwner = null)
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
                throw new NotImplementedException("synchronise deletions not implemented yet");
                //ImportTermSetRemoveExtraTerms(termSet, importedTermIds);
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
                //try
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
                LoggingUtility.Internal.TraceVerbose("End ImportTermSet");
                return termSet;
            }
        }

        private static bool ImportTermSetLineImport(IList<string> entries, TermSet importTermSet, int lcid, int lineNumber, IDictionary<Guid, object> importedTermIds)
        {
            TermSetItem termSetItem = null;
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
                while (num < entries.Count - 5 && success)
                {
                    string termName = entries[5 + num];
                    Guid termId = Guid.Empty;
                    // Accept extended format of "Name|Guid", noting that | is not an allowed character in the term name
                    if (termName.Contains(TaxonomyGuidLabelDelimiter))
                    {
                        var split = termName.Split(new string[] { TaxonomyGuidLabelDelimiter }, StringSplitOptions.None);
                        termName = split[0];
                        termId = new Guid(split[1]);
                    }
                    if (string.IsNullOrEmpty(termName))
                    {
                        if (termCreated)
                        {
                            result = true;
                            break;
                        }
                        break;
                    }
                    else
                    {
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
                            termSetItem = importTermSet;
                        }
                        else
                        {
                            termSetItem = term;
                        }
                        term = null;
                        if (!termSetItem.IsObjectPropertyInstantiated("Terms"))
                        {
                            termSetItem.Context.Load(termSetItem, i => i.Terms.Include(t => t.Id, t => t.Name, t => t.Description, t => t.IsAvailableForTagging));
                            termSetItem.Context.ExecuteQuery();
                        }
                        foreach (Term current in termSetItem.Terms)
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
                        if (term == null && termSetItem != null)
                        {
                            if (termId == Guid.Empty)
                            {
                                termId = Guid.NewGuid();
                            }
                            LoggingUtility.Internal.TraceInformation((int)EventId.CreateTerm, CoreResources.TaxonomyExtension_CreateTerm01UnderParent2, termName, termId, termSetItem.Name);
                            term = termSetItem.CreateTerm(termName, lcid, termId);
                            termSetItem.Context.Load(term, t => t.Id, t => t.Name, t => t.Description, t => t.IsAvailableForTagging);
                            termSetItem.Context.ExecuteQuery();
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
                                //Diagnostics.TraceVerbose("Setting IsAvailableForTagging = {1} for term '{0}'.", term.Name, isAvailableForTagging);
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
                            if (!term.GetDescription(lcid).Equals(description))
                            {
                                //Diagnostics.TraceVerbose("Updating description for term '{0}'.", term.Name);
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
                        termSetItem.Context.ExecuteQuery();
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

        private static string NormalizeName(string name)
        {
            if (name == null)
                return (string)null;
            else
                return TrimSpacesRegex.Replace(name, " ").Replace('&', '＆').Replace('"', '＂');
        }

        private static List<string> ParseSubTerms(string subTermPath, Term term, bool includeId, string delimiter, ClientRuntimeContext clientContext)
        {
            List<string> items = new List<string>();
            if (term.ServerObjectIsNull == null || term.ServerObjectIsNull == false)
            {
                clientContext.Load(term.Terms);
                clientContext.ExecuteQuery();
            }

            foreach (Term subTerm in term.Terms)
            {
                //ClientResult<string> termName = TaxonomyItem.NormalizeName(clientContext, subTerm.Name);
                //clientContext.ExecuteQuery();
                string termName = DenormalizeName(subTerm.Name);
                string termPath = string.Format("{0}{3}{1}{2}", subTermPath, termName, (includeId) ? string.Format(";#{0}", subTerm.Id.ToString()) : "", delimiter);

                items.Add(termPath);

                if (term.TermsCount > 0)
                {
                    items.AddRange(ParseSubTerms(termPath, subTerm, includeId, delimiter, clientContext));
                }

            }
            return items;
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
        /// Can be used to create taxonomy field remotely to web. Associated to group and term set in the GetDefaultSiteCollectionTermStore 
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Unique Id for the taxonomy field</param>
        /// <param name="internalName">Internal Name of the field</param>
        /// <param name="displayName">Display name</param>
        /// <param name="group">Site column group</param>
        /// <param name="mmsGroupName">Taxonomy group </param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create a multi value field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            id.ValidateNotNullOrEmpty("id");
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            // Group can be emtpy
            mmsGroupName.ValidateNotNullOrEmpty("mmsGroupName");
            mmsTermSetName.ValidateNotNullOrEmpty("mmsTermSetName");

            TermStore termStore = GetDefaultTermStore(web);

            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");


            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            web.Context.Load(termStore);
            web.Context.Load(termSet);
            web.Context.ExecuteQuery();

            return web.CreateTaxonomyField(id, internalName, displayName, group, termSet, multiValue);
        }


        /// <summary>
        /// Can be used to create taxonomy field remotely to web.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Unique Id for the taxonomy field</param>
        /// <param name="internalName">Internal Name of the field</param>
        /// <param name="displayName">Display name</param>
        /// <param name="group">Site column group</param>
        /// <param name="termSet">Taxonomy Termset</param>
        /// <param name="multiValue">if true, create a multivalue taxonomy field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, TermSet termSet, bool multiValue = false)
        {
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            termSet.ValidateNotNullOrEmpty("termSet");

            try
            {
                var _field = web.CreateField(id, internalName, multiValue ? "TaxonomyFieldTypeMulti" : "TaxonomyFieldType", true, displayName, group, "ShowField=\"Term1033\"");

                WireUpTaxonomyField(web, _field, termSet, multiValue);
                _field.Update();

                web.Context.ExecuteQuery();

                return _field;
            }
            catch (Exception)
            {
                ///If there is an exception the hidden field might be present
                FieldCollection _fields = web.Fields;
                web.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                web.Context.ExecuteQuery();
                var _hiddenField = id.ToString().Replace("-", "");

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
        /// Can be used to create taxonomy field remotely in a list. Associated to group and term set in the GetDefaultSiteCollectionTermStore 
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="id">Unique Id for the taxonomy field</param>
        /// <param name="internalName">Internal Name of the field</param>
        /// <param name="displayName">Display name</param>
        /// <param name="group">Site column group</param>
        /// <param name="mmsGroupName">Taxonomy group </param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create multi value field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this List list, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            id.ValidateNotNullOrEmpty("id");
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            mmsGroupName.ValidateNotNullOrEmpty("mmsGroupName");
            mmsTermSetName.ValidateNotNullOrEmpty("mmsTermSetName");

            var clientContext = list.Context as ClientContext;
            TermStore termStore = clientContext.Site.GetDefaultSiteCollectionTermStore();


            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            list.Context.Load(termStore);
            list.Context.Load(termSet);
            list.Context.ExecuteQuery();

            return list.CreateTaxonomyField(id, internalName, displayName, group, termSet, multiValue);
        }

        /// <summary>
        /// Can be used to create taxonomy field remotely in a list. 
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="id">Unique Id for the taxonomy field</param>
        /// <param name="internalName">Internal Name of the field</param>
        /// <param name="displayName">Display name</param>
        /// <param name="group">Site column group</param>
        /// <param name="termSet">Taxonomy TermSet</param>
        /// <param name="multiValue">If true, create a multivalue field</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this List list, Guid id, string internalName, string displayName, string group, TermSet termSet, bool multiValue = false)
        {
            internalName.ValidateNotNullOrEmpty("internalName");
            displayName.ValidateNotNullOrEmpty("displayName");
            termSet.ValidateNotNullOrEmpty("termSet");

            try
            {
                var _field = list.CreateField(id, internalName, multiValue ? "TaxonomyFieldTypeMulti" : "TaxonomyFieldType", true, displayName, group, "ShowField=\"Term1033\"");

                WireUpTaxonomyField(list, _field, termSet, multiValue);
                _field.Update();

                list.Context.ExecuteQuery();

                return _field;
            }
            catch (Exception)
            {
                ///If there is an exception the hidden field might be present
                FieldCollection _fields = list.Fields;
                list.Context.Load(_fields, fc => fc.Include(f => f.Id, f => f.InternalName));
                list.Context.ExecuteQuery();
                var _hiddenField = id.ToString().Replace("-", "");

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
        public static void WireUpTaxonomyField(this Web web, Field field, string mmsGroupName, string mmsTermSetName, bool allowMultipleValues = false)
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

            WireUpTaxonomyField(web, field, termSet, allowMultipleValues);
        }

        public static void WireUpTaxonomyField(this Web web, Field field, TermSet termSet, bool allowMultipleValues = false)
        {
            var clientContext = termSet.Context as ClientContext;
            if (!termSet.IsPropertyAvailable("TermStore"))
            {
                clientContext.Load(termSet.TermStore);
                clientContext.ExecuteQuery();
            }
            // set the SSP ID and Term Set ID on the taxonomy field
            var taxField = web.Context.CastTo<TaxonomyField>(field);
            taxField.AllowMultipleValues = allowMultipleValues;
            taxField.SspId = termSet.TermStore.Id;
            taxField.TermSetId = termSet.Id;
            taxField.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this Web web, Guid id, string mmsGroupName, string mmsTermSetName, bool allowMultipleValues = false)
        {
            var field = web.Fields.GetById(id);
            web.Context.Load(field);
            web.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName, allowMultipleValues);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this List list, Field field, TermSet termSet, bool allowMultipleValues = false)
        {
            var clientContext = list.Context as ClientContext;

            if (!termSet.IsPropertyAvailable("TermStore"))
            {
                clientContext.Load(termSet.TermStore);
                clientContext.ExecuteQuery();
            }

            // set the SSP ID and Term Set ID on the taxonomy field
            var taxField = clientContext.CastTo<TaxonomyField>(field);
            taxField.SspId = termSet.TermStore.Id;
            taxField.TermSetId = termSet.Id;
            taxField.AllowMultipleValues = allowMultipleValues;
            taxField.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this List list, Field field, string mmsGroupName, string mmsTermSetName, bool allowMultipleValues = false)
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

            list.WireUpTaxonomyField(field, termSet, allowMultipleValues);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this List list, Guid id, string mmsGroupName, string mmsTermSetName, bool allowMultipleValues = false)
        {
            var clientContext = list.Context as ClientContext;
            var field = list.Fields.GetById(id);
            clientContext.Load(field);
            list.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName, allowMultipleValues);
        }

        /// <summary>
        /// Returns the Id for a term if present in the TaxonomyHiddenList. Otherwise returns -1;
        /// </summary>
        /// <param name="web"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static int GetWssIdForTerm(this Web web, Term term)
        {
            var list = web.GetListByUrl("Lists/TaxonomyHiddenList");
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
