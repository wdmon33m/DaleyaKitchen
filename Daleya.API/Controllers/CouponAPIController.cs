using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Daleya.API.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponRepository _dbCoupon;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public CouponAPIController(ICouponRepository db, IMapper mapper, IOrderHeaderRepository orderHeaderRepository)
        {
            _dbCoupon = db;
            _mapper = mapper;
            _response = new();
            _orderHeaderRepository = orderHeaderRepository;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Coupon> list = await _dbCoupon.GetAllAsync();

                if (list is null || list.Count() == 0)
                {
                    return _response.NotFound("Coupon table is empty!");
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(list);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return _response.BadRequest("Id 0 is not correct!");
                }

                var obj = await _dbCoupon.GetAsync(v => v.CouponId == id);

                if (obj == null)
                {
                    return _response.NotFound("Coupon with id: " + id + " not found");
                }

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }

        [HttpGet("GetByCouponCode/{couponCode}")]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(string couponCode)
        {
            try
            {
                var obj = await _dbCoupon.GetAsync(v => v.CouponCode == couponCode);

                if (obj == null)
                {
                    return _response.NotFound("Coupon : " + couponCode + " not found");
                }

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post([FromBody] CouponDto obj)
        {
            try
            {
                if (await _dbCoupon.GetAsync(u => u.CouponCode.ToLower() ==  obj.CouponCode.ToLower()) != null)
                {
                    return _response.BadRequest("Coupon is Already Exists!");
                }
                if (obj.CouponCode.IsNullOrEmpty())
                {
                    return _response.NotFound("You must enter Coupon name");
                }

                Coupon coupon = _mapper.Map<Coupon>(obj);

                await _dbCoupon.CreateAsync(coupon);

                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var obj = await _dbCoupon.GetAsync(u => u.CouponId == id);
                if (obj == null)
                {
                    return _response.NotFound("Coupon is not Exists!");
                }

                var product = await _orderHeaderRepository.GetAsync(p => p.CouponId == id);
                if (product != null)
                {
                    return _response.BadRequest("Cannot delete the Coupon because it has associated products.");
                }

                await _dbCoupon.RemoveAsync(obj);
                _response.Result = "Coupon has been deleted successfully.";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }
    }
}
