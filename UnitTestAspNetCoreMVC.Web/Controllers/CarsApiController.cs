using Microsoft.AspNetCore.Mvc;
using UnitTestAspNetCoreMVC.Web.Models;
using UnitTestAspNetCoreMVC.Web.Repository;

namespace UnitTestAspNetCoreMVC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsApiController : ControllerBase
    {
        private readonly IRepository<Car> _repository;

        public CarsApiController(IRepository<Car> repository)
        {
            _repository = repository;
        }

        // GET: api/CarsApi
        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _repository.GetAll();
            if (cars == null)
            {
                return NotFound();
            }
            return Ok(cars);
        }

        // GET: api/CarsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = _repository.GetById(id);
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        // PUT: api/CarsApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            _repository.Update(car);

            return NoContent();
        }

        // POST: api/CarsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostCar(Car car)
        {
            await _repository.Create(car);

            return CreatedAtAction("GetCar", new { id = car.Id }, car);
        }

        // DELETE: api/CarsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _repository.GetById(id);
            if (car == null)
            {
                return NotFound();
            }

            _repository.Delete(car);

            return NoContent();
        }

        private bool CarExists(int id)
        {
            Car car = _repository.GetById(id).Result;
            if (car == null)
            {
                return false;
            }
            return true;
        }
    }
}
