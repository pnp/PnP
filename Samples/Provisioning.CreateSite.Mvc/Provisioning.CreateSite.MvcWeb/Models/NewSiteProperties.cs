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
    public class NewSiteProperties
    {
        [Required]
        [Remote("ValidateSite", "Home")]
        public string Url { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Site Owner")]
        public string SiteOwnerEmail { get; set; }

        public string SelectedWebTemplate { get; set; }

        [Display(Name = "Template")]
        public List<SelectListItem> WebTemplate { get; set; }

        [HiddenInput]
        public string SPHostUrl { get; set; }
        public NewSiteProperties()
        {
            WebTemplate = new List<SelectListItem>();
            foreach (var key in ConfigurationManager.AppSettings.Keys)
            {
                if (key.ToString().StartsWith("Template:"))
                {
                    WebTemplate.Add(new SelectListItem { Value = ConfigurationManager.AppSettings[key.ToString()], Text = key.ToString().Split(':')[1] });
                }
            }
        }
    }
}