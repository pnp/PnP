using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    /// <summary>
    /// 
    /// </summary>
    public class ItemBody
    {
        /// <summary>
        /// 
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("contentType")]
        [JsonConverter(typeof(StringEnumConverter))]
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