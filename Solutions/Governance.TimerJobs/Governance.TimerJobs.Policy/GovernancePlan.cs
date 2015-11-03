using System;
using System.Collections.Generic;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// A set of site policies for a specific type of site collections
    /// </summary>
    public class GovernancePlan
    {
        /// <summary>
        /// Governance plan id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Governance plan name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Govenrance plan description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The attched site policies
        /// </summary>
        public IEnumerable<ISitePolicy> PolicyCollection { get; set; }
    }
}