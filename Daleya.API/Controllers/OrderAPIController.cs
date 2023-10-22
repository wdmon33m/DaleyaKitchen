using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;
using Daleya.API.Models.Dto.Order;
using Daleya.API.Service.IService;
using Daleya.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daleya.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderAPIController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{userId}")]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(string userId)
        {
            if(User.IsInRole(SD.RoleAdmin))
            {
                return await _orderService.GetAllAsync();
            }

            return await _orderService.GetAllAsync(userId);
        }

        [HttpGet("{orderHeaderId:int}")]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(int orderHeaderId)
        {
            return await _orderService.GetByOrderIdAsync(orderHeaderId);
        }

        [HttpPut("UpdateStatus/{orderId},{newStatus}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> UpdateStatus(int orderId, string newStatus)
        {
            return await _orderService.UpdateOrderStatus(orderId, newStatus);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post([FromBody] CartDto cartDto)
        {
            return await _orderService.CreateOrder(cartDto);
        }

        [HttpPost("CreateStripeSession")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            return await _orderService.CreateStripeSession(stripeRequestDto);
        }


        [HttpPost("ValidateStripeSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
           return await _orderService.ValidateStripeSession(orderHeaderId);
        }
    }
}
