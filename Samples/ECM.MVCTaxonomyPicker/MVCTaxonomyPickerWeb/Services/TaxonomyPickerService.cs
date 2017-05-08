using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using MVCTaxonomyPickerWeb.Models;
using MVCTaxonomyPickerWeb.Helpers;

namespace MVCTaxonomyPickerWeb.Services
{
    public class TaxonomyPickerService
    {
        public static string GetTaxonomyPickerData(TermSetQueryModel model)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var context = spContext.CreateAppOnlyClientContextForSPHost())
            {
                return GetTaxonomyPickerData(context, model);
            }
        }

        public static string DeleteTerm(TermQueryModel model)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var context = spContext.CreateAppOnlyClientContextForSPHost())
            {
                return DeleteTerm(context, model);
            }
        }

        public static string AddTerm(TermQueryModel model)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var context = spContext.CreateAppOnlyClientContextForSPHost())
            {
                return AddTerm(context, model);
            }
        }       

        public static string AddTerm(ClientContext clientContext, TermQueryModel model)
        {
            var pickerTerm = new PickerTermModel();
           
            if (clientContext != null)
            {                
                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

                Term parentTerm = null;
                TermSet termSet = GetParent(termStore, model, out parentTerm);
                Term newTerm = null;
                var newTermId = new Guid(model.Id);
                if (parentTerm != null)
                {
                    clientContext.Load(parentTerm, t => t.PathOfTerm,
                                               t => t.Id,
                                               t => t.Labels,
                                               t => t.Name,
                                               t => t.Terms);
                    clientContext.ExecuteQuery();                   
                    newTerm = parentTerm.CreateTerm(model.Name, model.LCID, newTermId);
                }
                else
                {
                    clientContext.Load(termSet);
                    clientContext.ExecuteQuery();                   
                    newTerm = termSet.CreateTerm(model.Name, 1033, newTermId);
                }
               
                clientContext.Load(newTerm, t => t.PathOfTerm,
                                                t => t.Id,
                                                t => t.Labels,
                                                t => t.Name);
                clientContext.ExecuteQuery();

                pickerTerm.Name = newTerm.Name;
                pickerTerm.Id = newTerm.Id.ToString();
                pickerTerm.PathOfTerm = newTerm.PathOfTerm;
                pickerTerm.Level = newTerm.PathOfTerm.Split(';').Length - 1;
                pickerTerm.Terms = new List<PickerTermModel>();
            }
            return JsonHelper.Serialize<PickerTermModel>(pickerTerm);
        }

        public static TermSet GetParent(TermStore termStore, TermQueryModel model, out Term parentTerm)
        {
            TermSet termSet = null;
            parentTerm = null;

            if (string.IsNullOrEmpty(model.TermSetId))
            {
                parentTerm = termStore.GetTerm(new Guid(model.ParentTermId));
            }
            else
            {
                termSet = termStore.GetTermSet(new Guid(model.TermSetId));
            }           

            return termSet;
        }       

        public static PickerTermModel GetPickerTerm(Term term, bool includeChildren = true)
        {
            var pTerm = new PickerTermModel()
            {
                Name = term.Name,
                Id = Convert.ToString(term.Id),
                PathOfTerm = term.PathOfTerm,
                Level = term.PathOfTerm.Split(';').Length - 1,
                Terms = new List<PickerTermModel>(),
                Labels = new List<PickerLabelModel>()
            };

            term.Labels.ToList<Label>().ForEach(l => pTerm.Labels.Add(new PickerLabelModel()
            {
                Value = l.Value,
                IsDefaultForLanguage = l.IsDefaultForLanguage
            }));


            if (term.TermsCount > 0 && includeChildren == true)
            {
                term.Context.Load(term.Terms, terms => terms.Include(t => t.PathOfTerm,
                                                    t => t.Id,
                                                    t => t.Labels.Include(l => l.IsDefaultForLanguage, l => l.Value),
                                                    t => t.Name,
                                                    t => t.TermsCount));
                term.Context.ExecuteQuery();
                term.Terms.ToList<Term>().ForEach(t => pTerm.Terms.Add(GetPickerTerm(t)));
            }

            return pTerm;        
        }       

        public static string GetTaxonomyPickerData(ClientContext clientContext, TermSetQueryModel model)
        {
            var pickerTermSet = new PickerTermSetModel();
            
            if (clientContext != null)
            {
                //Get terms from the 'Keywords' termset for autocomplete suggestions.
                // It might be a good idea to cache these values.
                
                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();                             

                TermSet termSet = null;

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    termSet = termStore.GetTermSet(new Guid(model.Id));
                }                   
                else if (!string.IsNullOrWhiteSpace(model.Name))
                {
                    var rawTermSets = termStore.GetTermSetsByName(model.Name, model.LCID);
                    termSet = rawTermSets.GetByName(model.Name);
                }
                else if (model.UseHashtags)
                {
                    termSet = termStore.HashTagsTermSet;
                }                
                else if (model.UseKeywords)
                {
                    termSet = termStore.KeywordsTermSet;
                }                
                            
                clientContext.Load(termSet, ts => ts.Id, ts => ts.IsOpenForTermCreation, ts => ts.CustomSortOrder, ts => ts.Name, 
                    ts => ts.Terms.Include(t => t.PathOfTerm,
                                            t => t.Id,
                                            t => t.Labels.Include(l => l.IsDefaultForLanguage, l => l.Value),
                                            t => t.Name,
                                            t => t.TermsCount));               
                clientContext.ExecuteQuery();

                var allTerms = termSet.GetAllTerms();

                clientContext.Load(allTerms, terms => terms.Include(t => t.PathOfTerm,
                                                    t => t.Id,
                                                    t => t.Labels.Include(l => l.IsDefaultForLanguage, l => l.Value),
                                                    t => t.Name,
                                                    t => t.TermsCount));
                clientContext.ExecuteQuery();

                pickerTermSet.Id = termSet.Id.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                pickerTermSet.Name = termSet.Name;
                pickerTermSet.IsOpenForTermCreation = termSet.IsOpenForTermCreation;
                pickerTermSet.CustomSortOrder = termSet.CustomSortOrder;
                pickerTermSet.Terms = new List<PickerTermModel>();
                pickerTermSet.FlatTerms = new List<PickerTermModel>();

                foreach (var term in termSet.Terms.ToList<Term>())
                {
                    pickerTermSet.Terms.Add(GetPickerTerm(term));                    
                }

                foreach (var term in allTerms.ToList<Term>())
                {                   
                    pickerTermSet.FlatTerms.Add(GetPickerTerm(term, false));
                }

            }
            return JsonHelper.Serialize<PickerTermSetModel>(pickerTermSet);
        }

        public static string DeleteTerm(ClientContext clientContext, TermQueryModel model)
        {
            var pickerTermSet = new PickerTermSetModel();
            //var searchString = (string)HttpContext.Current.Request["SearchString"];
            
            if (clientContext != null)
            {
                //Get terms from the 'Keywords' termset for autocomplete suggestions.
                // It might be a good idea to cache these values.

                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.GetDefaultKeywordsTermStore();                
                var termToDelete = termStore.GetTerm(new Guid(model.Id));

                clientContext.Load(termToDelete);
                clientContext.ExecuteQuery();

                termToDelete.DeleteObject();
                clientContext.ExecuteQuery();

                var termSetId = new Guid(model.TermSetId);
                var termSet = termStore.GetTermSet(termSetId);

                clientContext.Load(termSet, ts => ts.Id, ts => ts.IsOpenForTermCreation, ts => ts.CustomSortOrder, ts => ts.Name,
                  ts => ts.Terms.Include(t => t.PathOfTerm,
                                            t => t.Id,
                                            t => t.Labels.Include(l => l.IsDefaultForLanguage, l => l.Value),
                                            t => t.Name,
                                            t => t.TermsCount));
                clientContext.ExecuteQuery();

                pickerTermSet.Id = termSet.Id.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                pickerTermSet.Name = termSet.Name;
                pickerTermSet.IsOpenForTermCreation = termSet.IsOpenForTermCreation;
                pickerTermSet.CustomSortOrder = termSet.CustomSortOrder;
                pickerTermSet.Terms = new List<PickerTermModel>();

                foreach (var term in termSet.Terms.ToList<Term>())
                {
                    pickerTermSet.Terms.Add(GetPickerTerm(term));
                }
            }
            return JsonHelper.Serialize<PickerTermSetModel>(pickerTermSet);
        }
    }
}