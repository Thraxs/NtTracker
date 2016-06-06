using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NtTracker.Controllers;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Routing;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class MonitoringControllerTests
    {
        //Controller services
        private Mock<IMonitoringService> _monitoringService;
        private Mock<INbrSurveillanceService> _nbrSurveillanceService;
        private Mock<IPatientService> _patientService;

        //Tested controller
        private MonitoringController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _monitoringService = new Mock<IMonitoringService>();
            _nbrSurveillanceService = new Mock<INbrSurveillanceService>();
            _patientService = new Mock<IPatientService>();

            //Create controller
            _controller = new MonitoringController(_monitoringService.Object, _nbrSurveillanceService.Object, _patientService.Object);
        }

        [Test]
        public void GET_List()
        {
            //Arrange
            var monitoring = new Monitoring { Id = 123, PatientId = 321 };
            var monitorings = new List<Monitoring> { monitoring };
            _monitoringService.Setup(x => x.FindByPatientId(It.IsAny<int>())).Returns(monitorings);

            //Act
            var result = (ViewResult)_controller.List(321);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(((ListMonitoringViewModel)result.Model).PatientId, Is.EqualTo(monitoring.PatientId));
            Assert.That(((ListMonitoringViewModel)result.Model).Monitorings, Is.EqualTo(monitorings));
        }

        [Test]
        public void GET_List_NullId()
        {
            //Arrange

            //Act
            var result = (HttpStatusCodeResult)_controller.List(null);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void GET_View()
        {
            //Arrange
            var monitoring = new Monitoring { Id = 123, PatientId = 321 };
            _monitoringService.Setup(x => x.FindById(It.IsAny<int>())).Returns(monitoring);

            //Act
            var result = (ViewResult)_controller.View(321);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(((MonitoringViewModel)result.Model).PatientId, Is.EqualTo(monitoring.PatientId));
        }

        [Test]
        public void GET_View_NullId()
        {
            //Arrange

            //Act
            var result = (HttpStatusCodeResult)_controller.View(null);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void GET_Create()
        {
            //Act
            var result = (ViewResult)_controller.Create(1);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Create_InvalidModel()
        {
            //Arrange
            var viewModel = new MonitoringViewModel();
            _controller.ModelState.AddModelError("Error", @"ModelError");

            //Act
            var result = (ViewResult)_controller.Create(1, viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Create_Closed()
        {
            //Arrange
            var viewModel = new MonitoringViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(true);

            //Act
            var result = (HttpStatusCodeResult)_controller.Create(1, viewModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void POST_Create_Success()
        {
            //Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            });
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            var viewModel = new MonitoringViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Create(It.IsAny<MonitoringViewModel>(), It.IsAny<int>()));
            _monitoringService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult) _controller.Create(1, viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }

        [Test]
        public void GET_Edit()
        {
            //Arrange
            var monitoring = new Monitoring
            {
                Id = 123,
                PatientId = 321
            };
            _monitoringService.Setup(x => x.FindById(It.IsAny<int>())).Returns(monitoring);

            //Act
            var result = (ViewResult)_controller.Edit(1);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void GET_Edit_NullId()
        {
            //Arrange

            //Act
            var result = (HttpStatusCodeResult)_controller.Edit(null);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void GET_Edit_NotFound()
        {
            //Arrange
            _monitoringService.Setup(x => x.FindById(It.IsAny<int>())).Returns((Monitoring)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Edit(1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void POST_Edit_InvalidModel()
        {
            //Arrange
            var viewModel = new MonitoringViewModel();
            _controller.ModelState.AddModelError("Error", @"ModelError");

            //Act
            var result = (ViewResult)_controller.Edit(1, viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Edit_Closed()
        {
            //Arrange
            var viewModel = new MonitoringViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(true);

            //Act
            var result = (HttpStatusCodeResult)_controller.Edit(1, viewModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void POST_Edit_WrongId()
        {
            //Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            });
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            var viewModel = new MonitoringViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Update(It.IsAny<MonitoringViewModel>())).Returns(false);
            _monitoringService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (HttpStatusCodeResult)_controller.Edit(1, viewModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void POST_Edit_Success()
        {
            //Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            });
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            var viewModel = new MonitoringViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Update(It.IsAny<MonitoringViewModel>())).Returns(true);
            _monitoringService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Edit(1, viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }

        [Test]
        public void POST_Delete_Closed()
        {
            //Arrange
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(true);

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(1, 1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void POST_Delete_WrongId()
        {
            //Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            });
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Delete(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(1, 1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void POST_Delete_Success()
        {
            //Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            });
            var principal = new ClaimsPrincipal(identity);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User).Returns(principal);
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _monitoringService.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);
            _monitoringService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Delete(1, 1);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }
    }
}
