using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto.Cart;
using Daleya.WEB.Models.Dto.Order;

namespace Daleya.WEB.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> GetAllAsync(string? userId = "");
        Task<ResponseDto?> GetByOrderIdAsync(int orderHeaderID);
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
    }
}
