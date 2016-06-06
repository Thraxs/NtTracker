using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin.Security;
using Moq;
using NtTracker.Controllers;
using NtTracker.Filters;
using NtTracker.Models;
using NtTracker.Resources.Shared;
using NtTracker.Resources.UserAccount;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;
using PagedList;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class UserAccountControllerTest
    {
        //Controller services
        private Mock<IAuthenticationManager> _authenticationManager;
        private Mock<IUserAccountService> _userAccountService;

        //Tested controller
        private UserAccountController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _authenticationManager = new Mock<IAuthenticationManager>();
            _userAccountService = new Mock<IUserAccountService>();

            //Create controller
            _controller = new UserAccountController(_authenticationManager.Object, _userAccountService.Object);
        }

        [Test]
        public void GET_Register_Anonymous()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(false);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            //Act
            var result = (ViewResult)_controller.Register();

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void GET_Register_Authenticated()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            //Act
            var result = (RedirectToRouteResult)_controller.Register();

            //Assert
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void POST_Register_InvalidModel()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "",
                Password = "12345"
            };
            _controller.ModelState.AddModelError("UserName", @"Username required");

            //Act
            var result = (ViewResult)_controller.Register(viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Register_ThrottleRequest()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "",
                Password = ""
            };
            _controller.ModelState.AddModelError("UserName", @"Username required");

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());
            request.SetupGet(x => x.UserHostAddress).Returns("localhost");
            request.SetupGet(x => x.UserAgent).Returns("test user agent");
            request.SetupGet(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Cache).Returns(HttpRuntime.Cache);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            var filterContext = new Mock<ActionExecutingContext>();
            filterContext.SetupGet(x => x.HttpContext).Returns(context.Object);
            filterContext.SetupGet(x => x.RouteData).Returns(routeData);
            filterContext.SetupGet(x => x.Controller).Returns(_controller);

            var filter = new ThrottleRequest { ErrorResourceKey = "RegistrationThrottleError" };

            //Act
            filter.OnActionExecuting(filterContext.Object);
            _controller.Register(viewModel);
            filter.OnActionExecuting(filterContext.Object);
            var result = (ViewResult)_controller.Register(viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(SharedStrings.RegistrationThrottleError));
        }

        [Test]
        public void POST_Register_AlreadyRegistered()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);

            //Act
            var result = (ViewResult)_controller.Register(viewModel);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(Strings.UserAlreadyExists));
        }

        [Test]
        public void POST_Register_AuthenticationServerError()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns((bool?)null);

            //Act
            var result = (HttpStatusCodeResult)_controller.Register(viewModel);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public void POST_Register_InvalidCredentials()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            //Act
            _controller.Register(viewModel);

            //Assert
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(Strings.LoginError));
        }

        [Test]
        public void POST_Register_Success()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());
            request.SetupGet(x => x.UserHostAddress).Returns("localhost");
            request.SetupGet(x => x.UserAgent).Returns("test user agent");
            request.SetupGet(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);
            _controller.Url = new UrlHelper(new RequestContext(_controller.HttpContext, routeData));

            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.Create(It.IsAny<UserAccountViewModel>()))
                .Returns(new UserAccount { Id = 123, UserName = "TestUser" });

            //Act
            var result = (RedirectToRouteResult)_controller.Register(viewModel);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["registered"], Is.EqualTo(1));
        }

        [Test]
        public void GET_Login_Anonymous()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(false);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            //Act
            var result = (ViewResult)_controller.Login("", null);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void GET_Login_Authenticated()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);

            //Act
            var result = (RedirectToRouteResult)_controller.Login("", null);

            //Assert
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void POST_Login_InvalidModel()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "",
                Password = "12345"
            };
            _controller.ModelState.AddModelError("UserName", @"Username required");

            //Act
            var result = (ViewResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }

        [Test]
        public void POST_Login_NotRegistered()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(false);

            //Act
            var result = (ViewResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(Strings.LoginError));
        }

        [Test]
        public void POST_Login_AuthenticationServerError()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = false,
                UnlockDate = null
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns((bool?)null);

            //Act
            var result = (HttpStatusCodeResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public void POST_Login_AccountLockedIndefinitely()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = true,
                UnlockDate = null
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(true);
            _userAccountService.Setup(x => x.LoginFailed(It.IsAny<UserAccount>())).Returns(account);

            //Act
            var result = (ViewResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(Strings.LoginLockedPerm));
        }

        [Test]
        public void POST_Login_AccountLockedTemporarily()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = true,
                UnlockDate = DateTime.Today.AddDays(1),
                FailedLoginAttempts = 4
            };
            var lockMessage = Strings.LoginLocked + account.UnlockDate + Strings.LoginLocked2;
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(true);
            _userAccountService.Setup(x => x.LoginFailed(It.IsAny<UserAccount>())).Returns(account);

            //Act
            var result = (ViewResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(lockMessage));
        }

        [Test]
        public void POST_Login_InvalidPassword()
        {
            //Arrange
            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "aaa"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = false,
                UnlockDate = null,
                FailedLoginAttempts = 0
            };
            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _userAccountService.Setup(x => x.LoginFailed(It.IsAny<UserAccount>())).Returns(account);

            //Act
            var result = (ViewResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(_controller.ModelState[string.Empty].Errors[0].ErrorMessage, Is.EqualTo(Strings.LoginError));
        }

        [Test]
        public void POST_Login_Success()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());
            request.SetupGet(x => x.UserHostAddress).Returns("localhost");
            request.SetupGet(x => x.UserAgent).Returns("test user agent");
            request.SetupGet(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);
            _controller.Url = new UrlHelper(new RequestContext(_controller.HttpContext, routeData));

            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = false,
                UnlockDate = null,
                FailedLoginAttempts = 0
            };

            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.LoginSuccessful(It.IsAny<UserAccount>(), It.IsAny<string>())).Returns(account);

            //Act
            var result = (RedirectToRouteResult)_controller.Login(viewModel, "");

            //Assert
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void POST_Login_SuccessAndRedirect()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());
            request.SetupGet(x => x.UserHostAddress).Returns("localhost");
            request.SetupGet(x => x.UserAgent).Returns("test user agent");
            request.SetupGet(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var routeData = new RouteData();
            routeData.Values.Add("culture", "en");
            routeData.Values.Add("controller", "UserAccount");
            routeData.Values.Add("action", "Register");

            _controller.ControllerContext = new ControllerContext(context.Object, routeData, _controller);
            _controller.Url = new UrlHelper(new RequestContext(_controller.HttpContext, routeData));

            var viewModel = new UserAccountViewModel
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsLocked = false,
                UnlockDate = null,
                FailedLoginAttempts = 0
            };

            _userAccountService.Setup(x => x.IsRegistered(It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(account);
            _userAccountService.Setup(x => x.CheckLock(It.IsAny<UserAccount>())).Returns(false);
            _userAccountService.Setup(x => x.ValidateCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _userAccountService.Setup(x => x.LoginSuccessful(It.IsAny<UserAccount>(), It.IsAny<string>())).Returns(account);

            //Act
            var result = (RedirectResult)_controller.Login(viewModel, "/en/Patient/List");

            //Assert
            Assert.That(result.Url, Is.EqualTo("/en/Patient/List"));
        }

        [Test]
        public void POST_LogOff()
        {
            //Arrange

            //Act
            var result = (RedirectToRouteResult)_controller.LogOff();

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
        }

        [Test]
        public void GET_List()
        {
            //Arrange
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsAdmin = false,
                IsLocked = false,
                RegistrationDate = DateTime.Today
            };
            var searchResult = new List<UserAccount> { account };
            _userAccountService.Setup(
                x =>
                    x.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserAccountType>(),
                        It.IsAny<UserAccountStatus>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(searchResult.ToPagedList(1, 10));

            //Act
            var result = (ViewResult)_controller.List("123", "TestUser", UserAccountType.Normal, UserAccountStatus.Normal,
                DateTime.Today.AddDays(-1).ToShortDateString(), DateTime.Today.AddDays(1).ToShortDateString(), "id_a", null);

            //Assert
            Assert.That(((IPagedList<UserAccount>)result.Model)[0], Is.EqualTo(account));
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
        public void GET_View_NullUser()
        {
            //Arrange
            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns((UserAccount)null);

            //Act
            var result = (HttpNotFoundResult)_controller.View(1);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void GET_View_Success()
        {
            //Arrange
            var account = new UserAccount
            {
                Id = 123,
                UserName = "TestUser",
                IsAdmin = false,
                IsLocked = false,
                RegistrationDate = DateTime.Today
            };
            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns(account);

            //Act
            var result = (ViewResult)_controller.View(1);

            //Assert
            Assert.That(result.ViewName, Is.Empty);
            Assert.That(((UserAccountViewModel)result.Model).Id, Is.EqualTo(account.Id));
        }

        [Test]
        public void POST_Lock_OwnAccount()
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

            //Act
            var result = (HttpStatusCodeResult)_controller.Lock(123);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public void POST_Lock_NullAccount()
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

            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns((UserAccount)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Lock(321);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void POST_Lock_Success()
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

            var account = new UserAccount
            {
                Id = 321,
                UserName = "TestUser",
                IsAdmin = false,
                IsLocked = false,
                RegistrationDate = DateTime.Today
            };

            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns(account);
            _userAccountService.Setup(x => x.ChangeLock(It.IsAny<UserAccount>(), It.IsAny<bool>()))
                .Callback((UserAccount a, bool l) => a.IsLocked = l);

            //Act
            var result = (RedirectToRouteResult)_controller.Lock(321);

            //Assert
            Assert.That(account.IsLocked);
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(account.Id));
        }

        [Test]
        public void POST_Unlock_OwnAccount()
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

            //Act
            var result = (HttpStatusCodeResult)_controller.Unlock(123);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public void POST_Unlock_NullAccount()
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

            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns((UserAccount)null);

            //Act
            var result = (HttpNotFoundResult)_controller.Unlock(321);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void POST_Unlock_Success()
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

            var account = new UserAccount
            {
                Id = 321,
                UserName = "TestUser",
                IsAdmin = false,
                IsLocked = true,
                RegistrationDate = DateTime.Today
            };

            _userAccountService.Setup(x => x.FindById(It.IsAny<int>())).Returns(account);
            _userAccountService.Setup(x => x.ChangeLock(It.IsAny<UserAccount>(), It.IsAny<bool>()))
                .Callback((UserAccount a, bool l) => a.IsLocked = l);

            //Act
            var result = (RedirectToRouteResult)_controller.Unlock(321);

            //Assert
            Assert.That(account.IsLocked, Is.False);
            Assert.That(result.RouteValues["action"], Is.EqualTo("View"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(account.Id));
        }

        [Test]
        public void POST_Delete_OwnAccount()
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

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(123);

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public void POST_Delete_WrongAccountId()
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

            _userAccountService.Setup(x => x.Delete(It.IsAny<int>())).Returns(false);

            //Act
            var result = (HttpStatusCodeResult)_controller.Delete(321);

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

            _userAccountService.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            //Act
            var result = (RedirectToRouteResult)_controller.Delete(321);

            //Assert
            Assert.That(result.RouteValues["action"], Is.EqualTo("List"));
        }
    }
}
