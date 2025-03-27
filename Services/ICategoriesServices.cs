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
        Task<(IEnumerable<CategoryDto> Categories, PaginationMetadata Pagination)> GetCategoriesAsync(string? categoryCode, string? categoryName, int? status, string sortBy, bool isAscending, int page, int pageSize);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<(IEnumerable<CategoryDto> Categories, PaginationMetadata Pagination)> GetCategoriesAsync(string? categoryCode, string? categoryName, int? status, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _categoryRepository.Query();

            if (!string.IsNullOrEmpty(categoryCode))
                query = query.Where(c => c.CategoryCode.Contains(categoryCode));
            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(c => c.CategoryName.Contains(categoryName));
            if (status.HasValue)
                query = query.Where(c => c.Status == status);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Category).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(c => EF.Property<object>(c, sortBy))
                    : query.OrderByDescending(c => EF.Property<object>(c, sortBy));
            }

            var totalCount = await query.CountAsync();
            var categories = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryCode = c.CategoryCode,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription,
                Status = c.Status
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (categoryDtos, paginationMetadata);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                Status = category.Status
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryCode = categoryDto.CategoryCode,
                CategoryName = categoryDto.CategoryName,
                CategoryDescription = categoryDto.CategoryDescription,
                Status = categoryDto.Status
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            categoryDto.CategoryId = category.CategoryId;
            return categoryDto;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            category.CategoryCode = categoryDto.CategoryCode;
            category.CategoryName = categoryDto.CategoryName;
            category.CategoryDescription = categoryDto.CategoryDescription;
            category.Status = categoryDto.Status;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return categoryDto;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();
            return true;
        }
    }
}