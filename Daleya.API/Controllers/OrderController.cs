using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Daleya.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICouponRepository _dbCoupon;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public OrderController(ICouponRepository db, IMapper mapper, IOrderHeaderRepository orderHeaderRepository)
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
                IEnumerable<OrderHeader> objList;
                
                objList = await _orderHeaderRepository.GetAllAsync(includeProperties:"OrderDetails",
                                                                   orderByDescending:e => e.OrderId);
                
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);

                if (list is null || list.Count() == 0)
                {
                    return _response.NotFound("Order table is empty!");
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

                var obj = await _orderHeaderRepository.GetAsync(v => v.CouponId == id, includeProperties:"OrderDetails");

                if (obj == null)
                {
                    return _response.NotFound("Order with id: " + id + " not found");
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

        [HttpPost("{CouponCode}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeaderDto);
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetailsDto);

                OrderHeader orderCreated = _mapper.Map<OrderHeader>(orderHeaderDto);

                await _orderHeaderRepository.CreateAsync(orderCreated);

                _response.Result = _mapper.Map<OrderHeaderDto>(orderCreated);
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
