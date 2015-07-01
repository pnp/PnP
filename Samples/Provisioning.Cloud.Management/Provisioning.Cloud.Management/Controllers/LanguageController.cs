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
    public class LanguageController : ApiController
    {
        private SharePointRepository _sharePointRepository; 

        public LanguageController()
        {
            _sharePointRepository = new SharePointRepository();
        }

        public async Task<IEnumerable<model.LanguageVM>> Get()
        {
            // Get the languages
            var availableLanguages = await _sharePointRepository.GetAvailableLanguagesAsync();

            // Return the available languages
            return availableLanguages;
        }
    }
}
