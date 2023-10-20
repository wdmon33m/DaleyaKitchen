using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;
using Daleya.API.Repository.IRepository;
using Daleya.API.Service.IService;
using System.Net;

namespace Daleya.API.Service
{
    public class CartService : ICartService
    {
        private readonly ICartHeaderRepository _cartHeaderRepository;
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ResponseDto _response;
        public CartService(IProductRepository productRepository,
            ICouponRepository couponRepository,
            ICartHeaderRepository cartHeaderRepository,
            ICartDetailRepository cartDetailRepository)
        {

            _productRepository = productRepository;
            _couponRepository = couponRepository;
            _response = new();
            _cartHeaderRepository = cartHeaderRepository;
            _cartDetailRepository = cartDetailRepository;
        }

        public async Task<ResponseDto> ApplyCoupon(CartHeaderDto cartHeaderDto)
        {
            try
            {
                var cartHeaderFromDb = await _cartHeaderRepository.GetAsync(u => u.UserId == cartHeaderDto.UserId
                                            && u.CartHeaderId == cartHeaderDto.CartHeaderId);

                cartHeaderFromDb.CouponCode = cartHeaderDto.CouponCode.ToUpper();

                var coupon = await _couponRepository.GetAsync(c => c.CouponCode == cartHeaderFromDb.CouponCode);
                if (coupon.CouponCode == null)
                {
                    return _response.NotFound("Coupon is not exist!");
                }

                await _cartHeaderRepository.UpdateAsync(cartHeaderFromDb);
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = "Coupon has been applied successfully";
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> CartUpsert(Cart cart)
        {
            try
            {
                int productId = cart.CartDetails.FirstOrDefault().ProductId;
                var product = await _productRepository.GetAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return _response.NotFound("The Product is not exist!");
                }

                var couponCode = cart.CartHeader.CouponCode;
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = await _couponRepository.GetAsync(c => c.CouponCode == couponCode);
                    if (coupon == null)
                    {
                        return _response.NotFound("The coupon is not exist!");
                    }
                }

                var cartHeaderFromDb = await _cartHeaderRepository.GetAsync(u => u.UserId == cart.CartHeader.UserId, tracked: false);

                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    cart.CartHeader.CartHeaderId = 0;
                    await _cartHeaderRepository.CreateAsync(cart.CartHeader);

                    CartDetails cartDetails = cart.CartDetails.First();
                    cartDetails.CartHeaderId = cart.CartHeader.CartHeaderId;
                    cartDetails.CartDetailsId = 0;

                    await _cartDetailRepository.CreateAsync(cartDetails);
                }
                else
                {
                    //if header is not null
                    //check if detail has same product
                    var cartDetailsFromDb = await _cartDetailRepository.GetAsync(
                        u => u.ProductId == cart.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId, tracked: false);

                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails

                        CartDetails cartDetails = cart.CartDetails.First();
                        cartDetails.CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        cartDetails.CartDetailsId = 0;

                        await _cartDetailRepository.CreateAsync(cartDetails);
                    }
                    else
                    {
                        //update count in cartDetails
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;

                        CartDetails cartDetails = cart.CartDetails.First();
                        cartDetails.CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDetails.CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        await _cartDetailRepository.UpdateAsync(cartDetails);
                    }
                }

                _response.Result = cart;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> GetCartAsync(string userId)
        {
            try
            {
                Cart cart = new()
                {
                    CartHeader = await _cartHeaderRepository.GetAsync(e => e.UserId == userId)
                };

                if (cart.CartHeader == null)
                {
                    return _response.BadRequest("Cart is empty");
                }

                cart.CartDetails = await _cartDetailRepository.GetAllAsync(u => u.CartHeaderId == cart.CartHeader.CartHeaderId);

                var productDtos = await _productRepository.GetAllAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    Coupon coupon = await _couponRepository.GetAsync(c => c.CouponCode == cart.CartHeader.CouponCode);

                    if (coupon != null && cart.CartHeader.CartTotal >= coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discound = coupon.DiscountAmount;
                    }
                }
                _response.Result = cart;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> RemoveCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _cartDetailRepository.GetAsync(u => u.CartDetailsId == cartDetailsId);

                if (cartDetails == null)
                {
                    return _response.BadRequest("Cart is empty!");
                }

                int totalCountOfCartItems =  _cartDetailRepository.GetAllAsync(u =>
                u.CartHeaderId == cartDetails.CartHeaderId).Result.Count();

                await _cartDetailRepository.RemoveAsync(cartDetails);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _cartHeaderRepository.GetAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    await _cartHeaderRepository.RemoveAsync(cartHeaderToRemove);
                }

                _response.Result = "items has been removed successuflly";
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> RemoveCoupon(string userId, int cartHeaderId)
        {
            try
            {
                var cartHeaderFromDb = await _cartHeaderRepository.GetAsync(u => u.UserId == userId
                                             && u.CartHeaderId == cartHeaderId);
                cartHeaderFromDb.CouponCode = "";

                if (cartHeaderFromDb == null)
                {
                    return _response.BadRequest("Cart is not exist!");
                }

                await _cartHeaderRepository.UpdateAsync(cartHeaderFromDb);

                _response.Result = "Coupon has been removed successfully";
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> IncreaseItem(int cartDetailsId, int productId)
        {
            try
            {
                CartDetails cartDetails = await _cartDetailRepository.GetAsync(u =>
                u.CartDetailsId == cartDetailsId && u.ProductId == productId);

                if (cartDetails == null)
                {
                    return _response.BadRequest("Cart is empty!");
                }

                cartDetails.Count += 1;
                await _cartDetailRepository.UpdateAsync(cartDetails);

                _response.Result = "item has been increased successuflly";
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> DecreaseItem(int cartDetailsId, int productId)
        {
            try
            {
                CartDetails cartDetails = await _cartDetailRepository.GetAsync(u =>
                u.CartDetailsId == cartDetailsId && u.ProductId == productId);

                int totalCountOfCartItems = _cartDetailRepository.GetAllAsync(u =>
                u.CartHeaderId == cartDetails.CartHeaderId).Result.Count();

                if (cartDetails == null)
                {
                    return _response.BadRequest("Cart is empty!");
                }

                if (cartDetails.Count == 1)
                {
                    await _cartDetailRepository.RemoveAsync(cartDetails);
                    if (totalCountOfCartItems == 1)
                    {
                        var cartHeaderToRemove = await _cartHeaderRepository.GetAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                        await _cartHeaderRepository.RemoveAsync(cartHeaderToRemove);
                    }
                }
                else
                {
                    cartDetails.Count -= 1;
                    await _cartDetailRepository.UpdateAsync(cartDetails);
                }

                _response.Result = "item has been decreased successuflly";
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }
    }
}
