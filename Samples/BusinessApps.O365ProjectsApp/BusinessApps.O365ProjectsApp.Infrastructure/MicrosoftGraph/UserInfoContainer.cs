using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    /// <summary>
    /// Defines a recipient of an email message/meeting
    /// </summary>
    public class UserInfoContainer
    {
        /// <summary>
        /// The email address of the recipient
        /// </summary>
        [JsonProperty("emailAddress")]
        public UserInfo Recipient { get; set; }
    }
}