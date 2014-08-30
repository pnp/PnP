using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Entities;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class SPOContentTypePipeBind
    {
        private string _id;
        private string _name;
        private ContentType _contentType;

        public SPOContentTypePipeBind()
        {
            _id = string.Empty; ;
            _name = string.Empty;
            _contentType = null;
        }

        public SPOContentTypePipeBind(string id)
        {
            if (id.ToLower().StartsWith("0x0"))
            {
                this._id = id;
            }
            else
            {
                this._name = id;
            }

        }

        public SPOContentTypePipeBind(ContentTypeEntity contentType)
        {
            this._contentType = contentType.GetContextObject();
        }


        public SPOContentTypePipeBind(ContentType contentType)
        {
            this._contentType = contentType;
        }

        public string Id
        {
            get
            {
                if (_contentType != null)
                {
                    return _contentType.StringId;
                }
                else
                {
                    return _id;
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }
    }
}
