using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates.Impl.AzureDocDbRequestManager
{
    /// <summary>
    /// Implementation Class for working templates that are stored in Azure Document DB
    /// </summary>
    internal class AzureDocDbTemplateManager : AbstractModule, ISiteTemplateManager
    {
        #region Private Members
        const string DB_COLLECTION_ID = "SiteTemplateCollection";
        const string ACCOUNTENDPOINT_KEY = "AccountEndpoint";
        const string ACCOUNT_KEY = "AccountKey";
        

        /// <summary>
        /// Internal Member that is used to get the Properties to Connect to Azure DocumentDB
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetContainerValues()
        {
            if (string.IsNullOrWhiteSpace(this.ConnectionString)) throw new Exception("ConnectionString is null");
            if (string.IsNullOrWhiteSpace(this.Container)) throw new Exception("ConnectionString is null");

            var connectionProps = this.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var dict = connectionProps
                 .Select(x => x.Split('='))
                 .ToDictionary(i => i[0], i => i[1]);

            return dict;
        }
        #endregion

        /// <summary>
        /// Delegate for working with DocumentClient and Wire up authentication
        /// </summary>
        /// <param name="action"></param>
        private void UsingContext(Action<DocumentClient> action)
        {
            var dict = this.GetContainerValues();
            var _url = new Uri(dict[ACCOUNTENDPOINT_KEY]);
            var _authKey = dict[ACCOUNT_KEY];

            using (DocumentClient _client = new DocumentClient(_url, _authKey))
            {
                action(_client);
            }
        }

        public Template GetTemplateByName(string title)
        {
            Template _returnResult = null;
            UsingContext(client =>
            {
                try
                {
                    Task<Task<Database>> _taskResult = Task.FromResult<Task<Database>>(this.GetOrCreateDatabaseAsync(client));
                    Database _db;
                    if (!_taskResult.IsFaulted)
                    {
                        if (!_taskResult.Result.IsFaulted)
                        {
                            _db = _taskResult.Result.Result;
                            var _dbCollectionTasks = Task.FromResult(this.GetOrCreateCollectionAsync(client, _db.SelfLink, DB_COLLECTION_ID));
                            if (!_dbCollectionTasks.Result.IsFaulted)
                            {
                                _returnResult = this.InternalGetTemplateByName(client, _dbCollectionTasks.Result.Result.DocumentsLink, title);
                            }
                        }
                    }
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Log.Error("AzureDocDbRequestManager.GetTemplateByName", "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception ex)
                {
                    Exception baseException = ex.GetBaseException();
                    Log.Error("AzureDocDbRequestManager.GetTemplateByName", "Error: {0}, Message: {1}", ex.Message, baseException.Message);
                }
            });
            return _returnResult;
        }

        public List<Template> GetAvailableTemplates()
        {
            List<Template> _returnResult = new List<Template>();
            UsingContext(client =>
            {
                try
                {
                    Task<Task<Database>> _taskResult = Task.FromResult<Task<Database>>(this.GetOrCreateDatabaseAsync(client));
                    Database _db;
                    if (!_taskResult.IsFaulted)
                    {
                        if (!_taskResult.Result.IsFaulted)
                        {
                            _db = _taskResult.Result.Result;
                            var _dbCollectionTasks = Task.FromResult(this.GetOrCreateCollectionAsync(client, _db.SelfLink, DB_COLLECTION_ID));
                            if (!_dbCollectionTasks.Result.IsFaulted)
                            {
                                _returnResult = this.InternalGetAvailableTemplates(client, _dbCollectionTasks.Result.Result.DocumentsLink);
                            }
                        }
                    }
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Log.Error("AzureDocDbRequestManager", "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception ex)
                {
                    Exception baseException = ex.GetBaseException();
                    Log.Error("AzureDocDbRequestManager", "Error: {0}, Message: {1}", ex.Message, baseException.Message);
                }
            });
            return _returnResult;
        }

        public List<Template> GetSubSiteTemplates()
        {
            throw new NotImplementedException();
        }

        public ProvisioningTemplate GetProvisioningTemplate(string name)
        {
            try
            {
                ReflectionManager _reflectionHelper = new ReflectionManager();
                var _provider = _reflectionHelper.GetTemplateProvider(ModuleKeys.PROVISIONINGPROVIDER_KEY);
                var _pt = _provider.GetTemplate(name);
                return _pt;
            }
            catch (Exception _ex)
            {
                var _message = string.Format(PCResources.TemplateProviderBase_Exception_Message, _ex.Message);
                Log.Error("Provisioning.Common.Data.Templates.Impl.AzureDocDbTemplateManager", PCResources.TemplateProviderBase_Exception_Message, _ex);
                throw new DataStoreException(_message, _ex);
            }
        }

        private List<Template> InternalGetAvailableTemplates(DocumentClient client, string collectionLink)
        {
            List<Template> _returnResults = new List<Template>();
            var _templates = from record in client.CreateDocumentQuery<Template>(collectionLink)
                               where record.Enabled == true
                               select record;

            _returnResults = _templates.ToList();

            return _returnResults;
        }
        private Template InternalGetTemplateByName(DocumentClient client, string collectionLink, string name)
        {
             var template  = from record in client.CreateDocumentQuery<Template>(collectionLink)
                           where record.Title == name
                           select record;
            return template.ToList().FirstOrDefault();
        }

        /// <summary>
        /// Internal Member to Get or Create an Azure DocumentDB
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<Database> GetOrCreateDatabaseAsync(DocumentClient client)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == this.Container).ToArray().FirstOrDefault();
            if (database == null)
            {
                Log.Info("AzureDocDbRequestManager.GetOrCreateDatabaseAsync", "Created Document DB {0}", this.Container);
                database = await client.CreateDatabaseAsync(new Database { Id = this.Container });
            }
            return database;
        }

        /// <summary>
        /// Internal Member to get or return a Azure DocumentDB Collection
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dbLink"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<DocumentCollection> GetOrCreateCollectionAsync(DocumentClient client, string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }
            return collection;
        }
    }
}
