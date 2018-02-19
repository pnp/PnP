using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Provisioning.Async.Function
{
    public class Graph
    {
        public static string getToken()
        {
            string endPoint = Environment.GetEnvironmentVariable("TokenEndpoint");
            string data = "grant_type=client_credentials&client_id=" + Environment.GetEnvironmentVariable("ClientId") +
                "&client_secret=" + Environment.GetEnvironmentVariable("ClientSecret") + "&resource=https://graph.microsoft.com";
            string contenType = "application/x-www-form-urlencoded";
            string responseFromServer = requestGetToken(endPoint, data, contenType);
            AccessToken accessToken = JsonConvert.DeserializeObject<AccessToken>(responseFromServer);

            return accessToken.access_token;
        }

        public static string getUser(string token, string userPrincipalName)
        {
            string endPoint = "https://graph.microsoft.com/v1.0/users/" + userPrincipalName + "?$select=id";
            string responseFromServer = requestGet(endPoint, token);
            GraphUser graphUser = JsonConvert.DeserializeObject<GraphUser>(responseFromServer);

            return graphUser.id;
        }

        public static string createUnifiedGroup(string token, string data)
        {
            string endPoint = "https://graph.microsoft.com/v1.0/groups/";
            string contenType = "application/json";
            string responseFromServer = requestPost(token, endPoint, data, contenType);
            GraphGroup group = JsonConvert.DeserializeObject<GraphGroup>(responseFromServer);

            return group.id;
        }

        public static bool addOwnerToUnifiedGroup(string token, string groupId, string userId)
        {
            bool ownerAdded = false;

            string endPoint = "https://graph.microsoft.com/v1.0/groups/" + groupId + "/owners/$ref";

            string data = "{ '@odata.id': 'https://graph.microsoft.com/v1.0/users/" + userId + "' }";
            string contenType = "application/json";
            string responseFromServer = requestPost(token, endPoint, data, contenType);
            ownerAdded = true;

            return ownerAdded;
        }

        public static bool removeOwnerToUnifiedGroup(string token, string groupId, string userId)
        {
            bool ownerRemoved = false;
            string endPoint = "https://graph.microsoft.com/v1.0/groups/" + groupId + "/owners/" + userId + "/$ref";
            string data = "";
            string contenType = "application/json";
            string responseFromServer = requestDelete(token, endPoint, data, contenType);
            ownerRemoved = true;

            return ownerRemoved;
        }

        private static string requestGetToken(string endPoint, string postData, string contentType = null)
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create(endPoint);
            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            if (string.IsNullOrEmpty(contentType) == false)
            {
                // Set the ContentType property of the WebRequest.  
                request.ContentType = contentType;
            }
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        private static string requestGet(string endPoint, string token, string postData = null, string contentType = null)
        {
            // Create a request for the URL.   
            WebRequest request = WebRequest.Create(endPoint);
            // If required by the server, set the credentials.  
            //request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("Authorization", "Bearer " + token);
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.  
            reader.Close();
            response.Close();

            return responseFromServer;
        }

        private static string requestPost(string token, string endPoint, string postData, string contentType = null)
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create(endPoint);
            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.Headers.Add("Authorization", "Bearer " + token);
            if (string.IsNullOrEmpty(contentType) == false)
            {
                // Set the ContentType property of the WebRequest.  
                request.ContentType = contentType;
            }
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        private static string requestDelete(string token, string endPoint, string postData, string contentType = null)
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create(endPoint);
            // Set the Method property of the request to POST.  
            request.Method = "DELETE";
            // Create POST data and convert it to a byte array.  
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.Headers.Add("Authorization", "Bearer " + token);
            if (string.IsNullOrEmpty(contentType) == false)
            {
                // Set the ContentType property of the WebRequest.  
                request.ContentType = contentType;
            }
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public class AccessToken
        {
            public String token_type { get; set; }
            public String resource { get; set; }
            public String access_token { get; set; }
            public String expires_in { get; set; }
            public String ext_expires_in { get; set; }
            public String expires_on { get; set; }
            public String not_before { get; set; }
        }

        public class GraphGroup
        {
            public String id { get; set; }
        }

        public class GraphUser
        {
            public String id { get; set; }
        }
    }
}
