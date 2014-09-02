using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static class TaxonomyExtensions
    {
        #region Taxonomy Management
        private static Regex TrimSpacesRegex = new Regex("\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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


        #endregion

        #region Fields

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
                throw new Exception("Taxonomy Term not found.");
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

            if (string.IsNullOrEmpty(mmsGroupName))
            {
                throw (mmsGroupName == null)
                  ? new ArgumentNullException("mmsGroupName")
                  : new ArgumentException("Argument empty", "mmsGroup");
            }
            if (string.IsNullOrEmpty(mmsTermSetName))
                throw new ArgumentNullException("mmsTermSetName", "The MMS term set is not specified.");

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
            var clientContext = list.Context as ClientContext;
            TermStore termStore = clientContext.Site.GetDefaultSiteCollectionTermStore();

         
            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            if (string.IsNullOrEmpty(mmsTermSetName))
            {
                throw (mmsTermSetName == null)
                  ? new ArgumentNullException("mmsTermSetName")
                  : new ArgumentException(Constants.EXCEPTION_MSG_EMPTYSTRING_ARG, "mmsTermSetName");
            }

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
        #endregion
    }
}
