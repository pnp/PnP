
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
using System.Configuration;

namespace Contoso.Core.ProfilePictureUploader
{
    class Program
    {

        //sizes for profile pictures
        const int _smallThumbWidth = 48;
        const int _mediumThumbWidth = 72;
        const int _largeThumbWidth = 200;

        static UPSvc.UserProfileService _userProfileService;
        static ClientContext _clientContext;
        const string _sPOProfilePrefix = "i:0#.f|membership|";
        const string _profileSiteTemplateUrl = "https://{0}-admin.sharepoint.com";
        const string _mySiteHostTemplateUrl = "https://{0}-my.sharepoint.com";


        enum LogLevel { Information, Warning, Error };

        //tenant admin username and password, used for connecting to SPO
        static string _sPoAuthUserName = string.Empty;
        static string _sPoAuthPasword = string.Empty;
        static string _sourceUserName = string.Empty;
        static string _sourcePassword = string.Empty;
        static string _configfilepath = string.Empty;
        static string _profileSiteUrl = string.Empty;
        static string _mySiteUrl = string.Empty;

        static Configuration _appConfig;

        static void Main(string[] args)
        {
            int count = 0;

            if (SetupArguments(args)) //check if args passed are valid, only continue if they are
            {

                if (InitializeConfiguration()) //check that configuraiton file is valid, only continue if is
                {
                    if (InitializeWebService())//init the web service end point for SPO user profile service
                    {
                        //InitializeClientService(); //init the clientcontext SPO client object for SPO. Not used for now, can use it if you plan to use some of the Getproperty functions

                        //loop through each row in the CSV file
                        using (StreamReader readFile = new StreamReader(_appConfig.PictureSourceCsv))
                        {
                            string line;
                            string[] row;
                            string sPoUserProfileName;
                            string sourcePictureUrl;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                //ignore first line
                                if (count > 0)
                                {
                                    row = line.Split(',');
                                    sPoUserProfileName = row[0]; //first column must be profile username e.g. UPN from O365
                                    sourcePictureUrl = row[1]; //second column must be source picture path, e.g. c:\temp\pic.jpg, or \\server\path\pic.jpg or http://server/path/pic.jpg

                                    LogMessage("Begin processing for user " + sPoUserProfileName, LogLevel.Warning);

                                    //get source picture from source picture path
                                    using (MemoryStream picturefromExchange = GetImagefromHTTPUrl(sourcePictureUrl))
                                    {
                                        if (picturefromExchange != null)//if we got picture, upload to SPO
                                        {
                                            //create SP naming convetion for image file
                                            string newImageNamePrefix = sPoUserProfileName.Replace("@", "_").Replace(".", "_");
                                            //upload source image to SPO (might do some resize work, and multiple image upload depending on config file)
                                            string spoImageUrl = UploadImageToSpo(newImageNamePrefix, picturefromExchange);
                                            if (spoImageUrl.Length > 0)//if upload worked
                                            {
                                                string[] profilePropertyNamesToSet = new string[] { "PictureURL", "SPS-PicturePlaceholderState" };
                                                string[] profilePropertyValuesToSet = new string[] { spoImageUrl, "0" };
                                                //set these 2 required properties for user profile i.e path to image uploaded, and pictureplaceholder state
                                                SetMultipleProfileProperties(_sPOProfilePrefix + sPoUserProfileName, profilePropertyNamesToSet, profilePropertyValuesToSet);
                                                //this is going to call the service again and update additional profile props from the config file.
                                                // could include 2 props above into the below and send as 1 call, however worried that it fails and then the required 2 above are not set correctly
                                                // this will double the number of calls to SPO user profile service. Unless no additional props set in the config file
                                                SetAdditionalProfileProperties(_sPOProfilePrefix + sPoUserProfileName);
                                            }
                                        }
                                    }

                                    LogMessage("End processing for user " + sPoUserProfileName, LogLevel.Warning);

                                    int sleepTime = _appConfig.UploadDelay;
                                    System.Threading.Thread.Sleep(sleepTime); //may want to sleep if SPO sees you as DoS attach
                                }
                                count++;
                            }
                        }
                    }
                }
            }


            LogMessage("Processing finished for " + count + " user profiles (or so)", LogLevel.Information);
        }


