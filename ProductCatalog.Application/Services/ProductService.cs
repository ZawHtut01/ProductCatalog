
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var result = await _productRepository.GetAllAsync();

            if (!result.IsSuccess)
                return Result<IEnumerable<ProductDto>>.Failure(result.ErrorMessage!, result.StatusCode);

            var products = result.Data!.Select(p => MapToDto(p));
            return Result<IEnumerable<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
        {
            var result = await _productRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
                return Result<ProductDto>.Failure(result.ErrorMessage!, result.StatusCode);

            return Result<ProductDto>.Success(MapToDto(result.Data!));
        }

        public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            // Business validation
            var validationResult = ValidateProduct(dto);
            if (!validationResult.IsSuccess)
                return Result<ProductDto>.Failure(validationResult.ErrorMessage!, 400);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _productRepository.CreateAsync(product);

            if (!result.IsSuccess)
                return Result<ProductDto>.Failure(result.ErrorMessage!, result.StatusCode);

            return Result<ProductDto>.Created(MapToDto(result.Data!));
        }

        public async Task<Result<bool>> UpdateProductAsync(UpdateProductDto dto)
        {
            // Business validation
            //var validationResult = ValidateProduct(dto);
            //if (!validationResult.IsSuccess)
            //    return Result<bool>.Failure(validationResult.ErrorMessage!, 400);

            var existingProduct = await _productRepository.GetByIdAsync(dto.Id);
            if (!existingProduct.IsSuccess)
                return Result<bool>.Failure(existingProduct.ErrorMessage!, existingProduct.StatusCode);

            var product = existingProduct.Data!;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _productRepository.UpdateAsync(product);

            if (!result.IsSuccess)
                return Result<bool>.Failure(result.ErrorMessage!, result.StatusCode);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteProductAsync(int id)
        {
            var result = await _productRepository.DeleteAsync(id);

            if (!result.IsSuccess)
                return Result<bool>.Failure(result.ErrorMessage!, result.StatusCode);

            return Result<bool>.Success(true);
        }

        private Result<bool> ValidateProduct(CreateProductDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product name is required");
            else if (dto.Name.Length < 3)
                errors.Add("Product name must be at least 3 characters");
            else if (dto.Name.Length > 200)
                errors.Add("Product name cannot exceed 200 characters");

            if (dto.Price <= 0)
                errors.Add("Price must be greater than 0");

            if (dto.Price > 1000000)
                errors.Add("Price cannot exceed 1,000,000");

            if (!string.IsNullOrEmpty(dto.Category) && dto.Category.Length > 100)
                errors.Add("Category cannot exceed 100 characters");

            if (errors.Any())
                return Result<bool>.ValidationFailure(errors);

            return Result<bool>.Success(true);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                CreatedAt = product.CreatedAt
            };
        }
    }


    public interface IProductService
    {
        Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<Result<ProductDto>> GetProductByIdAsync(int id);
        Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto);
        Task<Result<bool>> UpdateProductAsync(UpdateProductDto dto);
        Task<Result<bool>> DeleteProductAsync(int id);
        //Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        //Task<ProductDto?> GetProductByIdAsync(int id);
        //Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        //Task<bool> UpdateProductAsync(UpdateProductDto dto);
        //Task<bool> DeleteProductAsync(int id);
    }
}
