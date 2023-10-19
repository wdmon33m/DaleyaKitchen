using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;

namespace Daleya.WEB.Service
{
    public class CartService : ICartService
    {

        private readonly IBaseService _baseService;
        private const string cartApiUrl = "/api/cart/";
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> ApplyCoupon(CartHeaderDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = SD.DaleyaApiBase + cartApiUrl + "ApplyCoupon",
            });
        }

        public async Task<ResponseDto?> CartUpsert(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.DaleyaApiBase + cartApiUrl + "CartUpsert"
            });
        }

        public async Task<ResponseDto?> GetCartAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + cartApiUrl + userId
            });
        }

        public async Task<ResponseDto?> RemoveCart(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + cartApiUrl + cartDetailsId
            });
        }

        public async Task<ResponseDto?> RemoveCoupon(string userId, int cartHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + cartApiUrl + "RemoveCoupon/" + userId + "," + cartHeaderId 
            });
        }

        public async Task<ResponseDto?> IncreaseItem(int cartDetailsId, int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.DaleyaApiBase + cartApiUrl + "IncreaseItem/" + cartDetailsId + "," + productId 
            });
        }

        public async Task<ResponseDto?> DecreaseItem(int cartDetailsId, int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + cartApiUrl + "DecreaseItem/" + cartDetailsId + "," + productId
            });
        }
    }
}
