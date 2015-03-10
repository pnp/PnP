using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.MMSNavigationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = GetSite();

            /* Prompt for Credentials */
            Console.WriteLine("Enter Credentials for {0}", siteUrl);

            string userName = GetUserName();
            SecureString pwd = GetPassword();

            /* End Program if no Credentials */
            if (string.IsNullOrEmpty(userName) || (pwd == null))
                return;

            ClientContext cc = new ClientContext(siteUrl);
            cc.AuthenticationMode = ClientAuthenticationMode.Default;
            //For SharePoint Online
            cc.Credentials = new SharePointOnlineCredentials(userName, pwd);
            //For SharePoint Online Dedicated or On-Prem 
            //cc.Credentials = new NetworkCredential(userName, pwd);


            try
            {
                Console.WriteLine("Connecting to the site using credentials provided...");
                // Let's ensure that the connectivity works.
                Web web = cc.Web;
                cc.Load(web);
                cc.ExecuteQuery();

                Console.WriteLine("Connecting to the taxonomy store...");
                //Let's connect to the Taxonomy Session. The hierarchy for connection is Taxonomy Session --> Term Store --> Term Group --> Term Set --> Terms
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(cc);
                taxonomySession.UpdateCache();
                cc.Load(taxonomySession.TermStores);
                cc.ExecuteQuery();

                Console.WriteLine("Starting to create the taxonomy..");
                
                CreateTaxonomyNavigation(cc, taxonomySession);

                Console.WriteLine("Taxonomy created. Reading the taxonomy...");

                GetMMSTermsFromCloud(cc);

                Console.WriteLine("Taxonomy Operations completed successfully.");
                Console.WriteLine("Press any key to exit the application now......");
                Console.Read();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Exception!"), ex.ToString());
                Console.WriteLine("Press any key to continue.");
                Console.Read();
                throw;
            }
        }

        private static void CreateTermSetAndTerms(ClientContext clientContext, XElement termSetElement, TermGroup termGroup)
        {
            clientContext.Load(termGroup.TermSets); clientContext.ExecuteQuery();
            string termSetElementName = termSetElement.Attribute("Name").Value;
            string termSetElementGuid = termSetElement.Attribute("ID").Value;

            TermSet termSet = termGroup.TermSets.FirstOrDefault(e => e.Name.Equals(termSetElementName) == true);
            if (termSet == null)
            {
                termSet = termGroup.CreateTermSet(termSetElementName, new Guid(termSetElementGuid), 1033);
            }
            bool isNavTermSet = Convert.ToBoolean(termSetElement.Attribute("IsForSiteNav").Value);
            if (isNavTermSet)
            {
                //set term set to work for site navigation
                termSet.SetCustomProperty("_Sys_Nav_IsNavigationTermSet", "True");
            }

            clientContext.Load(termSet);clientContext.Load(termSet.Terms); clientContext.ExecuteQuery();

            foreach (XElement termElement in termSetElement.Elements())
            {
                #region set properties for each Term
                string termElementName = termElement.Attribute("Name").Value;
                Term term = termSet.Terms.FirstOrDefault(e => e.Name.Equals(termElementName) == true);
                if (term == null)
                {
                    term = termSet.CreateTerm(termElementName, 1033, Guid.NewGuid());
                }
                //clientContext.Load(term); clientContext.ExecuteQuery();
                if (termElement.Attribute("NavNodeTitle").Value.Length>0)
                {
                    term.SetLocalCustomProperty("_Sys_Nav_Title", termElement.Attribute("NavNodeTitle").Value);
                }
                bool showinGlobal = Convert.ToBoolean(termElement.Attribute("ShowinGlobal").Value);
                bool showinLocal = Convert.ToBoolean(termElement.Attribute("ShowinLocal").Value);
                if (!showinGlobal || !showinLocal)
                {
                    if(!showinGlobal && !showinLocal)
                    {
                        term.SetLocalCustomProperty("_Sys_Nav_ExcludedProviders", String.Concat("GlobalNavigationTaxonomyProvider", ",", "CurrentNavigationTaxonomyProvider"));
                    }
                    if(!showinGlobal)
                    {
                        term.SetLocalCustomProperty("_Sys_Nav_ExcludedProviders", "\"GlobalNavigationTaxonomyProvider\"");
                    }
                    else
                    {
                        term.SetLocalCustomProperty("_Sys_Nav_ExcludedProviders", "\"CurrentNavigationTaxonomyProvider\"");
                    }
                }
                if(termElement.Attribute("NavNodeSimpleLink").Value.Length>0)
                {
                    term.SetLocalCustomProperty("_Sys_Nav_SimpleLinkUrl", termElement.Attribute("NavNodeSimpleLink").Value);
                }
                if(termElement.Attribute("termFriendlyURLLink").Value.Length>0)
                {
                    term.SetLocalCustomProperty("_Sys_Nav_FriendlyUrlSegment", termElement.Attribute("termFriendlyURLLink").Value);
                }
                if (termElement.Attribute("NavTargetURL").Value.Length > 0)
                {
                    term.SetLocalCustomProperty("_Sys_Nav_TargetUrl", termElement.Attribute("NavTargetURL").Value);
                }
                clientContext.Load(term);clientContext.ExecuteQuery();
                #endregion
            }
        }

        private static void CreateTaxonomyNavigation(ClientContext clientContext, TaxonomySession taxonomySession)
        {
            string taxonomyInputFileName = "mms.xml";
            string applicationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, taxonomyInputFileName);

            XDocument termsXML = XDocument.Load(applicationPath);

            #region for each taxonomy sessions, repeat for each term store, term group
            foreach (XElement termStoreElement in termsXML.Elements())
            {
                TermStore termStore = taxonomySession.TermStores.GetByName(termStoreElement.Attribute("Name").Value);
                clientContext.Load(termStore.Groups);
                clientContext.ExecuteQuery();
                if (termStore != null)
                {
                    foreach (XElement termGroupElement in termStoreElement.Elements())
                    {
                        string termgroupElementName = termGroupElement.Attribute("Name").Value;
                        string termgroupElementGuid = termGroupElement.Attribute("ID").Value;
                        TermGroup termGroup = termStore.Groups.FirstOrDefault(e => e.Name.Equals(termgroupElementName) == true);
                        if (termGroup == null)
                        {
                            termGroup = termStore.CreateGroup(termgroupElementName, new Guid(termgroupElementGuid));
                        }
                        clientContext.Load(termGroup);
                        clientContext.ExecuteQuery();
                        foreach (XElement termSetElement in termGroupElement.Elements())
                        {
                            CreateTermSetAndTerms(clientContext, termSetElement, termGroup);
                        }
                    }
                }
                termStore.CommitAll();
            }
            #endregion
        }


        /// <summary>
        /// Member to return all the Taxonomy fields 
        /// </summary>
        /// <param name="cc"></param>
        private static void GetMMSTermsFromCloud(ClientContext cc)
        {
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
                            //cc.Load(termSet,ts=>ts.CustomProperties);cc.ExecuteQuery();

                            foreach (Term term in termSet.Terms)
                            {
                                cc.Load(term, tt => tt.CustomProperties,tt1=>tt1.LocalCustomProperties); cc.ExecuteQuery();
                                //Writes root-level terms only.
                                Console.WriteLine("Term " + term.Name);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper to Return a Site Collection URL
        /// </summary>
        /// <returns></returns>
        public static string GetSite()
        {
            string siteUrl = string.Empty;
            try
            {
                Console.Write("Give Office365 site URL: ");
                siteUrl = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                siteUrl = string.Empty;
            }
            return siteUrl;
        }

        /// <summary>
        /// Helper to return the password
        /// </summary>
        /// <returns>SecureString representing the password</returns>
        public static SecureString GetPassword()
        {
            SecureString sStrPwd = new SecureString();

            try
            {
                Console.Write("SharePoint Password: ");

                for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (sStrPwd.Length > 0)
                        {
                            sStrPwd.RemoveAt(sStrPwd.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        sStrPwd.AppendChar(keyInfo.KeyChar);
                    }

                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                sStrPwd = null;
                Console.WriteLine(e.Message);
            }

            return sStrPwd;
        }

        /// <summary>
        /// Helper to return the User name.
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write("SharePoint Username: ");
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

    }
}
