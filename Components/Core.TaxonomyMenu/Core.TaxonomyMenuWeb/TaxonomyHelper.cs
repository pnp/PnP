using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Core.TaxonomyMenuWeb
{
    public static class TaxonomyHelper
    {
        public static void SetupTermStore(ClientContext clientContext)
        {
            var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
            var termStore = taxSession.GetDefaultSiteCollectionTermStore();

            AddLanguages(clientContext, termStore);
            var termGroup = AddGroup(clientContext, termStore);
            var termSet = AddTermSet(clientContext, termStore, termGroup);

            AddTerm(clientContext, termSet, "Departments", "Départements", "Abteilungen", "Avdelningar");
            AddTerm(clientContext, termSet, "Employee", "Employé", "Mitarbeiter", "Anställd");
            AddTerm(clientContext, termSet, "News", "Actualités", "Nachricht", "Nyheter");
            AddTerm(clientContext, termSet, "Search", "Rechercher", "Suche", "Sök");
        }

        private static void AddTerm(ClientContext clientContext, TermSet termSet, string termName, string termNameFrench, string termNameGerman, string termNameSwedish)
        {
            if (!termSet.Terms.Any(t => t.Name.Equals(termName)))
            {
                var term = termSet.CreateTerm(termName, 1033, Guid.NewGuid());
                term.CreateLabel(termNameGerman, 1031, false);
                term.CreateLabel(termNameFrench, 1036, false);
                term.CreateLabel(termNameSwedish, 1053, false);
                term.SetLocalCustomProperty("_Sys_Nav_SimpleLinkUrl", clientContext.Web.ServerRelativeUrl);
                clientContext.ExecuteQuery();
            }
        }

        private static TermSet AddTermSet(ClientContext clientContext, TermStore termStore, TermGroup termGroup)
        {
            var termSetId = new Guid("56ca0eea-635e-4cc1-ac35-fc2040f4cfe5");
            var termSet = termStore.GetTermSet(termSetId);
            clientContext.Load(termSet, ts => ts.Terms);
            clientContext.ExecuteQuery();

            if (termSet.ServerObjectIsNull.Value)
            {
                termSet = termGroup.CreateTermSet("Taxonomy Navigation", termSetId, 1033);
                termSet.SetCustomProperty("_Sys_Nav_IsNavigationTermSet", "True");
                clientContext.Load(termSet, ts => ts.Terms);
                clientContext.ExecuteQuery();                
            }
            
            return termSet;
        }

        private static TermGroup AddGroup(ClientContext clientContext, TermStore termStore)
        {
            var groupId = new Guid("8de44223-5a8f-41cd-b0e2-5634b0bb953b");
            var termGroup = termStore.GetGroup(groupId);
            clientContext.Load(termGroup);
            clientContext.ExecuteQuery();

            if (termGroup.ServerObjectIsNull.Value)
            {
                termGroup = termStore.CreateGroup("Taxonomy Navigation", groupId);
                clientContext.Load(termGroup);
                clientContext.ExecuteQuery();
            }

            return termGroup;
        }

        private static void AddLanguages(ClientContext clientContext, TermStore termStore)
        {
            clientContext.Load(clientContext.Web, w => w.ServerRelativeUrl);
            clientContext.Load(termStore, ts => ts.Languages);
            clientContext.ExecuteQuery();

            var languages = new int[] { 1031, 1033, 1036, 1053 };
            Array.ForEach(languages, l => { 
                if (!termStore.Languages.Contains(l)) 
                    termStore.AddLanguage(l); 
            });

            termStore.CommitAll();
            clientContext.ExecuteQuery();
        }
    }
}