using System;
using System.Diagnostics.CodeAnalysis;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    class UserAccountTests
    {
        //Database context
        private DataContext _dataContext;

        //Tested service
        private IUserAccountService _userAccountService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize service
            _userAccountService = new UserAccountService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _userAccountService.Dispose();
        }

        [Test]
        public void FindById()
        {
            //Arrange
            var userId = _userAccountService.FindByUserName("admin").Id;

            //Act
            var user = _userAccountService.FindById(userId);

            //Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Id, Is.EqualTo(userId));
        }

        [Test]
        public void FindById_NotFound()
        {
            //Arrange
            const int userId = -123;

            //Act
            var user = _userAccountService.FindById(userId);

            //Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void FindByUserName()
        {
            //Arrange
            const string userName = "admin";

            //Act
            var user = _userAccountService.FindByUserName(userName);

            //Assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void FindByUserName_NotFound()
        {
            //Arrange
            const string userName = "randomAccount123";

            //Act
            var user = _userAccountService.FindByUserName(userName);

            //Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void Search_ById()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            string id = expected.Id.ToString();
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status, 
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ById_WrongIdFormat()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            string id = "#asd" + expected.Id;
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ById_NoResults()
        {
            //Arrange
            const string id = "123";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByUserName()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = expected.UserName;
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByUserName_NoResults()
        {
            //Arrange
            const string id = "";
            const string userName = "asd";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByIdAndUserName()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            string id = expected.Id.ToString();
            string userName = expected.UserName;
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByIdAndUserName_NoResults()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            string id = expected.Id.ToString();
            const string userName = "asd";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByUserAccountType()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = UserAccountType.Admin;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByUserAccountType_NoResults()
        {
            //Arrange
            const string id = "";
            string userName = "";
            UserAccountType? type = UserAccountType.Normal;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByUserAccountStatus()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = UserAccountStatus.Normal;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByUserAccountStatus_NoResults()
        {
            //Arrange
            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = UserAccountStatus.Locked;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByRegistrationDateFrom()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            string registeredFrom = expected.RegistrationDate.AddHours(-1).ToString("g");
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByRegistrationDateFrom_NoResults()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            string registeredFrom = expected.RegistrationDate.AddHours(+1).ToString("g");
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByRegistrationDateFrom_WrongDateFormat()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            string registeredFrom = "asd" + expected.RegistrationDate.AddHours(+1).ToString("g");
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByRegistrationDateTo()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            string registeredTo = expected.RegistrationDate.AddHours(+1).ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByRegistrationDateTo_NoResults()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            string registeredTo = expected.RegistrationDate.AddHours(-1).ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByRegistrationDateTo_WrongDateFormat()
        {
            //Arrange
            var expected = _userAccountService.FindByUserName("admin");

            const string id = "";
            string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            string registeredTo = "asd" + expected.RegistrationDate.AddHours(-1).ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_SortDefault()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortById()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "id_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user1));
            Assert.That(result[1], Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortById_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "id_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortByUserName()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "userName_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user1));
            Assert.That(result[1], Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortByUserName_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "userName_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortByRegistrationDate()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "registration_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user1));
            Assert.That(result[1], Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortByRegistrationDate_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "registration_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortByAccountType()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "isAdmin_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortByAccountType_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "isAdmin_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user1));
            Assert.That(result[1], Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortByAccountStatus()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });
            _userAccountService.ChangeLock(user2, true);

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "isLocked_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user1));
            Assert.That(result[1], Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortByAccountStatus_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });
            _userAccountService.ChangeLock(user2, true);

            const string id = "";
            const string userName = "";
            UserAccountType? type = null;
            UserAccountStatus? status = null;
            const string registeredFrom = "";
            const string registeredTo = "";
            const string sorting = "isLocked_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _userAccountService.Search(id, userName, type, status,
                registeredFrom, registeredTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0], Is.EqualTo(user2));
            Assert.That(result[1], Is.EqualTo(user1));
        }

        [Test]
        public void IsRegistered_True()
        {
            //Arrange
            const string existingAccount = "admin";

            //Act
            var isRegistered = _userAccountService.IsRegistered(existingAccount);

            //Assert
            Assert.That(isRegistered, Is.True);
        }

        [Test]
        public void IsRegistered_False()
        {
            //Arrange
            const string nonexistingAccount = "randomAccount123";

            //Act
            var isRegistered = _userAccountService.IsRegistered(nonexistingAccount);

            //Assert
            Assert.That(isRegistered, Is.False);
        }

        [Test, Property("Service", "External")]
        public void ValidateCredentials_Correct()
        {
            //Arrange
            const string validUser = "admin";
            const string validPwd = "adminpwd";

            //Act
            var valid = _userAccountService.ValidateCredentials(validUser, validPwd);

            //Assert
            Assert.That(valid, Is.True);
        }

        [Test, Property("Service", "External")]
        public void ValidateCredentials_Incorrect()
        {
            //Arrange
            const string validUser = "admin";
            const string wrongPassword = "wrongpassword";

            //Act
            var valid = _userAccountService.ValidateCredentials(validUser, wrongPassword);

            //Assert
            Assert.That(valid, Is.False);
        }

        [Test]
        public void Create()
        {
            //Arrange
            var accountVm = new UserAccountViewModel
            {
                UserName = "TestUser123",
                Password = "TestPassword123"
            };

            //Act
            _userAccountService.Create(accountVm);
            var account = _userAccountService.FindByUserName("TestUser123");

            //Assert
            Assert.That(account, Is.Not.Null);
            Assert.That(account.Id, Is.Not.Null);
            Assert.That(account.RegistrationDate, Is.Not.Null);
        }

        [Test]
        public void LoginSuccessful()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            const string requestInfo = "localhost - test user agent";

            //Act
            account = _userAccountService.LoginSuccessful(account, requestInfo);

            //Assert
            Assert.That(account.LastLogin, Is.Not.Null);
            Assert.That(account.LastLoginInfo, Is.Not.Null);
        }

        [Test]
        public void LoginSuccessful_ResetAttempts()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            const string requestInfo = "localhost - test user agent";
            _userAccountService.LoginFailed(account);

            //Act
            account = _userAccountService.LoginSuccessful(account, requestInfo);

            //Assert
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(0));
        }

        [Test]
        public void LoginFailed()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");

            //Act
            account = _userAccountService.LoginFailed(account);

            //Assert
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(1));
        }

        [Test]
        public void LoginFailed_LockAccount1h()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.FailedLoginAttempts = 2;

            //Act
            account = _userAccountService.LoginFailed(account);

            //Assert
            Assert.That(account.IsLocked);
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(3));
            Assert.That(account.UnlockDate, Is.InRange(DateTime.Now, DateTime.Now.AddHours(2)));
        }

        [Test]
        public void LoginFailed_LockAccount24h()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.FailedLoginAttempts = 4;

            //Act
            account = _userAccountService.LoginFailed(account);

            //Assert
            Assert.That(account.IsLocked);
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(5));
            Assert.That(account.UnlockDate, Is.InRange(DateTime.Now, DateTime.Now.AddHours(25)));
        }

        [Test]
        public void LoginFailed_LockAccount7d()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.FailedLoginAttempts = 6;

            //Act
            account = _userAccountService.LoginFailed(account);

            //Assert
            Assert.That(account.IsLocked);
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(7));
            Assert.That(account.UnlockDate, Is.InRange(DateTime.Now, DateTime.Now.AddDays(8)));
        }

        [Test]
        public void LoginFailed_LockAccountIndefinitely()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.FailedLoginAttempts = 9;

            //Act
            account = _userAccountService.LoginFailed(account);

            //Assert
            Assert.That(account.IsLocked);
            Assert.That(account.FailedLoginAttempts, Is.EqualTo(10));
            Assert.That(account.UnlockDate, Is.Null);
        }

        [Test]
        public void CheckLock_NotLocked()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");

            //Act
            var isLocked = _userAccountService.CheckLock(account);

            //Assert
            Assert.That(isLocked, Is.False);
        }

        [Test]
        public void CheckLock_LockedIndefinitely()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.IsLocked = true;
            account.UnlockDate = null;

            //Act
            var isLocked = _userAccountService.CheckLock(account);

            //Assert
            Assert.That(isLocked);
        }

        [Test]
        public void CheckLock_UnlockInFuture()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.IsLocked = true;
            account.UnlockDate = DateTime.Now.AddDays(1);

            //Act
            var isLocked = _userAccountService.CheckLock(account);

            //Assert
            Assert.That(isLocked);
        }

        [Test]
        public void CheckLock_AutoUnlock()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            account.IsLocked = true;
            account.UnlockDate = DateTime.Now.AddHours(-1);

            //Act
            var isLocked = _userAccountService.CheckLock(account);
            account = _userAccountService.FindByUserName("admin");

            //Assert
            Assert.That(isLocked, Is.False);
            Assert.That(account.UnlockDate, Is.Null);
        }

        [Test]
        public void ChangeLock_Lock()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");

            //Act
            _userAccountService.ChangeLock(account, true);
            account = _userAccountService.FindByUserName("admin");

            //Assert
            Assert.That(account.IsLocked, Is.True);
            Assert.That(account.UnlockDate, Is.Null);
            Assert.That(account.FailedLoginAttempts, Is.Zero);
        }

        [Test]
        public void ChangeLock_Unlock()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            _userAccountService.ChangeLock(account, true);

            //Act
            _userAccountService.ChangeLock(account, false);
            account = _userAccountService.FindByUserName("admin");

            //Assert
            Assert.That(account.IsLocked, Is.False);
            Assert.That(account.UnlockDate, Is.Null);
            Assert.That(account.FailedLoginAttempts, Is.Zero);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var account = _userAccountService.FindByUserName("admin");
            var accountId = account.Id;

            //Act
            var deleted = _userAccountService.Delete(accountId);
            account = _userAccountService.FindById(accountId);

            //Assert
            Assert.That(account, Is.Null);
            Assert.That(deleted, Is.True);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange

            //Act
            var deleted = _userAccountService.Delete(123654);

            //Assert
            Assert.That(deleted, Is.False);
        }
    }
}
