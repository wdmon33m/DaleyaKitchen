using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Daleya.API.Repository
{
    public class CartRepository : Repository<CartHeader>, ICartRepository
    {
        private readonly AppDbContext _db;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ResponseDto _response;
        public CartRepository(AppDbContext db, IProductRepository productRepository, ICouponRepository couponRepository) : base(db)
        {
            _db = db;
            _productRepository = productRepository;
            _couponRepository = couponRepository;
            _response = new();
        }

        public async Task<ResponseDto> CartUpsert(Cart cart)
        {
            try
            {
                int productId = cart.CartDetails.FirstOrDefault().ProductId;
                var product = await _productRepository.GetAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage = "The Product is not exist!";
                    return _response;
                }

                var couponCode = cart.CartHeader.CouponCode;
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = await _couponRepository.GetAsync(c => c.CouponCode == couponCode);
                    if (coupon == null)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessage = "The coupon is not exist!";
                        return _response;
                    }
                }
                

                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                                            .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    cart.CartHeader.CartHeaderId = 0;
                    _db.CartHeaders.Add(cart.CartHeader);
                    await _db.SaveChangesAsync();

                    CartDetails cartDetails = cart.CartDetails.First();
                    cartDetails.CartHeaderId = cart.CartHeader.CartHeaderId;
                    cartDetails.CartDetailsId = 0;

                    await _db.CartDetails.AddAsync(cartDetails);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if detail has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cart.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails

                        CartDetails cartDetails = cart.CartDetails.First();
                        cartDetails.CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        await _db.CartDetails.AddAsync(cartDetails);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cartDetails
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;

                        CartDetails cartDetails = cart.CartDetails.First();
                        cartDetails.CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDetails.CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _db.CartDetails.Update(cartDetails);
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }


        public async Task<ResponseDto> GetCartAsync(string userId)
        {
            try
            {
                Cart cart = new()
                {
                    CartHeader = _db.CartHeaders.FirstOrDefault(e => e.UserId == userId)
                };

                if (cart.CartHeader == null)
                {
                    _response.ErrorMessage = "Cart is empty";
                    _response.IsSuccess = false;
                    return _response;
                }

                cart.CartDetails = _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId);

                var productDtos = await _productRepository.GetAllAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    Coupon coupon = await _couponRepository.GetAsync(c => c.CouponCode == cart.CartHeader.CouponCode);

                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discound = coupon.DiscountAmount;
                    }
                }
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }
    }
}
