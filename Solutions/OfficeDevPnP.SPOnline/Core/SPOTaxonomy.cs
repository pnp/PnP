using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOTaxonomy
    {
        internal static Regex trimSpacesRegex = new Regex("\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static TaxonomySession GetTaxonomySession(ClientContext clientContext)
        {
            TaxonomySession tSession = TaxonomySession.GetTaxonomySession(clientContext);
            clientContext.ExecuteQuery();
            return tSession;
        }

        public static TermStore GetDefaultKeywordsTermStore(ClientContext clientContext)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
            var termStore = session.GetDefaultKeywordsTermStore();
            clientContext.Load(termStore);
            clientContext.ExecuteQuery();

            return termStore;
        }

        public static TermStore GetDefaultSiteCollectionTermStore(ClientContext clientContext)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
            var termStore = session.GetDefaultSiteCollectionTermStore();
            clientContext.Load(termStore);
            clientContext.ExecuteQuery();

            return termStore;
        }


        public static TermSetCollection GetTermSetsByName(string name, int lcid, ClientContext clientContext)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
            TermStore store = session.GetDefaultKeywordsTermStore();
            var termsets = store.GetTermSetsByName(name, lcid);
            clientContext.Load(termsets);
            clientContext.ExecuteQuery();
            return termsets;
        }


        public static TermGroup GetTermGroupByName(string name, ClientContext clientContext)
        {
            TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
            var store = session.GetDefaultSiteCollectionTermStore();
            IEnumerable<TermGroup> groups = clientContext.LoadQuery(store.Groups.Include(g => g.Name, g => g.Id, g => g.TermSets)).Where(g => g.Name == name);
            clientContext.ExecuteQuery();
            return groups.FirstOrDefault();
        }
        /// <summary>
        /// Gets a Taxonomy Term by Name
        /// </summary>
        /// <param name="termSetId"></param>
        /// <param name="term"></param>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public static Term GetTermByName(Guid termSetId, string term, ClientContext clientContext)
        {

            TermCollection termMatches = null;
            ExceptionHandlingScope scope = new ExceptionHandlingScope(clientContext);

            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    string termId = string.Empty;
                    TaxonomySession tSession = TaxonomySession.GetTaxonomySession(clientContext);
                    TermStore ts = tSession.GetDefaultSiteCollectionTermStore();
                    TermSet tset = ts.GetTermSet(termSetId);

                    var lmi = new LabelMatchInformation(clientContext);

                    lmi.Lcid = 1033;
                    lmi.TrimUnavailable = true;
                    lmi.TermLabel = term;

                    termMatches = tset.GetTerms(lmi);
                    clientContext.Load(tSession);
                    clientContext.Load(ts);
                    clientContext.Load(tset);
                    clientContext.Load(termMatches);
                }
                using (scope.StartCatch())
                {
                    if (scope.HasException)
                    {
                        return null;
                    }
                }
            }
            clientContext.ExecuteQuery();

            if (termMatches.AreItemsAvailable)
                return termMatches.FirstOrDefault();

            return null;

        }

        public static Term AddTermToTermset(Guid termSetId, string term, ClientContext clientContext)
        {
            return AddTermToTermset(termSetId, term, Guid.NewGuid(), clientContext);
        }

        public static Term AddTermToTermset(Guid termSetId, string term, Guid termId, ClientContext clientContext)
        {
            Term t = null;
            var scope = new ExceptionHandlingScope(clientContext);
            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    TaxonomySession tSession = TaxonomySession.GetTaxonomySession(clientContext);
                    TermStore ts = tSession.GetDefaultSiteCollectionTermStore();
                    TermSet tset = ts.GetTermSet(termSetId);

                    t = tset.CreateTerm(term, 1033, termId);
                    clientContext.Load(tSession);
                    clientContext.Load(ts);
                    clientContext.Load(tset);
                    clientContext.Load(t);
                }
                using (scope.StartCatch())
                {
                    if (scope.HasException)
                    {
                        return null;
                    }
                }
            }
            clientContext.ExecuteQuery();

            return t;
        }

        public static void ImportTerms(string[] termLines, int lcid, string delimiter, ClientContext clientContext)
        {
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
                                                term = AddTermToTerm(term, lcid, termName, termId, clientContext);
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

        private static Term AddTermToTerm(Term term, int lcid, string termLabel, Guid termId, ClientContext clientContext)
        {
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
        public static List<string> ExportTermSet(Guid termSetId, bool includeId, ClientContext clientContext,string delimiter = "|")
        {
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
        public static List<string> ExportAllTerms(bool includeId, ClientContext clientContext, string delimiter = "|")
        {
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

        private static List<string> ParseSubTerms(string subTermPath, Term term, bool includeId, string delimiter, ClientContext clientContext)
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
                return SPOTaxonomy.trimSpacesRegex.Replace(name, " ").Replace('&', '＆').Replace('"', '＂');
        }

        private static string DenormalizeName(string name)
        {
            if (name == null)
                return (string)null;
            else
                return SPOTaxonomy.trimSpacesRegex.Replace(name, " ").Replace('＆', '&').Replace('＂', '"');
        }

        public static TaxonomyItem GetTaxonomyItemByPath(string path, ClientContext context)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            var pathSplit = path.Split('|');

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

        public static void SetTaxonomyFieldValueByTermPath(ListItem item, string TermPath, Guid fieldId, Web web)
        {
            TaxonomyItem taxItem = GetTaxonomyItemByPath(TermPath, web.Context as ClientContext);
            SetTaxonomyFieldValue(item, fieldId, taxItem.Name, taxItem.Id, web);
        }

        public static void SetTaxonomyFieldValue(ListItem item, Guid fieldId, string label, Guid termGuid, Web web)
        {
            ClientContext clientContext = web.Context as ClientContext;


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
    }
}
