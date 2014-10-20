using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Services.Authenticate.SharePointWeb.Models;
using OfficeDevPnP.Core.WebAPI;

namespace Core.Services.Authenticate.SharePointWeb.Controller
{
    public class DemoController : ApiController
    {

        Product[] products = new Product[] 
        { 
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 }, 
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M }, 
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M } 
        };

        [HttpPut]
        //[Route("api/demo/register")]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }

        [WebAPIContextFilter]
        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            Microsoft.SharePoint.Client.User spUser = null;
            using (var clientContext = WebAPIHelper.GetClientContext(ControllerContext))
            { 
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title);
                    clientContext.ExecuteQuery();                    
                }
            }

            return products;
        }

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }


    }
}
