using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class FruitController : Controller
    {
        private readonly FruitStoreContext _context;
        private readonly IWebHostEnvironment _environment;

        public FruitController(FruitStoreContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // For all users - fruit list for admins (shows add/edit/delete)
        public IActionResult List()
        {
            var fruits = _context.Fruits.ToList();
            return View(fruits); // returns to List.cshtml
        }

        // Dashboard for customers (NO add/edit/delete)
        public IActionResult CustomerHome()
        {
            var fruits = _context.Fruits.ToList();
            return View("CustomerHome", fruits); // returns to CustomerHome.cshtml
        }

        public IActionResult Edit(int id)
        {
            var fruit = _context.Fruits.FirstOrDefault(f => f.Id == id);
            if (fruit == null) return NotFound();
            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fruit fruit, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Fruits.FirstOrDefault(f => f.Id == id);
                if (existing == null) return NotFound();

                existing.Name = fruit.Name;
                existing.Description = fruit.Description;
                existing.Price = fruit.Price;
                existing.Stock = fruit.Stock;

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existing.ImagePath))
                    {
                        DeleteImage(existing.ImagePath);
                    }

                    existing.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(fruit);
        }

        public IActionResult Delete(int id)
        {
            var fruit = _context.Fruits.FirstOrDefault(f => f.Id == id);
            if (fruit == null) return NotFound();
            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Fruit fruit)
        {
            var deleteFruit = _context.Fruits.FirstOrDefault(f => f.Id == id);
            if (deleteFruit != null)
            {
                // Delete associated image
                if (!string.IsNullOrEmpty(deleteFruit.ImagePath))
                {
                    DeleteImage(deleteFruit.ImagePath);
                }

                _context.Fruits.Remove(deleteFruit);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        // GET: Fruit/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fruit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fruit fruit, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    fruit.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.Fruits.Add(fruit);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(fruit);
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
}
