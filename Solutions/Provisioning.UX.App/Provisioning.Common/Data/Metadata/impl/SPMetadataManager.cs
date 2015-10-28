using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Provisioning.Common.Authentication;
using System.Diagnostics;
using Provisioning.Common.Utilities;

namespace Provisioning.Common.Data.Metadata.impl
{
    /// <summary>
    /// Implementation Class for working Metadata Repository
    /// </summary>
    class SPMetadataManager : AbstractModule, ISharePointClientService, IMetadataManager
    {
        #region instance Members
        const string CAML_GET_ENABLED_CLASSIFICATIONS = "<View><Query><Where><Eq><FieldRef Name='SP_Enabled'/><Value Type='Text'>True</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";

        #endregion
        #region Properties
        /// <summary>
        /// Returns the implementation for AppOnlyAuthentication
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                var _auth = new AppOnlyAuthenticationSite();
               _auth.SiteUrl = this.ConnectionString;
               return _auth;
            }

        }
        #endregion
        #region ISharePointClientService
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = this.Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion

        #region IMetadataManager
        public ICollection<SiteClassification> GetAvailableSiteClassifications()
        {
            ICollection<SiteClassification> _returnResults = new List<SiteClassification>();
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                try
                {
                    var _web = ctx.Web;
                    ctx.Load(_web);
                    if (!_web.ListExists(SPDataConstants.LIST_TITLE_SITECLASSIFICATION))
                    {
                        var _message = String.Format("The List {0} does not exist in Site {1}",
                         SPDataConstants.LIST_TITLE_SITECLASSIFICATION,
                         ctx.Url);

                        Log.Fatal("SPMetadataManager.GetAvailableSiteClassifications", _message);
                        throw new DataStoreException(_message);
                    }
                  
                    var _camlQuery = new CamlQuery();
                    _camlQuery.ViewXml = CAML_GET_ENABLED_CLASSIFICATIONS;

                    var _list = ctx.Web.Lists.GetByTitle(SPDataConstants.LIST_TITLE_SITECLASSIFICATION);
                    var _listItemCollection = _list.GetItems(_camlQuery);
                    ctx.Load(_listItemCollection,
                        eachItem => eachItem.Include(
                            item => item,
                            item => item["ID"],
                            item => item["SP_Key"],
                            item => item["SP_Value"],
                            item => item["SP_DisplayOrder"],
                            item => item["SP_Enabled"],
                            item => item["SP_SiteExpirationMonths"],
                            item => item["SP_AddAllAuthenticatedUsers"]));
                    ctx.ExecuteQuery();
                 
                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "SPMetadataManager.GetAvailableSiteClassifications", _timespan.Elapsed);
   
                    foreach (ListItem _item in _listItemCollection)
                    {
                        var _classification = new SiteClassification()
                        {
                            Id  = _item.BaseGetInt("ID"),
                            Key = _item.BaseGet("SP_Key"),
                            Value = _item.BaseGet("SP_Value"),
                            DisplayOrder = _item.BaseGetInt("SP_DisplayOrder"),
                            Enabled = _item.BaseGet<bool>("SP_Enabled"),
                            ExpirationMonths = _item.BaseGetInt("SP_SiteExpirationMonths"),
                            AddAllAuthenticatedUsers = _item.BaseGet<bool>("SP_AddAllAuthenticatedUsers"),
                        };
                        _returnResults.Add(_classification);
                    }
                 
                  }
                catch(ServerException ex)
                {
                   //TODO LOG
                }
                catch(DataStoreException ex)
                {
                    throw;
                }
              
            });
            return _returnResults;
        }

        public SiteClassification GetSiteClassificationByName(string name)
        {
            throw new NotImplementedException();
        }

        public void CreateNewSiteClassification(SiteClassification classification)
        {
            throw new NotImplementedException();
        }

        public void UpdateSiteClassification(SiteClassification classification)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
