using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Framework.TimerJob
{
    /// <summary>
    /// Event arguments for the TimerJobRun event
    /// </summary>
    public class TimerJobRunEventArgs: EventArgs
    {
        public ClientContext SiteClientContext;
        public ClientContext WebClientContext;
        public string Url;
        public DateTime? PreviousRun;
        public bool? PreviousRunSuccessful;
        public bool? CurrentRunSuccessful;
        public string PreviousRunVersion;
        public Dictionary<String, String> Properties;

        /// <summary>
        /// Constructor used when state is being managed by the timer job framework
        /// </summary>
        /// <param name="Url">Url of the site the timer job is operating against</param>
        /// <param name="SiteClientContext">ClientContext object for the root site of the site collection</param>
        /// <param name="WebClientContext">ClientContext object for passed site Url</param>
        /// <param name="previousRun">Datetime of the last run</param>
        /// <param name="previousRunSuccessful">Bool showing if the previous run was successful</param>
        /// <param name="previousRunVersion">Version of the timer job that was used for the previous run</param>
        /// <param name="properties">Custom keyword value collection that can be used to persist custom properties</param>
        internal TimerJobRunEventArgs(string url, ClientContext siteClientContext, ClientContext webClientContext, DateTime? previousRun, bool? previousRunSuccessful, string previousRunVersion, Dictionary<String, String> properties)
        {
            this.Url = url;
            this.SiteClientContext = siteClientContext;
            this.WebClientContext = webClientContext;
            this.PreviousRun = previousRun;
            this.PreviousRunSuccessful = previousRunSuccessful;
            this.PreviousRunVersion = previousRunVersion;
            this.Properties = properties;
        }
        /// <summary>
        /// Constructor used when state is not managed
        /// </summary>
        /// <param name="Url">Url of the site the timer job is operating against</param>
        /// <param name="SiteClientContext">ClientContext object for the root site of the site collection</param>
        /// <param name="WebClientContext">ClientContext object for passed site Url</param>
        internal TimerJobRunEventArgs(string url, ClientContext ccSite, ClientContext ccWeb) : this (url, ccSite, ccWeb, null, null, null, null)
        {
        }

        /// <summary>
        /// Gets a property from the custom properties list
        /// </summary>
        /// <param name="propertyKey">Key of the property to retrieve</param>
        /// <returns>Value of the requested property</returns>
        public string GetProperty(string propertyKey)
        {
            if (Properties != null && Properties.ContainsKey(propertyKey))
            {
                return Properties[propertyKey];
            }
            return "";
        }

        /// <summary>
        /// Adds or updates a property in the custom properties list
        /// </summary>
        /// <param name="propertyKey">Key of the property to add or update</param>
        /// <param name="propertyValue">Value of the property to add or update</param>
        public void SetProperty(string propertyKey, string propertyValue)
        {
            if (Properties == null)
            {
                Properties = new Dictionary<string, string>();
            }

            if (!Properties.ContainsKey(propertyKey))
            {
                Properties.Add(propertyKey, propertyValue);
            }
            else
            {
                Properties[propertyKey] = propertyValue;
            }
        }

        /// <summary>
        /// Deletes a property from the custom property list
        /// </summary>
        /// <param name="propertyKey">Name of the property to delete</param>
        public void DeleteProperty(string propertyKey)
        {
            if (Properties != null && Properties.ContainsKey(propertyKey))
            {
                Properties.Remove(propertyKey);
            }
        }

    }
}
