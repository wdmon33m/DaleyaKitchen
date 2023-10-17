using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;

namespace Daleya.WEB.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetAsync(int productId);
        Task<ResponseDto?> GetAllAsync();
        Task<ResponseDto?> CreateAsync(ProductDto dto);
        Task<ResponseDto?> UpdateAsync(ProductDto dto);
        Task<ResponseDto?> DeleteAsync(int id);
    }
}
