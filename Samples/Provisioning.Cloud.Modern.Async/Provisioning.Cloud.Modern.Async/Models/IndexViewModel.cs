using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Provisioning.Cloud.Modern.Async.Models
{
    public class IndexViewModel
    {
        public String CurrentUserPrincipalName { get; set; }

        [Required(ErrorMessage = "Please select a \"modern\" site type")]
        [DisplayName("Site type")]
        [UIHint("SiteType")]
        public SiteType SiteType { get; set; }

        [Required(ErrorMessage = "Please provide  a title for the \"modern\" site")]
        [DisplayName("Site Title")]
        public String SiteTitle { get; set; }

        [Required(ErrorMessage = "Please provide an alias for the \"modern\" site")]
        [DisplayName("Site Alias")]
        public String SiteAlias { get; set; }

        [DisplayName("Site Description")]
        public String SiteDescription { get; set; }

        [Required(ErrorMessage = "Please choose if the \"modern\" site will be Public or Private")]
        [DisplayName("Privacy Settings")]
        [UIHint("PrivacySettings")]
        public Boolean IsPublic { get; set; }

        [JsonIgnore]
        public List<String> Classifications { get; set; }

        [DisplayName("Site Creation Technique")]
        [UIHint("AsynchronousTechnique")]
        [JsonIgnore]
        public AsynchronousTechnique AsyncTech { get; set; }

        [DisplayName("PnP Provisioning Template")]
        [UIHint("PnPTemplate")]
        public String PnPTemplate { get; set; }

        public String UserAccessToken { get; set; }

        public String SPORootSiteUrl { get; set; }
    }

    /// <summary>
    /// Defines the available "modern" site options
    /// </summary>
    public enum SiteType
    {
        /// <summary>
        /// "modern" team site
        /// </summary>
        [Display(Name = "Team Site")]
        TeamSite,
        /// <summary>
        /// "modern" communication site
        /// </summary>
        [Display(Name = "Communication Site")]
        CommunicationSite,
    }

    /// <summary>
    /// Defines the two async site creation options
    /// </summary>
    public enum AsynchronousTechnique
    {
        /// <summary>
        /// "modern" team site
        /// </summary>
        [Display(Name = "Azure Function")]
        AzureFunction,
        /// <summary>
        /// "modern" communication site
        /// </summary>
        [Display(Name = "Azure WebJob")]
        AzureWebJob,
    }
}