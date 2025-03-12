using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(
            int? ProductId, string? ProductName, decimal? Price, int? StockQuantity, int? Status, int? CategoryId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(
            int? ProductId, string? ProductName, decimal? Price, int? StockQuantity, int? Status, int? CategoryId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _productRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (ProductId.HasValue)
            {
                query = query.Where(p => p.ProductId == ProductId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                query = query.Where(p => p.ProductName.Contains(ProductName));
                hasFilters = true;
            }
            if (Price.HasValue)
            {
                query = query.Where(p => p.Price == Price);
                hasFilters = true;
            }
            if (StockQuantity.HasValue)
            {
                query = query.Where(p => p.StockQuantity == StockQuantity);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(p => p.Status == Status);
                hasFilters = true;
            }
            if (CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == CategoryId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _productRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Product).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (products, paginationMetadata);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.Query()
                .Where(p => p.ProductId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.Status = product.Status;
            existingProduct.CategoryId = product.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);
            await _productRepository.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            return true;
        }
    }
}