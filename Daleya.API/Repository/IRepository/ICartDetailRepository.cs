using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface ICartDetailRepository : IRepository<CartDetails>
    {
        Task<CartDetails> UpdateAsync(CartDetails entity);
    }
}
