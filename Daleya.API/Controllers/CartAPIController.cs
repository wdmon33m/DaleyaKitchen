using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;
using Daleya.API.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Daleya.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public CartAPIController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet("{userId}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(string userId)
        {
            return await _cartService.GetCartAsync(userId);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post([FromBody]CreateCartDto cartDto)
        {
            try
            {
                Cart cart = new();
                cart.CartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails);

                if (cart.CartHeader is null)
                {
                    return _response.BadRequest("Cart header is empty");
                }
                if (cart.CartDetails is null)
                {
                    return _response.BadRequest("Cart is empty");
                }

                return await _cartService.CartUpsert(cart);
            }
            catch (Exception ex)
            {
                return _response.InternalServerError(ex.Message);
            }
        }

        [HttpDelete("{cartDetailsId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> RemoveCart(int cartDetailsId)
        {
            return await _cartService.RemoveCart(cartDetailsId);
        }
                                                         
        [HttpPost("IncreaseItem/{cartDetailsId},{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> IncreaseItem(int cartDetailsId,int productId)
        {
            return await _cartService.IncreaseItem(cartDetailsId,productId);
        }

        [HttpDelete("DecreaseItem/{cartDetailsId:int},{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> DecreaseItem(int cartDetailsId, int productId)
        {
            return await _cartService.DecreaseItem(cartDetailsId, productId);
        }

        [HttpPost("ApplyCoupon")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> ApplyCoupon(CartHeaderDto cartHeaderDto)
        {
            return await _cartService.ApplyCoupon(cartHeaderDto);
        }

        [HttpDelete("RemoveCoupon/{userId},{cartHeaderId}"),]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> RemoveCoupon(string userId, int cartHeaderId)
        {
            return await _cartService.RemoveCoupon(userId,cartHeaderId);
        }
    }
}
