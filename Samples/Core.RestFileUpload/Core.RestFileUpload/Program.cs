using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Core.RestFileUpload
{
	class Program
	{
		/// <summary>
		/// Upload large files using REST
		/// </summary>
		/// <param name="baseUrl">Tenant URL</param>
		/// <param name="folderUrl">SharePoint folder relative Url (eg: /Documents or /Sites/XPTO/Documents)</param>
		/// <param name="filePath">File Path</param>
		/// <returns>File full url</returns>
		static string UploadRest(string baseUrl, string folderUrl, string filePath)
		{
			string token = GetAccessToken(baseUrl);

			HttpWebRequest restRqst = (HttpWebRequest)HttpWebRequest.Create(baseUrl + "/_api/ContextInfo");
			restRqst.Headers.Add("Authorization", "Bearer " + token);
			restRqst.Method = "POST";
			restRqst.Accept = "application/json;odata=verbose";
			restRqst.UseDefaultCredentials = false;
			

			restRqst.ContentLength = 0;

			using (HttpWebResponse restResponse = (HttpWebResponse)restRqst.GetResponse())
			{
				Stream postStream = restResponse.GetResponseStream();
				StreamReader postReader = new StreamReader(postStream);
				string results = postReader.ReadToEnd();

				JavaScriptSerializer jss = new JavaScriptSerializer();
				var d = jss.Deserialize<dynamic>(results);
				string xHeader = d["d"]["GetContextWebInformation"]["FormDigestValue"];

				var fileName = System.IO.Path.GetFileName(filePath);
				using (var docStream = System.IO.File.Open(filePath, FileMode.Open))
				{
					string uploadUrl = baseUrl + "/_api/web/GetFolderByServerRelativeUrl('" + folderUrl + "')/Files/Add(url='" + fileName + "', overwrite=true)";

					restRqst = (HttpWebRequest)HttpWebRequest.Create(uploadUrl);
					restRqst.Method = "POST";
					restRqst.Accept = "application/json;odata=verbose";
					restRqst.Headers.Add("Authorization", "Bearer " + token);
					restRqst.Headers.Add("X-RequestDigest", xHeader);
					restRqst.ContentLength = docStream.Length;
					restRqst.Timeout = Timeout.Infinite;
					postStream.Dispose();
					
					docStream.Seek(0, SeekOrigin.Begin);
					postStream = restRqst.GetRequestStream();

					byte[] fileBytes;
					using (var memoryStream = new MemoryStream())
					{
						docStream.CopyTo(memoryStream);
						fileBytes = memoryStream.ToArray();
					}

					postStream.Write(fileBytes, 0, fileBytes.Length);
					postStream.Close();

					using (var uploadResponse = (HttpWebResponse)restRqst.GetResponse())
					{
						postStream = uploadResponse.GetResponseStream();
						postReader = new StreamReader(postStream);

						results = postReader.ReadToEnd();
						var data = jss.Deserialize<dynamic>(results);
						var fileUrl = data["d"]["ServerRelativeUrl"];
						return baseUrl + fileUrl;
					}
				}
			}
		}

		internal static string GetAccessToken(string url)
		{
			Uri uri = new Uri(url);

			string realm = TokenHelper.GetRealmFromTargetUrl(uri);

			//Get the access token for the URL.  
			//   Requires this app to be registered with the tenant
			string accessToken = TokenHelper.GetAppOnlyAccessToken(
				TokenHelper.SharePointPrincipal,
				uri.Authority, realm).AccessToken;

			return accessToken;
		}

		static void Main(string[] args)
		{
			string url = "https://[tenant].sharepoint.com";
			
			/// SharePoint Folder Relative Url
			string folderUrl = "";

			var fileUrl = UploadRest(url, folderUrl, "../../Files/SP2013_LargeFile1.pptx");
			Console.Write(fileUrl);
		}
	}
}
