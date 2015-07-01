// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
using System;
using System.Configuration;

namespace Provisioning.Cloud.Management.Utils
{
    //Stores all of the AADSettings required for single sign-on
    public class SettingsHelper
    {

        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"] ?? ConfigurationManager.AppSettings["ida:ClientID"];
        private static string _appKey = ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];
        private static string _authorizationUri = ConfigurationManager.AppSettings["ida:AuthorizationUri"];
        private static string _graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];
        private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantID"];
        private static string _sharePointAdminResourceUri = ConfigurationManager.AppSettings["ida:SharePointAdminResourceUri"];

        private static string _authority = "https://login.windows.net/" + _tenantId;
      
        private static string _discoverySvcResourceId = "https://api.office.com/discovery/";
        private static string _discoverySvcEndpointUri = "https://api.office.com/discovery/v1.0/me/";

        public static string ClientId
        {
            get
            {
                return _clientId;
            }
        }

        public static string AppKey
        {
            get
            {
                return _appKey;
            }
        }

        public static string AuthorizationUri
        {
            get
            {
                return _authorizationUri;
            }
        }

        public static string Authority
        {
            get
            {
                return _authority;
            }
        }

        public static string AADGraphResourceId
        {
            get
            {
                return _graphResourceId;
            }
        }

        public static string DiscoveryServiceResourceId
        {
            get
            {
                return _discoverySvcResourceId;
            }
        }

        public static Uri DiscoveryServiceEndpointUri
        {
            get
            {
                return new Uri(_discoverySvcEndpointUri);
            }
        }

        public static string SharePointAdminResourceUri
        {
            get { return _sharePointAdminResourceUri; }
        }
    }
}