using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
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

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash ?? "");
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { message = "Registered!", user.Email, user.Role });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            string hash = HashPassword(login.PasswordHash ?? "");
            var user = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.PasswordHash == hash);
            if (user != null)
                return Ok(new { message = "Logged in!", user.Email, user.Role });
            return Unauthorized(new { message = "Invalid login" });
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
