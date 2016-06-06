using System;
using NtTracker.Data;
using NtTracker.Services;
using NUnit.Framework;
using System.Linq;
using NtTracker.ViewModels;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    class MonitoringTests
    {
        //Database context
        private DataContext _dataContext;

        //Tested service
        private IMonitoringService _monitoringService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize service
            _monitoringService = new MonitoringService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _monitoringService.Dispose();
        }

        [Test]
        public void FindById()
        {
            //Arrange
            var monitoringId = _dataContext.Monitorings.First().Id;

            //Act
            var monitoring = _monitoringService.FindById(monitoringId);

            //Assert
            Assert.That(monitoring, Is.Not.Null);
            Assert.That(monitoring.Id, Is.EqualTo(monitoringId));
        }

        [Test]
        public void FindById_NotFound()
        {
            //Arrange
            const int monitoringId = -123;

            //Act
            var monitoring = _monitoringService.FindById(monitoringId);

            //Assert
            Assert.That(monitoring, Is.Null);
        }

        [Test]
        public void FindByPatientId()
        {
            //Arrange
            var patientId = _dataContext.Monitorings.First().PatientId;

            //Act
            var monitorings = _monitoringService.FindByPatientId(patientId);

            //Assert
            Assert.That(monitorings, Is.Not.Empty);
            Assert.That(monitorings.First().PatientId, Is.EqualTo(patientId));
        }

        [Test]
        public void FindByPatientId_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var monitorings = _monitoringService.FindByPatientId(patientId);

            //Assert
            Assert.That(monitorings, Is.Empty);
        }

        [Test]
        public void Create()
        {
            //Arrange
            var patientId = _dataContext.Patients.First().Id;
            var viewModel = new MonitoringViewModel
            {
                DateTime = DateTime.Now,
                Description = "Test123",
                Comments = "No comments"
            };

            //Act
            _monitoringService.Create(viewModel, patientId);
            var monitoring = _monitoringService.FindByPatientId(patientId).First();

            //Assert
            Assert.That(monitoring, Is.Not.Null);
            Assert.That(monitoring.Description, Is.EqualTo("Test123"));
        }

        [Test]
        public void Update()
        {
            //Arrange
            var monitoring = _dataContext.Monitorings.First();
            var viewModel = new MonitoringViewModel(monitoring)
            {
                Id = monitoring.Id,
                Comments = "TestEdit"
            };

            //Act
            var updated = _monitoringService.Update(viewModel);
            monitoring = _monitoringService.FindById(monitoring.Id);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(monitoring.Description, Is.EqualTo(viewModel.Description));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var viewModel = new MonitoringViewModel {Id = -123};

            //Act
            var updated = _monitoringService.Update(viewModel);

            //Assert
            Assert.That(updated, Is.False);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var monitoringId = _dataContext.Monitorings.First().Id;

            //Act
            var deleted = _monitoringService.Delete(monitoringId);
            var monitoring = _monitoringService.FindById(monitoringId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(monitoring, Is.Null);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int monitoringId = -123;

            //Act
            var deleted = _monitoringService.Delete(monitoringId);

            //Assert
            Assert.That(deleted, Is.False);
        }
    }
}
