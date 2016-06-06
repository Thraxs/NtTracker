using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    class HypothermiaTests
    {
        //Database context
        private DataContext _dataContext;

        //Support services
        private ICnsExplorationService _cnsExplorationService;
        private IAnalysisService _analysisService;

        //Tested service
        private IHypothermiaService _hypothermiaService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize support services
            _cnsExplorationService = new CnsExplorationService(_dataContext);
            _analysisService = new AnalysisService(_dataContext);

            //Initialize service
            _hypothermiaService = new HypothermiaService(_dataContext, _cnsExplorationService, _analysisService);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _hypothermiaService.Dispose();

            //Dispose support services
            _cnsExplorationService.Dispose();
            _analysisService.Dispose();
        }

        [Test]
        public void FindById()
        {
            //Arrange
            var hypothermiaId = _dataContext.Hypothermias.First().Id;

            //Act
            var hypothermia = _hypothermiaService.FindById(hypothermiaId);

            //Assert
            Assert.That(hypothermia, Is.Not.Null);
            Assert.That(hypothermia.Id, Is.EqualTo(hypothermiaId));
        }

        [Test]
        public void FindById_NotFound()
        {
            //Arrange
            const int hypothermiaId = -123;

            //Act
            var hypothermia = _hypothermiaService.FindById(hypothermiaId);

            //Assert
            Assert.That(hypothermia, Is.Null);
        }

        [Test]
        public void FindByPatientId()
        {
            //Arrange
            var patientId = _dataContext.Hypothermias.First().PatientId;

            //Act
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            //Assert
            Assert.That(hypothermias, Is.Not.Empty);
        }

        [Test]
        public void FindByPatientId_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            //Assert
            Assert.That(hypothermias, Is.Empty);
        }

        [Test]
        public void Create()
        {
            //Arrange
            var viewModel = new EditHypothermiaViewModel
            {
                TimeSlot = TimeSlot.H72,
                CnsUs = CnsUltrasound.Edema,
                AEeg = AElectroencephalogram.Convulsion,
                Eeg = Electroencephalogram.Slow,
                Convulsion = false,
                Cr = CerebralResonance.CorpusCallosumInjury,
                CnsExploration = new CnsExplorationViewModel(),
                Analysis = new AnalysisViewModel(),
                ModelState = new ModelStateDictionary()
            };
            var patientId = _dataContext.Patients.First().Id;

            //Act
            _hypothermiaService.Create(viewModel, patientId);
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            //Assert
            Assert.That(viewModel.ModelState.IsValid, Is.True);
            Assert.That(hypothermias, Is.Not.Empty);
            Assert.That(hypothermias.First().TimeSlot, Is.EqualTo(TimeSlot.H72));
            Assert.That(hypothermias.First().CnsExploration, Is.Not.Null);
            Assert.That(hypothermias.First().Analysis, Is.Not.Null);
        }

        [Test]
        public void Create_RepeatedTimeSlot()
        {
            //Arrange
            var viewModel = new EditHypothermiaViewModel
            {
                TimeSlot = TimeSlot.H72,
                CnsUs = CnsUltrasound.Edema,
                AEeg = AElectroencephalogram.Convulsion,
                Eeg = Electroencephalogram.Slow,
                Convulsion = false,
                Cr = CerebralResonance.CorpusCallosumInjury,
                CnsExploration = new CnsExplorationViewModel(),
                Analysis = new AnalysisViewModel(),
                ModelState = new ModelStateDictionary()
            };
            var patientId = _dataContext.Patients.First().Id;
            _hypothermiaService.Create(viewModel, patientId);

            //Act
            _hypothermiaService.Create(viewModel, patientId);
            var hypothermias = _hypothermiaService.FindByPatientId(patientId);

            //Assert
            Assert.That(viewModel.ModelState.IsValid, Is.False);
            Assert.That(hypothermias.Count, Is.EqualTo(1));
        }

        [Test]
        public void Update()
        {
            //Arrange
            var hypothermia = _dataContext.Hypothermias
                .Include(h => h.CnsExploration)
                .Include(h => h.Analysis)
                .First();

            var viewModel = new EditHypothermiaViewModel(hypothermia)
            {
                Id = hypothermia.Id,
                Convulsion = true,
                CnsExploration = { Reflexes = Reflexes.Irregular },
                Analysis = { Cpk = 20 }
            };

            //Act
            var updated = _hypothermiaService.Update(viewModel);
            hypothermia = _hypothermiaService.FindById(viewModel.Id);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(hypothermia.Convulsion, Is.True);
            Assert.That(hypothermia.CnsExploration.Reflexes, Is.EqualTo(Reflexes.Irregular));
            Assert.That(hypothermia.Analysis.Cpk, Is.EqualTo(20));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var viewModel = new EditHypothermiaViewModel
            {
                Id = -123
            };

            //Act
            var updated = _hypothermiaService.Update(viewModel);

            //Assert
            Assert.That(updated, Is.False);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var hypothermia = _dataContext.Hypothermias
                .Include(h => h.CnsExploration)
                .Include(h => h.Analysis)
                .First();
            var hypothermiaId = hypothermia.Id;
            var explorationId = hypothermia.CnsExploration.Id;
            var analysisId = hypothermia.Analysis.Id;

            //Act
            var deleted = _hypothermiaService.Delete(hypothermiaId);
            hypothermia = _hypothermiaService.FindById(hypothermiaId);
            var exploration = _dataContext.CnsExplorations.FirstOrDefault(e => e.Id == explorationId);
            var analysis = _dataContext.Analyses.FirstOrDefault(a => a.Id == analysisId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(hypothermia, Is.Null);
            Assert.That(exploration, Is.Null);
            Assert.That(analysis, Is.Null);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int hypothermiaId = -123;

            //Act
            var deleted = _hypothermiaService.Delete(hypothermiaId);

            //Assert
            Assert.That(deleted, Is.False);
        }
    }
}
