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
            _controller = new CarsController(_mockRepo.Object);
            _cars = new List<Car>() { 
                new Car { Id = 1, Model = "BMW 320d", ModelYear = 2022, Color = "Blue", Price = 700000 },
                new Car { Id = 1, Model = "Mercedes CLA200", ModelYear = 2022, Color = "White", Price = 800000 } 
            };
        }
    }
}
