using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Framework.TimerJob.Enums;
using OfficeDevPnP.Framework.TimerJob.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;

namespace OfficeDevPnP.Framework.TimerJob
{
    #region Delegates
    /// <summary>
    /// TimerJobRun delegate
    /// </summary>
    /// <param name="sender">calling object instance</param>
    /// <param name="e">TimerJobRunEventArgs event arguments instance</param>
    public delegate void TimerJobRunHandler(object sender, TimerJobRunEventArgs e);
    #endregion

    /// <summary>
    /// Abstract base class for creating timer jobs (background processes) that operate against SharePoint sites. These timer jobs 
    /// are designed to use the CSOM API and thus can run on any server that can communicate with SharePoint.
    /// </summary>
    public abstract class TimerJob
    {
        #region Private Variables
        // Timerjob information
        private string name;
        private string version;
        private bool isRunning = false;
        // property management information
        private bool manageState = false;
        // Logging related variables
        private const string LOGGING_SOURCE = "Core.TimerJobs";
        // Authentication related variables
        private Dictionary<string, AuthenticationManager> authenticationManagers;
        private AuthenticationType authenticationType;
        private string tenantName;
        private string username;
        private string password;
        private string domain;
        private string realm;
        private string clientId;
        private string clientSecret;
        private int sharePointVersion = 16;
        private string enumerationUser;
        private string enumerationPassword;
        private string enumrationDomain;
        // Site scope variables
        private List<string> requestedSites;
        private List<string> sitesToProcess;
        private bool expandSubSites = false;
        // Threading
        private static int numerOfThreadsNotYetCompleted;
        private static ManualResetEvent doneEvent;
        private bool useThreading = true;
        private int maximumThreads = 5;
        #endregion

        #region Events
        /// <summary>
        /// TimerJobRun event
        /// </summary>
        public event TimerJobRunHandler TimerJobRun;
        #endregion

        #region Constructor
        /// <summary>
        /// Simpliefied constructor for timer job, version is always set to "1.0"
        /// </summary>
        /// <param name="name">Name of the timer job</param>
        public TimerJob(string name)
            : this(name, "1.0")
        {
        }

        /// <summary>
        /// Default constructor for timer job
        /// </summary>
        /// <param name="name">Name of the timer job</param>
        /// <param name="version">Version of the timer job</param>
        public TimerJob(string name, string version)
        {
            this.name = name;
            this.version = version;
            this.requestedSites = new List<string>(10);
            this.sharePointVersion = GetSharePointVersion();
            
            // Default authentication model will be Office365
            this.authenticationType = Enums.AuthenticationType.Office365;
            this.authenticationManagers = new Dictionary<string, AuthenticationManager>();

            Log.Info(LOGGING_SOURCE, "Timer job constructed with name {0}, version {1}", this.name, this.version);
        }
        #endregion

