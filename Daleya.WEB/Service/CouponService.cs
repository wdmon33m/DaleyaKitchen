using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;
using Newtonsoft.Json;

namespace Daleya.WEB.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        private const string couponApiUrl = "/api/coupon/";
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> GetAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + couponApiUrl + couponId
            });
        }

        public async Task<ResponseDto?> GetAllAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + couponApiUrl
            });
        }

        public async Task<ResponseDto?> CreateAsync(CouponDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = SD.DaleyaApiBase + couponApiUrl
            });
        }

        public async Task<ResponseDto?> UpdateAsync(CouponDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = SD.DaleyaApiBase + couponApiUrl
            });
        }

        public async Task<ResponseDto?> DeleteAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + couponApiUrl + id
            });
        }

        public IEnumerable<CategoryDto>? GetAllDeserialize()
        {
            ResponseDto? response = GetAllAsync().GetAwaiter().GetResult();

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(response.Result));
            }

            return null;
        }

        public async Task<ResponseDto?> GetByCouponCodeAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + couponApiUrl + "GetByCouponCode/" + couponCode
            });
        }
    }
}
