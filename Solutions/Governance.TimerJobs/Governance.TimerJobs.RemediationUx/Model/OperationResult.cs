using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.RemediationUx.Model
{
    public class OperationResult
    {
        public bool IsSuccess
        {
            get;
            set;
        }
        
        public string Message
        {
            get;
            set;
        }
    }
}
