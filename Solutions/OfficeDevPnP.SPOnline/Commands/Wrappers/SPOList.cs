using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands
{
    public class SPOList : SPOContextObject<List>
    {
        private string _title;
        private Guid _id;
        private bool _onquicklaunch;
        private string _defaultviewurl;
        private int _basetemplate;
        private bool _hidden;

        public string Title { get { return _title; } }
        public Guid Id { get { return _id; } }
        public bool OnQuickLaunch { get { return _onquicklaunch; } }
        public string DefaultViewUrl { get { return _defaultviewurl; } }
        public int BaseTemplate { get { return _basetemplate; } }
        public bool Hidden { get { return _hidden; } }

        public SPOList(List list)
        {
            _contextObject = list;
            _title = list.Title;
            _id = list.Id;
            _onquicklaunch = list.OnQuickLaunch;
            _defaultviewurl = list.DefaultViewUrl;
            _basetemplate = list.BaseTemplate;
            _hidden = list.Hidden;
        }
    }
}
