using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.DocumentSetsWeb.Models
{
    public class AddAllowedContentTypeViewModel
    {
        //public SelectList ContentTypeList { get; set; }
        public string SelectedStringId { get; set; }

        public AddAllowedContentTypeViewModel(string selectedStringId)
        {
            SelectedStringId = selectedStringId;
        }
        public AddAllowedContentTypeViewModel()
        {
        }
    }
}