using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.DocumentSetsWeb.Models
{
    public class ContentTypeModel
    {
        public string Name { get; set; }
        public string StringId { get { return Id.StringValue; } }
        public ContentTypeId  Id { get; set; }
    }
}