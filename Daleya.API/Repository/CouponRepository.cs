using Daleya.API.Data;
using Daleya.API.Models;
using Daleya.API.Repository.IRepository;

namespace Daleya.API.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly AppDbContext _db;

        public CouponRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Coupon> UpdateAsync(Coupon entity)
        {
            _db.Coupons.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
