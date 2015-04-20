using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration.Template
{
    public class SiteTemplateManager
    {

        public SiteTemplate GetSiteTemplate(string siteTemplateName)
        {
            if(string.IsNullOrEmpty(siteTemplateName))
            {
                throw new ArgumentException("Paramater is not valid or an empty string", "siteTemplateName");
            }

            var _path = PathHelper.GetAssemblyDirectory();
            var _fileName = siteTemplateName;
            string _filePath = string.Format("{0}{1}{2}", _path, @"\", _fileName);

            return null;
        }
    }
}
