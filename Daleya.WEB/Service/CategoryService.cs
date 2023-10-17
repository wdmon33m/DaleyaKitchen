using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Service.IService;
using Daleya.WEB.Utility;
using Newtonsoft.Json;

namespace Daleya.WEB.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseService _baseService;
        private const string categoryApiUrl = "/api/category/";
        public CategoryService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAsync(int categoryId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + categoryApiUrl  + categoryId
            });
        }

        public async Task<ResponseDto?> GetAllAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.DaleyaApiBase + categoryApiUrl
            });
        }

        public async Task<ResponseDto?> CreateAsync(CategoryDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = SD.DaleyaApiBase + categoryApiUrl
            });
        }

        public async Task<ResponseDto?> UpdateAsync(CategoryDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = SD.DaleyaApiBase + categoryApiUrl
            });
        }

        public async Task<ResponseDto?> DeleteAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.DaleyaApiBase + categoryApiUrl + id
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
    }
}
