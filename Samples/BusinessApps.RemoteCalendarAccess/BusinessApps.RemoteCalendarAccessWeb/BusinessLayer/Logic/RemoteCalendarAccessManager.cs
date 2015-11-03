using BLM = BusinessApps.RemoteCalendarAccessWeb.BusinessLayer.Model;
using BusinessApps.RemoteCalendarAccessWeb.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLM = BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model;

namespace BusinessApps.RemoteCalendarAccessWeb.BusinessLayer.Logic
{
    public class RemoteCalendarAccessManager
    {
        public BLM.RemoteCalendarAccess GetRemoteCalendarAccess(Guid Id)
        {
            RemoteCalendarAccessRepository repository = new RemoteCalendarAccessRepository();
            DLM.RemoteCalendarAccess remoteCalendarAccess = repository.GetRemoteCalendarAccessById(Id);
            if (remoteCalendarAccess == null)
                return null;
            else
                return new BLM.RemoteCalendarAccess(remoteCalendarAccess);
        }

        public BLM.RemoteCalendarAccess AddRemoteCalendarAccess(Guid calendarId, string siteAddress, string userId)
        {
            BLM.RemoteCalendarAccess remoteCalendarAccess = new BLM.RemoteCalendarAccess(calendarId, siteAddress, userId);
            
            RemoteCalendarAccessRepository repository = new RemoteCalendarAccessRepository();
            repository.CreateRemoteCalendarAccess(remoteCalendarAccess.ToDataModel<DLM.RemoteCalendarAccess>());

            return remoteCalendarAccess;
        }

        public BLM.RemoteCalendarAccess UpdateLastAccessTime(BLM.RemoteCalendarAccess remoteCalendarAccess)
        {
            remoteCalendarAccess.LastAccess = DateTime.UtcNow;

            RemoteCalendarAccessRepository repository = new RemoteCalendarAccessRepository();
            remoteCalendarAccess = new BLM.RemoteCalendarAccess(repository.UpdateRemoteCalendarAccess(remoteCalendarAccess.ToDataModel<DLM.RemoteCalendarAccess>()));

            return remoteCalendarAccess;
        }
    }
}
