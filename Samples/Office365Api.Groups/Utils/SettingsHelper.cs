//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using System;
using System.Configuration;

namespace O365Groups.Utils
{
	public class SettingsHelper
	{
		private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"] ?? ConfigurationManager.AppSettings["ida:ClientID"];
		private static string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret"] ?? ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];

		private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
		private static string _authorizationUri = "https://login.windows.net";
		private static string _authority = "https://login.windows.net/{0}/";

		private static string _adGraphResource = "https://graph.windows.net";
		private static string _graphResource = "https://graph.microsoft.com";
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

		public static string TenantId
		{
			get
			{
				return _tenantId;
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
				return String.Format(_authority, _tenantId);
			}
		}

		public static string AADGraphResource
		{
			get
			{
				return _adGraphResource;
			}
		}

		public static string MSGraphResource
		{
			get
			{
				return _graphResource;
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
	}
}
