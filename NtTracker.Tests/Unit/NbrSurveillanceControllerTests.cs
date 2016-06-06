using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NtTracker.Controllers;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class NbrSurveillanceControllerTests
    {
        //Controller services
        private Mock<INbrSurveillanceService> _nbrSurveillanceService;
        private Mock<IPatientService> _patientService;

        //Tested controller
        private NbrSurveillanceController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _nbrSurveillanceService = new Mock<INbrSurveillanceService>();
            _patientService = new Mock<IPatientService>();

            //Create controller
            _controller = new NbrSurveillanceController(_nbrSurveillanceService.Object, _patientService.Object);
        }

        [Test]
        public void GET_View()
        {
            //Arrange
            var nbrSurveillance = new NbrSurveillance { Id = 123, PatientId = 321 };
            var explorations = new List<NbrSurveillance> { nbrSurveillance };
            _nbrSurveillanceService.Setup(x => x.FindByPatientId(It.IsAny<int>())).Returns(explorations);

            //Act
            var result = (ViewResult)_controller.View(321);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(((ViewNbrSurveillanceViewModel)result.Model).PatientId, Is.EqualTo(nbrSurveillance.PatientId));
            Assert.That(((ViewNbrSurveillanceViewModel)result.Model).NbrSurveillances, Is.EqualTo(explorations));
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
            var viewModel = new EditNbrSurveillanceViewModel();
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
            var viewModel = new EditNbrSurveillanceViewModel();
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

            var viewModel = new EditNbrSurveillanceViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _nbrSurveillanceService.Setup(x => x.Create(It.IsAny<EditNbrSurveillanceViewModel>(), It.IsAny<int>()));
            _nbrSurveillanceService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Create(1, viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
        }

        [Test]
        public void GET_Edit()
        {
            //Arrange
            var nbrSurveillance = new NbrSurveillance
            {
                Id = 123,
                PatientId = 321,
                CnsExploration = new CnsExploration { Id = 1 },
                Analysis = new Analysis { Id = 1 }
            };
            _nbrSurveillanceService.Setup(x => x.FindById(It.IsAny<int>())).Returns(nbrSurveillance);

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
            _nbrSurveillanceService.Setup(x => x.FindById(It.IsAny<int>())).Returns((NbrSurveillance)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Edit(1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void POST_Edit_InvalidModel()
        {
            //Arrange
            var viewModel = new EditNbrSurveillanceViewModel();
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
            var viewModel = new EditNbrSurveillanceViewModel();
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

            var viewModel = new EditNbrSurveillanceViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _nbrSurveillanceService.Setup(x => x.Update(It.IsAny<EditNbrSurveillanceViewModel>())).Returns(false);
            _nbrSurveillanceService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

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

            var viewModel = new EditNbrSurveillanceViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _nbrSurveillanceService.Setup(x => x.Update(It.IsAny<EditNbrSurveillanceViewModel>())).Returns(true);
            _nbrSurveillanceService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Edit(1, viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
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
            _nbrSurveillanceService.Setup(x => x.Delete(It.IsAny<int>())).Returns(false);
            _nbrSurveillanceService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

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
            _nbrSurveillanceService.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);
            _nbrSurveillanceService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Delete(1, 1);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
        }
    }
}
