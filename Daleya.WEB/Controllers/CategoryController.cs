using Microsoft.AspNetCore.Mvc;

namespace Daleya.WEB.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
