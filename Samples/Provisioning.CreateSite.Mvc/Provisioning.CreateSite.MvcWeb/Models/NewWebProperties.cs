using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Provisioning.CreateSite.MvcWeb.Models
{
    public class NewWebProperties
    {
        [Required]
        [Remote("ValidateWeb", "Home")]
        public string Url { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Site Owner")]

        public string SelectedWebTemplate { get; set; }

        [Display(Name = "Template")]
        public List<SelectListItem> WebTemplate { get; set; }

        [HiddenInput]
        public string SPHostUrl { get; set; }

        public NewWebProperties()
        {

        }

        public NewWebProperties(SP.WebTemplateCollection webTemplates)
        {
            WebTemplate = new List<SelectListItem>();

            foreach (var template in webTemplates)
            {
                WebTemplate.Add(new SelectListItem { Value = template.Name, Text = template.Title });
            }
        }
    }
}