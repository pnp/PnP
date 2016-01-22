using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Office365Api.Graph.Simple.MailAndFiles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Office365Api.Graph.Simple.MailAndFiles.Helpers
{
    public class GraphHelper
    {
        static MediaTypeWithQualityHeaderValue Json = new MediaTypeWithQualityHeaderValue("application/json");

        /// <summary>
        /// Query email information from the grap
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<EmailMessage>> GetEmails(string accessToken)
        {
            List<EmailMessage> emailMessages = new List<EmailMessage>();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, Settings.GetMyEmails))
                {
                    request.Headers.Accept.Add(Json);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // Do something with the data... 
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

                            foreach (JToken result in json["value"])
                            {
                                if (!result["categories"].Any())
                                {
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.MessageID = result["id"].ToString();
                                    emailMessage.Sender = result["sender"]["emailAddress"]["name"].ToString();
                                    emailMessage.SentTimestamp = (DateTime)result["sentDateTime"];
                                    emailMessage.SentTimestampString = result["sentDateTime"].ToString();
                                    emailMessage.Subject = result["subject"].ToString();
                                    emailMessage.Body = result["body"]["content"].ToString();

                                    emailMessages.Add(emailMessage);
                                }
                            }

                            return emailMessages;
                        }
                    }
                }
            }

            return emailMessages;
        }

        /// <summary>
        /// Query personal files from the Microsoft Grap
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<OD4BFile>> GetPersonalFiles(string accessToken)
        {
            List<OD4BFile> files = new List<OD4BFile>();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, Settings.GetMyFilesUrl))
                {
                    request.Headers.Accept.Add(Json);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // Do something with the data... 
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

                            foreach (JToken result in json["value"])
                            {
                                OD4BFile file = new OD4BFile();
                                file.Id = result["id"].ToString();
                                file.FileName = result["name"].ToString();
                                file.LastModifiedBy = result["lastModifiedBy"]["user"]["displayName"].ToString();
                                file.LastModifiedDate = (DateTime)result["lastModifiedDateTime"];
                                file.LastModifiedDateString = result["lastModifiedDateTime"].ToString();
                                file.WebUrl = result["webUrl"].ToString();

                                files.Add(file);
                            }
                        }
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Get information around the logged user
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<UserInformation> GetUserInfoAsync(string accessToken)
        {
            UserInformation myInfo = new UserInformation();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, Settings.GetMeUrl))
                {
                    request.Headers.Accept.Add(Json);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            myInfo.Name = json?["displayName"]?.ToString();
                            myInfo.Address = json?["mail"]?.ToString().Trim().Replace(" ", string.Empty);

                        }
                    }
                }
            }

            return myInfo;
        }
    }
}