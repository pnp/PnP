using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class DocumentSetPipeBind
    {
        private readonly string _id;
        private readonly string _name;
        private readonly ContentType _contentType;
        private readonly DocumentSetTemplate _documentSetTemplate;

        public DocumentSetPipeBind()
        {
            _id = string.Empty;
            _name = string.Empty;
            _contentType = null;
            _documentSetTemplate = null;
        }

        public DocumentSetPipeBind(string id)
        {
            if (id.ToLower().StartsWith("0x0"))
            {
                _id = id;
            }
            else
            {
                _name = id;
            }

        }

        public DocumentSetPipeBind(ContentType contentType)
        {
            _contentType = contentType;
        }

        public DocumentSetPipeBind(DocumentSetTemplate documentSetTemplate)
        {
            _documentSetTemplate = documentSetTemplate;
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

        public DocumentSetTemplate GetDocumentSetTemplate(Web web)
        {
            if (_contentType != null)
            {
                var docSet = DocumentSetTemplate.GetDocumentSetTemplate(web.Context, _contentType);
                return docSet;
            }
            else if (_documentSetTemplate != null)
            {
                return _documentSetTemplate;
            }
            else
            {
                ContentType ct;
                if (!string.IsNullOrEmpty(Id))
                {
                    ct = web.GetContentTypeById(Id, true);

                }
                else
                {
                    ct = web.GetContentTypeByName(Name, true);
                }
                var docSet = DocumentSetTemplate.GetDocumentSetTemplate(web.Context, ct);
                return docSet;
            }
        }
    }
}
