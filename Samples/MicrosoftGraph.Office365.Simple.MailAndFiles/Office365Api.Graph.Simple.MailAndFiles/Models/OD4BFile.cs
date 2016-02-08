using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Office365Api.Graph.Simple.MailAndFiles.Models
{
    public class OD4BFile
    {
        public string FileName { get; set; }

        public string Id { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedDateString { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string WebUrl { get; set; }
    }
}