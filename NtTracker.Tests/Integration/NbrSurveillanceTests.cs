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
    class NbrSurveillanceTests
    {
        //Database context
        private DataContext _dataContext;

        //Support services
        private ICnsExplorationService _cnsExplorationService;
        private IAnalysisService _analysisService;

        //Tested service
        private INbrSurveillanceService _nbrSurveillanceService;

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
            _nbrSurveillanceService = new NbrSurveillanceService(_dataContext, _cnsExplorationService, _analysisService);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _nbrSurveillanceService.Dispose();

            //Dispose support services
            _cnsExplorationService.Dispose();
            _analysisService.Dispose();
        }

        [Test]
        public void FindById()
        {
            //Arrange
            var surveillanceId = _dataContext.NbrSurveillances.First().Id;

            //Act
            var surveillance = _nbrSurveillanceService.FindById(surveillanceId);

            //Assert
            Assert.That(surveillance, Is.Not.Null);
            Assert.That(surveillance.Id, Is.EqualTo(surveillanceId));
        }

        [Test]
        public void FindById_NotFound()
        {
            //Arrange
            const int surveillanceId = -123;

            //Act
            var surveillance = _nbrSurveillanceService.FindById(surveillanceId);

            //Assert
            Assert.That(surveillance, Is.Null);
        }

        [Test]
        public void FindByPatientId()
        {
            //Arrange
            var patientId = _dataContext.NbrSurveillances.First().PatientId;

            //Act
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);

            //Assert
            Assert.That(surveillances, Is.Not.Empty);
            Assert.That(surveillances.First().PatientId, Is.EqualTo(patientId));
        }

        [Test]
        public void FindByPatientId_NotFound()
        {
            //Arrange
            const int patientId = -123;

            //Act
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);

            //Assert
            Assert.That(surveillances, Is.Empty);
        }

        [Test]
        public void AtRisk_No()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.Continuous,
                TfUltrasound = TransfontanellarUltrasound.Normal,
                Analysis = new Analysis(),
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.Normal,
                    CranialNerves = CranialNerves.Normal,
                    Position = Position.Good,
                    Reflexes = Reflexes.Normal,
                    Tone = Tone.Good
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.False);
        }

        [Test]
        public void AtRisk_Eeg()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                Eeg = Electroencephalogram.Slow,
                Analysis = new Analysis(),
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.Normal,
                    CranialNerves = CranialNerves.Normal,
                    Position = Position.Good,
                    Reflexes = Reflexes.Normal,
                    Tone = Tone.Good
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.True);
        }

        [Test]
        public void AtRisk_AEeg()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                AEeg = AElectroencephalogram.Flat,
                Analysis = new Analysis(),
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.Normal,
                    CranialNerves = CranialNerves.Normal,
                    Position = Position.Good,
                    Reflexes = Reflexes.Normal,
                    Tone = Tone.Good
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.True);
        }

        [Test]
        public void AtRisk_TfUltrasound()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                TfUltrasound = TransfontanellarUltrasound.ThalamusInjury,
                Analysis = new Analysis(),
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.Normal,
                    CranialNerves = CranialNerves.Normal,
                    Position = Position.Good,
                    Reflexes = Reflexes.Normal,
                    Tone = Tone.Good
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.True);
        }

        [Test]
        public void AtRisk_CnsExploration()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                Analysis = new Analysis(),
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.NoResponse,
                    CranialNerves = CranialNerves.Irregular,
                    Position = Position.Bad,
                    Reflexes = Reflexes.NoReaction,
                    Tone = Tone.Low
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.True);
        }

        [Test]
        public void AtRisk_Analysis()
        {
            //Arrange
            var surveillance = new NbrSurveillance
            {
                TimeSlot = TimeSlot.H6,
                Analysis = new Analysis
                {
                    Hemoglobin = 100,
                    Hematocrit = 0.10,
                    PlateletCount = 50000,
                    Alt = 3,
                    Ast = 5,
                    Cpk = 5,
                    Proteins = 1,
                    Sodium = 1,
                    Potassium = 1,
                    Chloride = 1
                },
                CnsExploration = new CnsExploration
                {
                    Behavior = Behavior.Normal,
                    CranialNerves = CranialNerves.Normal,
                    Position = Position.Good,
                    Reflexes = Reflexes.Normal,
                    Tone = Tone.Good
                }
            };

            //Act
            var atRisk = _nbrSurveillanceService.AtRisk(surveillance);

            //Assert
            Assert.That(atRisk, Is.True);
        }

        [Test]
        public void Create()
        {
            //Arrange
            var viewModel = new EditNbrSurveillanceViewModel
            {
                TimeSlot = TimeSlot.H72,
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.BurstSupression,
                TfUltrasound = TransfontanellarUltrasound.Normal,
                CnsExploration = new CnsExplorationViewModel(),
                Analysis = new AnalysisViewModel(),
                ModelState = new ModelStateDictionary()
            };
            var patientId = _dataContext.Patients.First().Id;

            //Act
            _nbrSurveillanceService.Create(viewModel, patientId);
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);

            //Assert
            Assert.That(viewModel.ModelState.IsValid, Is.True);
            Assert.That(surveillances, Is.Not.Empty);
            Assert.That(surveillances.First().TimeSlot, Is.EqualTo(TimeSlot.H72));
            Assert.That(surveillances.First().CnsExploration, Is.Not.Null);
            Assert.That(surveillances.First().Analysis, Is.Not.Null);
        }

        [Test]
        public void Create_RepeatedTimeSlot()
        {
            //Arrange
            var viewModel = new EditNbrSurveillanceViewModel
            {
                TimeSlot = TimeSlot.H72,
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.BurstSupression,
                TfUltrasound = TransfontanellarUltrasound.Normal,
                CnsExploration = new CnsExplorationViewModel(),
                Analysis = new AnalysisViewModel(),
                ModelState = new ModelStateDictionary()
            };
            var patientId = _dataContext.Patients.First().Id;
            _nbrSurveillanceService.Create(viewModel, patientId);

            //Act
            _nbrSurveillanceService.Create(viewModel, patientId);
            var surveillances = _nbrSurveillanceService.FindByPatientId(patientId);

            //Assert
            Assert.That(viewModel.ModelState.IsValid, Is.False);
            Assert.That(surveillances.Count, Is.EqualTo(1));
        }

        [Test]
        public void Update()
        {
            //Arrange
            var surveillance = _dataContext.NbrSurveillances
                .Include(s => s.CnsExploration)
                .Include(s => s.Analysis)
                .First();

            var viewModel = new EditNbrSurveillanceViewModel(surveillance)
            {
                Id = surveillance.Id,
                Eeg = Electroencephalogram.Slow,
                CnsExploration = { Reflexes = Reflexes.Irregular },
                Analysis = { Cpk = 20 }
            };

            //Act
            var updated = _nbrSurveillanceService.Update(viewModel);
            surveillance = _nbrSurveillanceService.FindById(viewModel.Id);

            //Assert
            Assert.That(updated, Is.True);
            Assert.That(surveillance.Eeg, Is.EqualTo(Electroencephalogram.Slow));
            Assert.That(surveillance.CnsExploration.Reflexes, Is.EqualTo(Reflexes.Irregular));
            Assert.That(surveillance.Analysis.Cpk, Is.EqualTo(20));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var viewModel = new EditNbrSurveillanceViewModel
            {
                Id = -123
            };

            //Act
            var updated = _nbrSurveillanceService.Update(viewModel);

            //Assert
            Assert.That(updated, Is.False);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var surveillance = _dataContext.NbrSurveillances
                .Include(h => h.CnsExploration)
                .Include(h => h.Analysis)
                .First();
            var hypothermiaId = surveillance.Id;
            var explorationId = surveillance.CnsExploration.Id;
            var analysisId = surveillance.Analysis.Id;

            //Act
            var deleted = _nbrSurveillanceService.Delete(hypothermiaId);
            surveillance = _nbrSurveillanceService.FindById(hypothermiaId);
            var exploration = _dataContext.CnsExplorations.FirstOrDefault(e => e.Id == explorationId);
            var analysis = _dataContext.Analyses.FirstOrDefault(a => a.Id == analysisId);

            //Assert
            Assert.That(deleted, Is.True);
            Assert.That(surveillance, Is.Null);
            Assert.That(exploration, Is.Null);
            Assert.That(analysis, Is.Null);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int surveillanceId = -123;

            //Act
            var deleted = _nbrSurveillanceService.Delete(surveillanceId);

            //Assert
            Assert.That(deleted, Is.False);
        }
    }
}