        /// <summary>
        /// Check the arguments passed into exe. If not correct, write out correct usage.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static bool SetupArguments(string[] args)
        {

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-SPOAdmin", StringComparison.InvariantCultureIgnoreCase) && (i + 1 < args.Length) && !args[i + 1].StartsWith("-"))
                    _sPoAuthUserName = args[i + 1];
                else if (args[i].Equals("-SPOAdminPassword", StringComparison.InvariantCultureIgnoreCase) && (i + 1 < args.Length) && !args[i + 1].StartsWith("-"))
                    _sPoAuthPasword = args[i + 1];
                else if (args[i].Equals("-Configuration", StringComparison.InvariantCultureIgnoreCase) && (i + 1 < args.Length) && !args[i + 1].StartsWith("-"))
                    _configfilepath = args[i + 1];
                else if (args[i].Equals("-SourceUser", StringComparison.InvariantCultureIgnoreCase) && (i + 1 < args.Length) && !args[i + 1].StartsWith("-"))
                    _sourceUserName = args[i + 1];
                else if (args[i].Equals("-SourcePassword", StringComparison.InvariantCultureIgnoreCase) && (i + 1 < args.Length) && !args[i + 1].StartsWith("-"))
                    _sourcePassword = args[i + 1];
            }

            if ((_sPoAuthUserName.Length == 0) || (_sPoAuthPasword.Length == 0) || (_configfilepath.Length == 0))
            {
                //show usage command
                Console.WriteLine();
                Console.WriteLine("Error: SPO admin username, password and configuration file path are three required arguments");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("      ProfilePictureUploader.exe -SPOAdmin value -SPOAdminPassword value -Configuration value [-SourceUser] [-SourcePassword]");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine("         ProfilePictureUploader.exe -SPOAdmin user@contoso.onmicrosoft.com -SPOAdminPassword password -Configuration configuration.xml");
                Console.WriteLine("         ProfilePictureUploader.exe -SPOAdmin user@contoso.onmicrosoft.com -SPOAdminPassword password -Configuration configuration.xml -SourceUser contoso\\username -SourcePassword password");
                Console.WriteLine();
                return false;

            }

            if (!_configfilepath.Contains(":")) //if they didnt put full path to configuration file, assume current exe file path 
                _configfilepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + _configfilepath;

