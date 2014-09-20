using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security.Server;
using System.Linq.Expressions;

namespace LightSwitchApplication
{
    public partial class ApplicationDataService
    {
        partial void Tasks_Inserting(Task entity)
        {
            // Set the HostURL
            entity.HostURL = this.Application.SharePoint.HostUrl.Host;
        }

        partial void Tasks_Updating(Task entity)
        {
            // Set the HostURL
            entity.HostURL = this.Application.SharePoint.HostUrl.Host;
        }

        partial void Tasks_Filter(ref Expression<Func<Task, bool>> filter)
        {
            // Only allow users to see records from their own site
            filter = e => e.HostURL == this.Application.SharePoint.HostUrl.Host;
        }
    }
}
