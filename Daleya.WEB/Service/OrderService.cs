using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Cart;
using Daleya.WEB.Models.Dto.Order;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;

namespace Daleya.WEB.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        private const string orderApiUrl = "/api/order/";
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.DaleyaApiBase + orderApiUrl
            });
        }

        public async Task<ResponseDto?> GetAllAsync(string? userId = "")
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + orderApiUrl + userId
            });
        }

        public async Task<ResponseDto?> GetByOrderIdAsync(int orderHeaderID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + orderApiUrl + orderHeaderID
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.DaleyaApiBase + orderApiUrl + orderId + "," + newStatus
            });
        }

        public async Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = SD.DaleyaApiBase + orderApiUrl + "CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.DaleyaApiBase + orderApiUrl + "ValidateStripeSession"
            });
        }
    }
}

