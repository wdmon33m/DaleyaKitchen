using Daleya.API.Models;

namespace Daleya.API.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> UpdateAsync(Product entity);
    }
}
