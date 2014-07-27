using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.MMS
{
    /// <summary>
    /// Sample solution that demonstrates working with Taxonomy-related operations.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry-point for the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Request Office365 site from the user
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
                // Let's ensure that the connectivity works.
                Web web = cc.Web;
                cc.Load(web);
                cc.ExecuteQuery();

                // Call creation of terms
                CreateNecessaryMMSTermsToCloud(cc);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" - MMS terms created to cloud successfully.");
                Console.WriteLine(" - Reconnecting and listing what terms we have in cloud.");

                // Call looping of the terms
                GetMMSTermsFromCloud(cc);

                Console.WriteLine("Press any key to continue.");
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

        /// <summary>
        /// Demonstrates creating Terms and associated a new Guid. If you have the Guid from an on-prem Term.
        /// The user executing this code must have the approriate access to the Term Store to create Terms.
        /// </summary>
        /// <param name="cc">The ClientContext</param>
        private static void CreateNecessaryMMSTermsToCloud(ClientContext cc)
        {
            // Get access to taxonomy CSOM
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(cc);
            cc.Load(taxonomySession);
            cc.ExecuteQuery();

            if (taxonomySession != null)
            {
                TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                if (termStore != null)
                {
                    //
                    //  Create group, termset, and terms.
                    //
                    TermGroup myGroup = termStore.CreateGroup("Custom", Guid.NewGuid());
                    TermSet myTermSet = myGroup.CreateTermSet("Colors", Guid.NewGuid(), 1033);
                    myTermSet.CreateTerm("Red", 1033, Guid.NewGuid());
                    myTermSet.CreateTerm("Orange", 1033, Guid.NewGuid());
                    myTermSet.CreateTerm("Yellow", 1033, Guid.NewGuid());
                    myTermSet.CreateTerm("Green", 1033, Guid.NewGuid());
                    myTermSet.CreateTerm("Blue", 1033, Guid.NewGuid());
                    myTermSet.CreateTerm("Purple", 1033, Guid.NewGuid());

                    cc.ExecuteQuery();
                }
            }
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

                            foreach (Term term in termSet.Terms)
                            {
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
