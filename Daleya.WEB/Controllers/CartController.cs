using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Cart;
using Daleya.WEB.Models.Dto.Order;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Daleya.WEB.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
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

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartHeaderDto cartHeaderDto)
        {
            ResponseDto? response = await _cartService.ApplyCoupon(cartHeaderDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartHeaderDto cartHeaderDto)
        {
            cartHeaderDto.CouponCode = "";
            ResponseDto? response = await _cartService.RemoveCoupon(cartHeaderDto.UserId, cartHeaderDto.CartHeaderId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon removed successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }

            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("CheckOut")]
        public async Task<IActionResult> CheckOut(CartDto cartDto)
        {
            CartDto updatedCart = new();
            updatedCart = await LoadCartDtoBasedOnLoggedInUser();
            updatedCart.CartHeader.Phone = cartDto.CartHeader?.Phone;
            updatedCart.CartHeader.Email = cartDto.CartHeader?.Email;
            updatedCart.CartHeader.FirstName = cartDto.CartHeader?.FirstName;
            updatedCart.CartHeader.LastName = cartDto.CartHeader?.LastName;

            var response = await _orderService.CreateOrder(updatedCart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                // get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                var stripeResponse = await _orderService.CreateStripeSessionAsync(stripeRequestDto);
                StripeRequestDto stripeRequestDtoResult = 
                    JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));

                Response.Headers.Add("Location", stripeRequestDtoResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }
            return View(updatedCart);
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if (orderHeaderDto.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to error page based on status
            return View(orderId);
        }
    }
}
