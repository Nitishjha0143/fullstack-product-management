using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductController(ProductDbContext context)
        {
            _context = context;
        }

        // ADMIN: Add new product
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromForm] Product product, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                // Define upload path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Create folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create a unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                // Full file path
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Save image path in product
                product.ImageUrl = $"/images/{uniqueFileName}";
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added successfully", product });
        }

        // PUBLIC: Get all products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        // PUBLIC: Get product by id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // ADMIN: Update product
        [HttpPut("{id}")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.StockQuantity = updatedProduct.StockQuantity;
            product.ImageUrl = updatedProduct.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Product updated successfully", product });
        }

        // ADMIN: Delete product
        [HttpDelete("{id}")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
