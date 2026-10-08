using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Stock_Exchange.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TrackerController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public TrackerController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // First try rendering MVC Razor View if available
            try
            {
                return View();
            }
            catch
            {
                // Fallback to static index.html if Razor view is not available
                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var staticFile = Path.Combine(webRoot, "net-tracker", "index.html");
                if (System.IO.File.Exists(staticFile))
                {
                    return PhysicalFile(staticFile, "text/html");
                }

                throw;
            }
        }
    }
}
