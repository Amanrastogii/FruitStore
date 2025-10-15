using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models; // or your model namespace

namespace MyStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FruitApiController : ControllerBase
    {
        private readonly FruitStoreContext _context;

        public FruitApiController(FruitStoreContext context)
        {
            _context = context;
        }

        // GET: api/FruitApi
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Fruits.ToList());
        }

        // GET: api/FruitApi/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var fruit = _context.Fruits.Find(id);
            return fruit is null ? NotFound() : Ok(fruit);
        }

        // POST: api/FruitApi
        [HttpPost]
        public IActionResult Create([FromBody] Fruit fruit)
        {
            _context.Fruits.Add(fruit);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = fruit.Id }, fruit);
        }

        // PUT: api/FruitApi/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Fruit fruit)
        {
            var existing = _context.Fruits.Find(id);
            if (existing is null) return NotFound();
            existing.Name = fruit.Name;
            existing.Description = fruit.Description;
            existing.Price = fruit.Price;
            existing.Stock = fruit.Stock;
            existing.ImagePath = fruit.ImagePath;
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/FruitApi/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var fruit = _context.Fruits.Find(id);
            if (fruit is null) return NotFound();
            _context.Fruits.Remove(fruit);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
