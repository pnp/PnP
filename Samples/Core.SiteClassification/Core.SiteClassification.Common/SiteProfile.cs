using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common
{
    public class SiteProfile
    {
     
        public Dictionary<string, string> CustomProperties 
        {
            get;
            set;
        }
        public DateTime ExpirationDate { get; internal set; }

        public string SitePolicy
        {
            get;
            set;
        }
    }
}
