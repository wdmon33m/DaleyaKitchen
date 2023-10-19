using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Repository.IRepository;

namespace Daleya.API.Repository
{
    public class CartHeaderRepository : Repository<CartHeader>, ICartHeaderRepository
    {
        private readonly AppDbContext _db;

        public CartHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CartHeader> UpdateAsync(CartHeader entity)
        {
            _db.CartHeaders.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
