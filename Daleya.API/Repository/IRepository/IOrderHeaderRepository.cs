using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        Task<OrderHeader> UpdateAsync(OrderHeader entity);
    }
}
