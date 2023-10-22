using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Order;
using Daleya.WEB.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace Daleya.WEB.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async IJEnumerable<IActionResult> OrderDetails()
        //{
        //    var ordersList = await LoadOrderDtoBasedOnLoggedInUser();

        //    if (ordersList == null)
        //    {
        //        return RedirectToAction("Index", "Home", TempData["error"] = "Cart is empty! please add items");
        //    }

        //    return View(ordersList);
        //}

        private async Task<IEnumerable<OrderDto>> LoadOrderDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _orderService.GetAllAsync(userId);

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(Convert.ToString(response.Result));
            }

            TempData["error"] = response?.ErrorMessage;
            return null;
        }
    }
}
