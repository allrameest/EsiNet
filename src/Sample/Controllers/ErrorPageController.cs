using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    public class ErrorPageController : Controller
    {
        [HttpGet("errorpage")]
        public IActionResult Index(int? statusCode = null)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
    }
}