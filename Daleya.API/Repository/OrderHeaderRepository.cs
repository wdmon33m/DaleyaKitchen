using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Repository.IRepository;

namespace Daleya.API.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _db;

        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<OrderHeader> UpdateAsync(OrderHeader entity)
        {
            _db.OrderHeaders.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
