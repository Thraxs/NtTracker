using System.Web.Mvc;
using Moq;
using NtTracker.Controllers;
using NtTracker.Services;
using NUnit.Framework;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class HomeControllerTests
    {
        //Controller services
        private Mock<IService> _service;

        //Tested controller
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _service = new Mock<IService>();

            //Create controller
            _controller = new HomeController(_service.Object);
        }

        [Test]
        public void GET_Index()
        {
            //Act
            var result = (ViewResult)_controller.Index();

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }
    }
}
