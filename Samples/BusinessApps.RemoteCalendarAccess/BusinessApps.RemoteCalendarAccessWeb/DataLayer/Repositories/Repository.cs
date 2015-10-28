using BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApps.RemoteCalendarAccessWeb.DataLayer.Repositories
{
    public class Repository
    {
        protected DataModel _db;

        protected Repository()
        {
            _db = new DataModel();
        }
    }
}
