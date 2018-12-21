using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPOGraphConsumer.Models
{
    public class ListsViewModel
    {
        public String SiteId { get; set; }

        [JsonProperty("value")]
        public List<ListInfoViewModel> Lists { get; set; }
    }
}