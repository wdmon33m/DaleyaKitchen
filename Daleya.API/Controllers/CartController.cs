using AutoMapper;
using Azure;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Daleya.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public CartController(ICartRepository cartRepository, IMapper mapper, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(string userId)
        {
            try
            {
                var response =  await _cartRepository.GetCartAsync(userId);
                if (!response.IsSuccess)
                {
                    response.NotFound(response.ErrorMessage);
                }

                _response.IsSuccess = response.IsSuccess;
                _response.ErrorMessage = response.ErrorMessage;
                _response.Result = response.Result;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
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
        public async Task<ResponseDto> Post([FromBody]CartDto cartDto)
        {
            try
            {
                Cart cart = new();
                cart.CartHeader = _mapper.Map<CartHeader>(cartDto.CartHeaderDto);
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetailsDto);

                if (cart.CartHeader is null)
                {
                    return _response.BadRequest("Cart header is empty");
                }
                if (cart.CartDetails is null)
                {
                    return _response.BadRequest("Cart is empty");
                }

                var response = await _cartRepository.CartUpsert(cart);

                if (!response.IsSuccess)
                {
                    response.NotFound(response.ErrorMessage);
                }

                _response.IsSuccess = response.IsSuccess;
                _response.ErrorMessage = response.ErrorMessage;
                _response.Result = response.Result;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
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
