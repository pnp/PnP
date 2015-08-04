using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Utilities;
using WebInformation = Governance.TimerJobs.Data.WebInformation;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    ///     HbiBroadAccessPolicy make sure there is no permission granted to predefined large security groups at either site collection level of sub web level
    /// </summary>
    public class HbiBroadAccessPolicy : SitePolicy
    {
        /// <summary>
        /// PermissionAssignment represents a SharePoint permission assignment of a securable object
        /// </summary>
        private class PermissionAssignment
        {
            /// <summary>
            /// The URL of the current web
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// The large security group which the permission has been granted to
            /// </summary>
            public string Group { get; set; }
            /// <summary>
            /// The BasePermission mask of the permission assignment
            /// </summary>
            public ClientResult<BasePermissions> Permission { get; set; }
        }

        /// <summary>
        /// A set of predefined large security groups to be detected
        /// </summary>
        private Dictionary<string, string> BroadAccessGroups { get; set; }

        /// <summary>
        /// Get predefined large security groups from app.config file
        /// </summary>
        private void LoadBroadAccessGroups()
        {
            var groups = ConfigurationManager.AppSettings["BroadAccessGroups"];
            BroadAccessGroups = JsonUtility.Deserialize<Dictionary<string, string>>(groups);
        }

        /// <summary>
        /// Construct a new HbiBroadAccessPolicy instance 
        /// </summary>
        public HbiBroadAccessPolicy()
        {
            LoadBroadAccessGroups();
        }

        /// <summary>
        /// All HBI webs will be selected from DB repository for policy preprocess
        /// </summary>
        public override Expression<Func<WebInformation, bool>> PreprocessPredictor
        {
            get { return web => web.BusinessImpact == SitePolicyResources.BusinessImpact_HBI; }
        }

        /// <summary>
        /// All site collection records with HasBroadAccess flag equals to 1 will be selected from DB repository as in-compliant ones
        /// </summary>
        public override Expression<Func<SiteInformation, bool>> NoncompliancePredictor
        {
            get { return site => site.HasBroadAccess; }
        }

        public override IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site)
        {
            if (IsCompliant(site))
                yield break;
            yield return NoncomplianceType.NoAdditionalSiteAdmin;
        }

        /// <summary>
        /// All site information entity with HasBroadAccess property set to false is compliant ones
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public override bool IsCompliant(SiteInformation site)
        {
            return !site.HasBroadAccess;
        }

        /// <summary>
        /// Update HasBroadAccess to 1 for both of the site collection and the current web record if any large security group permission assignment was found on the current web
        /// </summary>
        /// <param name="dbSiteRecord">The site collection record</param>
        /// <param name="dbWebRecord">The current web record</param>
        /// <param name="e">The timer job run event arguments</param>
        public override void Preprocess(SiteInformation dbSiteRecord, WebInformation dbWebRecord, TimerJobRunEventArgs e)
        {
            var tenant = new Tenant(e.TenantClientContext);
            var site = tenant.GetSiteByUrl(e.Url);
            var web = e.Url == dbWebRecord.SiteUrl
                ? site.RootWeb
                : site.OpenWeb(e.Url.Substring(dbWebRecord.Name.IndexOf('/') + 1));
            // load additional properties could be used to optimize the permission checking process
            e.TenantClientContext.Load(web,
                w => w.HasUniqueRoleAssignments,
                w => w.Url,
                w => w.ServerRelativeUrl,
                w => w.ParentWeb.ServerRelativeUrl);

            var assignments = new List<PermissionAssignment>();
            var entries = from groupLoginName in BroadAccessGroups.Keys
                select new PermissionAssignment
                {
                    Url = e.Url,
                    Group = groupLoginName,
                    Permission = web.GetUserEffectivePermissions(groupLoginName)
                };
            assignments.AddRange(entries);
            e.TenantClientContext.ExecuteQuery();

            var incompliantAssignments = assignments.Where(
                p => p.Permission.Value != null && (
                    p.Permission.Value.Has(PermissionKind.ViewPages) ||
                    p.Permission.Value.Has(PermissionKind.ViewListItems)));

            dbWebRecord.HasBroadAccess = incompliantAssignments.Any();
            if (dbWebRecord.HasBroadAccess)
            {
                dbWebRecord.BroadAccessGroups = string.Join(";",
                    (from a in incompliantAssignments select a.Group).ToArray());
                dbSiteRecord.HasBroadAccess = true;
            }
        }
    }
}