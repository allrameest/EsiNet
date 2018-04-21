using System;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [Route("[controller]")]
    public class DateController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = 10)]
        public IActionResult Index()
        {
            return Content(DateTime.Now.ToString());
        }
    }
}