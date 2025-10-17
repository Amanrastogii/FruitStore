using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FruitApiController : ControllerBase
    {
        private readonly FruitStoreContext _context;
        private readonly IWebHostEnvironment _environment;

        public FruitApiController(FruitStoreContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/FruitApi
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Fruits.ToList());
        }

        // GET: api/FruitApi/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var fruit = _context.Fruits.Find(id);
            return fruit is null ? NotFound() : Ok(fruit);
        }

        // POST: api/FruitApi
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FruitUploadDto fruitDto)
        {
            var fruit = new Fruit
            {
                Name = fruitDto.Name,
                Description = fruitDto.Description,
                Price = fruitDto.Price,
                Stock = fruitDto.Stock
            };

            // Handle image upload
            if (fruitDto.Image != null && fruitDto.Image.Length > 0)
            {
                fruit.ImagePath = await SaveImageAsync(fruitDto.Image);
            }

            _context.Fruits.Add(fruit);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = fruit.Id }, fruit);
        }

        // PUT: api/FruitApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] FruitUploadDto fruitDto)
        {
            var existing = _context.Fruits.Find(id);
            if (existing is null) return NotFound();

            existing.Name = fruitDto.Name;
            existing.Description = fruitDto.Description;
            existing.Price = fruitDto.Price;
            existing.Stock = fruitDto.Stock;

            // Handle image upload if a new image is provided
            if (fruitDto.Image != null && fruitDto.Image.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    DeleteImage(existing.ImagePath);
                }

                existing.ImagePath = await SaveImageAsync(fruitDto.Image);
            }

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/FruitApi/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var fruit = _context.Fruits.Find(id);
            if (fruit is null) return NotFound();

            // Delete associated image file if exists
            if (!string.IsNullOrEmpty(fruit.ImagePath))
            {
                DeleteImage(fruit.ImagePath);
            }

            _context.Fruits.Remove(fruit);
            _context.SaveChanges();
            return NoContent();
        }

        // Helper method to save uploaded image
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            // Validate file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file type. Only image files are allowed.");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imagesPath = Path.Combine(_environment.WebRootPath, "images");

            // Create images directory if it doesn't exist
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var filePath = Path.Combine(imagesPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return relative path for database storage
            return $"images/{fileName}";
        }

        // Helper method to delete image
        private void DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }

    // DTO for handling file uploads with form data
    public class FruitUploadDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Image { get; set; }
    }
}
