using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.Publishing.Navigation;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectTermGroups : ObjectHandlerBase
    {

        public override string Name
        {
            get { return "Term Groups"; }
        }
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, "Term Groups");

            TaxonomySession taxSession = TaxonomySession.GetTaxonomySession(web.Context);

            var termStore = taxSession.GetDefaultKeywordsTermStore();

            web.Context.Load(termStore,
                ts => ts.DefaultLanguage,
                ts => ts.Groups.Include(
                    tg => tg.Name,
                    tg => tg.Id,
                    tg => tg.TermSets.Include(
                        tset => tset.Name,
                        tset => tset.Id)));
            web.Context.ExecuteQueryRetry();

            foreach (var modelTermGroup in template.TermGroups)
            {
                #region Group

                var newGroup = false;

                TermGroup group = termStore.Groups.FirstOrDefault(g => g.Id == modelTermGroup.Id);
                if (group == null)
                {
                    group = termStore.Groups.FirstOrDefault(g => g.Name == modelTermGroup.Name);

                    if (group == null)
                    {
                        if (modelTermGroup.Id == Guid.Empty)
                        {
                            modelTermGroup.Id = Guid.NewGuid();
                        }
                        group = termStore.CreateGroup(modelTermGroup.Name.ToParsedString(), modelTermGroup.Id);

                        group.Description = modelTermGroup.Description;

                        termStore.CommitAll();
                        web.Context.Load(group);
                        web.Context.ExecuteQueryRetry();

                        newGroup = true;

                    }
                }

                #endregion

                #region TermSets

                foreach (var modelTermSet in modelTermGroup.TermSets)
                {
                    TermSet set = null;
                    var newTermSet = false;
                    if (!newGroup)
                    {
                        set = group.TermSets.FirstOrDefault(ts => ts.Id == modelTermSet.Id);
                        if (set == null)
                        {
                            set = group.TermSets.FirstOrDefault(ts => ts.Name == modelTermSet.Name);

                        }
                    }
                    if (set == null)
                    {
                        if (modelTermSet.Id == Guid.Empty)
                        {
                            modelTermSet.Id = Guid.NewGuid();
                        }
                        set = group.CreateTermSet(modelTermSet.Name.ToParsedString(), modelTermSet.Id, modelTermSet.Language ?? termStore.DefaultLanguage);
                        TokenParser.AddToken(new TermSetIdToken(web, modelTermGroup.Name, modelTermSet.Name, modelTermSet.Id));
                        newTermSet = true;
                        set.IsOpenForTermCreation = modelTermSet.IsOpenForTermCreation;
                        set.IsAvailableForTagging = modelTermSet.IsAvailableForTagging;
                        foreach (var property in modelTermSet.Properties)
                        {
                            set.SetCustomProperty(property.Key, property.Value);
                        }
                        if (modelTermSet.Owner != null)
                        {
                            set.Owner = modelTermSet.Owner;
                        }
                        termStore.CommitAll();
                        web.Context.Load(set);
                        web.Context.ExecuteQueryRetry();
                    }

                    web.Context.Load(set, s => s.Terms.Include(t => t.Id, t => t.Name));
                    web.Context.ExecuteQueryRetry();
                    var terms = set.Terms;

                    // do we need custom sorting?
                    if (modelTermSet.Terms.Count(t => t.CustomSortOrder != null) > 0)
                    {
                        // Precreate the IDs of the terms if not set
                        foreach (var term in modelTermSet.Terms.Where(t => t.Id == Guid.Empty))
                        {
                            term.Id = Guid.NewGuid();
                        }

                        var sortedTerms = modelTermSet.Terms.OrderBy(t => t.CustomSortOrder);

                        var customSortString = sortedTerms.Aggregate(string.Empty, (a, i) => a + i.Id.ToString() + ":");
                        customSortString = customSortString.TrimEnd(new[] { ':' });

                        set.CustomSortOrder = customSortString;
                        termStore.CommitAll();

                    }

                    foreach (var modelTerm in modelTermSet.Terms)
                    {
                        if (!newTermSet)
                        {
                            if (terms.Any())
                            {
                                var term = terms.FirstOrDefault(t => t.Id == modelTerm.Id);
                                if (term == null)
                                {
                                    term = terms.FirstOrDefault(t => t.Name == modelTerm.Name);
                                    if (term == null)
                                    {
                                        CreateTerm<TermSet>(web, modelTerm, set, termStore);
                                    }
                                }
                            }
                            else
                            {
                                CreateTerm<TermSet>(web, modelTerm, set, termStore);
                            }
                        }
                        else
                        {
                            CreateTerm<TermSet>(web, modelTerm, set, termStore);
                        }
                    }
                }

                #endregion

            }
        }

        private void CreateTerm<T>(Web web, Model.Term modelTerm, TaxonomyItem parent, TermStore termStore) where T : TaxonomyItem
        {
            Term term;
            if (modelTerm.Id == Guid.Empty)
            {
                modelTerm.Id = Guid.NewGuid();
            }

            if (parent is Term)
            {
                var languages = termStore.DefaultLanguage;
                term = ((Term)parent).CreateTerm(modelTerm.Name.ToParsedString(), modelTerm.Language ?? termStore.DefaultLanguage, modelTerm.Id);

            }
            else
            {
                term = ((TermSet)parent).CreateTerm(modelTerm.Name.ToParsedString(), modelTerm.Language ?? termStore.DefaultLanguage, modelTerm.Id);
            }
            if (!String.IsNullOrEmpty(modelTerm.Description))
            {
                term.SetDescription(modelTerm.Description, modelTerm.Language ?? termStore.DefaultLanguage);
            }
            if (!String.IsNullOrEmpty(modelTerm.Owner))
            {
                term.Owner = modelTerm.Owner;
            }

            term.IsAvailableForTagging = modelTerm.IsAvailableForTagging;

            term.CustomSortOrder = modelTerm.CustomSortOrder.ToString();
            
            if (modelTerm.Properties.Any() || modelTerm.Labels.Any() || modelTerm.LocalProperties.Any())
            {
                if (modelTerm.Labels.Any())
                {
                    foreach (var label in modelTerm.Labels)
                    {
                        var l = term.CreateLabel(label.Value.ToParsedString(), label.Language, label.IsDefaultForLanguage);
                    }
                }

                if (modelTerm.Properties.Any())
                {
                    foreach (var property in modelTerm.Properties)
                    {
                        term.SetCustomProperty(property.Key.ToParsedString(), property.Value.ToParsedString());
                    }
                }
                if (modelTerm.LocalProperties.Any())
                {
                    foreach (var property in modelTerm.LocalProperties)
                    {
                        term.SetLocalCustomProperty(property.Key.ToParsedString(), property.Value.ToParsedString());
                    }
                }
            }
            termStore.CommitAll();

            web.Context.Load(term);
            web.Context.ExecuteQueryRetry();

            if (modelTerm.Terms.Any())
            {
                if (modelTerm.Terms.Any(t => t.CustomSortOrder != null))
                {
                    // Precreate the IDs of the terms if not set
                    foreach (var termToSet in modelTerm.Terms.Where(t => t.Id == Guid.Empty))
                    {
                        termToSet.Id = Guid.NewGuid();
                    }

                    var sortedTerms = modelTerm.Terms.OrderBy(t => t.CustomSortOrder);

                    var customSortString = sortedTerms.Aggregate(string.Empty, (a, i) => a + i.Id.ToString() + ":");
                    customSortString = customSortString.TrimEnd(new[] { ':' });

                    term.CustomSortOrder = customSortString;
                    termStore.CommitAll();

                }

                CreateTerms(web, termStore, term, modelTerm.Terms);
            }
        }

        private void CreateTerms(Web web, TermStore store, Term parent, List<OfficeDevPnP.Core.Framework.Provisioning.Model.Term> modelTerms)
        {
            foreach (var modelTerm in modelTerms)
            {
                web.Context.Load(parent.Terms);
                web.Context.ExecuteQueryRetry();
                var terms = parent.Terms;
                if (terms.Any())
                {
                    var term = terms.FirstOrDefault(t => t.Id == modelTerm.Id);
                    if (term == null)
                    {
                        term = terms.FirstOrDefault(t => t.Name == modelTerm.Name);
                        if (term == null)
                        {
                            CreateTerm<Term>(web, modelTerm, parent, store);
                        }
                    }
                }
                else
                {
                    CreateTerm<Term>(web, modelTerm, parent, store);
                }
            }
        }


        public override Model.ProvisioningTemplate CreateEntities(Web web, Model.ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (creationInfo.IncludeSiteCollectionTermGroup || creationInfo.IncludeAllTermGroups)
            {
                // Find the site collection termgroup, if any
                TaxonomySession session = TaxonomySession.GetTaxonomySession(web.Context);
                var termStore = session.GetDefaultSiteCollectionTermStore();
                web.Context.Load(termStore, t => t.Id, t => t.DefaultLanguage);
                web.Context.ExecuteQueryRetry();

                List<TermGroup> termGroups = new List<TermGroup>();
                if (creationInfo.IncludeAllTermGroups)
                {
                    web.Context.Load(termStore.Groups, groups => groups.Include(tg => tg.Name,
                        tg => tg.Id,
                        tg => tg.Description,
                        tg => tg.TermSets.IncludeWithDefaultProperties(ts => ts.CustomSortOrder)));
                    web.Context.ExecuteQuery();
                    termGroups = termStore.Groups.ToList();
                }
                else
                {
                    var propertyBagKey = string.Format("SiteCollectionGroupId{0}", termStore.Id);

                    var siteCollectionTermGroupId = web.GetPropertyBagValueString(propertyBagKey, "");

                    Guid termGroupGuid = Guid.Empty;
                    if (Guid.TryParse(siteCollectionTermGroupId, out termGroupGuid))
                    {
                        var termGroup = termStore.GetGroup(termGroupGuid);
                        web.Context.Load(termGroup,
                            tg => tg.Name,
                            tg => tg.Id,
                            tg => tg.Description,
                            tg => tg.TermSets.IncludeWithDefaultProperties(ts => ts.CustomSortOrder));

                        web.Context.ExecuteQueryRetry();

                        termGroups = new List<TermGroup>() {termGroup};
                    }
                }

                foreach (var termGroup in termGroups)
                {
                    var modelTermGroup = new Model.TermGroup
                    {
                        Name = termGroup.Name,
                        Id = termGroup.Id,
                        Description = termGroup.Description
                    };

                    foreach (var termSet in termGroup.TermSets)
                    {
                        var modelTermSet = new Model.TermSet();
                        modelTermSet.Name = termSet.Name;
                        modelTermSet.Id = termSet.Id;
                        modelTermSet.IsAvailableForTagging = termSet.IsAvailableForTagging;
                        modelTermSet.IsOpenForTermCreation = termSet.IsOpenForTermCreation;
                        modelTermSet.Description = termSet.Description;
                        modelTermSet.Terms.AddRange(GetTerms<TermSet>(web.Context, termSet, termStore.DefaultLanguage));
                        foreach (var property in termSet.CustomProperties)
                        {
                            modelTermSet.Properties.Add(property.Key, property.Value);
                        }
                        modelTermGroup.TermSets.Add(modelTermSet);
                    }

                    template.TermGroups.Add(modelTermGroup);
                }
            }
            return template;
        }

        private List<Model.Term> GetTerms<T>(ClientRuntimeContext context, TaxonomyItem parent, int defaultLanguage)
        {
            List<Model.Term> termsToReturn = new List<Model.Term>();
            TermCollection terms = null;
            var customSortOrder = string.Empty;
            if (parent is TermSet)
            {
                terms = ((TermSet)parent).Terms;
                customSortOrder = ((TermSet)parent).CustomSortOrder;
            }
            else
            {
                terms = ((Term)parent).Terms;
                customSortOrder = ((Term)parent).CustomSortOrder;
            }
            context.Load(terms, tms => tms.IncludeWithDefaultProperties(t => t.Labels, t => t.CustomSortOrder));
            context.ExecuteQueryRetry();

            foreach (var term in terms)
            {
                var modelTerm = new Model.Term();
                modelTerm.Id = term.Id;
                modelTerm.Name = term.Name;
                modelTerm.IsAvailableForTagging = term.IsAvailableForTagging;

                if (term.Labels.Count == 1)
                {
                    var label = term.Labels[0];
                    if ((label.Language == defaultLanguage && label.Value != term.Name) || label.Language != defaultLanguage)
                    {
                        var modelLabel = new Model.TermLabel();
                        modelLabel.IsDefaultForLanguage = label.IsDefaultForLanguage;
                        modelLabel.Value = label.Value;
                        modelLabel.Language = label.Language;

                        modelTerm.Labels.Add(modelLabel);
                    }
                }
                else
                {
                    foreach (var label in term.Labels)
                    {
                        var modelLabel = new Model.TermLabel();
                        modelLabel.IsDefaultForLanguage = label.IsDefaultForLanguage;
                        modelLabel.Value = label.Value;
                        modelLabel.Language = label.Language;

                        modelTerm.Labels.Add(modelLabel);
                    }
                }

                foreach (var localProperty in term.LocalCustomProperties)
                {
                    modelTerm.LocalProperties.Add(localProperty.Key, localProperty.Value);
                }

                foreach (var customProperty in term.CustomProperties)
                {
                    modelTerm.Properties.Add(customProperty.Key, customProperty.Value);
                }
                if (term.TermsCount > 0)
                {
                    modelTerm.Terms.AddRange(GetTerms<Term>(context, term, defaultLanguage));
                }
                termsToReturn.Add(modelTerm);
            }
            if (!string.IsNullOrEmpty(customSortOrder))
            {
                int count = 1;
                foreach (var id in customSortOrder.Split(new[] { ':' }))
                {
                    var term = termsToReturn.FirstOrDefault(t => t.Id == Guid.Parse(id));
                    if (term != null)
                    {
                        term.CustomSortOrder = count;
                        count++;
                    }
                }
                termsToReturn = termsToReturn.OrderBy(t => t.CustomSortOrder).ToList();
            }


            return termsToReturn;
        }

    }
}
