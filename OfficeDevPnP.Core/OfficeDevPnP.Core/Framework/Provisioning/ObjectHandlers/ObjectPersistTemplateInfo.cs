using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectPersistTemplateInfo : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Persist Template Info"; }
        }

        public ObjectPersistTemplateInfo()
        {
            this.ReportProgress = false;
        }

        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            web.SetPropertyBagValue("_PnP_ProvisioningTemplateId", template.Id != null ? template.Id : "");
            web.AddIndexedPropertyBagKey("_PnP_ProvisioningTemplateId");

            ProvisioningTemplateInfo info = new ProvisioningTemplateInfo();
            info.TemplateId = template.Id != null ? template.Id : "";
            info.TemplateVersion = template.Version;
            info.TemplateSitePolicy = template.SitePolicy;
            info.Result = true;
            info.ProvisioningTime = DateTime.Now;

            var s = new  JavaScriptSerializer();
            string jsonInfo = s.Serialize(info);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo", jsonInfo);
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            return template;
        }
    }
}
