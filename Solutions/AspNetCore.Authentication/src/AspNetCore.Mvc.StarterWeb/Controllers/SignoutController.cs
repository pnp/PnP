namespace AspNetCore.Mvc.StarterWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    [Authorize]
    public class SignoutController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            
            await HttpContext.SignOutAsync();

            // Should load session asynchronously with a wrapper class that ensures await HttpContext.Session.LoadAsync(); is called.
            // If this is not called first the session will load synchronously see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state Loading Session Asynchronously section             

            // Clear session
            HttpContext.Session.Clear();

            return View();
        }
    }
}