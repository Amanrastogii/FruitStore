namespace MyStore.DTOs.User
{
    /// <summary>
    /// DTO for user API responses
    /// Excludes sensitive data like password hash
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // ❌ PasswordHash is intentionally excluded for security
    }
}
