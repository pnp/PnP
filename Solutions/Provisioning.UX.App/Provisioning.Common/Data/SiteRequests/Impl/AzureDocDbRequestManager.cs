using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.SiteRequests.Impl
{
    class AzureDocDbRequestManager : AbstractModule, ISiteRequestManager
    {
        #region private instance members
        const string DB_COLLECTION_ID = "SiteRequestsCollection";
        const string ACCOUNTENDPOINT_KEY = "AccountEndpoint";
        const string ACCOUNT_KEY = "AccountKey";
        private static readonly IConfigurationFactory _cf = ConfigurationFactory.GetInstance();
        private static readonly IAppSettingsManager _manager = _cf.GetAppSetingsManager();
        #endregion

        /// <summary>
        /// Delegate for working with DocumentClient and Wire up authentication
        /// </summary>
        /// <param name="action"></param>
        public virtual void UsingContext(Action<DocumentClient> action)
        {
            var dict = this.GetContainerValues();
            var _url = new Uri(dict[ACCOUNTENDPOINT_KEY]);
            var _authKey = dict[ACCOUNT_KEY];

            using (DocumentClient _client = new DocumentClient(_url, _authKey))
            {
                action(_client);
            }
        }

        #region ISiteRequestManager Members
        public ICollection<SiteRequestInformation> GetOwnerRequests(string email)
        {
            List<SiteRequestInformation> _returnResults = new List<SiteRequestInformation>();
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
                                _returnResults = this.GetSiteRequestsbyEmail(client, _dbCollectionTasks.Result.Result.DocumentsLink, email);
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
            return _returnResults;
        }
        public void CreateNewSiteRequest(SiteRequestInformation siteRequest)
        {
            siteRequest.EnumStatus = SiteRequestStatus.New;
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
                                var _siteInfo = this.GetSiteRequestByUrl(client, _dbCollectionTasks.Result.Result.DocumentsLink, siteRequest.Url);
                                if (_siteInfo == null)
                                {
                                    var doc = this.SaveNewRequest(client, _dbCollectionTasks.Result.Result.SelfLink, siteRequest);
                                }
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
        }
      
        public SiteRequestInformation GetSiteRequestByUrl(string url)
        {
            SiteRequestInformation _returnResult = null;
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
                                _returnResult = this.GetSiteRequestByUrl(client, _dbCollectionTasks.Result.Result.DocumentsLink, url);
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

        public ICollection<SiteRequestInformation> GetNewRequests()
        {
            List<SiteRequestInformation> _returnResults = new List<SiteRequestInformation>();
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
                                _returnResults = this.GetSiteRequestsByStatus(client, _dbCollectionTasks.Result.Result.DocumentsLink, SiteRequestStatus.New);
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
            return _returnResults;
        }

        public ICollection<SiteRequestInformation> GetApprovedRequests()
        {
            List<SiteRequestInformation> _returnResults = new List<SiteRequestInformation>();
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
                                _returnResults = this.GetSiteRequestsByStatus(client, _dbCollectionTasks.Result.Result.DocumentsLink, SiteRequestStatus.Approved);
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
            return _returnResults;
        }

        public bool DoesSiteRequestExist(string url)
        {
            SiteRequestInformation _siteRequest = null;
            bool _returnResult = false;

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
                            var _dbCollectionTasks = Task.FromResult(this.GetOrCreateCollectionAsync(client,_db.SelfLink, DB_COLLECTION_ID));
                            if (!_dbCollectionTasks.Result.IsFaulted)
                            {
                                _siteRequest = this.GetSiteRequestByUrl(client, _dbCollectionTasks.Result.Result.DocumentsLink, url);
                                if (_siteRequest == null)
                                {
                                    _returnResult = false;
                                }
                                else
                                {
                                    _returnResult = true;
                                }
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

        public void UpdateRequestStatus(string url, SiteRequestStatus status)
        {
            this.UpdateRequestStatus(url, status, string.Empty);
        }

        public void UpdateRequestStatus(string url, SiteRequestStatus status, string statusMessage)
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
                                var _siteInfo = this.GetSiteRequestByUrl(client, _dbCollectionTasks.Result.Result.DocumentsLink, url);
                                if (_siteInfo != null)
                                {
                                    var doc = this.UpdateSiteRequestStatusByUrl(client, _dbCollectionTasks.Result.Result.SelfLink, _siteInfo, status, statusMessage);
                                }
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
        }
        #endregion

        #region Private Members

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

        /// <summary>
        /// Internal Member to Update Request and Status properties Azure DocumentDB by url 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="selfLink"></param>
        /// <param name="request"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Document UpdateSiteRequestStatusByUrl(DocumentClient client, string selfLink, SiteRequestInformation request, SiteRequestStatus status, string message)
        {

            dynamic requestDocument = client.CreateDocumentQuery<Document>(selfLink).Where(d => d.Id == request.Id).AsEnumerable().FirstOrDefault();
            Document _d = requestDocument; //Cast to Document to get the Selflink property
            Document _dReturnResult = null;
            SiteRequestInformation _requestToUpdate = requestDocument;
            if(!string.IsNullOrEmpty(message))
            {
                _requestToUpdate.RequestStatusMessage = message;
            }
            _requestToUpdate.EnumStatus = status;
            var savedRequestTask = Task.FromResult(client.ReplaceDocumentAsync(_d.SelfLink, _requestToUpdate));

            if (!savedRequestTask.Result.IsFaulted)
            {
                _dReturnResult = savedRequestTask.Result.Result;
            }
            return _dReturnResult;
        }
      
        private List<SiteRequestInformation> GetSiteRequestsbyEmail(DocumentClient client, string collectionLink, string email)
        {
            List<SiteRequestInformation> _returnResults = new List<SiteRequestInformation>();
            var siteRequests = from record in client.CreateDocumentQuery<SiteRequestInformation>(collectionLink)
                               where record.SiteOwner.Name == email
                               select record;

            _returnResults = siteRequests.ToList();

            return _returnResults;
        }

        /// <summary>
        /// Internal Member to Return a Site Request By Status
        /// </summary>
        /// <param name="client"></param>
        /// <param name="collectionLink"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private List<SiteRequestInformation> GetSiteRequestsByStatus(DocumentClient client, string collectionLink, SiteRequestStatus status)
        {
            List<SiteRequestInformation> _returnResults = new List<SiteRequestInformation>();
            var siteRequests = from record in client.CreateDocumentQuery<SiteRequestInformation>(collectionLink)
                               where record.RequestStatus == status.ToString()
                               select record;

            _returnResults = siteRequests.ToList();

            return _returnResults;
        }
        
        /// <summary>
        /// Internal Member to get a site Request by Url
        /// </summary>
        /// <param name="client"></param>
        /// <param name="collectionLink"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private SiteRequestInformation GetSiteRequestByUrl(DocumentClient client, string collectionLink, string url)
        { 
            var siteRequests  = from record in client.CreateDocumentQuery<SiteRequestInformation>(collectionLink)
                           where record.Url == url
                           select record;

            return siteRequests.ToList().FirstOrDefault();
        }
      
        /// <summary>
        /// Internal Member to save a new site request
        /// </summary>
        /// <param name="client"></param>
        /// <param name="collectionLink"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private Document SaveNewRequest(DocumentClient client, string collectionLink, SiteRequestInformation info )
        {
            //If Settings are set to autoapprove then automatically approve the requests
            if (_manager.GetAppSettings().AutoApprove)
            {
                 info.RequestStatus = SiteRequestStatus.Approved.ToString();
                 info.ApprovedDate = DateTime.Now;
            }
            else
            {
                info.RequestStatus = SiteRequestStatus.New.ToString();
            }
            info.SubmitDate = DateTime.Now;

            var saveDocumentTask = Task.FromResult(client.CreateDocumentAsync(collectionLink, info));
            Document document = null;
            if(!saveDocumentTask.IsFaulted)
            {
                document = saveDocumentTask.Result.Result;
            }

            return document;
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
                Log.Info("AzureDocDbRequestManager", "Created Document DB {0}", this.Container);
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
        #endregion 
    

     
    }
}
