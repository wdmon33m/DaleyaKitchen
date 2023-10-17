using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;

namespace Daleya.WEB.Service.IService
{
    public interface ICategoryService
    {
        Task<ResponseDto?> GetAsync(int categoryId);
        Task<ResponseDto?> GetAllAsync();
        IEnumerable<CategoryDto>? GetAllDeserialize();
        Task<ResponseDto?> CreateAsync(CategoryDto dto);
        Task<ResponseDto?> UpdateAsync(CategoryDto dto);
        Task<ResponseDto?> DeleteAsync(int id);
    }
}
