using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;
using Daleya.API.Models.Dto.Order;

namespace Daleya.API.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> GetAllAsync(string? userId = "");
        Task<ResponseDto> GetByOrderIdAsync(int orderHeaderID);
        Task<ResponseDto> CreateOrder(CartDto cartDto);
        Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus);
        Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto> ValidateStripeSession(int orderHeaderId);
    }
}
