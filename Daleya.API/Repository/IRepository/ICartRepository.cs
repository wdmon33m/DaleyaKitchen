using Daleya.API.Models;
using Daleya.API.Models.Dto;

namespace Daleya.API.Repository.IRepository
{
    public interface ICartRepository : IRepository<CartHeader>
    {
        Task<ResponseDto> GetCartAsync(string userId);
        Task<ResponseDto> CartUpsert(Cart cartHeader);
    }
}
