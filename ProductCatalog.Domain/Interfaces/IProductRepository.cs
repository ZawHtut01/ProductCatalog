
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Result<IEnumerable<Product>>> GetAllAsync();
        Task<Result<Product?>> GetByIdAsync(int id);
        Task<Result<Product>> CreateAsync(Product product);
        Task<Result<bool>> UpdateAsync(Product product);
        Task<Result<bool>> DeleteAsync(int id);
        //Task<Result<bool>> ExistsAsync(int id);
    }
}
