using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;

namespace Daleya.WEB.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartAsync(string userId);
        Task<ResponseDto?> CartUpsert(CartDto cartHeader);
        Task<ResponseDto?> RemoveCart(int cartDetailsId);
        Task<ResponseDto?> IncreaseItem(int cartDetailsId, int productId);
        Task<ResponseDto?> DecreaseItem(int cartDetailsId, int productId);
        Task<ResponseDto?> ApplyCoupon(CartHeaderDto dto);
        Task<ResponseDto?> RemoveCoupon(string userId, int cartHeaderId);
    }
}
