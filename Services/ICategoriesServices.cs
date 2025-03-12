using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface ICategoryService
    {
        Task<(IEnumerable<Category> Categories, PaginationMetadata Pagination)> GetCategoriesAsync(
            int? CategoryId, string? CategoryName, int? Status,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int id, Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<(IEnumerable<Category> Categories, PaginationMetadata Pagination)> GetCategoriesAsync(
            int? CategoryId, string? CategoryName, int? Status,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _categoryRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == CategoryId);
                hasFilters = true;
            }
            if (!string.IsNullOrEmpty(CategoryName))
            {
                query = query.Where(c => c.CategoryName.Contains(CategoryName));
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(c => c.Status == Status);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _categoryRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Category).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(c => EF.Property<object>(c, sortBy))
                    : query.OrderByDescending(c => EF.Property<object>(c, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var categories = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (categories, paginationMetadata);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.Query()
                .Where(c => c.CategoryId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDescription = category.CategoryDescription;
            existingCategory.Status = category.Status;

            await _categoryRepository.UpdateAsync(existingCategory);
            await _categoryRepository.SaveChangesAsync();

            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }
    }
}