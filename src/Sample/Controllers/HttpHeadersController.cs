using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    public class HttpHeadersController : Controller
    {
        [HttpGet("httpheaders")]
        public IActionResult Index()
        {
            return View();
        }
    }
}