        #region Job information & state management
        /// <summary>
        /// Gets the name of this timer job
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the version of this timer job
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }
        }

        /// <summary>
        /// Gets and sets the state management value: when true the timer job will automatically handle state by 
        /// storing a json serialized class as a web property bag entry. Default value is false
        /// </summary>
        public bool ManageState
        {
            get
            {
                return this.manageState;
            }
            set
            {
                this.manageState = value;
                Log.Info(LOGGING_SOURCE, "Manage state set to {0}", this.manageState);
            }
        }

        /// <summary>
        /// Is this timer job running?
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        /// <summary>
        /// Can this timer job use multiple threads. Defaults to true
        /// </summary>
        public bool UseThreading
        {
            get
            {
                return this.useThreading;
            }
            set
            {
                this.useThreading = value;
                Log.Info(LOGGING_SOURCE, "UseThreading set to {0}", this.useThreading);
            }
        }

        /// <summary>
        /// How many threads can be used by this timer job. Default value is 5.
        /// </summary>
        public int MaximumThreads
        {
            get
            {
                return this.maximumThreads;
            }
            set
            {
                if (value > 100)
                {
                    throw new ArgumentException("You cannot use more than 100 threads.");
                }

                if (value == 1)
                {
                    throw new ArgumentException("If you only want 1 thread then set the UseThreading property to false.");
                }
                else if (value < 1)
                {
                    throw new ArgumentException("Number of threads must be between 2 and 100.");
                }

                this.maximumThreads = value;
                Log.Info(LOGGING_SOURCE, "MaximumThreads set to {0}", this.maximumThreads);
            }
        }
        #endregion

        #region Run job
        /// <summary>
        /// Triggers the timer job to start running
        /// </summary>
        public void Run()
        {
            try
            {
                Log.Info(LOGGING_SOURCE, "Run of timer job has started");

                //mark the job as running
                this.isRunning = true;

                // This method call doesn't do anything but allows the inheriting task to override the passed list of requested sites
                Log.Info(LOGGING_SOURCE, "Before calling the virtual UpdateAddedSites method. Current count of site url's = {0}", requestedSites.Count);
                this.requestedSites = UpdateAddedSites(requestedSites);
                Log.Info(LOGGING_SOURCE, "After calling the virtual UpdateAddedSites method. Current count of site url's = {0}", requestedSites.Count);

                // Prepare the list of sites to process. This will resolve the wildcard site url's to a list of actual url's
                Log.Info(LOGGING_SOURCE, "Before calling the virtual ResolveAddedSites method. Current count of site url's = {0}", requestedSites.Count);
                this.sitesToProcess = ResolveAddedSites(this.requestedSites);
                Log.Info(LOGGING_SOURCE, "After calling the virtual ResolveAddedSites method. Current count of site url's = {0}", this.sitesToProcess.Count);

                // No sites to process...we're done
                if (this.sitesToProcess.Count == 0)
                {
                    // Job ended, so set isrunning accordingly
                    this.isRunning = false;
                    Log.Warning(LOGGING_SOURCE, "Job does not have sites to process, bailing out.");
                    return;
                }

                // We're using multiple threads, the default option
                if (useThreading)
                {
                    // Divide the workload in batches based on the maximum number of threads that we want
                    List<List<string>> batchWork = CreateWorkBatches();

                    // Determine the number of threads we'll spin off. Will be less or equal to the set maximum number of threads
                    numerOfThreadsNotYetCompleted = batchWork.Count;
                    // Prepare the reset event for indicating thread completion
                    doneEvent = new ManualResetEvent(false);

                    Log.Info(LOGGING_SOURCE, "Ready to start a thread for each of the {0} work batches.", batchWork.Count);
                    // execute an thread per batch
                    foreach (List<string> batch in batchWork)
                    {
                        // add thread to queue 
                        ThreadPool.QueueUserWorkItem(o => DoWorkBatch(batch));
                        Log.Info(LOGGING_SOURCE, "Thread launched for processing {0} sites", batch.Count);
                    }

                    // Wait for all threads to finish
                    doneEvent.WaitOne();
                    Log.Info(LOGGING_SOURCE, "Done processing the {0} work batches", batchWork.Count);
                }
                else
                {

                    Log.Info(LOGGING_SOURCE, "Ready to process each of the {0} sites in a sequential manner.", this.sitesToProcess.Count);
                    // No threading, just execute an event per site
                    foreach (string site in this.sitesToProcess)
                    {
                        DoWork(site);
                    }
                    Log.Info(LOGGING_SOURCE, "Done with processing each of the {0} sites.", this.sitesToProcess.Count);
                }
            }
            finally
            {
                // Job ended, so set isrunning accordingly
                this.isRunning = false;
                Log.Info(LOGGING_SOURCE, "Run of timer job has ended");
            }
        }

        /// <summary>
        /// Processes the amount of work that will be done by one thread
        /// </summary>
        /// <param name="sites">Batch of sites that the thread will need to process</param>
        private void DoWorkBatch(List<string> sites)
        {
            try
            {
                // Call our work routine per site in the passed batch of sites
                foreach (string site in sites)
                {
                    DoWork(site);
                }
            }
            finally
            {
                // Decrement counter in a thread safe manner
                if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
                {
                    // we're done, all threads have ended, signal that this was the last thread that ended
                    doneEvent.Set();
                }
            }
        }

        /// <summary>
        /// Processes the amount of work that will be done for a single site/web
        /// </summary>
        /// <param name="site">Url of the site to process</param>
        private void DoWork(string site)
        {
            Log.Info(LOGGING_SOURCE, "Doing work for site {0}.", site);

            // Get the root site of the passed site
            string rootSite = GetRootSite(site);
            
            // Instantiate the needed ClientContext objects
            ClientContext ccWeb = CreateClientContext(site);
            ClientContext ccSite = null;

            if (rootSite.Equals(site, StringComparison.InvariantCultureIgnoreCase))
            {
                ccSite = ccWeb;
            }
            else
            {
                ccSite = CreateClientContext(rootSite);
            }

            // Prepare the timerjob callback event arguments
            TimerJobRunEventArgs e = new TimerJobRunEventArgs(site, ccSite, ccWeb, null, null, "", new Dictionary<string, string>());

            // Trigger the event to fire, but only when there's an event handler connected
            if (TimerJobRun != null)
            {
                OnTimerJobRun(e);
            }
            else
            {
                Log.Warning(LOGGING_SOURCE, "No event receiver connected to the TimerJobRun event.");
            }

            Log.Info(LOGGING_SOURCE, "Work for site {0} done.", site);
        }

        /// <summary>
        /// Triggers the event to fire and deals with all the pre/post processing needed to automatically manage state
        /// </summary>
        /// <param name="e">TimerJobRunEventArgs event arguments class that will be passed to the event handler</param>
        private void OnTimerJobRun (TimerJobRunEventArgs e)
        {
            try
            {
                // Copy for thread safety?
                TimerJobRunHandler timerJobRunHandlerThreadCopy = TimerJobRun;
                if (timerJobRunHandlerThreadCopy != null)
                {
                    PropertyValues props = null;
                    JavaScriptSerializer s = null;

                    // if state is managed then the state value is stored in a property named "<timerjobname>_Properties"
                    string propertyKey = String.Format("{0}_Properties", NormalizedTimerJobName(this.name));

                    // read the properties from the web property bag
                    if (this.manageState)
                    {
                        props = e.webClientContext.Web.AllProperties;
                        e.webClientContext.Load(props);
                        e.webClientContext.ExecuteQueryRetry();

                        s = new JavaScriptSerializer();

                        // we've found previously stored state, so this is not the first timer job run
                        if (props.FieldValues.ContainsKey(propertyKey))
                        {
                            string timerJobProps = props.FieldValues[propertyKey].ToString();

                            // We should have a value, but you never know...
                            if (!string.IsNullOrEmpty(timerJobProps))
                            {
                                Log.Info(LOGGING_SOURCE, "Timerjob properties read using key {0} for site {1}", propertyKey, e.url);

                                // Deserialize the json string into a TimerJobRun class instance
                                TimerJobRun timerJobRunProperties = s.Deserialize<TimerJobRun>(timerJobProps);

                                // Pass the state information as part of the event arguments
                                if (timerJobRunProperties != null)
                                {
                                    e.PreviousRun = timerJobRunProperties.PreviousRun;
                                    e.PreviousRunSuccessful = timerJobRunProperties.PreviousRunSuccessful;
                                    e.PreviousRunVersion = timerJobRunProperties.PreviousRunVersion;
                                    e.Properties = timerJobRunProperties.Properties;

                                    Log.Info(LOGGING_SOURCE, "Timerjob for site {1}, PreviousRun = {0}", e.PreviousRun, e.url);
                                    Log.Info(LOGGING_SOURCE, "Timerjob for site {1}, PreviousRunSuccessful = {0}", e.PreviousRunSuccessful, e.url);
                                    Log.Info(LOGGING_SOURCE, "Timerjob for site {1}, PreviousRunVersion = {0}", e.PreviousRunVersion, e.url);
                                }
                            }
                        }                        
                    }

                    Log.Info(LOGGING_SOURCE, "Calling the eventhandler for site {0}", e.url);
                    // trigger the event
                    timerJobRunHandlerThreadCopy(this, e);
                    Log.Info(LOGGING_SOURCE, "Eventhandler called for site {0}", e.url);

                    // Update and store the properties to the web property bag
                    if (this.manageState)
                    {
                        // Retrieve the values of the event arguments and complete them with defaults
                        TimerJobRun timerJobRunProperties = new TimerJobRun()
                        {
                            PreviousRun = DateTime.Now,
                            PreviousRunSuccessful = e.CurrentRunSuccessful,
                            PreviousRunVersion = this.version,
                            Properties = e.Properties,
                        };

                        Log.Info(LOGGING_SOURCE, "Set Timerjob for site {1}, PreviousRun to {0}", timerJobRunProperties.PreviousRun, e.url);
                        Log.Info(LOGGING_SOURCE, "Set Timerjob for site {1}, PreviousRunSuccessful to {0}", timerJobRunProperties.PreviousRunSuccessful, e.url);
                        Log.Info(LOGGING_SOURCE, "Set Timerjob for site {1}, PreviousRunVersion to {0}", timerJobRunProperties.PreviousRunVersion, e.url);

                        // Serialize to json string
                        string timerJobProps = s.Serialize(timerJobRunProperties);

                        props = e.webClientContext.Web.AllProperties;

                        // Get the value, if the web properties are already loaded
                        if (props.FieldValues.Count > 0)
                        {
                            props[propertyKey] = timerJobProps;
                        }
                        else
                        {
                            // Load the web properties
                            e.webClientContext.Load(props);
                            e.webClientContext.ExecuteQueryRetry();

                            props[propertyKey] = timerJobProps;
                        }

                        // Persist the web property bag entries
                        e.webClientContext.Web.Update();
                        e.webClientContext.ExecuteQueryRetry();
                        Log.Info(LOGGING_SOURCE, "Timerjob properties written using key {0} for site {1}", propertyKey, e.url);
                    }

                }
            }
            catch(Exception ex)
            {
                // Catch error in this case as we don't want to the whole program to terminate if one single site operation fails
                Log.Error(LOGGING_SOURCE, "Error during timerjob execution of site {0}. Exception message = {1}", e.url, ex.Message);
            }
        }

        /// <summary>
        /// Creates batches of sites to process. Batch size is based on max number of threads
        /// </summary>
        /// <returns>List of Lists holding the work batches</returns>
        private List<List<string>> CreateWorkBatches()
        {
            // How many batches do we need, can't have more batches then sites to process
            int numberOfBatches = Math.Min(this.sitesToProcess.Count, this.maximumThreads);
            // Size of batch
            int batchCount = (this.sitesToProcess.Count / numberOfBatches);
            // Increase batch size by 1 to avoid the last batch being overloaded, rahter spread out over all batches and make the last batch smaller
            if (this.sitesToProcess.Count % numberOfBatches != 0)
            {
                batchCount++;
            }

            // Initialize batching variables
            List<List<string>> batches = new List<List<string>>(numberOfBatches);
            List<string> sitesBatch = new List<string>(batchCount);
            int batchCounter = 0;
            int batchesAdded = 1;

            for (int i = 0; i < this.sitesToProcess.Count; i++)
            {
                sitesBatch.Add(this.sitesToProcess[i]);
                batchCounter++;
                
                // we've filled one batch, let's create a new one
                if (batchCounter == batchCount && batchesAdded < numberOfBatches)
                {
                    batches.Add(sitesBatch);
                    batchesAdded++;
                    sitesBatch = new List<string>(batchCount);
                    batchCounter = 0;
                }
            }

            // add the last batch to the list of batches
            if (sitesBatch.Count > 0)
            {
                batches.Add(sitesBatch);
            }

            return batches;
        }
        #endregion

        #region Authentication methods and attributes

        /// <summary>
        /// Gets the authentication type that the timer job will use. This will be set as part 
        /// of the UseOffice365Authentication and UseNetworkCredentialsAuthentication methods
        /// </summary>
        public AuthenticationType AuthenticationType
        {
            get
            {
                return this.authenticationType;
            }
        }

        /// <summary>
        /// Gets or sets the SharePoint version. Default value is detected based on the laoded CSOM assembly version, but can be overriden
        /// in case you want to for example use v16 assemblies in v15 (on-premises)
        /// </summary>
        public int SharePointVersion
        {
            get
            {
                return this.sharePointVersion;
            }
            set 
            { 
                if (value < 15 || value > 16)
                {
                    throw new ArgumentException("SharePoint version must be 15 or 16");
                }

                this.sharePointVersion = value;
                Log.Info(LOGGING_SOURCE, "SharePointVersion set to {0}", this.sharePointVersion);
            }
        }

        /// <summary>
        /// Prepares the timerjob to operate against Office 365 with user and password credentials. Sets AuthenticationType 
        /// to AuthenticationType.Office365
        /// </summary>
        /// <param name="tenantName">Shortname of tenant: bertonline for tenant bertonline.onmicrosoft.com</param>
        /// <param name="username">UPN of the user that will be used to operate the timer job work</param>
        /// <param name="password">Password of the user that will be used to operate the timer job work</param>
        public void UseOffice365Authentication(string tenantName, string userUPN, string password)
        {
            if (String.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentNullException("tenantName");
            } 
            
            if (String.IsNullOrEmpty(userUPN))
            {
                throw new ArgumentNullException("userName");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            this.authenticationType = Enums.AuthenticationType.Office365;
            this.tenantName = tenantName;
            this.username = userUPN;
            this.password = password;

            Log.Info(LOGGING_SOURCE, "Timer job authentication set to type Office 365 with user {0}", userUPN);
        }

        /// <summary>
        /// Prepares the timerjob to operate against SharePoint on-premises with user name password credentials. Sets AuthenticationType 
        /// to AuthenticationType.NetworkCredentials
        /// </summary>
        /// <param name="samAccountName">samAccontName of the windows user</param>
        /// <param name="password">Password of the windows user</param>
        /// <param name="domain">NT domain of the windows user</param>
        public void UseNetworkCredentialsAuthentication(string samAccountName, string password, string domain)
        {
            if (String.IsNullOrEmpty(samAccountName))
            {
                throw new ArgumentNullException("userName");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }

            this.authenticationType = Enums.AuthenticationType.NetworkCredentials;
            this.username = samAccountName;
            this.password = password;
            this.domain = domain;

            Log.Info(LOGGING_SOURCE, "Timer job authentication set to type NetworkCredentials with user {0} in domain {1}", samAccountName, domain);
        }

        /// <summary>
        /// Prepares the timerjob to operate against SharePoint on-premises with app-only credentials. Sets AuthenticationType 
        /// to AuthenticationType.AppOnly
        /// </summary>
        /// <param name="realm">Realm of the ACS tenant</param>
        /// <param name="clientId">Client ID of the app</param>
        /// <param name="clientSecret">Client Secret of the app</param>
        public void UseAppOnlyAuthentication(string realm, string clientId, string clientSecret)
        {
            if (String.IsNullOrEmpty(realm))
            {
                throw new ArgumentNullException("realm");
            }

            if (String.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (String.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret");
            }

            this.authenticationType = Enums.AuthenticationType.AppOnly;
            this.tenantName = "";
            this.realm = realm;
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            Log.Info(LOGGING_SOURCE, "Timer job authentication set to type App-Only with realm {1} and clientId {0}", clientId, realm);
        }

        /// <summary>
        /// Prepares the timerjob to operate against Office 365 with app-only credentials. Sets AuthenticationType 
        /// to AuthenticationType.AppOnly
        /// </summary>
        /// <param name="tenantName">Shortname of tenant: bertonline for tenant bertonline.onmicrosoft.com</param>
        /// <param name="realm">Realm of the ACS tenant</param>
        /// <param name="clientId">Client ID of the app</param>
        /// <param name="clientSecret">Client Secret of the app</param>
        public void UseAppOnlyAuthentication(string tenantName, string realm, string clientId, string clientSecret)
        {
            UseAppOnlyAuthentication(realm, clientId, clientSecret);

            if (String.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentNullException("tenantName");
            }
            this.tenantName = tenantName;
        }

        /// <summary>
        /// Get an AuthenticationManager instance per host url. Needed to make this work properly, else we're getting access denied 
        /// because of Invalid audience Uri
        /// </summary>
        /// <param name="url">url of the site</param>
        /// <returns>An instantiated AuthenticationManager</returns>
        private AuthenticationManager GetAuthenticationManager(string url)
        {
            // drop the wild card if still there
            Uri uri = new Uri(url.Replace("*", ""));

            if (this.authenticationManagers.ContainsKey(uri.Host))
            {
                return this.authenticationManagers[uri.Host];
            }
            else
            {
                AuthenticationManager am = new AuthenticationManager();
                this.authenticationManagers.Add(uri.Host, am);
                return am;
            }
        }
        #endregion

        #region Site scope methods and attributes
        /// <summary>
        /// Does the timerjob need to fire as well for every sub site in the site?
        /// </summary>
        public bool ExpandSubSites
        {
            get
            {
                return this.expandSubSites;
            }
            set
            {
                this.expandSubSites = value;
                Log.Info(LOGGING_SOURCE, "ExpandSubSites set to {0}", this.expandSubSites);
            }
        }

        /// <summary>
        /// Returns the user account used for enumaration. Enumeration is done using search and the search API requires a user context
        /// </summary>
        private string EnumerationUser
        {
            get
            {
                if (!String.IsNullOrEmpty(this.enumerationUser))
                {
                    return this.enumerationUser;
                }
                else if (!String.IsNullOrEmpty(this.username))
                {
                    return this.username;   
                }
                else
                {
                    throw new Exception("No user specified that can be used for site enumeration. Use the SetEnumeration... method to provide credentials as app-only does not work with search.");
                }
            }
        }

        /// <summary>
        /// Returns the password of the user account used for enumaration. Enumeration is done using search and the search API requires a user context
        /// </summary>
        private string EnumerationPassword
        {
            get
            {
                if (!String.IsNullOrEmpty(this.enumerationPassword))
                {
                    return this.enumerationPassword;
                }
                else if (!String.IsNullOrEmpty(this.password))
                {
                    return this.password;
                }
                else
                {
                    throw new Exception("No password specified that can be used for site enumeration. Use the SetEnumeration... method to provide credentials as app-only does not work with search.");
                }
            }
        }

        /// <summary>
        /// Returns the domain of the user account used for enumaration. Enumeration is done using search and the search API requires a user context
        /// </summary>
        private string EnumerationDomain
        {
            get
            {
                if (!String.IsNullOrEmpty(this.EnumerationDomain))
                {
                    return this.EnumerationDomain;
                }
                else if (!String.IsNullOrEmpty(this.domain))
                {
                    return this.domain;
                }
                else
                {
                    throw new Exception("No domain specified that can be used for site enumeration. Use the SetEnumerationNetworkCredentials method to provide credentials as app-only does not work with search.");
                }
            }
        }

        /// <summary>
        /// Provides the timer job with the enumeration credentials. For Office 365 username and password is sufficient
        /// </summary>
        /// <param name="username">UPN of the enumeration user</param>
        /// <param name="password">Password of the enumeration user</param>
        public void SetEnumerationCredentials(string userUPN, string password)
        {
            this.enumerationUser = userUPN;
            this.enumerationPassword = password;
            Log.Info(LOGGING_SOURCE, "Enumeration credentials specified for Office 365 enumeration with user {0}", userUPN);
        }

        /// <summary>
        /// Provides the timer job with the enumeration credentials. For SharePoint on-premises username, password and domain are needed
        /// </summary>
        /// <param name="username">UPN of the enumeration user</param>
        /// <param name="password">Password of the enumeration user</param>
        /// <param name="domain">Domain of the enumeration user</param>
        public void SetEnumerationCredentials(string samAccountName, string password, string domain)
        {
            this.enumerationUser = samAccountName;
            this.enumerationPassword = password;
            this.enumrationDomain = domain;
            Log.Info(LOGGING_SOURCE, "Enumeration credentials specified for on-premises enumeration with user {0} and demain {1}", samAccountName, domain);
        }

        /// <summary>
        /// Adds a site url or wildcard site url to the collection of sites that the timer job will process
        /// </summary>
        /// <param name="site">Site url or wildcard site url to be processed by the timer job</param>
        public void AddSite(string site)
        {
            if (String.IsNullOrEmpty(site))
            {
                throw new ArgumentNullException("site");
            }

            site = site.ToLower();

            if (!site.Contains("*"))
            {
                if (!IsValidUrl(site))
                {
                    throw new ArgumentException(string.Format("Site url ({0}) contains invalid characters", site), "site");
                }
            }

            if (!requestedSites.Contains(site))
            {
                this.requestedSites.Add(site);
                Log.Info(LOGGING_SOURCE, "Site {0} url/wildcard added", site);
            }            
        }

        /// <summary>
        /// Clears the list of added site url's and/or wildcard site url's
        /// </summary>
        public void ClearAddedSites()
        {
            this.requestedSites.Clear();
            Log.Info(LOGGING_SOURCE, "All added sites are cleared.");
        }

        /// <summary>
        /// Virtual method that can be overriden to allow the timer job itself to control the list of sites to operate against.
        /// Scenario is for example timer job that reads this data from a database instead of being fed by the calling program
        /// </summary>
        /// <param name="addedSites">List of added site url's and/or wildcard site url's</param>
        /// <returns>List of added site url's and/or wildcard site url's</returns>
        public virtual List<string> UpdateAddedSites(List<string> addedSites)
        {
            // Default behavior is just pass back the given list
            return addedSites;
        }

        /// <summary>
        /// Virtual method that can be overriden to control the list of resolved sites
        /// </summary>
        /// <param name="addedSites">List of added site url's and/or wildcard site url's</param>
        /// <returns>List of resolved sites</returns>
        public virtual List<string> ResolveAddedSites(List<string> addedSites)
        {
            Log.Info(LOGGING_SOURCE, "Resolving sites started");

            List<string> resolvedSites = new List<string>();

            // Step 1: obtain the list of all site collections
            foreach(string site in this.requestedSites)
            {
                if (site.Contains("*"))
                {
                    Log.Info(LOGGING_SOURCE, "Resolving wildcard site {0}", site);
                    // get the actual sites matching to the wildcard site url
                    ResolveSite(site, resolvedSites);
                    Log.Info(LOGGING_SOURCE, "Done resolving wildcard site {0}", site);
                }
                else
                {
                    resolvedSites.Add(site);
                }
            }

            // Step 2 (optional): If the job wants to run at sub site level then we'll need to resolve all sub sites
            if (expandSubSites)
            {
                List<string> resolvedSitesAndSubSites = new List<string>();

                // Prefered option is to use threading to increase the list resolving speed
                if (useThreading)
                {
                    // Split the sites to resolve in batches
                    List<List<string>> expandBatches = CreateExpandBatches(resolvedSites);

                    // Determine the number of threads we'll spin off. Will be less or equal to the maximum number of threads
                    numerOfThreadsNotYetCompleted = expandBatches.Count;
                    // Prepare the reset event for indicating thread completion
                    doneEvent = new ManualResetEvent(false);

                    Log.Info(LOGGING_SOURCE, "Expand subsites by lanuching a thread per {0} of the work batches", numerOfThreadsNotYetCompleted);
                    foreach (List<string> expandBatch in expandBatches)
                    {
                        // Launch a thread per batch of sites to expand
                        ThreadPool.QueueUserWorkItem(o => DoExpandBatch(expandBatch, resolvedSitesAndSubSites));
                        Log.Info(LOGGING_SOURCE, "Thread started to expand a batch of {0} sites", expandBatch.Count);
                    }

                    // Wait for all threads to finish
                    doneEvent.WaitOne();
                    Log.Info(LOGGING_SOURCE, "Done waiting for all site expanding threads");
                }
                else
                {
                    Log.Info(LOGGING_SOURCE, "Start sequentially expanding all sites");
                    // When no threading just sequentially expand the sub sites for each site collection
                    for (int i = 0; i < resolvedSites.Count; i++)
                    {
                        ExpandSite(resolvedSitesAndSubSites, resolvedSites[i]);
                    }
                    Log.Info(LOGGING_SOURCE, "Done sequentially expanding all sites");
                }

                Log.Info(LOGGING_SOURCE, "Resolving sites done, sub sites have been expanded");
                return resolvedSitesAndSubSites;
            }
            else
            {
                Log.Info(LOGGING_SOURCE, "Resolving sites done, no expansion needed");
                // no sub site resolving was needed, so just return the original list of resolved sites
                return resolvedSites;
            }
        }

        /// <summary>
        /// Processes one bach of sites to expand, whcih is the workload of one thread
        /// </summary>
        /// <param name="sites">Batch of sites to expand</param>
        /// <param name="resolvedSitesAndSubSites">List holding the expanded sites</param>
        private void DoExpandBatch(List<string> sites, List<string> resolvedSitesAndSubSites)
        {
            try
            {
                foreach (string site in sites)
                {
                    // perform the site expansion for a single site collection
                    ExpandSite(resolvedSitesAndSubSites, site);
                }
            }
            finally
            {
                // Decrement counter in a thread safe manner
                if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
                {
                    // we're done, all threads have ended, signal that this was the last thread that ended
                    doneEvent.Set();
                }
            }
        }

        /// <summary>
        /// Creates batches of sites to expand
        /// </summary>
        /// <param name="resolvedSites">List of sites to expand</param>
        /// <returns>List of list with batches of sites to expand</returns>
        private List<List<string>> CreateExpandBatches(List<string> resolvedSites)
        {
            // How many batches do we need, can't have more batches then sites to expand
            int numberOfBatches = Math.Min(resolvedSites.Count, this.maximumThreads);
            // Size of batch
            int batchCount = (resolvedSites.Count / numberOfBatches);
            // Increase batch size by 1 to avoid the last batch being overloaded, rahter spread out over all batches and make the last batch smaller
            if (resolvedSites.Count % numberOfBatches != 0)
            {
                batchCount++;
            }

            // Initialize batching variables
            List<List<string>> batches = new List<List<string>>(numberOfBatches);
            List<string> sitesBatch = new List<string>(batchCount);
            int batchCounter = 0;
            int batchesAdded = 1;

            for (int i = 0; i < resolvedSites.Count; i++)
            {
                sitesBatch.Add(resolvedSites[i]);
                batchCounter++;

                // we've filled one batch, let's create a new one
                if (batchCounter == batchCount && batchesAdded < numberOfBatches)
                {
                    batches.Add(sitesBatch);
                    batchesAdded++;
                    sitesBatch = new List<string>(batchCount);
                    batchCounter = 0;
                }
            }

            // add the last batch to the list of batches
            if (sitesBatch.Count > 0)
            {
                batches.Add(sitesBatch);
            }

            return batches;
        }

        /// <summary>
        /// Expands and individual site into sub sites
        /// </summary>
        /// <param name="resolvedSitesAndSubSites">list of sites and subsites resulting from the expanding</param>
        /// <param name="site">site to expand</param>
        private void ExpandSite(List<string> resolvedSitesAndSubSites, string site)
        {
            try
            {
                ClientContext ccExpand = CreateClientContext(site);
                IEnumerable<string> expandedSites = GetAllSubSites(ccExpand.Site);
                resolvedSitesAndSubSites.AddRange(expandedSites);
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Message.IndexOf("The remote server returned an error: (500) Internal Server Error") > -1)
                {
                    //eath the exception
                    Log.Warning(LOGGING_SOURCE, "Eating exception {0}", ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a ClientContext object based on the set AuthenticationType and the used version of SharePoint
        /// </summary>
        /// <param name="site">Site url to create a ClientContext for</param>
        /// <returns>The created ClientContext object. Returns null if no ClientContext was created</returns>
        private ClientContext CreateClientContext(string site)
        {
            if (SharePointVersion == 15)
            {
                if (AuthenticationType == Enums.AuthenticationType.NetworkCredentials)
                {
                    return GetAuthenticationManager(site).GetNetworkCredentialAuthenticatedContext(site, username, password, domain);
                }
                else if (AuthenticationType == Enums.AuthenticationType.AppOnly)
                {
                    return GetAuthenticationManager(site).GetAppOnlyAuthenticatedContext(site, this.realm, this.clientId, this.clientSecret);
                }
            }
            else
            {
                if (AuthenticationType == Enums.AuthenticationType.Office365)
                {
                    return GetAuthenticationManager(site).GetSharePointOnlineAuthenticatedContextTenant(site, username, password);
                }
                else if (AuthenticationType == Enums.AuthenticationType.AppOnly)
                {
                    return GetAuthenticationManager(site).GetAppOnlyAuthenticatedContext(site, this.realm, this.clientId, this.clientSecret);
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves a wildcard site url into a list of actual site url's
        /// </summary>
        /// <param name="site">Wildcard site url to resolve</param>
        /// <param name="resolvedSites">List of resolved site url's</param>
        private void ResolveSite(string site, List<string> resolvedSites)
        {
            if (SharePointVersion == 15)
            {
                //Good we can use search
                ClientContext ccEnumerate = GetAuthenticationManager(site).GetNetworkCredentialAuthenticatedContext(site, EnumerationUser, EnumerationPassword, EnumerationDomain);
                SiteEnumeration.Instance.ResolveSite(ccEnumerate, site, resolvedSites);
            }
            else
            {
                //Good, we can use search for user profile and tenant API enumeration for regular sites
                ClientContext ccEnumerate = GetAuthenticationManager(site).GetSharePointOnlineAuthenticatedContextTenant(GetTenantAdminSite(), EnumerationUser, EnumerationPassword);
                Tenant tenant = new Tenant(ccEnumerate);
                SiteEnumeration.Instance.ResolveSite(tenant, site, resolvedSites);
            }
        }

        /// <summary>
        /// Gets all sub sites for a given site
        /// </summary>
        /// <param name="site">Site to find all sub site for</param>
        /// <returns>IEnumerable of strings holding the sub site urls</returns>
        public IEnumerable<string> GetAllSubSites(Site site)
        {
            var siteContext = site.Context;
            siteContext.Load(site, s => s.Url);
            siteContext.ExecuteQueryRetry();
            var queue = new Queue<string>();
            queue.Enqueue(site.Url);
            while (queue.Count > 0)
            {
                var currentUrl = queue.Dequeue();
                using (var webContext = siteContext.Clone(currentUrl))
                {
                    webContext.Load(webContext.Web, web => web.Webs);
                    webContext.ExecuteQueryRetry();
                    foreach (var subWeb in webContext.Web.Webs)
                    {
                        if (!subWeb.WebTemplate.Equals("App", StringComparison.InvariantCultureIgnoreCase))
                        {
                            queue.Enqueue(subWeb.Url);
                        }
                    }
                }
                yield return currentUrl;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Verifies if the passed url has a valid structure
        /// </summary>
        /// <param name="url">Url to validate</param>
        /// <returns>True is valid, false otherwise</returns>
        private bool IsValidUrl(string url)
        {
            Uri uri;

            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the current SharePoint version based on the loaded assembly
        /// </summary>
        /// <returns></returns>
        private int GetSharePointVersion()
        {
            Assembly asm = Assembly.GetAssembly(typeof(Microsoft.SharePoint.Client.Site)); 
            return asm.GetName().Version.Major;
        }

        /// <summary>
        /// Gets the tenant admin site based on the tenant name provided when setting the authentication details
        /// </summary>
        /// <returns>The tenant admin site</returns>
        private string GetTenantAdminSite()
        {
            return String.Format("https://{0}-admin.sharepoint.com", this.tenantName);
        }

        /// <summary>
        /// Gets the root site for a given site url
        /// </summary>
        /// <param name="site">Site url</param>
        /// <returns>Root site url of the given site url</returns>
        private string GetRootSite(string site)
        {
            Uri uri = new Uri(site.TrimEnd(new[] { '/' }));

            //e.g. https://bertonline.sharepoint.com
            if (String.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/", StringComparison.InvariantCultureIgnoreCase))
            {
                // Site must be root site, no doubts possible
                return string.Format("{0}://{1}", uri.Scheme, uri.DnsSafeHost);
            }

            string[] siteParts = uri.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            // e.g. https://bertonline.sharepoint.com/sub1
            // e.g. https://bertonline.sharepoint.com/sub1/sub11/sub111
            // e.g. https://bertonline.sharepoint.com/sites/dev/sub1
            if (siteParts.Length == 1 || siteParts.Length > 2)
            {
                if (siteParts.Length == 1)
                {
                    return string.Format("{0}://{1}", uri.Scheme, uri.DnsSafeHost);
                }
                else
                {
                    if (siteParts[0].Equals("sites", StringComparison.InvariantCultureIgnoreCase) ||
                        siteParts[0].Equals("teams", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return string.Format("{0}://{1}/{2}/{3}", uri.Scheme, uri.DnsSafeHost, siteParts[0], siteParts[1]);
                    }
                    else
                    {
                        return string.Format("{0}://{1}", uri.Scheme, uri.DnsSafeHost);
                    }
                }
            }
            else
            {
                // e.g. https://bertonline.sharepoint.com/sub1/sub11
                // e.g. https://bertonline.sharepoint.com/sites/dev
                if (siteParts[0].Equals("sites", StringComparison.InvariantCultureIgnoreCase) ||
                    siteParts[0].Equals("teams", StringComparison.InvariantCultureIgnoreCase))
                {
                    // sites and teams are default managed paths, so assume this is a root site
                    return site;
                }
                else
                {
                    return string.Format("{0}://{1}", uri.Scheme, uri.DnsSafeHost);
                }
            }
        }

        /// <summary>
        /// Normalizes the timer job name
        /// </summary>
        /// <param name="timerJobName">Timer job name</param>
        /// <returns>Normalized timer job name</returns>
        private string NormalizedTimerJobName(string timerJobName)
        {
            return timerJobName.Replace(" ", "_");
        }
        #endregion
    }
}
