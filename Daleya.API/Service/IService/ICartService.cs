using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;

namespace Daleya.API.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartAsync(string userId);
        Task<ResponseDto> CartUpsert(Cart cartHeader);
        Task<ResponseDto> RemoveCart(int cartDetailsId);
        Task<ResponseDto> IncreaseItem(int cartDetailsId, int productId);
        Task<ResponseDto> DecreaseItem(int cartDetailsId, int productId);
        Task<ResponseDto> ApplyCoupon(CartHeaderDto cartHeaderDto);
        Task<ResponseDto> RemoveCoupon(string userId, int cartHeaderId);
    }
}
