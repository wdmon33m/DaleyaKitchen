using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;

namespace Daleya.WEB.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetAsync(int couponId);
        Task<ResponseDto?> GetByCouponCodeAsync(string couponCode);
        Task<ResponseDto?> GetAllAsync();
        Task<ResponseDto?> CreateAsync(CouponDto dto);
        Task<ResponseDto?> UpdateAsync(CouponDto dto);
        Task<ResponseDto?> DeleteAsync(int id);
    }
}
