using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface ICartHeaderRepository : IRepository<CartHeader>
    {
        Task<CartHeader> UpdateAsync(CartHeader entity);
    }
}
