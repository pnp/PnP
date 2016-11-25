using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIGroups.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Utils
{
	public class HttpHelper
	{
		public static async Task<List<Group>> GetGroups(string apiUrl)
		{
			if (String.IsNullOrEmpty(apiUrl)) { throw new ArgumentNullException("apiUrl"); }

			List<Group> groups = new List<Group>();

			string responseContent = await HttpHelper.GetHttpResource(apiUrl);
			var responseObject = JsonConvert.DeserializeObject<GraphResponse<Group>>(responseContent);
			foreach (var item in responseObject.value)
			{
				groups.Add(item);
			}

			return groups;

		}

		public static async Task<string> GetHttpResource(string url)
		{
			string responseContent = String.Empty;

			string token = await TokenHelper.GetAccessToken();

			var request = (HttpWebRequest)HttpWebRequest.Create(url);

			request.Method = "GET";
			request.Accept = "application/json";
			request.Headers.Add("Authorization", "Bearer " + token);

			var response = request.GetResponse();
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				responseContent = reader.ReadToEnd();
			}

			return responseContent;
		}
	}
}