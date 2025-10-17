using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyStore.Controllers
{
    public class AdminController : Controller  // CHANGED from AccountController
    {
        private readonly FruitStoreContext _context;

        public AdminController(FruitStoreContext context)  // CHANGED constructor name
        {
            _context = context;
        }

        // All your existing methods stay the same
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string passwordHash)
        {
            string hash = HashPassword(passwordHash);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);

            if (user != null)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);

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
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
