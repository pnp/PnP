using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Collections.Generic;
using System.Web.Mvc;

namespace TaxonomyPickerWeb.Controllers
{
    public class HomeController : Controller
    {


        [SharePointContextFilter]
        public ActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    //Get the value of the current user's skills property from the UserProfile service.

                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    var properties = peopleManager.GetMyProperties();
                    clientContext.Load(properties, props => props.UserProfileProperties);
                    clientContext.ExecuteQuery();

                    var currentSkills = properties.UserProfileProperties["SPS-Skills"];

                    ViewBag.CurrentSkills = currentSkills.Replace('|',',');
                }
            }
            

            return View();
        }

        [HttpGet]
        [SharePointContextFilter]
        public ActionResult Keywords()
        {
            var skillsList = new List<string>();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    //Get terms from the 'Keywords' termset for autocomplete suggestions.
                    // It might be a good idea to cache these values.

                    var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                    var termStore = taxonomySession.GetDefaultKeywordsTermStore();
                    var termSet = termStore.KeywordsTermSet;
                    var termCollection = termSet.GetAllTerms();

                    clientContext.Load(termCollection, terms => terms.Include(t => t.Name));

                    clientContext.ExecuteQuery();

                   
                    foreach (var term in termCollection)
                    {
                        skillsList.Add(term.Name);
                    }   
                }
            }

            return Json(skillsList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult Skills(List<string> skills) 
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    //Write the values to the Skills property of the current user. 

                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    
                    var properties = peopleManager.GetMyProperties();
                    
                    clientContext.Load(properties, props => props.AccountName);
                    
                    clientContext.ExecuteQuery();

                    peopleManager.SetMultiValuedProfileProperty(properties.AccountName, "SPS-Skills", skills);

                    clientContext.ExecuteQuery();

                    return Json("success");

                }
            }

            return Json("error");
        }
    }
}
