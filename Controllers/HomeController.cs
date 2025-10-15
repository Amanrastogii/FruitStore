using MyStore.Models;
using MyStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGenericRepository<Fruit> _fruitRepository;

        public HomeController(IGenericRepository<Fruit> fruitRepository)
        {
            _fruitRepository = fruitRepository;
        }

        public async Task<IActionResult> Index()
        {
            var fruits = await _fruitRepository.GetAllAsync();
            return View(fruits);
        }
    }
}
