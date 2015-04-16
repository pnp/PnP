
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Configuration;
using Contoso.ProfileProperty.Migration.Import;

namespace Contoso.ProfileProperty.Migration.Import
{
    class Program
    {
        
        // Global Vars
        enum LogLevel { Information, Warning, Error };

        // Tenant information for connecting to SPO
        static UPSvc.UserProfileService _userProfileService;
        static ClientContext _clientContext;
        const string _sPOProfilePrefix = "i:0#.f|membership|";
        const string _profileSiteTemplateUrl = "https://{0}-admin.sharepoint.com";
        const string _mySiteHostTemplateUrl = "https://{0}-my.sharepoint.com";
        static string _TenantName = string.Empty;
        static bool   _enableLogging = false;
        static string _sPoAuthUserName = string.Empty;
        static string _sPoAuthPasword = string.Empty;
        static string _sourceUserName = string.Empty;
        static string _sourcePassword = string.Empty;
        static string _configfilepath = string.Empty;
        static string _profileSiteUrl = string.Empty;
        static string _mySiteUrl = string.Empty;
        static string _ProfileStore = string.Empty;


        static void Main(string[] args)
        {
            int count = 1;


            if (InitializeConfiguration())
            {
                // init the web service end point for SPO user profile service
                if (InitializeWebService()) 
                {

                    try
                    {
                        // Deserialize our Migration DataStore XML provided by the output of the first tool.
                        XmlSerializer y = new XmlSerializer(typeof(UserProfileCollection));
                        FileStream fs = new FileStream(_ProfileStore, FileMode.Open);
                        UserProfileCollection upsDes;
                        upsDes = (UserProfileCollection)y.Deserialize(fs);
                        fs.Close();

                        Random random = new Random();

                        int totalUsers = upsDes.ProfileData.Count;
                        foreach (UserProfileData userProfile in upsDes.ProfileData)
                        {
                            string userName = userProfile.UserName;
                            LogMessage(string.Format("processing user '{0}' of {1}...", count, totalUsers), LogLevel.Information);
                            SetSingleMVProfileProperty(userName, "SPS-Responsibility", userProfile.AskMeAbout);
                            SetSingleMVProfileProperty(userName, "AboutMe", userProfile.AboutMe);
                            LogMessage(string.Format("processing user '{0}' of {1}...Done!", count++, totalUsers), LogLevel.Information);

                            // Sleeping if SPO sees us as DoS attack
                            System.Threading.Thread.Sleep(random.Next(500, 3000));

                        }

                        LogMessage("Processing finished for " + totalUsers + " user profiles. Import Complete!", LogLevel.Warning);
                       
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Exception trying to set profile properties:\n" + ex.Message, LogLevel.Error);
                    }
                }

            }

        }

        
        /// <summary>
        /// Fill Global Variables with Configuration information...
        /// </summary>
        /// <returns></returns>
        static bool InitializeConfiguration()
        {

            try
            {

                _TenantName = ConfigurationManager.AppSettings["tenantName"];
                _enableLogging = Convert.ToBoolean(ConfigurationManager.AppSettings["enableLogging"]);
                _configfilepath = ConfigurationManager.AppSettings["logFile"];
                _sPoAuthUserName = ConfigurationManager.AppSettings["SPOAdminUserName"];
                _sPoAuthPasword = ConfigurationManager.AppSettings["SPOAdminPasword"];
                _ProfileStore = ConfigurationManager.AppSettings["ProfileStore"];
               
                // Remove onmicrosoft.com from tenant name, all we need is friendly name, which will be used in SPO site collection URLs
                int pos = _TenantName.IndexOf(".onmicrosoft.com",StringComparison.CurrentCultureIgnoreCase);
                if (pos > 0)
                    _TenantName = _TenantName.Remove(pos, 16);

                _profileSiteUrl = string.Format(_profileSiteTemplateUrl, _TenantName); //build URL for admin site e.g. https://tenantname-admin.sharepoint.com
                _mySiteUrl = string.Format(_mySiteHostTemplateUrl, _TenantName); //build URL for my site host e.g. https://tenantname-my.sharepoint.com
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("Error reading configuration information\n" + ex.Message, LogLevel.Error);
                return false;
            }
            finally
            {
                //do nothing
            }

        }
    
