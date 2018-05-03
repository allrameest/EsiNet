using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Get()
        {
            return View();
        }
    }
}