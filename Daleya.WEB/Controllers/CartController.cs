using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Cart;
using Daleya.WEB.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Daleya.WEB.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            
            if (cart == null)
            {
                return RedirectToAction("Index", "Home", TempData["error"] = "Cart is empty! please add items");
            }

            return View(cart);
        }
        public async Task<IActionResult> IncreaseItem(int cartDetailsId, int productId)
        {
            ResponseDto? response = await _cartService.IncreaseItem(cartDetailsId, productId);

            if (response != null && response.IsSuccess)
            {
                var cart = await LoadCartDtoBasedOnLoggedInUser();
                if (cart == null)
                {
                    return RedirectToAction("Index", "Home", TempData["error"] = "Cart is empty! please add items");
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecreaseItem(int cartDetailsId, int productId)
        {
            ResponseDto? response = await _cartService.DecreaseItem(cartDetailsId, productId);

            if (response != null && response.IsSuccess)
            {
                var cart = await LoadCartDtoBasedOnLoggedInUser();
                if (cart == null)
                {
                    return RedirectToAction("Index", "Home", TempData["error"] = "Cart is empty! please add items");
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartAsync(userId);

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            TempData["error"] = response?.ErrorMessage;
            return null;
        }
    }
}
