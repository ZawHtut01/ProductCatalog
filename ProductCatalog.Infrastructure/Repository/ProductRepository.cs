

using Dapper;
using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(DapperContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Product>>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM Products WHERE IsActive = 1 ORDER BY CreatedAt DESC";

                using var connection = _context.CreateConnection();
                var products = await connection.QueryAsync<Product>(query);

                return Result<IEnumerable<Product>>.Success(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                return Result<IEnumerable<Product>>.Failure(
                    "An error occurred while retrieving products", 500);
            }
        }

        public async Task<Result<Product>> GetByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM Products WHERE Id = @Id AND IsActive = 1";

                using var connection = _context.CreateConnection();
                var product = await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });

                if (product == null)
                    return Result<Product>.NotFound($"Product with ID {id} not found");

                return Result<Product>.Success(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                return Result<Product>.Failure(
                    $"An error occurred while retrieving product with ID {id}", 500);
            }
        }

        public async Task<Result<Product>> CreateAsync(Product product)
        {
            try
            {
                // Check for duplicate name
                var duplicateCheck = await CheckDuplicateNameAsync(product.Name);
                if (!duplicateCheck.IsSuccess)
                    return Result<Product>.Failure(duplicateCheck.ErrorMessage!, 400);

                var query = @"
                INSERT INTO Products (Name, Description, Price, Category, CreatedAt) 
                VALUES (@Name, @Description, @Price, @Category, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

                var parameters = new DynamicParameters();
                parameters.Add("Name", product.Name);
                parameters.Add("Description", product.Description);
                parameters.Add("Price", product.Price);
                parameters.Add("Category", product.Category);
                parameters.Add("CreatedAt", product.CreatedAt);

                using var connection = _context.CreateConnection();
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                product.Id = id;

                _logger.LogInformation("Product created with ID {ProductId}", id);
                return Result<Product>.Created(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", product.Name);
                return Result<Product>.Failure(
                    "An error occurred while creating the product", 500);
            }
        }

        public async Task<Result<bool>> UpdateAsync(Product product)
        {
            try
            {
                // Verify product exists
                var existingProduct = await GetByIdAsync(product.Id);
                if (!existingProduct.IsSuccess)
                    return Result<bool>.NotFound($"Product with ID {product.Id} not found");

                // Check for duplicate name (excluding current product)
                var duplicateCheck = await CheckDuplicateNameAsync(product.Name, product.Id);
                if (!duplicateCheck.IsSuccess)
                    return Result<bool>.Failure(duplicateCheck.ErrorMessage!, 400);

                var query = @"
                UPDATE Products 
                SET Name = @Name, 
                    Description = @Description, 
                    Price = @Price, 
                    Category = @Category, 
                    UpdatedAt = @UpdatedAt 
                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", product.Id);
                parameters.Add("Name", product.Name);
                parameters.Add("Description", product.Description);
                parameters.Add("Price", product.Price);
                parameters.Add("Category", product.Category);
                parameters.Add("UpdatedAt", product.UpdatedAt);

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, parameters);

                if (affectedRows > 0)
                {
                    _logger.LogInformation("Product with ID {ProductId} updated", product.Id);
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Product could not be updated", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", product.Id);
                return Result<bool>.Failure(
                    "An error occurred while updating the product", 500);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                // Verify product exists
                var existingProduct = await GetByIdAsync(id);
                if (!existingProduct.IsSuccess)
                    return Result<bool>.NotFound($"Product with ID {id} not found");

                var query = "UPDATE Products SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

                if (affectedRows > 0)
                {
                    _logger.LogInformation("Product with ID {ProductId} deleted", id);
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Product could not be deleted", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
                return Result<bool>.Failure(
                    "An error occurred while deleting the product", 500);
            }
        }

        private async Task<Result<bool>> CheckDuplicateNameAsync(string name, int? excludeId = null)
        {
            try
            {
                var query = "SELECT COUNT(1) FROM Products WHERE Name = @Name AND IsActive = 1";
                var parameters = new DynamicParameters();
                parameters.Add("Name", name);

                if (excludeId.HasValue)
                {
                    query += " AND Id != @ExcludeId";
                    parameters.Add("ExcludeId", excludeId.Value);
                }

                using var connection = _context.CreateConnection();
                var count = await connection.ExecuteScalarAsync<int>(query, parameters);

                if (count > 0)
                    return Result<bool>.Failure($"Product with name '{name}' already exists", 400);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking duplicate product name {ProductName}", name);
                return Result<bool>.Failure("Error checking product name", 500);
            }
        }
       
        
        //private readonly DapperContext _context;

        //public ProductRepository(DapperContext context)
        //{
        //    _context = context;
        //}

        //public async Task<IEnumerable<Product>> GetAllAsync()
        //{
        //    var query = "SELECT * FROM Products WHERE IsActive = 1 ORDER BY CreatedAt DESC";

        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryAsync<Product>(query);
        //}

        //public async Task<Product?> GetByIdAsync(int id)
        //{
        //    var query = "SELECT * FROM Products WHERE Id = @Id AND IsActive = 1";

        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
        //}

        //public async Task<Product> CreateAsync(Product product)
        //{
        //    var query = @"
        //    INSERT INTO Products (Name, Description, Price, Category, CreatedAt) 
        //    VALUES (@Name, @Description, @Price, @Category, @CreatedAt);
        //    SELECT CAST(SCOPE_IDENTITY() as int)";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("Name", product.Name);
        //    parameters.Add("Description", product.Description);
        //    parameters.Add("Price", product.Price);
        //    parameters.Add("Category", product.Category);
        //    parameters.Add("CreatedAt", product.CreatedAt);

        //    using var connection = _context.CreateConnection();
        //    var id = await connection.QuerySingleAsync<int>(query, parameters);

        //    product.Id = id;
        //    return product;
        //}

        //public async Task<bool> UpdateAsync(Product product)
        //{
        //    var query = @"
        //    UPDATE Products 
        //    SET Name = @Name, 
        //        Description = @Description, 
        //        Price = @Price, 
        //        Category = @Category, 
        //        UpdatedAt = @UpdatedAt 
        //    WHERE Id = @Id";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("Id", product.Id);
        //    parameters.Add("Name", product.Name);
        //    parameters.Add("Description", product.Description);
        //    parameters.Add("Price", product.Price);
        //    parameters.Add("Category", product.Category);
        //    parameters.Add("UpdatedAt", product.UpdatedAt);

        //    using var connection = _context.CreateConnection();
        //    var affectedRows = await connection.ExecuteAsync(query, parameters);

        //    return affectedRows > 0;
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    // Soft delete
        //    var query = "UPDATE Products SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE Id = @Id";

        //    using var connection = _context.CreateConnection();
        //    var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

        //    return affectedRows > 0;
        //}

        //public async Task<bool> ExistsAsync(int id)
        //{
        //    var query = "SELECT COUNT(1) FROM Products WHERE Id = @Id AND IsActive = 1";

        //    using var connection = _context.CreateConnection();
        //    var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });

        //    return count > 0;
        //}
    }
}
