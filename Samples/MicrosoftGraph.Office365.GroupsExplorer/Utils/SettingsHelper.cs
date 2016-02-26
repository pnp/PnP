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

namespace OfficeDevPnP.MSGraphAPIGroups.Utils
{
	public class SettingsHelper
	{
		private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"] ?? ConfigurationManager.AppSettings["ida:ClientID"];
		private static string _appKey = ConfigurationManager.AppSettings["ida:ClientSecret"] ?? ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];
		private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
		private static string _aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];

		private static string _graphResource = "https://graph.microsoft.com";

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

		public static string Authority
		{
			get
			{
				return _aadInstance + _tenantId;
			}
		}

		public static string MSGraphResource
		{
			get
			{
				return _graphResource;
			}
		}

	}
}
