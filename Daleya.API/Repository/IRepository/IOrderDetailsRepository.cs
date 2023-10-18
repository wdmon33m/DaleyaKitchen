using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        Task<OrderDetails> UpdateAsync(OrderDetails entity);
    }
}
