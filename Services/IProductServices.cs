using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto> Products, PaginationMetadata Pagination)> GetProductsAsync(
            int? productId, string? productName, decimal? price, int? stockQuantity, int? status, int? categoryId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<ProductDto> Products, PaginationMetadata Pagination)> GetProductsAsync(
    int? productId, string? productName, decimal? price, int? stockQuantity, int? status, int? categoryId,
    string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _productRepository.Query();

            var predicate = PredicateBuilder.New<Product>(true);
            if (productId.HasValue) predicate = predicate.And(p => p.ProductId == productId);
            if (!string.IsNullOrEmpty(productName)) predicate = predicate.And(p => p.ProductName.Contains(productName));
            if (price.HasValue) predicate = predicate.And(p => p.Price == price);
            if (stockQuantity.HasValue) predicate = predicate.And(p => p.StockQuantity == stockQuantity);
            if (status.HasValue) predicate = predicate.And(p => p.Status == status);
            if (categoryId.HasValue) predicate = predicate.And(p => p.CategoryId == categoryId);

            query = query.Where(predicate);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Product).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(DynamicSort<Product>(sortBy))
                    : query.OrderByDescending(DynamicSort<Product>(sortBy));
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (products.Select(MapToDto), paginationMetadata); // ✅ Fix applied
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.Query()
                .Where(p => p.ProductId == id)
                .FirstOrDefaultAsync();

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = MapToEntity(productDto);
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return null;

            if (!string.IsNullOrWhiteSpace(productDto.ProductCode)) existingProduct.ProductCode = productDto.ProductCode;
            if (!string.IsNullOrWhiteSpace(productDto.ProductName)) existingProduct.ProductName = productDto.ProductName;
            if (!string.IsNullOrWhiteSpace(productDto.Description)) existingProduct.Description = productDto.Description;
            if (productDto.Price.HasValue) existingProduct.Price = productDto.Price;
            if (productDto.StockQuantity.HasValue) existingProduct.StockQuantity = productDto.StockQuantity;
            if (productDto.Status==null) existingProduct.Status = productDto.Status;
            if (productDto.CategoryId != 0) existingProduct.CategoryId = productDto.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);
            await _productRepository.SaveChangesAsync();

            return MapToDto(existingProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            long inid = (long)id;
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        private static Expression<Func<T, object>> DynamicSort<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(param, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Path = product.Path,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Status = product.Status,
                CategoryId = product.CategoryId
            };
        }

        private static Product MapToEntity(ProductDto dto)
        {
            return new Product
            {
                ProductId = dto.ProductId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                Path = dto.Path,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
                CategoryId = dto.CategoryId
            };
        }
    }
}
