using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.RemediationUx.Model
{
    public class SiteInformation : OperationResult
    {
        public string AudienceScope
        {
            get;
            set;
        }
        
        public DateTime ExpireDate
        {
            get;
            set;
        }
                
        public bool CanDecommission
        {
            get;
            set;
        }

        public bool NeedExtend
        {
            get;
            set;
        }

        public bool IsExtend
        {
            get;
            set;
        }

        public DateTime ExtendDate
        {
            get;
            set;
        }
    }
}
