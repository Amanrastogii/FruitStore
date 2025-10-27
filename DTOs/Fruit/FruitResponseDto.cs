namespace MyStore.DTOs.Fruit
{
    /// <summary>
    /// DTO for fruit API responses with proper image URLs
    /// </summary>
    public class FruitResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        /// <summary>
        /// Relative image path stored in database (e.g., "images/abc.jpg")
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Full image URL for direct use in frontend
        /// Example: https://fruitstore-1.onrender.com/images/abc.jpg
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}
