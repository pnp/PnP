using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Core.Utils;

using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOnlineView : SPOContextObject<View>
    {
        private string _title;
        private Guid _id;
        private bool _isdefault;
        private bool _ispersonal;
        private string _viewType;
        private uint _rowLimit;
        private string _query;
        private string[] _fields;

        public string Title { get { return _title; } }
        public Guid Id { get { return _id; } }
        public bool IsDefault { get { return _isdefault; } }
        public bool IsPersonal { get { return _ispersonal; } }
        public string ViewType { get { return _viewType; } }
        public uint RowLimit { get { return _rowLimit; } }
        public string Query { get { return _query; } }
        public string[] Fields { get { return _fields; } }
        public SPOnlineView(View view)
        {
            _contextObject = view;
            _title = view.Title;
            _id = view.Id;
            _viewType = view.ViewType;
            _rowLimit = view.RowLimit;
            _isdefault = view.DefaultView;
            _ispersonal = view.PersonalView;
            _query = view.ViewQuery;
            _fields = view.ViewFields.ToArray();

        }

    }
}
