using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> Get()
        {
            return View();
        }
    }
}