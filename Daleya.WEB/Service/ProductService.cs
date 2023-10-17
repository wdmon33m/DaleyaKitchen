using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;

namespace Daleya.WEB.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        private const string productApiUrl = "/api/product/";
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAsync(int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + productApiUrl + productId
            });
        }

        public async Task<ResponseDto?> GetAllAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + productApiUrl
            });
        }

        public async Task<ResponseDto?> CreateAsync(ProductDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                ContentType = SD.ContentType.MultipartFormData,
                Url = SD.DaleyaApiBase + productApiUrl
            });
        }

        public async Task<ResponseDto?> UpdateAsync(ProductDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                ContentType = SD.ContentType.MultipartFormData,
                Url = SD.DaleyaApiBase + productApiUrl
            });
        }

        public async Task<ResponseDto?> DeleteAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + productApiUrl + id
            });
        }
    }
}
