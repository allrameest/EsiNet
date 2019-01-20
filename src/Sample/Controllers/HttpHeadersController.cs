using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    public class HttpHeadersController : Controller
    {
        [HttpGet("httpheaders")]
        [ResponseCache(Duration = 60, VaryByHeader = "User-Agent")]
        public IActionResult Index()
        {
            return View();
        }
    }
}