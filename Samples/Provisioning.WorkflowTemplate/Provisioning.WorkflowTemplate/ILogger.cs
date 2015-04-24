using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.WorkflowTemplate
{
    interface ILogger
    {
        void WriteMessage(string message);
    }
}
