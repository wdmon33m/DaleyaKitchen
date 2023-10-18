using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Repository.IRepository;

namespace Daleya.API.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly AppDbContext _db;

        public OrderDetailsRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<OrderDetails> UpdateAsync(OrderDetails entity)
        {
            _db.OrderDetails.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
