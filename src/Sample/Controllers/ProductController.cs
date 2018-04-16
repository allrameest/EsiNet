using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60*30)]
        public async Task<IActionResult> Index(int id)
        {
            await Task.Delay(100);

            var product = new Product
            {
                Id = id,
                Title = $"Title {id}",
                Description = "Lorem ipsum"
            };
            return PartialView(product);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}