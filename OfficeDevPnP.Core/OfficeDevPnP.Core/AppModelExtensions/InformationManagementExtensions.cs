using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.InformationPolicy;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            web.Context.ExecuteQuery();
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
                web.Context.ExecuteQuery();
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
                web.Context.ExecuteQuery();
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
            web.Context.ExecuteQuery();

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
                web.Context.ExecuteQuery();
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
                SitePolicyEntity policy = policies.Where(p => p.Name == sitePolicy).FirstOrDefault();
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
            web.Context.ExecuteQuery();

            if (sitePolicies != null && sitePolicies.Count > 0)
            {
                ProjectPolicy policyToApply = sitePolicies.Where(p => p.Name == sitePolicy).FirstOrDefault();
                                
                if (policyToApply != null)
                {
                    ProjectPolicy.ApplyProjectPolicy(web.Context, web, policyToApply);
                    web.Context.ExecuteQuery();
                    result = true;
                }
            }

            return result;
        }

        #region Experimental, not ready
        //Note: this does not work and will break the site policy stuff in your site...do not yet use it!
        //public static void CreateNewSitePolicy(this Web web, string sitePolicy)
        //{
        //    ContentTypeCollection contentTypes = web.ContentTypes;
        //    web.Context.Load(contentTypes);
        //    web.Context.ExecuteQuery();

        //    ContentType parentContentType = null;
        //    foreach (ContentType ct in contentTypes)
        //    {
        //        if (ct.Id.StringValue.Equals("0x010085EC78BE64F9478aAE3ED069093B9963", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            parentContentType = ct;
        //            break;
        //        }
        //    }

        //    if (parentContentType != null)
        //    {
        //        // Specifies properties that are used as parameters to initialize a new content type.
        //        ContentTypeCreationInformation contentTypeCreation = new ContentTypeCreationInformation();
        //        contentTypeCreation.Name = sitePolicy;
        //        contentTypeCreation.Description = "";
        //        contentTypeCreation.Group = parentContentType.Group;
        //        contentTypeCreation.ParentContentType = parentContentType;

        //        ContentType newPolicyContentType = contentTypes.Add(contentTypeCreation);
        //        web.Context.ExecuteQuery();

                
        //    }
        //
        ////cleanup code for when above method goes wrong    
        ////ContentTypeCollection contentTypes = cc.Web.ContentTypes;
        ////cc.Load(contentTypes);
        ////cc.ExecuteQuery();

        ////foreach (ContentType ct in contentTypes)
        ////{
        ////    cc.Load(ct.Parent);
        ////    cc.ExecuteQuery();
        ////    if (ct.Parent.Id.StringValue.Equals("0x010085EC78BE64F9478aAE3ED069093B9963", StringComparison.InvariantCultureIgnoreCase))
        ////    {

        ////        cc.Load(ct.Fields);
        ////        cc.ExecuteQuery();    

        ////        if (ct.Name.Equals("hello world", StringComparison.InvariantCultureIgnoreCase))
        ////        {
        ////            ct.DeleteObject();
        ////            cc.ExecuteQuery();
        ////        }
        ////    }
        ////}
        //}
        #endregion




    }
}
