using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
using MyStore.DTOs.User;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly FruitStoreContext _context;

        public AccountApiController(FruitStoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// POST: api/AccountApi/register - Register new user
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Check if user already exists
                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest(new { message = "User with this email already exists" });

                // Validate role
                if (dto.Role != "Admin" && dto.Role != "Customer")
                    return BadRequest(new { message = "Role must be either 'Admin' or 'Customer'" });

                // Create new user
                var user = new User
                {
                    Email = dto.Email,
                    PasswordHash = HashPassword(dto.Password),
                    Role = dto.Role
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                // Return response without password
                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(new
                {
                    message = "Registration successful",
                    user = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/AccountApi/login - User login
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Hash the provided password
                string passwordHash = HashPassword(dto.Password);

                // Find user with matching email and password
                var user = _context.Users.FirstOrDefault(u =>
                    u.Email == dto.Email &&
                    u.PasswordHash == passwordHash);

                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                // Return response without password
                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(new
                {
                    message = "Login successful",
                    user = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/AccountApi/users - Get all users (Admin only - add authorization later)
        /// </summary>
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users.ToList();

                var response = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving users", error = ex.Message });
            }
        }

        #region Helper Methods

        /// <summary>
        /// Hash password using SHA256
        /// Note: In production, use BCrypt or Argon2 for better security
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        #endregion
    }
}
