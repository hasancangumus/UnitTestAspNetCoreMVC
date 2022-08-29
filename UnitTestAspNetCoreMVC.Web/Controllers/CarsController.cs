using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnitTestAspNetCoreMVC.Web.Models;
using UnitTestAspNetCoreMVC.Web.Repository;

namespace UnitTestAspNetCoreMVC.Web.Controllers
{
    public class CarsController : Controller
    {
        private readonly IRepository<Car> _repository;

        public CarsController(IRepository<Car> repository)
        {
            _repository = repository;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            return _repository != null ?
                        View(await _repository.GetAll()) :
                        Problem("Entity set 'UnitTestAspNetCoreContext.Cars'  is null.");
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var car = await _repository.GetById((int)id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Model,Price,ModelYear,Color")] Car car)
        {
            if (ModelState.IsValid)
            {
                await _repository.Create(car);
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var car = await _repository.GetById((int)id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Model,Price,ModelYear,Color")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(car);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var car = await _repository.GetById((int)id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_repository.GetAll() == null)
            {
                return Problem("Entity set 'UnitTestAspNetCoreContext.Cars'  is null.");
            }
            var car = await _repository.GetById((int)id);
            if (car != null)
            {
                _repository.Delete(car);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            var car = _repository.GetById((int)id).Result;
            if (car == null)
            {
                return false;
            }
            return true;
        }
    }
}
