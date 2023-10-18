using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> UpdateAsync(Coupon entity);
    }
}
