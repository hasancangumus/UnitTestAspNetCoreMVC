using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestAspNetCoreMVC.Web.Controllers;
using UnitTestAspNetCoreMVC.Web.Models;
using UnitTestAspNetCoreMVC.Web.Repository;

namespace UnitTestAspNetCoreMVC.Test
{
    public class CarsControllerTest
    {
        private readonly Mock<IRepository<Car>> _mockRepo;

        private readonly CarsController _controller;

        private List<Car> _cars;

        public CarsControllerTest()
        {
            _mockRepo = new Mock<IRepository<Car>>();
            //This is default value.
            //You do not have to Create mock setup for every method. 
            //_mockRepo = new Mock<IRepository<Car>>(MockBehavior.Loose);

            //You have to Create mock setup for every method.
            //_mockRepo = new Mock<IRepository<Car>>(MockBehavior.Strict);

            _controller = new CarsController(_mockRepo.Object);
            _cars = new List<Car>() {
                new Car { Id = 1, Model = "BMW 320d", ModelYear = 2022, Color = "Blue", Price = 700000 },
                new Car { Id = 2, Model = "Mercedes CLA200", ModelYear = 2022, Color = "White", Price = 800000 }
            };
        }

        [Fact]
        public void Index_ActionExecutes_ReturnView()
        {
            var result = _controller.Index();

            Assert.IsType<ViewResult>(result.Result);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_cars);

            var result = _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result.Result);

            var carList = Assert.IsAssignableFrom<IEnumerable<Car>>(viewResult.Model);

            Assert.Equal<int>(2, carList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdIsInvalid_ReturnNotFound()
        {
            Car car = null;
            _mockRepo.Setup(repo => repo.GetById(0)).ReturnsAsync(car);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_IdIsValid_ReturnCar(int id)
        {
            Car car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.Details(id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultCar = Assert.IsAssignableFrom<Car>(viewResult.Model);

            Assert.Equal(car.Id, resultCar.Id);
            Assert.Equal(car.Model, resultCar.Model);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Model", "Model field is required");

            var reuslt = await _controller.Create(_cars.First());

            var viewResult = Assert.IsType<ViewResult>(reuslt);

            Assert.IsType<Car>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(
                new Car { Id = 3, Model = "Audi A7", ModelYear = 2023, Color = "Black", Price = 900000 });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Car newCar = null;
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Car>())).Callback<Car>(x => newCar = x);

            var result = await _controller.Create(_cars.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Car>()), Times.Once);

            Assert.Equal(_cars.First().Id, newCar.Id);
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Model", "Model field is required");

            var result = await _controller.Create(_cars.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Car>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void Edit_IdIsInvalid_ReturnNotFound(int id)
        {
            Car car = null;
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.Edit(id);

            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnCar(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.Edit(id);
            var viewResult = Assert.IsType<ViewResult>(result);

            var resultCar = Assert.IsAssignableFrom<Car>(viewResult.Model);

            Assert.Equal(car.Id, resultCar.Id);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqual_ReturnNotFound(int id)
        {
            var result = _controller.Edit(2, _cars.First(x => x.Id == id));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InvalidModelState_ReturnView(int id)
        {
            _controller.ModelState.AddModelError("Model", "Model field is required");

            var result = _controller.Edit(id, _cars.First(x => x.Id == id));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Car>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int id)
        {
            var result = _controller.Edit(id, _cars.First(x => x.Id == id));

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.Update(car));
            _controller.Edit(id, car);

            _mockRepo.Verify(repo => repo.Update(It.IsAny<Car>()), Times.Once);
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqual_ReturnNotFound(int id)
        {
            Car car = null;
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnCar(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.Delete(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Car>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int id)
        {
            var result = await _controller.DeleteConfirmed(id);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecutes(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);
            _mockRepo.Setup(repo => repo.Delete(car));

            await _controller.DeleteConfirmed(id);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Car>()), Times.Once);
        }


    }
}
