using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class FruitController : Controller
    {
        private readonly FruitStoreContext _context;

        public FruitController(FruitStoreContext context)
        {
            _context = context;
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
        public IActionResult Edit(Fruit fruit)
        {
            if (ModelState.IsValid)
            {
                _context.Fruits.Update(fruit);
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
        public IActionResult Create(MyStore.Models.Fruit fruit)
        {
            if (ModelState.IsValid)
            {
                _context.Fruits.Add(fruit);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(fruit);
        }
    }
}
