using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestAspNetCoreMVC.Web.Controllers;
using UnitTestAspNetCoreMVC.Web.Models;
using UnitTestAspNetCoreMVC.Web.Repository;

namespace UnitTestAspNetCoreMVC.Test
{
    public class CarApiControllerTest
    {
        private readonly Mock<IRepository<Car>> _mockRepo;
        private readonly CarsApiController _controller;
        private List<Car> _cars;

        public CarApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Car>>();
            _controller = new CarsApiController(_mockRepo.Object);

            _cars = new List<Car>() {
                new Car { Id = 1, Model = "BMW 320d", ModelYear = 2022, Color = "Blue", Price = 700000 },
                new Car { Id = 2, Model = "Mercedes CLA200", ModelYear = 2022, Color = "White", Price = 800000 }
            };
        }

        [Fact]
        public async void GetCars_ActionExecutes_ReturnOkResultWithCars()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_cars);

            var result = await _controller.GetCars();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);

            Assert.Equal<int>(2, returnCars.Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetCar_IdIsInvalid_ReturnNotFound(int id)
        {
            Car car = null;
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.GetCar(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetCar_IdValid_ReturnOkResult(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var result = await _controller.GetCar(id);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnCar = Assert.IsType<Car>(okResult.Value);

            Assert.Equal(id, returnCar.Id);
        }

        [Theory]
        [InlineData(1)]
        public void PutCar_IdIsNotEqualCarId_ReturnBadRequest(int id)
        {
            var car = _cars.First(x => x.Id == id);

            var result = _controller.PutCar(2, car);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutCar_ActionExecutes_ReturnNoContent(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.Update(car));

            var result = _controller.PutCar(id, car);

            _mockRepo.Verify(repo => repo.Update(car), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAction()
        {
            var car = _cars.First();
            _mockRepo.Setup(repo => repo.Create(car)).Returns(Task.CompletedTask);

            var result = await _controller.PostCar(car);

            var createdActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(repo => repo.Create(car), Times.Once);

            Assert.Equal("GetCar", createdActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteCar_IdIsNull_ReturnNotFound(int id)
        {
            Car car = null;
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);

            var resultNotFound = await _controller.DeleteCar(id);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteCar_ActionExecute_ReturnNoContent(int id)
        {
            var car = _cars.First(x => x.Id == id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(car);
            _mockRepo.Setup(repo => repo.Delete(car));

            var noContentResult = await _controller.DeleteCar(id);

            _mockRepo.Verify(repo => repo.Delete(car), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }
    }
}
