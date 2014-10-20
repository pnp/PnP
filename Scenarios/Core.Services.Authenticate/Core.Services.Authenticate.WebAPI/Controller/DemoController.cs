using OfficeDevPnP.Core.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Core.Services.Authenticate.WebAPI.Controller
{
    public class DemoController : ApiController
    {
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }

        [HttpGet]
        public IEnumerable<string> GetAllProducts()
        {
            String[] products = new string[] {"a", "b"};

            return products;
        }


    }
}
