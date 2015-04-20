using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Framework.ObjectHandlers.TokenDefinitions;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectTermGroups : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
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

                TermGroup group = termStore.Groups.FirstOrDefault(g => g.Id == modelTermGroup.ID);
                if (group == null)
                {
                    group = termStore.Groups.FirstOrDefault(g => g.Name == modelTermGroup.Name);

                    if (group == null)
                    {
                        if (modelTermGroup.ID == Guid.Empty)
                        {
                            modelTermGroup.ID = Guid.NewGuid();
                        }
                        group = termStore.CreateGroup(modelTermGroup.Name.ToParsedString(), modelTermGroup.ID);

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
                        set = group.TermSets.FirstOrDefault(ts => ts.Id == modelTermSet.ID);
                        if (set == null)
                        {
                            set = group.TermSets.FirstOrDefault(ts => ts.Name == modelTermSet.Name);

                            if (set == null)
                            {
                                if (modelTermSet.ID == Guid.Empty)
                                {
                                    modelTermSet.ID = Guid.NewGuid();
                                }
                                set = group.CreateTermSet(modelTermSet.Name.ToParsedString(), modelTermSet.ID, modelTermSet.Language ?? termStore.DefaultLanguage);
                                TokenParser.AddToken(new TermSetIdToken(web, modelTermGroup.Name, modelTermSet.Name, modelTermSet.ID));
                                newTermSet = true;

                                set.Description = modelTermSet.Description;

                                termStore.CommitAll();
                                web.Context.Load(set);
                                web.Context.ExecuteQueryRetry();
                            }
                        }
                    }
                    else
                    {
                        if (modelTermSet.ID == Guid.Empty)
                        {
                            modelTermSet.ID = Guid.NewGuid();
                        }
                        set = group.CreateTermSet(modelTermSet.Name.ToParsedString(), modelTermSet.ID, modelTermSet.Language ?? termStore.DefaultLanguage);
                        TokenParser.AddToken(new TermSetIdToken(web, modelTermGroup.Name, modelTermSet.Name, modelTermSet.ID));
                        newTermSet = true;
                        termStore.CommitAll();
                        web.Context.Load(set);
                        web.Context.ExecuteQueryRetry();
                    }

                    web.Context.Load(set, s => s.Terms.Include(t => t.Id, t => t.Name, t => t));
                    web.Context.ExecuteQueryRetry();

                    // do we need custom sorting?
                    if (modelTermSet.Terms.Count(t => t.CustomSortOrder != null) > 0)
                    {
                        // Precreate the IDs of the terms if not set
                        foreach (var term in modelTermSet.Terms.Where(t => t.ID == Guid.Empty))
                        {
                            term.ID = Guid.NewGuid();
                        }

                        var sortedTerms = modelTermSet.Terms.OrderBy(t => t.CustomSortOrder);

                        var customSortString = sortedTerms.Aggregate(string.Empty, (a, i) => a + i.ID.ToString() + ":");
                        customSortString = customSortString.TrimEnd(new[] { ':' });

                        set.CustomSortOrder = customSortString;
                        termStore.CommitAll();

                    }
                    foreach (var modelTerm in modelTermSet.Terms)
                    {
                        if (!newTermSet)
                        {
                            web.Context.Load(set, s => s.Terms.Include(t => t.Id, t => t.Name));
                            web.Context.ExecuteQueryRetry();
                            var terms = set.Terms;
                            if (terms.Any())
                            {
                                var term = terms.FirstOrDefault(t => t.Id == modelTerm.ID);
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
            if (modelTerm.ID == Guid.Empty)
            {
                modelTerm.ID = Guid.NewGuid();
            }

            if (parent is Term)
            {
                term = ((Term)parent).CreateTerm(modelTerm.Name.ToParsedString(), modelTerm.Language ?? termStore.DefaultLanguage, modelTerm.ID);

            }
            else
            {
                term = ((TermSet)parent).CreateTerm(modelTerm.Name.ToParsedString(), modelTerm.Language ?? termStore.DefaultLanguage, modelTerm.ID);
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

            if (!String.IsNullOrEmpty(modelTerm.CustomSortOrder))
            {
                term.CustomSortOrder = modelTerm.CustomSortOrder;
            }

            termStore.CommitAll();
            web.Context.Load(term);
            web.Context.ExecuteQueryRetry();
            if (modelTerm.Properties.Any() || modelTerm.Labels.Any() || modelTerm.LocalProperties.Any())
            {
                var isDirty = false;

                if (modelTerm.Properties.Any())
                {
                    isDirty = true;
                    foreach (var property in modelTerm.Properties)
                    {
                        term.SetCustomProperty(property.Key.ToParsedString(), property.Value.ToParsedString());
                    }
                }
                if (modelTerm.LocalProperties.Any())
                {
                    isDirty = true;
                    foreach (var property in modelTerm.LocalProperties)
                    {
                        term.SetLocalCustomProperty(property.Key.ToParsedString(), property.Value.ToParsedString());
                    }
                }
                if (modelTerm.Labels.Any())
                {
                    isDirty = true;
                    foreach (var label in modelTerm.Labels)
                    {
                        var l = term.CreateLabel(label.Value.ToParsedString(), label.Language,
                            label.IsDefaultForLanguage ?? false);
                    }
                }
                if (isDirty)
                {
                    termStore.CommitAll();
                    web.Context.ExecuteQueryRetry();
                }
            }

            if (modelTerm.Terms.Any())
            {
                if (modelTerm.Terms.Count(t => t.CustomSortOrder != null) > 0)
                {
                    // Precreate the IDs of the terms if not set
                    foreach (var termToSet in modelTerm.Terms.Where(t => t.ID == Guid.Empty))
                    {
                        termToSet.ID = Guid.NewGuid();
                    }

                    var sortedTerms = modelTerm.Terms.OrderBy(t => t.CustomSortOrder);

                    var customSortString = sortedTerms.Aggregate(string.Empty, (a, i) => a + i.ID.ToString() + ":");
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
                    var term = terms.FirstOrDefault(t => t.Id == modelTerm.ID);
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


        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }
    }
}
