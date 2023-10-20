using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Models.Dto.Cart;
using Daleya.WEB.Service.IService;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Daleya.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,
            ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();
            ResponseDto? response = await _productService.GetAllAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }

            return View(list);
        }
        [Authorize]
        public async Task<IActionResult> UpsertCart(int productId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value;

            var response = await _cartService.GetCartAsync(userId);
            CreateCartDetailsDto cartDetailsDto = new();

            CreateCartDto cart = JsonConvert.DeserializeObject<CreateCartDto>(Convert.ToString(response.Result));
            CartDto cartFromDb = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            CartDetailsDto cartDetailFromDb = new();
            if (cartFromDb != null)
            {
                cartDetailFromDb = cartFromDb.CartDetails.FirstOrDefault(c => c.ProductId == productId);
            }

            if (cart != null && cartDetailFromDb != null)
            {
                cartDetailsDto = cart.CartDetails.FirstOrDefault(c => c.ProductId == productId);

                int cartDetialId = cartDetailFromDb.CartDetailsId;

                ResponseDto? result = await _cartService.IncreaseItem(cartDetialId, productId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Added to Cart!";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                cartDetailsDto = new() { Count = 1, ProductId = productId };

                cart = new()
                {
                    CartHeader = new() { UserId = userId },
                    CartDetails = new List<CreateCartDetailsDto>() { cartDetailsDto }
                };

                var resultFromDb = await _cartService.CartUpsert(cart);
                if (resultFromDb == null || !resultFromDb.IsSuccess)
                {
                    TempData["error"] = resultFromDb?.ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["success"] = "Added to Cart!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}