        /// <summary>
        /// Helper funtion to log messages to the console window, and to a text file. Log level is currently not used other than for display colors in console
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        static void LogMessage(string Message, LogLevel Level)
        {
            
            switch (Level)
            {
                case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogLevel.Information: Console.ForegroundColor = ConsoleColor.White; break;
                
            }

            Console.WriteLine(Message);
            Console.ResetColor();

            try
            {

               
                if (_enableLogging) //check if logging is enabled in configuration file
                {
                    System.IO.File.AppendAllText(_configfilepath, Environment.NewLine + DateTime.Now + " : " + Message);
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error writing to log file. " + ex.Message);
                Console.ResetColor();
            }
        }


        /// <summary>
        /// Write a MultiValued property to SPO
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PropertyName"></param>
        /// <param name="PropertyValue"></param>
        static void SetSingleMVProfileProperty(string UserName, string PropertyName, string PropertyValue)
        {

            try
            {
                string[] arrs = PropertyValue.Split(ConfigurationManager.AppSettings["PROPERTYSEPERATOR"][0]);
                
               UPSvc.ValueData[] vd = new UPSvc.ValueData[arrs.Count()];
               
               for (int i=0;i<=arrs.Count()-1;i++)
               {
                    vd[i] = new UPSvc.ValueData();
                    vd[i].Value = arrs[i];
                }
               
                UPSvc.PropertyData[] data = new UPSvc.PropertyData[1];
                data[0] = new UPSvc.PropertyData();
                data[0].Name = PropertyName;
                data[0].IsValueChanged = true;
                data[0].Values = vd;
                               
                _userProfileService.ModifyUserPropertyByAccountName(string.Format(@"i:0#.f|membership|{0}", UserName), data);

            }
            catch (Exception ex)
            {
                LogMessage("Exception trying to update profile property " + PropertyName + " for user " + UserName + "\n" + ex.Message, LogLevel.Error);
            }

        }
        
        /// <summary>
        /// Write a Single valued property to SPO
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PropertyName"></param>
        /// <param name="PropertyValue"></param>
        static void SetSingleProfileProperty(string UserName, string PropertyName, string PropertyValue)
        {

            try
            {
                UPSvc.PropertyData[] data = new UPSvc.PropertyData[1];
                data[0] = new UPSvc.PropertyData();
                data[0].Name = PropertyName;
                data[0].IsValueChanged = true;
                data[0].Values = new UPSvc.ValueData[1];
                data[0].Values[0] = new UPSvc.ValueData();
                data[0].Values[0].Value = PropertyValue;
                _userProfileService.ModifyUserPropertyByAccountName(UserName, data);
            }
            catch (Exception ex)
            {
                LogMessage("Exception trying to update profile property " + PropertyName + " for user " + UserName + "\n" + ex.Message, LogLevel.Error);
            }

        }

         /// <summary>
        /// Creates a SP Client object using SPO admin credentials, and saves client object into global variable _clientContext. Not used in the code as provided, but would be if you decide to use some of the get property information
        /// </summary>
        /// <returns></returns>
        static bool InitializeClientService()
        {
            
            try
            {

                LogMessage("Initializing service object for SPO Client API " + _profileSiteUrl, LogLevel.Information);
                _clientContext = new ClientContext(_profileSiteUrl);
                SecureString securePassword = GetSecurePassword(_sPoAuthPasword);
                _clientContext.Credentials = new SharePointOnlineCredentials(_sPoAuthUserName, securePassword);

                //LogMessage("Finished creating service object for SPO Client API " + _profileSiteUrl, LogLevel.Information);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("Error creating client context for SPO " + _profileSiteUrl + " " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// No SPO client API for administering user profiles, so need to use traditional ASMX service for user profile work. This function initiates the 
        /// web service end point, and authenticates using Office 365 auth ticket. Use SharePointOnlineCredentials to assist with this auth.
        /// </summary>
        /// <returns></returns>
        static bool InitializeWebService()
        {
            try
            {
                string webServiceExt = "_vti_bin/userprofileservice.asmx";
                string adminWebServiceUrl = string.Empty;

                //append the web service (ASMX) url onto the admin web site URL
                if (_profileSiteUrl.EndsWith("/"))
                    adminWebServiceUrl = _profileSiteUrl + webServiceExt;
                else
                    adminWebServiceUrl = _profileSiteUrl + "/" + webServiceExt;

                LogMessage("Initializing SPO web service " + adminWebServiceUrl, LogLevel.Information);

                //get secure password from clear text password
                SecureString securePassword = GetSecurePassword(_sPoAuthPasword);

                //get credentials from SP Client API, used later to extract auth cookie, so can replay to web services
                SharePointOnlineCredentials onlineCred = new SharePointOnlineCredentials(_sPoAuthUserName, securePassword);

                // Get the authentication cookie by passing the url of the admin web site 
                string authCookie = onlineCred.GetAuthenticationCookie(new Uri(_profileSiteUrl));

                // Create a CookieContainer to authenticate against the web service 
                CookieContainer authContainer = new CookieContainer();

                // Put the authenticationCookie string in the container 
                authContainer.SetCookies(new Uri(_profileSiteUrl), authCookie);

                // Setting up the user profile web service 
                _userProfileService = new UPSvc.UserProfileService();

                // assign the correct url to the admin profile web service 
                _userProfileService.Url = adminWebServiceUrl;

                // Assign previously created auth container to admin profile web service 
                _userProfileService.CookieContainer = authContainer;
               // LogMessage("Finished creating service object for SPO Web Service " + adminWebServiceUrl, LogLevel.Information);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("Error initiating connection to profile web service in SPO " + ex.Message, LogLevel.Error);
                return false;

            }

            
        }

        /// <summary>
        /// Convert clear text password into secure string
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        static SecureString GetSecurePassword(string Password)
        {
            SecureString sPassword = new SecureString();
            foreach (char c in Password.ToCharArray()) sPassword.AppendChar(c);
            return sPassword;
        }
    }
}


