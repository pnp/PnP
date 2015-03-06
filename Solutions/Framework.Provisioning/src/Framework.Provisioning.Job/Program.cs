using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Job
{
    /// <summary>
    /// Main Program for the Remote Timer Job Pattern to Create Site Collections
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main Entry Point of the program to create site collections.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var _spProvisioningJob = new SiteProvisioningJob();
            _spProvisioningJob.ProcessRequestQueue();
        }
    }
}
