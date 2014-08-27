using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    [Obsolete("Use OfficeDev/PnP.Core")]
    public static class SPOContentType
    {

        public static ContentType CreateContentType(string contentTypeId, string name, string description, string group, ContentType contentType, Web web, ClientContext clientContext)
        {
            ContentTypeCreationInformation creationInformation = new ContentTypeCreationInformation();
            if (!string.IsNullOrEmpty(contentTypeId))
            {
                creationInformation.Id = contentTypeId;
            }
            creationInformation.Description = description;
            if (!string.IsNullOrEmpty(group))
            {
                creationInformation.Group = group;
            }
            creationInformation.Name = name;
            if (contentType != null)
            {
                creationInformation.ParentContentType = contentType;
            }

            ContentType cType = web.ContentTypes.Add(creationInformation);
            clientContext.Load(cType);
            clientContext.ExecuteQuery();
            return cType;
        }

        [Obsolete("Use CSOM")]
        public static List<ContentType> GetContentTypes(Web web, ClientContext clientContext)
        {
            List<ContentType> cts = new List<ContentType>();
            clientContext.Load(web.ContentTypes);
            clientContext.ExecuteQuery();
            foreach (var ct in web.ContentTypes)
            {
                cts.Add(ct);
            }

            return cts;
        }

        public static void RemoveContentTypeById(string contentTypeId, Web web, ClientContext clientContext)
        {
            var cts = from ct in GetContentTypes(web, clientContext)
                      where ct.StringId == contentTypeId
                      select ct;

            if (cts.FirstOrDefault() != null)
            {
                var ct = cts.FirstOrDefault();
                ct.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                throw new Exception("Content Type not found");
            }
        }

        public static void RemoveContentTypeByName(string name, Web web, ClientContext clientContext)
        {
            var cts = from ct in GetContentTypes(web, clientContext)
                      where ct.Name.ToLower() == name.ToLower()
                      select ct;

            if (cts.FirstOrDefault() != null)
            {
                var ct = cts.FirstOrDefault();
                ct.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                throw new Exception("Content Type not found");
            }
        }
    }
}
