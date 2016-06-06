using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NtTracker.Services;
using NUnit.Framework;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    class PatientTests
    {
        //Database context
        private DataContext _dataContext;

        //Support services
        private ICnsExplorationService _cnsExplorationService;
        private IAnalysisService _analysisService;
        private INbrSurveillanceService _nbrSurveillanceService;
        private IHypothermiaService _hypothermiaService;
        private IMonitoringService _monitoringService;

        //Tested service
        private IPatientService _patientService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize support services
            _cnsExplorationService = new CnsExplorationService(_dataContext);
            _analysisService = new AnalysisService(_dataContext);
            _nbrSurveillanceService = new NbrSurveillanceService(_dataContext, _cnsExplorationService, _analysisService);
            _hypothermiaService = new HypothermiaService(_dataContext, _cnsExplorationService, _analysisService);
            _monitoringService = new MonitoringService(_dataContext);

            //Initialize service
            _patientService = new PatientService(_dataContext, _nbrSurveillanceService, _hypothermiaService);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _patientService.Dispose();

            //Dispose support services
            _cnsExplorationService.Dispose();
            _analysisService.Dispose();
            _nbrSurveillanceService.Dispose();
            _hypothermiaService.Dispose();
            _monitoringService.Dispose();
        }

        [Test]
        public void FindById()
        {
            //Arrange
            var patientId = _dataContext.Patients.First().Id;

            //Act
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.Id, Is.EqualTo(patientId));
        }

        [Test]
        public void FindById_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(patient, Is.Null);
        }

        [Test]
        public void Search_ById()
        {
            //Arrange
            var expected = _dataContext.Patients.First();

            var id = expected.Id.ToString();
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ById_WrongIdFormat()
        {
            //Arrange
            var expected = _dataContext.Patients.First();

            string id = "#asd" + expected.Id;
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ById_NoResults()
        {
            //Arrange
            const string id = "-1";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByNhc()
        {
            //Arrange
            var expected = _dataContext.Patients.First(p => p.Nhc == "4");

            const string id = "";
            const string nhc = "4";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Contains.Item(expected));
        }

        [Test]
        public void Search_ByNhc_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "-321";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByNuhsa()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "AN0123456789";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(pageSize));
            Assert.That(result.First().Nuhsa, Is.EqualTo(nuhsa));
        }

        [Test]
        public void Search_ByNuhsa_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "-123";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByName()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "Jake";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo(name));
        }

        [Test]
        public void Search_ByName_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "James";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_BySurname()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "Smith";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result.First().Surnames, Is.EqualTo(surnames));
        }

        [Test]
        public void Search_BySurname_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "Johnson";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByStatus()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = PatientStatus.Monitoring;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().PatientStatus, Is.EqualTo(status));
        }

        [Test]
        public void Search_ByStatus_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = PatientStatus.Monitoring;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            var patient = _dataContext.Patients.AsNoTracking().First(p => p.PatientStatus == PatientStatus.Monitoring);
            _patientService.Delete(patient.Id);

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByBirthFrom()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            var birthFrom = DateTime.Now.AddHours(-1).ToString("g");
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(pageSize));
            Assert.That(result.First().BirthDate, Is.GreaterThan(DateTime.Parse(birthFrom)));
        }

        [Test]
        public void Search_ByBirthFrom_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            var birthFrom = DateTime.Now.AddHours(1).ToString("g");
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByBirthFrom_WrongDateFormat()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            var birthFrom = "asd" + DateTime.Now.AddHours(1).ToString("g");
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByBirthTo()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            var birthTo = DateTime.Parse("2015-01-25 8:25").AddHours(1).ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result.First().BirthDate, Is.LessThan(DateTime.Parse(birthTo)));
        }

        [Test]
        public void Search_ByBirthTo_NoResults()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            var birthTo = DateTime.MinValue.ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_ByBirthTo_WrongDateFormat()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            var birthTo = "asd" + DateTime.MaxValue.ToString("g");
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Search_SortDefault()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Id, Is.GreaterThan(result[1].Id));
        }

        [Test]
        public void Search_SortById()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "id_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Id, Is.LessThan(result[1].Id));
        }

        [Test]
        public void Search_SortById_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "id_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Id, Is.GreaterThan(result[1].Id));
        }

        [Test]
        public void Search_SortByNhc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "nhc_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Nhc, Is.LessThan(result[1].Nhc));
        }

        [Test]
        public void Search_SortByNhc_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "nhc_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Nhc, Is.GreaterThan(result[1].Nhc));
        }

        [Test]
        public void Search_SortByNuhsa()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "nuhsa_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Nuhsa, Is.LessThan(result.Last().Nuhsa));
        }

        [Test]
        public void Search_SortByNuhsa_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "nuhsa_d";
            const int pageNumber = 1;
            const int pageSize = 100;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Nuhsa, Is.GreaterThan(result.Last().Nuhsa));
        }

        [Test]
        public void Search_SortByName()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "name_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Name, Is.LessThan(result[1].Name));
        }

        [Test]
        public void Search_SortByName_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "name_d";
            const int pageNumber = 1;
            const int pageSize = 100;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Name, Is.GreaterThan(result.Last().Name));
        }

        [Test]
        public void Search_SortBySurname()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "surnames_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Surnames, Is.LessThan(result[1].Surnames));
        }

        [Test]
        public void Search_SortBySurname_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "surnames_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Surnames, Is.GreaterThan(result.Last().Surnames));
        }

        [Test]
        public void Search_SortByBirthDate()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "birthdate_a";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].BirthDate, Is.LessThan(result.Last().BirthDate));
        }

        [Test]
        public void Search_SortByBirthDate_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "birthdate_d";
            const int pageNumber = 1;
            const int pageSize = 100;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].BirthDate, Is.GreaterThan(result.Last().BirthDate));
        }

        [Test]
        public void Search_SortByStatus()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "status_a";
            const int pageNumber = 1;
            const int pageSize = 100;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].PatientStatus, Is.LessThan(result.Last().PatientStatus));
        }

        [Test]
        public void Search_SortByStatus_Desc()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "status_d";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.Search(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].PatientStatus, Is.GreaterThan(result[1].PatientStatus));
        }

        [Test]
        public void DeepSearch()
        {
            //Arrange
            const string id = "";
            const string nhc = "";
            const string nuhsa = "";
            const string name = "";
            const string surnames = "";
            PatientStatus? status = null;
            const string birthFrom = "";
            const string birthTo = "";
            const string sorting = "";
            const int pageNumber = 1;
            const int pageSize = 10;

            //Act
            var result = _patientService.DeepSearch(id, nhc, nuhsa, name,
                surnames, status, birthFrom, birthTo, sorting, pageNumber, pageSize);
            var item = result.First();

            //Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(item.NbrSurveillances, Is.Not.Null);
            Assert.That(item.Hypothermias, Is.Not.Null);
            Assert.That(item.Monitorings, Is.Not.Null);
        }

        [Test]
        public void Create()
        {
            //Arrange
            var registrant = _dataContext.UserAccounts.First();
            var viewModel = new PatientViewModel
            {
                Nhc = "123",
                Nuhsa = "123456",
                Name = "Test",
                Surnames = "Testing",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Now,
                BirthType = BirthType.Eutocic,
                Ph = 8,
                Apgar = 10,
                Weight = 5000,
                CprType = null
            };

            //Act
            var patientId = _patientService.Create(viewModel, registrant.UserName, registrant.Id);
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(patientId, Is.Not.Zero);
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.Normal));
            Assert.That(patient.RegistrantName, Is.EqualTo(registrant.UserName));
            Assert.That(patient.RegistrantId, Is.EqualTo(registrant.Id));
        }

        [Test]
        public void Create_WithNbrSurveillance()
        {
            //Arrange
            var registrant = _dataContext.UserAccounts.First();
            var viewModel = new PatientViewModel
            {
                Nhc = "123",
                Nuhsa = "123456",
                Name = "Test",
                Surnames = "Testing",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Now,
                BirthType = BirthType.Eutocic,
                Ph = 8,
                Apgar = 4,
                Weight = 5000,
                CprType = null
            };

            //Act
            var patientId = _patientService.Create(viewModel, registrant.UserName, registrant.Id);
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(patientId, Is.Not.Zero);
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.NbrSurveillance));
            Assert.That(patient.RegistrantName, Is.EqualTo(registrant.UserName));
            Assert.That(patient.RegistrantId, Is.EqualTo(registrant.Id));
        }

        [Test]
        public void Create_WithHypothermia()
        {
            //Arrange
            var registrant = _dataContext.UserAccounts.First();
            var viewModel = new PatientViewModel
            {
                Nhc = "123",
                Nuhsa = "123456",
                Name = "Test",
                Surnames = "Testing",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Now,
                BirthType = BirthType.Eutocic,
                Ph = 8,
                Apgar = 4,
                Weight = 5000,
                CprType = null,
                Lethargy = true,
                AlteredTone = true
            };

            //Act
            var patientId = _patientService.Create(viewModel, registrant.UserName, registrant.Id);
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(patientId, Is.Not.Zero);
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.Hypothermia));
            Assert.That(patient.RegistrantName, Is.EqualTo(registrant.UserName));
            Assert.That(patient.RegistrantId, Is.EqualTo(registrant.Id));
        }

        [Test]
        public void Update()
        {
            //Arrange
            var patient = _dataContext.Patients.First();
            var patientViewModel = new PatientViewModel(patient)
            {
                Name = "TestChange"
            };

            //Act
            var updated = _patientService.Update(patientViewModel);
            patient = _patientService.FindById(patient.Id);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(patient.Name, Is.EqualTo("TestChange"));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var patientViewModel = new PatientViewModel()
            {
                Id = 123
            };

            //Act
            var updated = _patientService.Update(patientViewModel);

            //Assert
            Assert.That(updated, Is.False);
        }

        [Test]
        public void Update_ToNbrSurveillance()
        {
            //Arrange
            var registrant = _dataContext.UserAccounts.First();
            var viewModel = new PatientViewModel
            {
                Nhc = "123",
                Nuhsa = "123456",
                Name = "Test",
                Surnames = "Testing",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Now,
                BirthType = BirthType.Eutocic,
                Ph = 8,
                Apgar = 8,
                Weight = 5000,
                CprType = null
            };
            var patientId = _patientService.Create(viewModel, registrant.UserName, registrant.Id);
            var patient = _patientService.FindById(patientId);
            viewModel = new PatientViewModel(patient) { Apgar = 4 };

            //Act
            var updated = _patientService.Update(viewModel);
            patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.NbrSurveillance));
        }

        [Test]
        public void Update_ToHypothermia()
        {
            //Arrange
            var registrant = _dataContext.UserAccounts.First();
            var viewModel = new PatientViewModel
            {
                Nhc = "123",
                Nuhsa = "123456",
                Name = "Test",
                Surnames = "Testing",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Now,
                BirthType = BirthType.Eutocic,
                Ph = 8,
                Apgar = 8,
                Weight = 5000,
                CprType = null
            };
            var patientId = _patientService.Create(viewModel, registrant.UserName, registrant.Id);
            var patient = _patientService.FindById(patientId);
            viewModel = new PatientViewModel(patient)
            {
                Apgar = 4,
                Lethargy = true,
                AlteredTone = true
            };

            //Act
            var updated = _patientService.Update(viewModel);
            patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.Hypothermia));
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var patientId = _dataContext.Patients.AsNoTracking().First().Id;

            //Act
            var deleted = _patientService.Delete(patientId);
            var patient = _patientService.FindById(patientId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(patient, Is.Null);
        }

        [Test]
        public void Delete_WithMonitoring()
        {
            //Arrange
            var patientId = _dataContext.Patients.AsNoTracking().First(p => p.PatientStatus == PatientStatus.Monitoring).Id;

            //Act
            var deleted = _patientService.Delete(patientId);
            var patient = _patientService.FindById(patientId);
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);
            var monitorings = _monitoringService.FindByPatientId(patientId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(patient, Is.Null);
            Assert.That(surveillances, Is.Empty);
            Assert.That(monitorings, Is.Empty);
        }

        [Test]
        public void Delete_WithHypothermia()
        {
            //Arrange
            var patientId = _dataContext.Patients.AsNoTracking().First(p => p.PatientStatus == PatientStatus.Hypothermia).Id;

            //Act
            var deleted = _patientService.Delete(patientId);
            var patient = _patientService.FindById(patientId);
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(patient, Is.Null);
            Assert.That(hypothermias, Is.Empty);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var deleted = _patientService.Delete(patientId);

            //Assert
            Assert.That(deleted, Is.False);
        }

        [Test]
        public void GetStatus()
        {
            //Arrange
            var patient = _dataContext.Patients.First();

            //Act
            var status = _patientService.GetStatus(patient.Id);

            //Assert
            Assert.That(status, Is.Not.Null);
            Assert.That(status, Is.EqualTo(patient.PatientStatus));
        }

        [Test]
        public void GetStatus_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var status = _patientService.GetStatus(patientId);

            //Assert
            Assert.That(status, Is.EqualTo(PatientStatus.Normal));
        }

        [Test]
        public void UpdateStatus()
        {
            //Arrange
            var patient = _dataContext.Patients.First(p => p.PatientStatus == PatientStatus.Normal);

            //Act
            _patientService.UpdateStatus(patient.Id);
            patient = _patientService.FindById(patient.Id);

            //Assert
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.Normal));
        }

        [Test]
        public void IsClosed()
        {
            //Arrange
            _patientService.ClosePatientTracking(1);
            var patient = _dataContext.Patients.First(p => p.PatientStatus == PatientStatus.Closed);

            //Act
            var closed = _patientService.IsClosed(patient.Id);

            //Assert
            Assert.That(closed, Is.True);
        }

        [Test]
        public void IsClosed_NotClosed()
        {
            //Arrange
            var patient = _dataContext.Patients.First(p => p.PatientStatus != PatientStatus.Closed);

            //Act
            var closed = _patientService.IsClosed(patient.Id);

            //Assert
            Assert.That(closed, Is.False);
        }

        [Test]
        public void ClosePatientTracking()
        {
            //Arrange
            var patient = _dataContext.Patients.First(p => p.PatientStatus != PatientStatus.Closed);
            var lastStatus = patient.PatientStatus;

            //Act
            _patientService.ClosePatientTracking(patient.Id);
            patient = _patientService.FindById(patient.Id);

            //Assert
            Assert.That(patient.PatientStatus, Is.EqualTo(PatientStatus.Closed));
            Assert.That(patient.LastStatus, Is.EqualTo(lastStatus));
        }

        [Test]
        public void ClosePatientTracking_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            _patientService.ClosePatientTracking(patientId);
        }

        [Test]
        public void OpenPatientTracking()
        {
            //Arrange
            _patientService.ClosePatientTracking(1);
            var patient = _dataContext.Patients.First(p => p.PatientStatus == PatientStatus.Closed);
            var lastStatus = patient.LastStatus;

            //Act
            _patientService.OpenPatientTracking(patient.Id);
            patient = _patientService.FindById(patient.Id);

            //Assert
            Assert.That(patient.PatientStatus, Is.EqualTo(lastStatus));
        }

        [Test]
        public void OpenPatientTracking_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            _patientService.OpenPatientTracking(patientId);
        }
    }
}
