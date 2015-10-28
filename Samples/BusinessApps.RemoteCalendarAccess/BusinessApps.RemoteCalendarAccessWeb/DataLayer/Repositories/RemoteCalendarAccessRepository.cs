using DLM = BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model;
using BusinessApps.RemoteCalendarAccessWeb.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApps.RemoteCalendarAccessWeb.DataLayer.Repositories
{
    public class RemoteCalendarAccessRepository : Repository
    {
        public Model.RemoteCalendarAccess GetRemoteCalendarAccessById(Guid Id)
        {
            return _db.RemoteCalendarAccess.Find(Id);
        }


        public void CreateRemoteCalendarAccess(DLM.RemoteCalendarAccess remoteCalendarAccess)
        {
            remoteCalendarAccess = _db.RemoteCalendarAccess.Add(remoteCalendarAccess);
            _db.SaveChanges();
        }

        public DLM.RemoteCalendarAccess UpdateRemoteCalendarAccess(DLM.RemoteCalendarAccess remoteCalendarAccess)
        {
            DLM.RemoteCalendarAccess originalRCA = _db.RemoteCalendarAccess.Find(remoteCalendarAccess.ID);
            _db.Entry<DLM.RemoteCalendarAccess>(originalRCA).CurrentValues.SetValues(remoteCalendarAccess);
            _db.SaveChanges();
            return originalRCA;
        }
    }
}
