using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client.InformationPolicy;
using OfficeDevPnP.Core.Entities;

namespace Microsoft.SharePoint.Client {

    /// <summary>
    /// Class that deals with information management features
    /// </summary>
    public static class InformationManagementExtensions
    {

        /// <summary>
        /// Does this web have a site policy applied?
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <returns>True if a policy has been applied, false otherwise</returns>
        public static bool HasSitePolicyApplied(this Web web)
        {
            ClientResult<bool> hasSitePolicyApplied = ProjectPolicy.DoesProjectHavePolicy(web.Context, web);
            web.Context.ExecuteQueryRetry();
            return hasSitePolicyApplied.Value;
        }

        /// <summary>
        /// Gets the site expiration date
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <returns>DateTime value holding the expiration date, DateTime.MinValue in case there was no policy applied</returns>
        public static DateTime GetSiteExpirationDate(this Web web)
        {
            if (web.HasSitePolicyApplied())
            {
                ClientResult<DateTime> expirationDate = ProjectPolicy.GetProjectExpirationDate(web.Context, web);
                web.Context.ExecuteQueryRetry();
                return expirationDate.Value;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the site closure date
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <returns>DateTime value holding the closure date, DateTime.MinValue in case there was no policy applied</returns>
        public static DateTime GetSiteCloseDate(this Web web)
        {
            if (web.HasSitePolicyApplied())
            {
                ClientResult<DateTime> closeDate = ProjectPolicy.GetProjectCloseDate(web.Context, web);
                web.Context.ExecuteQueryRetry();
                return closeDate.Value;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets a list of the available site policies
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <returns>A list of <see cref="SitePolicyEntity"/> objects</returns>
        public static List<SitePolicyEntity> GetSitePolicies(this Web web)
        {
            ClientObjectList<ProjectPolicy> sitePolicies = ProjectPolicy.GetProjectPolicies(web.Context, web);
            web.Context.Load(sitePolicies);
            web.Context.ExecuteQueryRetry();

            List<SitePolicyEntity> policies = new List<SitePolicyEntity>();

            if (sitePolicies != null && sitePolicies.Count > 0)
            {
                foreach (var policy in sitePolicies)
                {
                    policies.Add(new SitePolicyEntity
                    {
                        Name = policy.Name,
                        Description = policy.Description,
                        EmailBody = policy.EmailBody,
                        EmailBodyWithTeamMailbox = policy.EmailBodyWithTeamMailbox,
                        EmailSubject = policy.EmailSubject
                    });
                }
            }

            return policies;
        }

        /// <summary>
        /// Gets the site policy that currently is applied
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <returns>A <see cref="SitePolicyEntity"/> object holding the applied policy</returns>
        public static SitePolicyEntity GetAppliedSitePolicy(this Web web)
        {
            if (web.HasSitePolicyApplied())
            {
                ProjectPolicy policy = ProjectPolicy.GetCurrentlyAppliedProjectPolicyOnWeb(web.Context, web);
                web.Context.Load(policy,
                             p => p.Name,
                             p => p.Description,
                             p => p.EmailSubject,
                             p => p.EmailBody,
                             p => p.EmailBodyWithTeamMailbox);
                web.Context.ExecuteQueryRetry();
                return new SitePolicyEntity
                    {
                        Name = policy.Name,
                        Description = policy.Description,
                        EmailBody = policy.EmailBody,
                        EmailBodyWithTeamMailbox = policy.EmailBodyWithTeamMailbox,
                        EmailSubject = policy.EmailSubject
                    };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the site policy with the given name
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <param name="sitePolicy">Site policy to fetch</param>
        /// <returns>A <see cref="SitePolicyEntity"/> object holding the fetched policy</returns>
        public static SitePolicyEntity GetSitePolicyByName(this Web web, string sitePolicy)
        {
            List<SitePolicyEntity> policies = web.GetSitePolicies();

            if (policies.Count > 0)
            {
                SitePolicyEntity policy = policies.FirstOrDefault(p => p.Name == sitePolicy);
                return policy;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Apply a policy to a site
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <param name="sitePolicy">Policy to apply</param>
        /// <returns>True if applied, false otherwise</returns>
        public static bool ApplySitePolicy(this Web web, string sitePolicy)
        {
            bool result = false;
            
            ClientObjectList<ProjectPolicy> sitePolicies = ProjectPolicy.GetProjectPolicies(web.Context, web);
            web.Context.Load(sitePolicies);
            web.Context.ExecuteQueryRetry();

            if (sitePolicies != null && sitePolicies.Count > 0)
            {
                ProjectPolicy policyToApply = sitePolicies.FirstOrDefault(p => p.Name == sitePolicy);
                                
                if (policyToApply != null)
                {
                    ProjectPolicy.ApplyProjectPolicy(web.Context, web, policyToApply);
                    web.Context.ExecuteQueryRetry();
                    result = true;
                }
            }

            return result;
        }

    }
}
