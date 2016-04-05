using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Provisioning.Common.Metadata;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Metadata.Impl
{
    class AzureDocDbMetadataManager : AbstractModule, IMetadataManager
    {
        #region private instance members
        const string DB_COLLECTION_ID = "SiteClassificationCollection";
        const string ACCOUNTENDPOINT_KEY = "AccountEndpoint";
        const string ACCOUNT_KEY = "AccountKey";
        #endregion

        #region Authentication
        /// <summary>
        /// Delegate for working with DocumentClient and Wire up authentication
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<DocumentClient> action)
        {
            var dict = this.GetContainerValues();
            var _url = new Uri(dict[ACCOUNTENDPOINT_KEY]);
            var _authKey = dict[ACCOUNT_KEY];

            using (DocumentClient _client = new DocumentClient(_url, _authKey))
            {
                action(_client);
            }
        }
        #endregion

        #region IMetadataManger Members
        /// <summary>
        /// Returns a Collection of Site Classification Objects.
        /// </summary>
        /// <returns></returns>
        public ICollection<SiteClassification> GetAvailableSiteClassifications()
        {
            ICollection<SiteClassification> _returnResult = new List<SiteClassification>();
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
                               _returnResult = this.InternalGetAvailableSiteClassifcations(client, _dbCollectionTasks.Result.Result.DocumentsLink);
                            }
                        }
                    }
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.GetSiteClassification", "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception ex)
                {
                    Exception baseException = ex.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.GetSiteClassification", "Error: {0}, Message: {1}", ex.Message, baseException.Message);
                }
            });

            return _returnResult;
        }

        /// <summary>
        /// Gets a Site Classification by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SiteClassification GetSiteClassificationByName(string name)
        {
            SiteClassification _returnResult = new SiteClassification();

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
                                _returnResult = this.InternalGetSiteClassificationByName(client, _dbCollectionTasks.Result.Result.DocumentsLink, name);
                            }
                        }
                    }
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.GetSiteClassificationByName", "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception ex)
                {
                    Exception baseException = ex.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.GetSiteClassificationByName", "Error: {0}, Message: {1}", ex.Message, baseException.Message);
                }
            });

            return _returnResult;
        }

        public void UpdateSiteClassification(SiteClassification classification)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new Site Classification Object in the repository
        /// </summary>
        /// <param name="classification"></param>
        public void CreateNewSiteClassification(SiteClassification classification)
        {
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
                                var _exist = this.InternalGetSiteClassificationByName(client, _dbCollectionTasks.Result.Result.SelfLink, classification.Key);
                                if(_exist == null)
                                {
                                    var doc = this.SaveNewRequest(client, _dbCollectionTasks.Result.Result.SelfLink, classification);
                                }
                            }
                        }
                        else
                        {
                            //if faulted write log
                        }
                    }
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.CreateNewSiteClassification", "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception ex)
                {
                    Exception baseException = ex.GetBaseException();
                    Log.Error("AzureDocDbMetadataManager.CreateNewSiteClassification", "Error: {0}, Message: {1}", ex.Message, baseException.Message);
                }
            });
        }
        #endregion

        #region Private Members
        private List<SiteClassification> InternalGetAvailableSiteClassifcations(DocumentClient client, string collectionLink)
        {
            List<SiteClassification> _returnResults = new List<SiteClassification>();
            var _siteClassifications = from record in client.CreateDocumentQuery<SiteClassification>(collectionLink)
                             where record.Enabled == true
                             select record;
            _returnResults = _siteClassifications.ToList();
            return _returnResults;
        }

        /// <summary>
        /// Internal Member to save a new site request
        /// </summary>
        /// <param name="client"></param>
        /// <param name="collectionLink"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private Document SaveNewRequest(DocumentClient client, string collectionLink, SiteClassification info)
        {
            var saveDocumentTask = Task.FromResult(client.CreateDocumentAsync(collectionLink, info));
            Document document = null;
            if (!saveDocumentTask.IsFaulted)
            {
                document = saveDocumentTask.Result.Result;
            }

            return document;
        }

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
        /// Internal Member to Get or Create an Azure DocumentDB
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<Database> GetOrCreateDatabaseAsync(DocumentClient client)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == this.Container).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = this.Container });
                Log.Info("AzureDocDbMetadataManager.GetOrCreateDatabaseAsync", "Created Document DB {0}", this.Container);
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
                Log.Info("AzureDocDbMetadataManager.GetOrCreateCollectionAsync", "Created Document DB Collection {0}", id);
            }
            return collection;
        }
        private SiteClassification InternalGetSiteClassificationByName(DocumentClient client, string collectionLink, string name)
        {
            var _siteClassification = from record in client.CreateDocumentQuery<SiteClassification>(collectionLink)
                           where record.Key == name
                           select record;
            return _siteClassification.ToList().FirstOrDefault();
        }
        public ICollection<SiteMetadata> GetAvailableOrganizationalFunctions() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableRegions() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableDivisions() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableTimeZones() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableSiteRegions() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableLanguages() { return new List<SiteMetadata>(); }
        public ICollection<SiteMetadata> GetAvailableBusinessUnits() { return new List<SiteMetadata>(); }

        public bool DoesUserHavePermissions()
        {
            throw new NotImplementedException();
        }

        public void UpdateSharingPropertyBag(string status)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePropertyBag(string property, string status)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyBagValue(string property, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePropertyBagItem(string property, string status)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyBagItem(string property, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public SiteEditMetadata GetSiteMetadata(SiteEditMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public SiteEditMetadata SetSiteMetadata(SiteEditMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public SiteEditMetadata SetSitePolicy(SiteEditMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
