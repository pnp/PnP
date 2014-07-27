namespace Contoso.Core.Mappers
{
    using Contoso.Core.Utilities;

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.DirectoryServices.Protocols;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// LDAP Connector
    /// </summary>
    public class LDAPConnector : BaseAction
    {
        #region Properties

        public string SPOAccountUPN
        {
            get;
            set;
        }

        public string ServerIP
        {
            get;
            set;
        }

        public string ServerName
        {
            get;
            set;
        }

        public string PortNumber
        {
            get;
            set;
        }

        public string SearchRoot
        {
            get;
            set;
        }

        public string BatchAction
        {
            get;
            set;
        }

        public string SPOClaimsString
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int QueryTimeout
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string LDAPAuthType
        {
            get;
            set;
        }

        public string DirectoryType
        {
            get;
            set;
        }

        public int ProtocolVers
        {
            get;
            set;
        }

        public string CertificatePath
        {
            get;
            set;
        }

        public int UserNameIndex
        {
            get;
            set;
        }

        public int DeltaPeriod
        {
            get;
            set;
        }

        private int _TotalUsers = 0;
        private int _TotalFailures = 0;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Iterates the row from the CSV file
        /// </summary>
        /// <param name="context">The ClientContext instance.</param>
        /// <param name="entries">The collection values per row.</param>
        /// <param name="logger">The logger.</param>
        public override void IterateCollection(Microsoft.SharePoint.Client.ClientContext context, System.Collections.ObjectModel.Collection<string> entries, LogHelper logger)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the LDAP logic
        /// </summary>
        /// <param name="parentAction">Inherit parent properties = null</param>
        /// <param name="CurrentTime">Locked program timestamp value</param>
        /// <param name="logger">The logger.</param>
        public override void Run(BaseAction parentAction, DateTime CurrentTime, LogHelper logger)
        {
            ExtractLDAPResults(logger, CurrentTime);
            logger.LogVerbose(string.Format("Successfully extracted {0} user objects from {1} with {2} failures", _TotalUsers, this.ServerName, _TotalFailures ));
        }

        /// <summary>
        /// Performs LDAP Search and extracts attributes.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="CurrentTime">Locked program timestamp value</param>
        private void ExtractLDAPResults(LogHelper logger, DateTime CurrentTime)
        {
            List<string> AttributesToAdd = new List<string>();
            foreach (PropertyBase item in this.Properties)
                AttributesToAdd.Add(item.Mapping);

            string[] _attrs = AttributesToAdd.ToArray();

            String sFilter = SetQueryFilter(this.BatchAction, CurrentTime);

            SearchRequest searchRequest = new SearchRequest(this.SearchRoot, sFilter, SearchScope.Subtree, _attrs);

            PageResultRequestControl PageResponse = new PageResultRequestControl(this.PageSize);
            SearchOptionsControl SearchOptions = new SearchOptionsControl(System.DirectoryServices.Protocols.SearchOption.DomainScope);

            searchRequest.Controls.Add(PageResponse);
            searchRequest.Controls.Add(SearchOptions);

            logger.LogVerbose(string.Format("Establishing LDAP Connection to: {0}", this.ServerName));
            
            using(LdapConnection connection = CreateLDAPConnection())
            {
                logger.LogVerbose(string.Format("Performing a {0} operation with filter: {1}", this.BatchAction, sFilter));

                while (true)
                {
                    SearchResponse response = null;

                    try
                    {
                        response = connection.SendRequest(searchRequest) as SearchResponse;
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("An error occurred whilst creating the SearchResponse", ex);
                    }

                    int ResponseCount = response.Entries.Count;
                    int CurrentBatchSize;

                    if (ResponseCount != this.PageSize)
                        CurrentBatchSize = ResponseCount;
                    else
                        CurrentBatchSize = this.PageSize;

                    string FilePath = CSVCreateFile(this.CSVDirectoryLocation, _TotalUsers, CurrentBatchSize);
                    

                    foreach (DirectoryControl control in response.Controls)
                    {
                        if (control is PageResultResponseControl)
                        {
                            PageResponse.Cookie = ((PageResultResponseControl)control).Cookie;
                            break;
                        }
                    }

                    // Create CSV file for current batch of users
                    using (CSVWriter BatchFile = new CSVWriter(FilePath))
                    {
                        // Create column headings for CSV file
                        CSVUserEntry heading = new CSVUserEntry();

                        // Iterate over attribute headings
                        foreach (PropertyBase item in this.Properties)
                            heading.Add(item.Name);

                        BatchFile.CSVWriteUser(heading, logger);

                        // Create new CSV row for each user
                        foreach (SearchResultEntry sre in response.Entries)
                        {
                            // Placeholder for CSV entry of current user
                            CSVUserEntry user = new CSVUserEntry();

                            // Exract each user attribute specified in XML file
                            foreach (PropertyBase item in this.Properties)
                            {
                                try
                                {
                                    DirectoryAttribute attr = sre.Attributes[item.Mapping];
                                    string value = string.Empty;
                                    if (null != attr && attr.Count > 0)
                                        value = attr[0].ToString();

                                    if (item.Index == this.UserNameIndex)
                                        user.Add(CreateUserAccountName(value, attr));
                                    else
                                        user.Add(value);
                                }
                                catch (Exception ex)
                                {
                                    if (logger != null)
                                    {
                                        logger.LogException(string.Empty, ex);
                                        _TotalFailures++;
                                    }
                                }
                            }

                            // Write current user to CSV file
                            BatchFile.CSVWriteUser(user, logger);

                            // Increment user count value
                            _TotalUsers++;
                        }
                    }

                    logger.LogVerbose(string.Format("Successfully extracted {0} users to {1} - the total user count is: {2}", CurrentBatchSize, FilePath, _TotalUsers));

                    if (PageResponse.Cookie.Length == 0)
                        break;
                }
            }           
        }

        private string CreateUserAccountName(string value, DirectoryAttribute attr)
        {
            int position = value.IndexOf('\\');
            if(position > 0)
                value = value.Substring(position+1);
            string AccountName = string.Format("{0}{1}@{2}", this.SPOClaimsString, value, this.SPOAccountUPN);
            return AccountName;
        }

        /// <summary>
        /// Create the CSV batch file.
        /// </summary>
        /// <param name="Location">Directory location.</param>
        /// <param name="TotalCount">Total number of users.</param>
        public string CSVCreateFile(string Location, int TotalCount, int CurrentBatchSize)
        {
            string FilePath = null;
            int StartValue;

            if (TotalCount == 0)
                StartValue = 1;
            else
                StartValue = TotalCount;
            
            if (Directory.Exists(Location))
            {
                FilePath = string.Format("{0}\\LDAPProfileExtract-{1}-{2}.csv", Location, StartValue, (TotalCount + CurrentBatchSize));
            }
            else
            {
                Directory.CreateDirectory(Location);
                FilePath = string.Format("{0}\\LDAPProfileExtract-{1}-{2}.csv", Location, StartValue, (TotalCount + CurrentBatchSize));
            }

            return FilePath;
        }

        /// <summary>
        /// Establish the LDAP Connection
        /// </summary>
        public LdapConnection CreateLDAPConnection()
        {
            LdapDirectoryIdentifier identifier = CreateIdentifier();
            System.Net.NetworkCredential credential = CreateCredentials();

            AuthType _authType = (System.DirectoryServices.Protocols.AuthType)Enum.Parse(typeof(System.DirectoryServices.Protocols.AuthType), this.LDAPAuthType);
            
            LdapConnection connection = new LdapConnection(identifier, credential, _authType);
            connection.SessionOptions.ProtocolVersion = this.ProtocolVers;

            connection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(this.VerifyServerCertificate);
            connection.SessionOptions.QueryClientCertificate = new QueryClientCertificateCallback(this.QueryClientCertificate);
            TimeSpan _timeSpan = new TimeSpan(0, 0, this.QueryTimeout);
            connection.Timeout = _timeSpan;
            connection.SessionOptions.SecureSocketLayer = true;
            connection.Bind();
            return connection;
        }

        /// <summary>
        /// Sets the query filter for LDAP search
        /// </summary>
        /// <param name="action">Flag to toggle search query scoper</param>
        /// <param name="CurrentTime">Locked time stamp when program started</param>
        public string SetQueryFilter(string action, DateTime CurrentTime)
        {
            string query = null;

            // Apply filter scope based on IsBulk value
            if (action == "bulk")
            {
                // Extract all users and attributes 
                query = "(objectCategory=Person)";
            }
            else if (action == "delta")
            {
                string TimeNow = CurrentTime.ToString("yyyyMMddHHmmss.0Z"); 
                string TimeDelta = CurrentTime.AddDays(-this.DeltaPeriod).ToString("yyyyMMddHHmmss.0Z");
                query = "(&(whenChanged>="+TimeDelta+")(whenChanged<="+TimeNow+")(objectCategory=Person))";
            }
            else
            {
                query = action;
            }

            return query;
        }

        private LdapDirectoryIdentifier CreateIdentifier()
        {
            string connectionString = this.ServerIP + ":" + this.PortNumber;
            LdapDirectoryIdentifier id = new LdapDirectoryIdentifier(connectionString);
            return id;
        }

        private System.Net.NetworkCredential CreateCredentials()
        {
            return new System.Net.NetworkCredential(this.UserName, this.Password);
        }

        private bool VerifyServerCertificate(LdapConnection connection, X509Certificate certificate)
        {
            LdapDirectoryIdentifier id = connection.Directory as LdapDirectoryIdentifier;
            X509Store store = new X509Store("CertStore", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2 newcert = new X509Certificate2(certificate);
            store.Add(newcert);
            return true;
        }

        private X509Certificate QueryClientCertificate(LdapConnection connection, byte[][] trustedCAs)
        {
            LdapDirectoryIdentifier id = connection.Directory as LdapDirectoryIdentifier;

            if (IsTrustedContosoCA(trustedCAs))
            {
                X509Certificate cert = new X509Certificate();
                cert.Import(GetPath(this.CertificatePath), this.Password, X509KeyStorageFlags.DefaultKeySet);
                connection.ClientCertificates.Add(cert);
                return null;
            }
            else
                return null;
        }
        
        private bool IsTrustedContosoCA(byte[][] trustedCAs)
        {
            foreach (byte[] ca in trustedCAs)
            {
                string utf8 = System.Text.Encoding.UTF8.GetString(ca);
                if (utf8.ToLower().Contains("contoso"))
                    return true;
            }
            return false;
        }

        private string GetPath(string path)
        {
            if (path.StartsWith("~/"))
            {
                path = path.Substring(2);
                path = System.IO.Path.Combine(System.Environment.CurrentDirectory, path);
            }
            else if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = System.IO.Path.Combine(System.Environment.CurrentDirectory, path);
            }

            return path;
        }

        #endregion Methods
    }
}