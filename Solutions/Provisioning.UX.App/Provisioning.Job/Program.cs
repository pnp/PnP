using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            var _spj = new SiteProvisioningJob();
            _spj.ProcessSiteRequests();
          
        }
    }
}
