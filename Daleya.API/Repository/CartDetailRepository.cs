using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Repository.IRepository;

namespace Daleya.API.Repository
{
    public class CartDetailRepository : Repository<CartDetails>, ICartDetailRepository
    {
        private readonly AppDbContext _db;

        public CartDetailRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CartDetails> UpdateAsync(CartDetails entity)
        {
            _db.CartDetails.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
