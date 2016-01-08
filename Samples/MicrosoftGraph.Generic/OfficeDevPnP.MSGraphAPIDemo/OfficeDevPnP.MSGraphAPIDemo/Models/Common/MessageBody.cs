using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBody
    {
        /// <summary>
        /// 
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("contentType")]
        public BodyType Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BodyType
    {
        Text,
        Html,
    }
}