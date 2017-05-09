using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Provisioning.Cloud.Management.Models
{
    public class LanguageVM
    {
        public int LanguageId { get; set; }

        public string DisplayName { get; set; }

        public LanguageVM(int languageId)
        {
            this.LanguageId = languageId;
            this.DisplayName = CultureInfo.GetCultureInfo(languageId).DisplayName;
        }        
    }

    public class WebTemplateVM
    {
        public int Id { get; set; }

        public uint Lcid { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public WebTemplateVM(WebTemplate template)
        {
            this.Id = template.Id;
            this.Lcid = template.Lcid;
            this.Name = template.Name;
            this.Title = template.Title;
        }
    }
}