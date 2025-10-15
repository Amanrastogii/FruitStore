using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly FruitStoreContext _context;

        public AccountController(FruitStoreContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string passwordHash)
        {
            string hash = HashPassword(passwordHash);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
            if (user != null)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);
                // Redirect based on role
                if (user.Role == "Admin")
                    return RedirectToAction("List", "Fruit");
                else
                    return RedirectToAction("CustomerHome", "Fruit");
            }
            ViewBag.Error = "Invalid login";
            return View();
        }

        private string HashPassword(string password)
        {
            // Simple hash, use a robust algorithm in production!
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
