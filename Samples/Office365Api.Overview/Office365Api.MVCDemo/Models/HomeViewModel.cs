using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office365Api.MVCDemo.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            this.Office365ActionResult = String.Empty;
            this.Items = new List<String>();
        }

        public String Office365ActionResult { get; set; }

        public List<String> Items { get; set; }
    }
}