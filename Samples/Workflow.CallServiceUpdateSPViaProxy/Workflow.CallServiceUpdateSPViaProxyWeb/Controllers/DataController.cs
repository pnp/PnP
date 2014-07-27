using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using Workflow.CallServiceUpdateSPViaProxyWeb.Models;
using Workflow.CallServiceUpdateSPViaProxyWeb.Services;

namespace Workflow.CallServiceUpdateSPViaProxyWeb.Controllers
{
    public class DataController : ApiController
    {
        public void Post(UpdatePartSupplierModel model)
        {
            var request = HttpContext.Current.Request;
            var authority = request.Url.Authority;
            var spAppWebUrl = request.Headers["SPAppWebUrl"];
            var accessToken = request.Headers["X-SP-AccessToken"];

            using (var clientContext = TokenHelper.GetClientContextWithContextToken(spAppWebUrl, accessToken, authority))
            {
                var service = new PartSuppliersService(clientContext);
                service.UpdateSuppliers(model.Id, model.Suppliers.Select(s => s.CompanyName));
            }
        }
    }
}