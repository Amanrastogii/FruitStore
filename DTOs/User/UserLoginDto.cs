using System.ComponentModel.DataAnnotations;

namespace MyStore.DTOs.User
{
    /// <summary>
    /// DTO for user login
    /// </summary>
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
