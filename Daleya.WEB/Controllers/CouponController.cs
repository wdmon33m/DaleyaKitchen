using Microsoft.AspNetCore.Mvc;

namespace Daleya.WEB.Controllers
{
    public class CouponController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
