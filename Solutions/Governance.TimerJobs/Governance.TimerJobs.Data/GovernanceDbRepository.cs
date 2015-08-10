using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.Data
{
    /// <summary>
    /// GovernanceDbRepository
    /// </summary>
    public class GovernanceDbRepository
    {
        /// <summary>
        /// The database connection string
        /// </summary>
        private string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a new instance of GovernanceDbRepository
        /// </summary>
        /// <param name="connectionString"></param>
        public GovernanceDbRepository(string connectionString)
        {
            ConnectionString = connectionString; 
        }

        /// <summary>
        /// Provide a DB context object for client code to execute database operations
        /// </summary>
        /// <param name="action">The ad-hoc database operation which takes a GovernanceDbContext instance</param>
        public void UsingContext(Action<GovernanceDbContext> action)
        {
            using (var context = new GovernanceDbContext(ConnectionString))
            {
                action(context);
            }
        }
    }
}
