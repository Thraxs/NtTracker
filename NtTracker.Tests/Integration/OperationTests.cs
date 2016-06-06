using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    class OperationTests
    {
        //Database context
        private DataContext _dataContext;

        //Support services
        private IUserAccountService _userAccountService;
        private ICnsExplorationService _cnsExplorationService;
        private IAnalysisService _analysisService;
        private INbrSurveillanceService _nbrSurveillanceService;
        private IHypothermiaService _hypothermiaService;
        private IPatientService _patientService;

        //Tested service
        private IOperationService _operationService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize support services
            _userAccountService = new UserAccountService(_dataContext);
            _cnsExplorationService = new CnsExplorationService(_dataContext);
            _analysisService = new AnalysisService(_dataContext);
            _nbrSurveillanceService = new NbrSurveillanceService(_dataContext, _cnsExplorationService, _analysisService);
            _hypothermiaService = new HypothermiaService(_dataContext, _cnsExplorationService, _analysisService);
            _patientService = new PatientService(_dataContext, _nbrSurveillanceService, _hypothermiaService);

            //Initialize service
            _operationService = new OperationService(_dataContext, _userAccountService);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _operationService.Dispose();

            //Dispose support services
            _userAccountService.Dispose();
            _cnsExplorationService.Dispose();
            _analysisService.Dispose();
            _nbrSurveillanceService.Dispose();
            _hypothermiaService.Dispose();
            _patientService.Dispose();
        }

        [Test]
        public void Log()
        {
            //Arrange
            const OperationType operationType = OperationType.Login;
            var user = _userAccountService.FindByUserName("admin");

            //Act
            var task = _operationService.Log(operationType, user.Id);
            task.Wait();
            var operation = _operationService.Search(user.UserName, operationType, 
                null, "", "", "", "", 1, 1).First();

            //Assert
            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.Action, Is.EqualTo(operationType));
            Assert.That(operation.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Log_WithPatientId()
        {
            //Arrange
            const OperationType operationType = OperationType.PatientUpdate;
            var user = _userAccountService.FindByUserName("admin");
            var patient = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();

            //Act
            var task = _operationService.Log(operationType, user.Id, patient.Id);
            task.Wait();
            var operation = _operationService.Search(user.UserName, operationType,
                patient.Id, "", "", "", "", 1, 1).First();

            //Assert
            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.PatientId, Is.EqualTo(patient.Id));
        }

        [Test]
        public void Log_WithData()
        {
            //Arrange
            const OperationType operationType = OperationType.PatientUpdate;
            var user = _userAccountService.FindByUserName("admin");
            var patient = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();
            string data = "SomeData: 123";

            //Act
            var task = _operationService.Log(operationType, user.Id, patient.Id, data);
            task.Wait();
            var operation = _operationService.Search(user.UserName, operationType,
                patient.Id, "", "", "", "", 1, 1).First();

            //Assert
            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.OperationData, Is.EqualTo("UserID: " + user.Id + ", PatientID: " + patient.Id + ", " + data));
        }

        [Test]
        public void Search_ByOperationUser()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            var userName = user.UserName;
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationUser_NoResults()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel {UserName = "user123"});

            var userName = user1.UserName;
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user2.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationUser_WrongUser()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "NonExistingUser";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationType()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, operationType, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationType_NoResults()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Register;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, OperationType.Login, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationPatient()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");
            var patient = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();

            const string userName = "";
            const OperationType operationType = OperationType.PatientUpdate;
            int? patientId = patient.Id;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id, patientId);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationPatient_NoResults()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");
            var patient = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();

            const string userName = "";
            const OperationType operationType = OperationType.PatientUpdate;
            int? patientId = patient.Id;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id, patientId);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, 123,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationDateFrom()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            var dateFrom = DateTime.Now.AddHours(-1).ToString("g");
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationDateFrom_NoResults()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            var dateFrom = DateTime.Now.AddHours(1).ToString("g");
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationDateFrom_WrongDateFormat()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            var dateFrom = "asd" + DateTime.Now.AddHours(5).ToString("g");
            const string dateTo = "";
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationDateTo()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            var dateTo = DateTime.Now.AddHours(1).ToString("g");
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationDateTo_NoResults()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            var dateTo = DateTime.Now.AddHours(-1).ToString("g");
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationDateTo_WrongDateFormat()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            var dateTo = "asd" + DateTime.Now.AddHours(-5).ToString("g");
            const string operationData = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void Search_ByOperationData()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "SomeData: 123";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id, data: operationData);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, operationData, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
            Assert.That(item.OperationData, Is.EqualTo("UserID: " + user.Id + ", " + operationData));
        }

        [Test]
        public void Search_ByOperationData_NoResults()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "SomeData: 123";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id, data: operationData);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, "WrongData: 321", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByOperationData_PartialMatch()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const string userName = "";
            const OperationType operationType = OperationType.Login;
            int? patientId = null;
            const string dateFrom = "";
            const string dateTo = "";
            const string operationData = "SomeData: 123";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task = _operationService.Log(operationType, user.Id, data: operationData);
            task.Wait();

            //Act
            var result = _operationService.Search(userName, null, patientId,
                dateFrom, dateTo, "123", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var item = result.First();
            Assert.That(item.Action, Is.EqualTo(operationType));
            Assert.That(item.UserId, Is.EqualTo(user.Id));
            Assert.That(item.OperationData, Is.EqualTo("UserID: " + user.Id + ", " + operationData));
        }

        [Test]
        public void Search_SortByOperationTime()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const OperationType operation1Type = OperationType.Register;
            const OperationType operation2Type = OperationType.Login;
            const string sorting = "time_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            var operation1 = new Operation
            {
                Action = operation1Type,
                TimeStamp = DateTime.Now.AddHours(-1),
                UserId = user.Id,
                OperationData = "test"
            };
            _dataContext.Operations.Add(operation1);
            _dataContext.SaveChanges();

            var task2 = _operationService.Log(operation2Type, user.Id);
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].Action, Is.EqualTo(operation1Type));
            Assert.That(result[1].Action, Is.EqualTo(operation2Type));
        }

        [Test]
        public void Search_SortByOperationTime_Desc()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const OperationType operation1Type = OperationType.Register;
            const OperationType operation2Type = OperationType.Login;
            const string sorting = "time_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            var operation1 = new Operation
            {
                Action = operation1Type,
                TimeStamp = DateTime.Now.AddHours(-1),
                UserId = user.Id,
                OperationData = "test"
            };
            _dataContext.Operations.Add(operation1);
            _dataContext.SaveChanges();

            var task2 = _operationService.Log(operation2Type, user.Id);
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].Action, Is.EqualTo(operation2Type));
            Assert.That(result[1].Action, Is.EqualTo(operation1Type));
        }

        [Test]
        public void Search_SortByOperationUser()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const OperationType operationType = OperationType.Login;
            const string sorting = "user_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operationType, user1.Id);
            var task2 = _operationService.Log(operationType, user2.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].User, Is.EqualTo(user1));
            Assert.That(result[1].User, Is.EqualTo(user2));
        }

        [Test]
        public void Search_SortByOperationUser_Desc()
        {
            //Arrange
            var user1 = _userAccountService.FindByUserName("admin");
            var user2 = _userAccountService.Create(new UserAccountViewModel { UserName = "user2" });

            const OperationType operationType = OperationType.Login;
            const string sorting = "user_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operationType, user1.Id);
            var task2 = _operationService.Log(operationType, user2.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].User, Is.EqualTo(user2));
            Assert.That(result[1].User, Is.EqualTo(user1));
        }

        [Test]
        public void Search_SortByOperationType()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const OperationType operation1Type = OperationType.PatientDelete;
            const OperationType operation2Type = OperationType.PatientCreate;
            const string sorting = "operation_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operation1Type, user.Id);
            var task2 = _operationService.Log(operation2Type, user.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].Action, Is.EqualTo(operation2Type));
            Assert.That(result[1].Action, Is.EqualTo(operation1Type));
        }

        [Test]
        public void Search_SortByOperationType_Desc()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");

            const OperationType operation1Type = OperationType.PatientDelete;
            const OperationType operation2Type = OperationType.PatientCreate;
            const string sorting = "operation_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operation1Type, user.Id);
            var task2 = _operationService.Log(operation2Type, user.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].Action, Is.EqualTo(operation1Type));
            Assert.That(result[1].Action, Is.EqualTo(operation2Type));
        }

        [Test]
        public void Search_SortByOperationPatient()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");
            var patient1 = _patientService.Search("", "", "", "", "", null, "", "", "", 2, 1).First();
            var patient2 = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();

            const OperationType operation1Type = OperationType.PatientDelete;
            const OperationType operation2Type = OperationType.PatientCreate;
            const string sorting = "patient_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operation1Type, user.Id, patientId: patient1.Id);
            var task2 = _operationService.Log(operation2Type, user.Id, patientId: patient2.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].PatientId, Is.EqualTo(patient1.Id));
            Assert.That(result[1].PatientId, Is.EqualTo(patient2.Id));
        }

        [Test]
        public void Search_SortByOperationPatient_Desc()
        {
            //Arrange
            var user = _userAccountService.FindByUserName("admin");
            var patient1 = _patientService.Search("", "", "", "", "", null, "", "", "", 2, 1).First();
            var patient2 = _patientService.Search("", "", "", "", "", null, "", "", "", 1, 1).First();

            const OperationType operation1Type = OperationType.PatientDelete;
            const OperationType operation2Type = OperationType.PatientCreate;
            const string sorting = "patient_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            var task1 = _operationService.Log(operation1Type, user.Id, patientId: patient1.Id);
            var task2 = _operationService.Log(operation2Type, user.Id, patientId: patient2.Id);
            task1.Wait();
            task2.Wait();

            //Act
            var result = _operationService.Search("", null, null, "", "", "", sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result[0].PatientId, Is.EqualTo(patient2.Id));
            Assert.That(result[1].PatientId, Is.EqualTo(patient1.Id));
        }
    }
}
