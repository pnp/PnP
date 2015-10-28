using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Governance.TimerJobs.Policy
{
    public abstract class SitePolicy : ISitePolicy
    {
        #region ISitePolicy Members

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid Id { get; set; }

        /// <summary>
        ///     SiteScopePredictor is used by GovernancePreprocessJob to query the matching site records from database, the
        ///     expression of each concrete SitePolicy will be query and process separately.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The body of the lambda expression must be a statement rather than a block. All supported canonical functions
        ///         can be found at here: https://msdn.microsoft.com/en-us/library/bb738681.aspx, all other variable calculation /
        ///         evaluation must be done out of the closure.
        ///     </para>
        /// </remarks>
        public virtual Expression<Func<WebInformation, bool>> PreprocessPredictor
        {
            get { return null; }
        }

        public virtual void Preprocess(SiteInformation siteCollection, WebInformation web, TimerJobRunEventArgs e)
        {
        }

        /// <summary>
        ///     NoncompliancePredictor is used by GovernanceJob to query the incompliant site records from database, the expression
        ///     of each concrete SitePolicy will be merged into the where clause of a Linq to Entity query.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The body of the lambda expression must be a statement rather than a block. All supported canonical functions
        ///         can be found at here: https://msdn.microsoft.com/en-us/library/bb738681.aspx, all other variable calculation /
        ///         evaluation must be done out of the closure.
        ///     </para>
        /// </remarks>
        public abstract Expression<Func<SiteInformation, bool>> NoncompliancePredictor { get; }

        public abstract IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site);

        public abstract bool IsCompliant(SiteInformation site);

        public virtual void Process(SiteInformation site)
        {
            var state = site.ComplianceState;
            var lockDate =
                GovernanceWorkflowHelper.GetCurrentBusinessTime()
                    .AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["DefaultFirstLockNotificationDays"]));
            if (state.LockedDate > lockDate || state.LockedDate == DateTime.MinValue)
                state.LockedDate = lockDate;
        }

        #endregion
    }
}