            return true;
        }


        /// <summary>
        /// Get configuration.xml file and convert to an object we can use through rest of code. Also make sure required config elements are there
        /// </summary>
        /// <returns></returns>
        static bool InitializeConfiguration()
        {

            try
            {
                if (!System.IO.File.Exists(_configfilepath))
                {
                    LogMessage("Cannot find configuration file " + _configfilepath, LogLevel.Error);
                    return false;
                }

                XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
                // To read the file, creates a FileStream.
                FileStream myFileStream = new FileStream(_configfilepath, FileMode.Open);
                // Calls the Deserialize method and casts to the object type.
                _appConfig = (Configuration)mySerializer.Deserialize(myFileStream);

                if (_appConfig.TenantName.Length == 0 || _appConfig.PictureSourceCsv.Length == 0)
                {

                    LogMessage("Missing tenant name or pictureSourceCsv path from configuration file " + _configfilepath, LogLevel.Error);
                    return false;
                }

                //check if picturesourcesvc file exists, without it application wont run
                if (!System.IO.File.Exists(_appConfig.PictureSourceCsv))
                {
                    LogMessage("Cannot find pictureSourceCsv  " + _appConfig.PictureSourceCsv, LogLevel.Error);
                    return false;
                }

                //remove onmicrosoft.com from tenant name, all we need is friendly name, which will be used in SPO site collection URLs
                int pos = _appConfig.TenantName.IndexOf(".onmicrosoft.com", StringComparison.CurrentCultureIgnoreCase);
                if (pos > 0)
                    _appConfig.TenantName = _appConfig.TenantName.Remove(pos, 16);

                _profileSiteUrl = string.Format(_profileSiteTemplateUrl, _appConfig.TenantName); //build URL for admin site e.g. https://tenantname-admin.sharepoint.com
                _mySiteUrl = string.Format(_mySiteHostTemplateUrl, _appConfig.TenantName); //build URL for my site host e.g. https://tenantname-my.sharepoint.com
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("Error getting or reading configuration file " + _configfilepath + " " + ex.Message, LogLevel.Error);
                return false;
            }
            finally
            {
                //do nothing
            }

        }


        /// <summary>
        /// Get image object from URL. End point is non-authenticated
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        static MemoryStream GetImagefromHTTPUrl(string imageUrl)
        {

            if (_sourceUserName.Length > 0)
                return GetImagefromHTTPUrl(_sourceUserName, _sourcePassword, imageUrl);
            else
                return GetImagefromHTTPUrl(string.Empty, string.Empty, imageUrl);
        }


        /// <summary>
        /// Get image object from URL. End point is basic auth authenticated. Add username and password for end points auth.
        /// </summary>
        /// <param name="AuthUser"></param>
        /// <param name="AuthPassword"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        static MemoryStream GetImagefromHTTPUrl(string AuthUser, string AuthPassword, string imageUrl)
        {

            try
            {

                LogMessage("Fetching source image: " + imageUrl, LogLevel.Information);

                System.Net.WebRequest webRequest = System.Net.HttpWebRequest.Create(imageUrl);
                //if a auth username has been passed into function, then fetch resource using authentication username and password
                if (AuthUser.Length > 0)
                {
                    webRequest.Credentials = new System.Net.NetworkCredential(AuthUser, AuthPassword);
                }

                WebResponse webResponse = webRequest.GetResponse();
                Stream imageStream = webResponse.GetResponseStream();

                MemoryStream tmpStream = new MemoryStream();
                imageStream.CopyTo(tmpStream);
                tmpStream.Seek(0, SeekOrigin.Begin);
                //LogMessage("Finished fetching image from URL " + imageUrl, LogLevel.Information);
                return tmpStream;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        LogMessage("User Error: Cannot find source image for URL " + imageUrl + ex.Message, LogLevel.Error);

                    }
                    else
                    {
                        LogMessage("User Error: Error fetching source image for URL " + imageUrl + ex.Message, LogLevel.Error);
                    }

                }
                else
                {
                    LogMessage("User Error: Error fetching source image for URL " + imageUrl + ex.Message, LogLevel.Error);
                }

                return null;
            }
            catch (Exception ex)
            {
                LogMessage("User Error: Error fetching source image for URL " + imageUrl + ex.Message, LogLevel.Error);
                return null;
            }
        }




        /// <summary>
        /// Upload picture stream to SPO My Site hoste (SkyDrive Pro Host) site collection user photos library.
        /// </summary>
        /// <param name="PictureName"></param>
        /// <param name="ProfilePicture"></param>
        /// <returns>URL to uploaded picture</returns>
        static string UploadImageToSpo(string PictureName, Stream ProfilePicture)
        {
            try
            {
                string spPhotoPathTempate = string.Concat(_appConfig.TargetLibraryPath.TrimEnd('/'), "/{0}_{1}Thumb.jpg"); //path template to photo lib in My Site Host
                string spImageUrl = string.Empty;

                //create SPO Client context to My Site Host
                ClientContext mySiteclientContext = new ClientContext(_mySiteUrl);
                SecureString securePassword = GetSecurePassword(_sPoAuthPasword);
                //provide auth crendentials using O365 auth
                mySiteclientContext.Credentials = new SharePointOnlineCredentials(_sPoAuthUserName, securePassword);

                if (!_appConfig.Thumbs.Upload3Thumbs) //just take single input image and upload to photo lib, no resizeing of image
                {
                    spImageUrl = string.Format(spPhotoPathTempate, PictureName, "M");
                    LogMessage("Uploading single image, no resize, to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);
                }
                else if (_appConfig.Thumbs.Upload3Thumbs && !_appConfig.Thumbs.CreateSMLThumbs)//upload 3 of the same size image
                {
                    //not pretty code below, but works. Upload same source image 3 times, but with different name
                    // no resizing of any images
                    LogMessage("Uploading threes image to SPO, no resize", LogLevel.Information);

                    spImageUrl = string.Format(spPhotoPathTempate, PictureName, "M");
                    LogMessage("Uploading medium image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);

                    ProfilePicture.Seek(0, SeekOrigin.Begin);
                    spImageUrl = string.Format(spPhotoPathTempate, PictureName, "L");
                    LogMessage("Uploading large image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);

                    ProfilePicture.Seek(0, SeekOrigin.Begin);
                    spImageUrl = string.Format(spPhotoPathTempate, PictureName, "S");
                    LogMessage("Uploading small image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);


                }
                else if (_appConfig.Thumbs.Upload3Thumbs && _appConfig.Thumbs.CreateSMLThumbs) //generate 3 different size thumbs
                {
                    LogMessage("Uploading threes image to SPO, with resizing", LogLevel.Information);
                    //create 3 images based on recommended sizes for SPO
                    //create small size,                   
                    using (Stream smallThumb = ResizeImageSmall(ProfilePicture, _smallThumbWidth))
                    {
                        if (smallThumb != null)
                        {
                            spImageUrl = string.Format(spPhotoPathTempate, PictureName, "S");
                            LogMessage("Uploading small image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, smallThumb, true);
                        }
                    }

                    //create medium size
                    using (Stream mediumThumb = ResizeImageSmall(ProfilePicture, _mediumThumbWidth))
                    {
                        if (mediumThumb != null)
                        {
                            spImageUrl = string.Format(spPhotoPathTempate, PictureName, "M");
                            LogMessage("Uploading medium image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, mediumThumb, true);

                        }
                    }

                    //create large size image, shown on SkyDrive Pro main page for user
                    using (Stream largeThumb = ResizeImageLarge(ProfilePicture, _largeThumbWidth))
                    {
                        if (largeThumb != null)
                        {

                            spImageUrl = string.Format(spPhotoPathTempate, PictureName, "L");
                            LogMessage("Uploading large image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, largeThumb, true);

                        }
                    }


                }
                //return medium sized URL, as this is the one that should be set in the user profile
                return _mySiteUrl + string.Format(spPhotoPathTempate, PictureName, "M");

            }
            catch (Exception ex)
            {
                LogMessage("User Error: Failed to upload thumbnail picture to SPO for " + PictureName + " " + ex.Message, LogLevel.Error);
                return string.Empty;
            }

        }


        /// <summary>
        /// Resize image stream to width passed into function. Will use source image dimension to scale image correctly
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <param name="NewWidth">New image size width in pixels</param>
        /// <returns></returns>
        static Stream ResizeImageSmall(Stream OriginalImage, int NewWidth)
        {

            //when resizing large images i.e. bigger than 200px, we lose quality using the GetThumbnailImage method. There are better ways to do this, but will look to imporve in a future version
            // e.g. http://stackoverflow.com/questions/87753/resizing-an-image-without-losing-any-quality
            try
            {
                OriginalImage.Seek(0, SeekOrigin.Begin);
                Image originalImage = Image.FromStream(OriginalImage, true, true);
                if (originalImage.Width == NewWidth) //if sourceimage is same as destination, no point resizing, as it loses quality
                {
                    OriginalImage.Seek(0, SeekOrigin.Begin);
                    originalImage.Dispose();
                    return OriginalImage; //return same image that was passed in
                }
                else
                {
                    Image resizedImage = originalImage.GetThumbnailImage(NewWidth, (NewWidth * originalImage.Height) / originalImage.Width, null, IntPtr.Zero);
                    MemoryStream memStream = new MemoryStream();
                    resizedImage.Save(memStream, ImageFormat.Jpeg);
                    resizedImage.Dispose();
                    originalImage.Dispose();
                    memStream.Seek(0, SeekOrigin.Begin);
                    return memStream;
                }


            }
            catch (Exception ex)
            {
                LogMessage("User Error: cannot create resized image to new width of " + NewWidth.ToString() + ex.Message, LogLevel.Error);
                return null;
            }
        }


        /// <summary>
        /// Delivers better quality image for scaling large thumbs e.g. 200px in width
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <param name="NewWidth"></param>
        /// <returns></returns>
        static Stream ResizeImageLarge(Stream OriginalImage, int NewWidth)
        {
            OriginalImage.Seek(0, SeekOrigin.Begin);
            Image originalImage = Image.FromStream(OriginalImage, true, true);
            int newHeight = (NewWidth * originalImage.Height) / originalImage.Width;

            Bitmap newImage = new Bitmap(NewWidth, newHeight);

            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(originalImage, new Rectangle(0, 0, NewWidth, newHeight)); //copy to new bitmap
            }


            MemoryStream memStream = new MemoryStream();
            newImage.Save(memStream, ImageFormat.Jpeg);
            originalImage.Dispose();
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;


        }

        /// <summary>
        /// Help funtion to log messages to the console window, and to a text file. Log level is currently not used other than for display colors in console
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        static void LogMessage(string Message, LogLevel Level)
        {
            //maybe write to log where image failed to upload or profile picture
            switch (Level)
            {
                case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Green; break;
                case LogLevel.Information: Console.ForegroundColor = ConsoleColor.White; break;

            }

            Console.WriteLine(Message);
            Console.ResetColor();

            try
            {

                if (_appConfig != null)
                    if (_appConfig.LogFile.EnableLogging) //check if logging is enabled in configuration file
                    {
                        System.IO.File.AppendAllText(_appConfig.LogFile.Path, Environment.NewLine + DateTime.Now + " : " + Message);
                    }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error writing to log file. " + ex.Message);
                Console.ResetColor();
            }
        }


        /// <summary>
        /// Use the properties in the configuration file and set these against the user profile
        /// </summary>
        /// <param name="UserName"></param>
        static void SetAdditionalProfileProperties(string UserName)
        {
            if (_appConfig.AdditionalProfileProperties.Properties == null) //if properties has been left out of config file
                return;

            int propsCount = _appConfig.AdditionalProfileProperties.Properties.Count();
            if (propsCount > 0)
            {
                string[] profilePropertyNamesToSet = new string[propsCount];
                string[] profilePropertyValuesToSet = new string[propsCount];
                //loop through each property in config
                for (int i = 0; i < propsCount; i++)
                {
                    profilePropertyNamesToSet[i] = _appConfig.AdditionalProfileProperties.Properties[i].Name;
                    profilePropertyValuesToSet[i] = _appConfig.AdditionalProfileProperties.Properties[i].Value;
                }

                //set all props in a single call
                SetMultipleProfileProperties(UserName, profilePropertyNamesToSet, profilePropertyValuesToSet);

            }
        }


        /// <summary>
        /// Use this function if you want to set a single property in the user profile store
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
        /// Use this function is in a single call to SPO you want to set multiple profile properties. 1st item in propertyname array is associated to first item in propertvalue array etc
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PropertyName"></param>
        /// <param name="PropertyValue"></param>
        static void SetMultipleProfileProperties(string UserName, string[] PropertyName, string[] PropertyValue)
        {

            LogMessage("Setting multiple SPO user profile properties for " + UserName, LogLevel.Information);

            try
            {
                int arrayCount = PropertyName.Count();

                UPSvc.PropertyData[] data = new UPSvc.PropertyData[arrayCount];
                for (int x = 0; x < arrayCount; x++)
                {
                    data[x] = new UPSvc.PropertyData();
                    data[x].Name = PropertyName[x];
                    data[x].IsValueChanged = true;
                    data[x].Values = new UPSvc.ValueData[1];
                    data[x].Values[0] = new UPSvc.ValueData();
                    data[x].Values[0].Value = PropertyValue[x];
                }

                _userProfileService.ModifyUserPropertyByAccountName(UserName, data);
                //LogMessage("Finished setting multiple SPO user profile properties for " + UserName, LogLevel.Information);

            }
            catch (Exception ex)
            {
                LogMessage("User Error: Exception trying to update profile properties for user " + UserName + "\n" + ex.Message, LogLevel.Error);
            }
        }


        /// <summary>
        /// Used to fetch user profile property from SPO.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        static string GetSingleProfileProperty(string UserName, string PropertyName)
        {
            try
            {

                var peopleManager = new PeopleManager(_clientContext);

                ClientResult<string> profileProperty = peopleManager.GetUserProfilePropertyFor(UserName, PropertyName);
                _clientContext.ExecuteQuery();

                //this is the web service way of retrieving the same thing as client API. Note: valye of propertyname is not case sensitive when using web service, but does seem to be with client API
                //UPSvc.PropertyData propertyData = userProfileService.GetUserPropertyByAccountName(UserName, PropertyName);

                if (profileProperty.Value.Length > 0)
                {
                    return profileProperty.Value;
                }
                else
                {
                    LogMessage("Cannot find a value for property " + PropertyName + " for user " + UserName, LogLevel.Information);
                    return string.Empty;
                }


            }
            catch (Exception ex)
            {
                LogMessage("User Error: Exception trying to get profile property " + PropertyName + " for user " + UserName + "\n" + ex.Message, LogLevel.Error);
                return string.Empty;
            }

        }

        /// <summary>
        /// Get multiple properties from SPO in a single call to the service
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        static string[] GetMultipleProfileProperties(string UserName, string[] PropertyNames)
        {
            try
            {

                var peopleManager = new PeopleManager(_clientContext);


                UserProfilePropertiesForUser profilePropertiesForUser = new UserProfilePropertiesForUser(_clientContext, UserName, PropertyNames);
                IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);

                // Load the request and run it on the server.
                _clientContext.Load(profilePropertiesForUser);
                _clientContext.ExecuteQuery();

                //convert to array and return
                return profilePropertyValues.ToArray();


            }
            catch (Exception ex)
            {
                LogMessage("Exception trying to get profile properties for user " + UserName + "\n" + ex.Message, LogLevel.Error);
                return null;
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


