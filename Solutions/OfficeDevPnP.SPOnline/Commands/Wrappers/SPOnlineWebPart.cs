using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands
{
    public class SPOnlineWebPart
    {
        internal string _title;
        internal bool _hidden;
        internal Guid _id;
        internal string _subtitle;
        internal string _titleurl;
        internal int _zoneindex;
        internal PropertyValues _properties;

        public Guid Id { get { return _id; } }
        public bool Hidden { get { return _hidden; } }
        public string Subtitle { get { return _subtitle; } }
        public string Title { get { return _title; } }
        public string TitleUrl { get { return _titleurl; } }
        public int ZoneIndex { get { return _zoneindex; } }
        public PropertyValues Properties { get { return _properties; } }

        public SPOnlineWebPart(WebPartDefinition definition)
        {
            _id = definition.Id;
            _hidden = definition.WebPart.Hidden;
            _subtitle = definition.WebPart.Subtitle;
            _title = definition.WebPart.Title;
            _titleurl = definition.WebPart.TitleUrl;
            _zoneindex = definition.WebPart.ZoneIndex;
            _properties = definition.WebPart.Properties;
        }

    }
}
