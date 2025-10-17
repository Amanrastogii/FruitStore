using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
using MyStore.DTOs.Fruit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// GET: api/FruitApi - Get all fruits with image URLs
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var fruits = _context.Fruits.ToList();

                // Map to DTO with image URLs
                var response = fruits.Select(f => new FruitResponseDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Stock = f.Stock,
                    ImagePath = f.ImagePath,
                    ImageUrl = GetImageUrl(f.ImagePath)
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving fruits", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/FruitApi/5 - Get single fruit by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var fruit = _context.Fruits.Find(id);
                if (fruit == null)
                    return NotFound(new { message = $"Fruit with ID {id} not found" });

                var response = new FruitResponseDto
                {
                    Id = fruit.Id,
                    Name = fruit.Name,
                    Description = fruit.Description,
                    Price = fruit.Price,
                    Stock = fruit.Stock,
                    ImagePath = fruit.ImagePath,
                    ImageUrl = GetImageUrl(fruit.ImagePath)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving fruit", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/FruitApi - Create new fruit with optional image
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FruitUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var fruit = new Fruit
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock
                };

                // Handle image upload
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    fruit.ImagePath = await SaveImageAsync(dto.Image);
                }

                _context.Fruits.Add(fruit);
                _context.SaveChanges();

                var response = new FruitResponseDto
                {
                    Id = fruit.Id,
                    Name = fruit.Name,
                    Description = fruit.Description,
                    Price = fruit.Price,
                    Stock = fruit.Stock,
                    ImagePath = fruit.ImagePath,
                    ImageUrl = GetImageUrl(fruit.ImagePath)
                };

                return CreatedAtAction(nameof(Get), new { id = fruit.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating fruit", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/FruitApi/5 - Update existing fruit
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] FruitUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var fruit = _context.Fruits.Find(id);
                if (fruit == null)
                    return NotFound(new { message = $"Fruit with ID {id} not found" });

                // Update properties
                fruit.Name = dto.Name;
                fruit.Description = dto.Description;
                fruit.Price = dto.Price;
                fruit.Stock = dto.Stock;

                // Handle new image upload
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(fruit.ImagePath))
                        DeleteImage(fruit.ImagePath);

                    fruit.ImagePath = await SaveImageAsync(dto.Image);
                }

                _context.SaveChanges();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating fruit", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/FruitApi/5 - Delete fruit and its image
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var fruit = _context.Fruits.Find(id);
                if (fruit == null)
                    return NotFound(new { message = $"Fruit with ID {id} not found" });

                // Delete associated image file
                if (!string.IsNullOrEmpty(fruit.ImagePath))
                    DeleteImage(fruit.ImagePath);

                _context.Fruits.Remove(fruit);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting fruit", error = ex.Message });
            }
        }

        #region Helper Methods

        /// <summary>
        /// Generate full image URL for API responses
        /// </summary>
        private string? GetImageUrl(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;

            // Build full URL: https://fruitstore-1.onrender.com/images/abc.jpg
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return $"{baseUrl}/{imagePath}";
        }

        /// <summary>
        /// Save uploaded image to wwwroot/images directory
        /// </summary>
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Invalid file type '{extension}'. Only image files (JPG, PNG, GIF, WEBP, JFIF) are allowed.");

            // Validate file size (5MB max)
            if (image.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("File size cannot exceed 5MB.");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imagesPath = Path.Combine(_environment.WebRootPath, "images");

            // Create images directory if it doesn't exist
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            var filePath = Path.Combine(imagesPath, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            Console.WriteLine($"Image saved: {fileName}");

            // Return relative path for database storage
            return $"images/{fileName}";
        }

        /// <summary>
        /// Delete image file from disk
        /// </summary>
        private void DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    Console.WriteLine($"Image deleted: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                // Don't throw - deletion failure shouldn't stop the operation
            }
        }

        #endregion
    }
}
