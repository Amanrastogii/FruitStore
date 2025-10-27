using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyStore.DTOs.Fruit
{
    /// <summary>
    /// DTO for uploading/creating/updating fruits with image files
    /// </summary>
    public class FruitUploadDto
    {
        [Required(ErrorMessage = "Fruit name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, 1000000, ErrorMessage = "Stock must be between 0 and 1000000")]
        public int Stock { get; set; }

        /// <summary>
        /// Image file for upload
        /// Accepts: jpeg, jpg, png, gif, webp, jfif
        /// Max size: 5MB
        /// </summary>
        public IFormFile? Image { get; set; }
    }
}
