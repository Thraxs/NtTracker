using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NtTracker.Controllers;
using NtTracker.Models;
using NtTracker.Resources.Patient;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;
using PagedList;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class PatientControllerTests
    {
        //Controller services
        private Mock<IPatientService> _patientService;

        //Tested controller
        private PatientController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _patientService = new Mock<IPatientService>();

            //Create controller
            _controller = new PatientController(_patientService.Object);
        }

        [Test]
        public void GET_List()
        {
            //Arrange
            var patient = new Patient
            {
                Id = 123
            };
            var searchResult = new List<Patient> { patient };
            _patientService.Setup(
                x =>
                    x.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<PatientStatus>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(searchResult.ToPagedList(1, 10));

            //Act
            var result = (ViewResult)_controller.List("123", "123", "123", "name", "surnames", PatientStatus.Closed,
                DateTime.Today.AddDays(-1).ToShortDateString(), DateTime.Today.AddDays(1).ToShortDateString(), "id_a", null);

            //Assert
            Assert.That(((IPagedList<Patient>)result.Model)[0], Is.EqualTo(patient));
        }

        [Test]
        public void GET_Export()
        {
            //Arrange
            var patient = new Patient
            {
                Id = 123,
                NbrSurveillances = new List<NbrSurveillance>(),
                Hypothermias = new List<Hypothermia>(),
                Monitorings = new List<Monitoring>()
            };
            var searchResult = new List<Patient> { patient };
            _patientService.Setup(
                x =>
                    x.DeepSearch(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<PatientStatus>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(searchResult.ToPagedList(1, 10));

            //Act
            var result = _controller.Export("123", "123", "123", "name", "surnames", PatientStatus.Closed,
                DateTime.Today.AddDays(-1).ToShortDateString(), DateTime.Today.AddDays(1).ToShortDateString(), "id_a");

            //Assert
            Assert.That(result.FileStream, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.That(result.FileDownloadName, Is.EqualTo(Strings.ExportedFileName + ".xlsx"));
        }

        [Test]
        public void GET_View()
        {
            //Arrange
            var patient = new Patient { Id = 123 };
            _patientService.Setup(x => x.FindById(It.IsAny<int>())).Returns(patient);

            //Act
            var result = (ViewResult)_controller.View(321);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(((PatientViewModel)result.Model).Id, Is.EqualTo(patient.Id));
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
        public void GET_View_NotFound()
        {
            //Arrange
            _patientService.Setup(x => x.FindById(It.IsAny<int>())).Returns((Patient)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Edit(1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void GET_Create()
        {
            //Act
            var result = (ViewResult)_controller.Create();

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Create_InvalidModel()
        {
            //Arrange
            var viewModel = new PatientViewModel();
            _controller.ModelState.AddModelError("Error", @"ModelError");

            //Act
            var result = (ViewResult)_controller.Create(viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
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

            var viewModel = new PatientViewModel();
            _patientService.Setup(x => x.Create(It.IsAny<PatientViewModel>(), It.IsAny<string>(), It.IsAny<int>()));
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Create(viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }

        [Test]
        public void GET_Edit()
        {
            //Arrange
            var patient = new Patient
            {
                Id = 123
            };
            _patientService.Setup(x => x.FindById(It.IsAny<int>())).Returns(patient);

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
            var result = (HttpStatusCodeResult)_controller.Edit((int?) null);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void GET_Edit_NotFound()
        {
            //Arrange
            _patientService.Setup(x => x.FindById(It.IsAny<int>())).Returns((Patient)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Edit(1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void POST_Edit_InvalidModel()
        {
            //Arrange
            var viewModel = new PatientViewModel();
            _controller.ModelState.AddModelError("Error", @"ModelError");

            //Act
            var result = (ViewResult)_controller.Edit(viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Edit_Closed()
        {
            //Arrange
            var viewModel = new PatientViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(true);

            //Act
            var result = (HttpStatusCodeResult)_controller.Edit(viewModel);

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

            var viewModel = new PatientViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _patientService.Setup(x => x.Update(It.IsAny<PatientViewModel>())).Returns(false);
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (HttpStatusCodeResult)_controller.Edit(viewModel);

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

            var viewModel = new PatientViewModel();
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(false);
            _patientService.Setup(x => x.Update(It.IsAny<PatientViewModel>())).Returns(true);
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Edit(viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
        }

        [Test]
        public void POST_Delete_Closed()
        {
            //Arrange
            _patientService.Setup(x => x.IsClosed(It.IsAny<int>())).Returns(true);

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(1);

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
            _patientService.Setup(x => x.Delete(It.IsAny<int>())).Returns(false);
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(1);

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
            _patientService.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Delete(1);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }

        [Test]
        public void POST_Close()
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

            _patientService.Setup(x => x.ClosePatientTracking(It.IsAny<int>()));
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Close(1);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
        }

        [Test]
        public void POST_Open()
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

            _patientService.Setup(x => x.OpenPatientTracking(It.IsAny<int>()));
            _patientService.Setup(x => x.Log(It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            //Act
            var result = (RedirectToRouteResult)_controller.Close(1);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
        }
    }
}
