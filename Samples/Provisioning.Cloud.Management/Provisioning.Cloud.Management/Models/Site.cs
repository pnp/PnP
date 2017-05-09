using Microsoft.Online.SharePoint.TenantAdministration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Provisioning.Cloud.Management.Models
{
    public class Site
    {
        public String Title { get; set; }

        public String Uri { get; set; }

        public uint Language { get; set; }

        public String Owner { get; set; }

        public String Status { get; set; }

        public String Template { get; set; }

        public long StorageMaximumLevel { get; set; }

        public double UserCodeMaximumLevel { get; set; }

        public Site()
        {
        }

        public Site(SiteProperties siteProperties) : this()
        {
            // Skip loading for now
            this.Title = siteProperties.Title;
            this.Uri = siteProperties.Url;
            this.Language = siteProperties.Lcid;
            this.Owner = siteProperties.Owner;
            this.Status = siteProperties.Status;
            this.Template = siteProperties.Template;
            this.StorageMaximumLevel = siteProperties.StorageMaximumLevel;
            this.UserCodeMaximumLevel = siteProperties.UserCodeMaximumLevel;
        }
    }
}