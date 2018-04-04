using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public async Task<IActionResult> Get()
        {
            return View();
        }
    }
}