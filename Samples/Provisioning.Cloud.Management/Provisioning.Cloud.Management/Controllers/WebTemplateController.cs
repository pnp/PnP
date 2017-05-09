using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Provisioning.Cloud.Management.Repositories;
using model = Provisioning.Cloud.Management.Models;

namespace Provisioning.Cloud.Management.Controllers
{
    [Authorize]
    public class WebTemplateController : ApiController
    {
        private SharePointRepository _sharePointRepository;

        public WebTemplateController()
        {
            _sharePointRepository = new SharePointRepository();
        }

        public async Task<IEnumerable<model.WebTemplateVM>> Get(int lcid)
        {
            // Get the webtemplates
            var webTemplates = await _sharePointRepository.GetWebTemplatesAsync((uint)lcid);

            // Return the available languages
            return webTemplates;
        }
    }
}
