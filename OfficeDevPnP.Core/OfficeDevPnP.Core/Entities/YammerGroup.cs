using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAMS.Core.Entities
{
    public class YammerGroupStats
    {
        public int members { get; set; }
        public int updates { get; set; }
        public int? last_message_id { get; set; }
        public string last_message_at { get; set; }
    }

    /// <summary>
    /// Reprisents Yammer Group information
    /// Generated based on Yammer response on 30th fo June 2014 and using http://json2csharp.com/ service 
    /// </summary>
    public class YammerGroup
    {
        public string type { get; set; }
        public int id { get; set; }
        public string full_name { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public string privacy { get; set; }
        public string url { get; set; }
        public string web_url { get; set; }
        public string mugshot_url { get; set; }
        public string mugshot_url_template { get; set; }
        public object mugshot_id { get; set; }
        public string show_in_directory { get; set; }
        public object office365_url { get; set; }
        public string created_at { get; set; }
        public string creator_type { get; set; }
        public int creator_id { get; set; }
        public string state { get; set; }
        public YammerGroupStats stats { get; set; }
        // Added manually as extended property which can be set if needed in the code. Set in YammerUtility class code automatically
        public int network_id { get; set; }
        public string network_name { get; set; }
    }
}
