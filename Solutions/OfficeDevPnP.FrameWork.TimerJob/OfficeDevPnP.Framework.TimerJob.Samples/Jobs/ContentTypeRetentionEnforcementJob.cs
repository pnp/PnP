using Microsoft.SharePoint.Client;
using OfficeDevPnP.Framework.TimerJob.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Framework.TimerJob.Samples.Jobs
{
    public class ContentTypeRetentionEnforcementJob: TimerJob
    {
        private NameValueCollection configContentTypeRetentionPolicyPeriods;

        public ContentTypeRetentionEnforcementJob(): base("ContentTypeRetentionEnforcementJob")
        {
            //Read retention policy settings from app.config file
            configContentTypeRetentionPolicyPeriods = (NameValueCollection)ConfigurationManager.GetSection("ContentTypeRetentionPolicyPeriod");                        
            
            TimerJobRun += ContentTypeRetentionEnforcementJob_TimerJobRun;
        }

        void ContentTypeRetentionEnforcementJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            try
            {
                Log.Info("ContentTypeRetentionEnforcementJob", "Scanning web {0}", e.Url);

                //Get all document libraries. Lists are excluded.
                var documentLibraries = GetAllDocumentLibrariesInWeb(e.WebClientContext, e.WebClientContext.Web);

                //Iterate through all document libraries
                foreach (var documentLibrary in documentLibraries)
                {
                    Log.Info("ContentTypeRetentionEnforcementJob", "Scanning library {0}", documentLibrary.Title);

                    //Iterate through configured content type retention policies in app.config
                    foreach (var contentTypeName in configContentTypeRetentionPolicyPeriods.Keys)
                    {
                        var retentionPeriods = configContentTypeRetentionPolicyPeriods.GetValues(contentTypeName as string);
                        if (retentionPeriods != null)
                        {
                            var retentionPeriod = int.Parse(retentionPeriods[0]);
                            ApplyRetentionPolicy(e.WebClientContext, documentLibrary, contentTypeName, retentionPeriod);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error("ContentTypeRetentionEnforcementJob", "Exception processing site {0}. Exception is {1}", e.Url, ex.Message);
            }
        }

        #region Helper methods
        private IEnumerable<List> GetAllDocumentLibrariesInWeb(ClientContext clientContext, Web web)
        {
            //Retrieve all lists from specified web
            var lists = web.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQueryRetry(); 

            //Filter out only document libraries and append to return list collection
            var libraries = new List<List>();
            foreach (var list in lists)
            {
                if (list.BaseType.ToString() == "DocumentLibrary")
                {
                    libraries.Add(list);
                }
            }
            Log.Info("ContentTypeRetentionEnforcementJob", "The number of libraries found: {0}", libraries.Count);
            return libraries;
        }

        private void ApplyRetentionPolicy(ClientContext clientContext, List documentLibrary, object contentTypeId, int retentionPeriodDays)
        {
            //Calculate validation date. Any document modified before that date is considered old
            var validationDate = DateTime.Now.AddDays(-retentionPeriodDays);
            var camlDate = validationDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

            //Get old documents in the library that are matching requested content type
            if (documentLibrary.ItemCount > 0)
            {
                var camlQuery = new CamlQuery();

                //This CAML query uses Content Type ID with BeginsWith.
                //You can replace with ContentType for CT Display Name, for example
                //<Eq><FieldRef Name='ContentType' /><Value Type='Computed'>{0}</Value></Eq>
                camlQuery.ViewXml = String.Format(
                    @"<View>
                        <Query>
                            <Where><And>
                                <BeginsWith><FieldRef Name='ContentTypeId'/><Value Type='ContentTypeId'>{0}</Value></BeginsWith>
                                <Lt><FieldRef Name='Modified' /><Value Type='DateTime'>{1}</Value></Lt>
                            </And></Where>
                        </Query>
                    </View>", contentTypeId, camlDate);

                var listItems = documentLibrary.GetItems(camlQuery);
                clientContext.Load(listItems,
                    items => items.Include(
                        item => item.Id,
                        item => item.DisplayName,
                        item => item.ContentType));

                clientContext.ExecuteQueryRetry(); 

                foreach (var listItem in listItems)
                {
                    Log.Info("ContentTypeRetentionEnforcementJob", "Document '{0}' has been modified earlier than {1}. Retention policy will be applied.", listItem.DisplayName, validationDate);
                    Console.WriteLine("Document '{0}' has been modified earlier than {1}. Retention policy will be applied.", listItem.DisplayName, validationDate);
                    
                    //Apply your retention actions here: e.g. archive document, start a disposition workflow,...
                }
            }
        }
        #endregion


    }
}
