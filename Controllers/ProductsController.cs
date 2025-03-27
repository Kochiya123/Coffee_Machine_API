using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.StaticFiles;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
        [FromQuery] int? productId,
        [FromQuery] string? productName,
        [FromQuery] decimal? price,
        [FromQuery] int? stockQuantity,
        [FromQuery] int? status,
        [FromQuery] int? categoryId,
        [FromQuery] string sortBy = "ProductId",
        [FromQuery] bool isAscending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var (products, pagination) = await _productService.GetProductsAsync(
                productId, productName, price, stockQuantity, status, categoryId,
                sortBy, isAscending, page, pageSize
            );

            // Convert relative paths to absolute URLs
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.Path))
                {
                    product.Path = $"{Request.Scheme}://{Request.Host}{product.Path}";
                }
            }

            return Ok(new { Products = products, Pagination = pagination });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // ✅ Include full image URL
            var productResponse = new
            {
                product.ProductId,
                product.ProductName,
                product.Price,
                product.StockQuantity,
                product.Status,
                product.CategoryId,
                ImageUrl = string.IsNullOrEmpty(product.Path) ? null : $"{Request.Scheme}://{Request.Host}{product.Path}"
            };

            return Ok(productResponse);
        }

        [HttpPost]
        [Consumes("multipart/form-data")] // ✅ Accept form-data
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Invalid product data.");
            }

            string? imagePath = null;

            if (productDto.Image != null && productDto.Image.Length > 0)
            {
                // ✅ Store in wwwroot/ProductImages/
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create unique file name
                string uniqueFileName = $"{Guid.NewGuid()}_{productDto.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to wwwroot/ProductImages/
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productDto.Image.CopyToAsync(fileStream);
                }

                // ✅ Store relative path for frontend access
                imagePath = $"/{uniqueFileName}";
            }

            var newProduct = new ProductDto
            {
                ProductCode = productDto.ProductCode,
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                Status = productDto.Status,
                CategoryId = productDto.CategoryId,
                Path = imagePath // Store relative path in DB
            };

            var createdProduct = await _productService.CreateProductAsync(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")] // ✅ Accept form-data
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto, IFormFile? imageFile)
        {
            if (productDto == null)
            {
                return BadRequest("Invalid product data.");
            }

            // 🔹 Fetch the existing product from the database
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // 🔹 Update the product details
            existingProduct.ProductCode = productDto.ProductCode;
            existingProduct.ProductName = productDto.ProductName;
            existingProduct.Price = productDto.Price;
            existingProduct.StockQuantity = productDto.StockQuantity;
            existingProduct.Status = productDto.Status;
            existingProduct.CategoryId = productDto.CategoryId;

            // 🔹 Handle Image Upload (If a new image is provided)
            if (imageFile != null)
            {
                var imagePath = await SaveImageAsync(imageFile);
                existingProduct.Path = imagePath; // Update Image Path
            }

            // 🔹 Update the product in the database
            var updatedProduct = await _productService.UpdateProductAsync(id,existingProduct);

            return Ok(new
            {
                updatedProduct.ProductCode,
                updatedProduct.ProductId,
                updatedProduct.ProductName,
                updatedProduct.Price,
                updatedProduct.StockQuantity,
                updatedProduct.Status,
                updatedProduct.CategoryId,
                ImageUrl = $"{Request.Scheme}://{Request.Host}{updatedProduct.Path}"
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory());
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/ProductImages/{fileName}"; // Return relative path for serving
        }

        [HttpGet("image/{fileName}")]
        public IActionResult GetProductImage(string fileName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages", fileName);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found.");
            }

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            var contentType = GetContentType(imagePath);
            return File(imageBytes, contentType);
        }

        // Helper method to determine the content type
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream"; // Default binary file type
            }
            return contentType;
        }
    }
}
