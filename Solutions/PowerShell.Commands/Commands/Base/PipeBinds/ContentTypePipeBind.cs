using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class ContentTypePipeBind
    {
        private string _id;
        private string _name;
        private ContentType _contentType;

        public ContentTypePipeBind()
        {
            _id = string.Empty; ;
            _name = string.Empty;
            _contentType = null;
        }

        public ContentTypePipeBind(string id)
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

        public ContentTypePipeBind(ContentType contentType)
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
