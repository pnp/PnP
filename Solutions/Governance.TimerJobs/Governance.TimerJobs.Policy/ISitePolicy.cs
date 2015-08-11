using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Governance.TimerJobs.Policy
{
    public interface ISitePolicy
    {
        string Name { get; set; }

        string Description { get; set; }

        Guid Id { get; set; }

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
        Expression<Func<SiteInformation, bool>> NoncompliancePredictor { get; }

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
        Expression<Func<WebInformation, bool>> PreprocessPredictor { get; }

        void Preprocess(SiteInformation siteCollection, WebInformation web, TimerJobRunEventArgs e);
        IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site);
        bool IsCompliant(SiteInformation site);
        void Process(SiteInformation site);
    }
}