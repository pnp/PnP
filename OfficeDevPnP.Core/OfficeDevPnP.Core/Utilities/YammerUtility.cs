using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OfficeDevPnP.Core.Utilities
{
    public static class YammerUtility
    {
        /// <summary>
        /// Lock object to make GetYammerJson requests thread safe
        /// </summary>
        private static readonly object lockObjGetJson = new object();

        /// <summary>
        /// Lock object to make PostYammerJson requests thread safe
        /// </summary>
        private static readonly object lockObjPostJson = new object();

        /// <summary>
        /// Cache to store yammer json response.
        /// This will help minimize extra GetYammerJson requests.
        /// </summary>
        private static List<KeyValuePair<string, string>> cacheYammerJsonResponse = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Returns Yammer Group if group exists. If the group does not exist, returns null.
        /// </summary>
        /// <param name="groupName">Group name to search for</param>
        /// <param name="accessToken">Access token to the Yammer network</param>
        /// <returns>Yammer Group, if exists. otherwise null.</returns>
        public static YammerGroup GetYammerGroupByName(string groupName, string accessToken)
        {
            YammerGroup yamGroup = null;
            var groups = GetYammerGroups(accessToken);

            foreach (var item in groups)
            {
                if (item.full_name.Equals(groupName, StringComparison.CurrentCultureIgnoreCase))
                {
                    yamGroup = item;
                }
            }

            return yamGroup;
        }

        /// <summary>
        /// Returns Yammer Group if group exists. If the group does not exist, returns null.
        /// </summary>
        /// <param name="groupId">Group id to search for</param>
        /// <param name="accessToken"></param>
        /// <returns>Yammer Group, if exists. otherwise null.</returns>
        public static YammerGroup GetYammerGroupById(int groupId, string accessToken)
        {
            YammerGroup yamGroup = null;
            var groups = GetYammerGroups(accessToken);

            foreach (var item in groups)
            {
                if (item.id == groupId)
                {
                    yamGroup = item;
                }
            }

            return yamGroup;
        }

        /// <summary>
        /// Returns Yammer groups based on the access token. All groups are returned for registered apps.
        /// </summary>
        /// <param name="accessToken">Access token to the Yammer network</param>
        /// <returns>All groups in the network</returns>
        public static List<YammerGroup> GetYammerGroups(string accessToken)
        {
            // Get user
            YammerUser user = GetYammerUser(accessToken);

            //get the users groups to check for the group
            var response = GetYammerJson(String.Format("https://www.yammer.com/api/v1/groups/for_user/{0}.json", user.id), accessToken);
            List<YammerGroup> groups = JsonUtility.Deserialize<List<YammerGroup>>(response);

            // Updated network information to the group data
            foreach (var item in groups)
            {
                item.network_id = user.network_id;
                item.network_name = user.network_name;
            }

            return groups;
        }

        /// <summary>
        /// Returns Yammer user based on the access token.
        /// </summary>
        /// <param name="accessToken">Access token to the Yammer network</param>
        /// <returns>Yammer user in the network</returns>
        public static YammerUser GetYammerUser(string accessToken)
        {
            // Get service account token
            var response = GetYammerJson(String.Format("https://www.yammer.com/api/v1/users/current.json?access_token={0}", accessToken), accessToken);
            return JsonUtility.Deserialize<YammerUser>(response);
        }

        /// <summary>
        /// Creates Yammer group to the Yammer network
        /// </summary>
        /// <param name="groupName">Name of the Yammer group to create</param>
        /// <param name="isPrivate">Visibility of the Yammer group</param>
        /// <param name="accessToken">Access token to the Yammer network</param>
        /// <returns>Yammer group</returns>
        public static YammerGroup CreateYammerGroup(string groupName, bool isPrivate, string accessToken)
        {
            // Get reference of existing group, if exists
            YammerGroup yamGroup = GetYammerGroupByName(groupName, accessToken);
            if (yamGroup == null)
            {
                // Create yammer group
                string url = String.Format("https://www.yammer.com/api/v1/groups.json?name={0}&private={1}", groupName, isPrivate.ToString().ToLower());
                PostYammerJson(url, accessToken);
                yamGroup = GetYammerGroupByName(groupName, accessToken);
            }

            return yamGroup;
        }

        /// <summary>
        /// Creates web part entity with the Yammer group structure on it
        /// </summary>
        /// <param name="yammerNetworkName">Name of the Yammer network</param>
        /// <param name="yammerGroupId">Id of the Yammer group</param>
        /// <param name="showHeader">Show header in web part</param>
        /// <param name="showFooter">Show footer in web part</param>
        /// <returns>Web part entity with the Yammer group</returns>
        public static WebPartEntity GetYammerGroupDiscussionPart(string yammerNetworkName, int yammerGroupId, bool showHeader, bool showFooter)
        {
            WebPartEntity wpYammer = new WebPartEntity();
            wpYammer.WebPartXml = CreateYammerGroupDiscussionPartXml(yammerNetworkName, yammerGroupId, showHeader, showFooter);
            wpYammer.WebPartIndex = 0;
            wpYammer.WebPartTitle = "Yammer";

            return wpYammer;
        }

        /// <summary>
        /// Creates web part entity with the Yammer OpenGraph structure on it for specific URL
        /// </summary>
        /// <param name="yammerNetworkName">Name of the Yammer network</param>
        /// <param name="url">Url of Yammer OpenGraph</param>
        /// <param name="showHeader">Show header in web part</param>
        /// <param name="showFooter">Show footer in web part</param>
        /// <returns>Web part entity with the Yammer OpenGraph</returns>
        public static WebPartEntity GetYammerOpenGraphDiscussionPart(string yammerNetworkName, string url, bool showHeader, bool showFooter, string postTitle = "", string postImageUrl = "", string defaultGroupId = "")
        {
            WebPartEntity wpYammer = new WebPartEntity();
            wpYammer.WebPartXml = CreateYammerOpenGraphDiscussionPartXml(yammerNetworkName, url, showHeader, showFooter, postTitle, postImageUrl, true, defaultGroupId);
            wpYammer.WebPartIndex = 0;
            wpYammer.WebPartTitle = "Yammer";

            return wpYammer;
        }

        /// <summary>
        /// Constructs the webpart XML for yammer group needed to inject as Yammer web part to SharePoint page
        /// </summary>
        /// <param name="yammerNetwork">Name of the Yammer network</param>
        /// <param name="yammerGroupId">ID of the Yammer group</param>
        /// <param name="showHeader">Show header in web part</param>
        /// <param name="showFooter">Show footer in web part</param>
        /// <param name="useSSO">Use SSO</param>
        /// <returns>The constructed web part XML</returns>
        public static string CreateYammerDiscussionPartXml(string yammerNetworkName, int yammerGroupId, bool showHeader, bool showFooter, bool useSSO = true)
        {
            return CreateYammerGroupDiscussionPartXml(yammerNetworkName, yammerGroupId, showHeader, showFooter, useSSO);
        }

        /// <summary>
        /// Constructs the webpart XML for yammer group needed to inject as Yammer web part to SharePoint page
        /// </summary>
        /// <param name="yammerNetwork">Name of the Yammer network</param>
        /// <param name="yammerGroupId">ID of the Yammer group</param>
        /// <param name="showHeader">Show header in web part</param>
        /// <param name="showFooter">Show footer in web part</param>
        /// <param name="useSSO">Use SSO</param>
        /// <returns>The constructed web part XML</returns>
        public static string CreateYammerGroupDiscussionPartXml(string yammerNetworkName, int yammerGroupId, bool showHeader, bool showFooter, bool useSSO = true)
        {
            StringBuilder wp = new StringBuilder(100);
            wp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            wp.Append("<webParts>");
            wp.Append("	<webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>");
            wp.Append("		<metaData>");
            wp.Append("			<type name='Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />");
            wp.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
            wp.Append("		</metaData>");
            wp.Append("		<data>");
            wp.Append("			<properties>");
            wp.Append("				<property name='Title' type='string'>$Resources:core,ScriptEditorWebPartTitle;</property>");
            wp.Append("				<property name='Description' type='string'>$Resources:core,ScriptEditorWebPartDescription;</property>");
            wp.Append("				<property name='ChromeType' type='chrometype'>None</property>");
            wp.Append("				<property name='Content' type='string'>");
            wp.Append("				<![CDATA[");
            wp.Append("				    <div id='embedded-feed' style='height: 500px;'></div>");
            wp.Append("				    <script type='text/javascript' src='https://assets.yammer.com/assets/platform_embed.js'></script>");
            wp.Append("				    <script type='text/javascript'>  yam.connect.embedFeed({ container: '#embedded-feed', network: '" + yammerNetworkName
                                                        + @"', feedType: 'group', feedId: '" + yammerGroupId
                                                        + @"', config: { use_sso: " + useSSO.ToString().ToLower()
                                                                + @", header: " + showHeader.ToString().ToLower()
                                                                + @", footer: " + showFooter.ToString().ToLower()
                                                                + " }}); </script>");
            wp.Append("				]]>");
            wp.Append("				</property>");
            wp.Append("			</properties>");
            wp.Append("		</data>");
            wp.Append("	</webPart>");
            wp.Append("</webParts>");

            return wp.ToString();
        }

        /// <summary>
        /// Constructs web part definition for Open Graph discussion web part definition
        /// </summary>
        /// <param name="yammerNetworkName">Name of the Yammer network</param>
        /// <param name="url">Url of the Yammer Open Graph Discussion</param>
        /// <param name="showHeader">Show header in web part</param>
        /// <param name="showFooter">Show footer in web part</param>
        /// <param name="postTitle">Title of the post</param>
        /// <param name="postImageUrl">Image url of the post</param>
        /// <param name="useSSO">Use SSO</param>
        /// <param name="groupId">Default Group Id</param>
        /// <returns>Web part definition for Open Graph discussion web part</returns>
        public static string CreateYammerOpenGraphDiscussionPartXml(string yammerNetworkName, string url, bool showHeader,
                                                                    bool showFooter, string postTitle = "", string postImageUrl = "",
                                                                    bool useSSO = true, string groupId = "")
        {
            StringBuilder wp = new StringBuilder(100);
            wp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            wp.Append("<webParts>");
            wp.Append("	<webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>");
            wp.Append("		<metaData>");
            wp.Append("			<type name='Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />");
            wp.Append("			<importErrorMessage>Cannot import this Web Part.</importErrorMessage>");
            wp.Append("		</metaData>");
            wp.Append("		<data>");
            wp.Append("			<properties>");
            wp.Append("				<property name='Title' type='string'>$Resources:core,ScriptEditorWebPartTitle;</property>");
            wp.Append("				<property name='Description' type='string'>$Resources:core,ScriptEditorWebPartDescription;</property>");
            wp.Append("				<property name='ChromeType' type='chrometype'>None</property>");
            wp.Append("				<property name='Content' type='string'>");
            wp.Append("				<![CDATA[");
            wp.Append("				    <div id='embedded-feed' style='height: 500px;'></div>");
            wp.Append("				    <script type='text/javascript' src='https://assets.yammer.com/assets/platform_embed.js'></script>");
            wp.Append("				    <script>");
            wp.Append("				        yam.connect.embedFeed({");
            wp.Append("				          container: '#embedded-feed'");
            wp.Append("				                , feedType: 'open-graph'");
            wp.Append("				                , feedId: ''");
            wp.Append("				                , config: {");
            wp.Append("				                     use_sso: " + useSSO.ToString().ToLower());
            wp.Append("				                     , header: " + showHeader.ToString().ToLower());
            wp.Append("				                     , footer: " + showFooter.ToString().ToLower());
            wp.Append("				                     , showOpenGraphPreview: false");
            wp.Append("				                     , defaultToCanonical: false");
            wp.Append("				                     , hideNetworkName: false");
            wp.Append("				                     , promptText: 'Start a conversation'");
            if (!string.IsNullOrEmpty(groupId))
            {
                wp.Append("				                 , defaultGroupId: '" + groupId + "'");
            }
            wp.Append("				                }");
            wp.Append("				                , objectProperties: {");
            wp.Append("				                  url: '" + url + "'");
            wp.Append("				                  , type: 'page'");
            wp.Append("				                  , title: '" + postTitle + "'");
            wp.Append("				                  , image: '" + postImageUrl + "'");
            wp.Append("				                }");
            wp.Append("				            });");
            wp.Append("				        </script>");
            wp.Append("				]]>");
            wp.Append("				</property>");
            wp.Append("			</properties>");
            wp.Append("		</data>");
            wp.Append("	</webPart>");
            wp.Append("</webParts>");

            return wp.ToString();
        }

        /// <summary>
        /// Get Yammer Json to create yammer response
        /// </summary>
        /// <param name="url">Yammer Json url</param>
        /// <param name="yammerAccessToken">Yammer access token</param>
        /// <returns>Yammer Json</returns>
        private static string GetYammerJson(string url, string yammerAccessToken)
        {
            // Yammer throws (429) too many requests exception for consecutive requests.
            // The "Rate Limits" are defined here: https://developer.yammer.com/restapi/
            // Thread safe to allow only one request at a time.
            lock (lockObjGetJson)
            {
                // Check if information is present in cache
                KeyValuePair<string, string> hashInformation = cacheYammerJsonResponse.FirstOrDefault(x => x.Key == yammerAccessToken + "_" + url);
                if (hashInformation.Key != null)
                {
                    return hashInformation.Value;
                }

                string json = null;

                // Make the request
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Headers.Add("Authorization", "Bearer" + " " + yammerAccessToken);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encode))
                    {
                        json = reader.ReadToEnd();
                    }
                }

                // Store information in cache
                cacheYammerJsonResponse.Add(new KeyValuePair<string, string>(yammerAccessToken + "_" + url, json));

                return json;
            }
        }

        /// <summary>
        /// Post Yammer json to get the result
        /// </summary>
        /// <param name="url">Yammer Json url</param>
        /// <param name="yammerAccessToken">Yammer access token</param>
        /// <returns>Yammer Json</returns>
        private static string PostYammerJson(string url, string yammerAccessToken)
        {
            // Yammer throws (429) too many requests exception for consecutive requests. 
            // The "Rate Limits" are defined here: https://developer.yammer.com/restapi/
            // Thread safe to allow only one request at a time.
            lock (lockObjPostJson)
            {
                string json = null;

                // Make the request
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer" + " " + yammerAccessToken);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encode))
                    {
                        json = reader.ReadToEnd();
                    }
                }

                return json;
            }
        }
    }
}
