using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    public class SPOCmdlet : PSCmdlet
    {
        public ClientContext ClientContext
        {
            get { return SPOnlineConnection.CurrentConnection.Context; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (SPOnlineConnection.CurrentConnection == null)
            {
                throw new InvalidOperationException(Properties.Resources.NoConnection);
            }
            if (ClientContext == null)
            {
                throw new InvalidOperationException(Properties.Resources.NoConnection);
            }
        }

        protected virtual void ExecuteCmdlet()
        { }

        protected override void ProcessRecord()
        {
            if (SPOnlineConnection.CurrentConnection.MinimalHealthScore != -1)
            {
                int healthScore = Utility.GetHealthScore(SPOnlineConnection.CurrentConnection.Url);
                if(healthScore <= SPOnlineConnection.CurrentConnection.MinimalHealthScore)
                {
                    ExecuteCmdlet();
                }
                else
                {
                    if (SPOnlineConnection.CurrentConnection.RetryCount != -1)
                    {
                        int retry = 1;
                        while (retry <= SPOnlineConnection.CurrentConnection.RetryCount)
                        {
                            WriteWarning(string.Format(Properties.Resources.Retry0ServerNotHealthyWaiting1seconds, retry, SPOnlineConnection.CurrentConnection.RetryWait, healthScore));
                            Thread.Sleep(SPOnlineConnection.CurrentConnection.RetryWait * 1000);
                            healthScore = Utility.GetHealthScore(SPOnlineConnection.CurrentConnection.Url);
                            if (healthScore <= SPOnlineConnection.CurrentConnection.MinimalHealthScore)
                            {
                                ExecuteCmdlet();
                                break;
                            }
                            retry++;
                        }
                        if(retry > SPOnlineConnection.CurrentConnection.RetryCount)
                        {
                            WriteError(new ErrorRecord(new Exception(Properties.Resources.HealthScoreNotSufficient),"HALT",ErrorCategory.LimitsExceeded,null));
                        }
                    }
                    else
                    {
                        WriteError(new ErrorRecord(new Exception(Properties.Resources.HealthScoreNotSufficient), "HALT", ErrorCategory.LimitsExceeded, null));
                    }
                }
            }
            else
            {
                ExecuteCmdlet();
            }
        }


    }
}
