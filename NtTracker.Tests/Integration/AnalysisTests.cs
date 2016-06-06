using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NtTracker.Data;
using NtTracker.Services;
using NtTracker.ViewModels;
using NUnit.Framework;

namespace NtTracker.Tests.Integration
{
    [TestFixture, Category("Integration")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    class AnalysisTests
    {
        //Database context
        private DataContext _dataContext;

        //Tested service
        private IAnalysisService _analysisService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize service
            _analysisService = new AnalysisService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _analysisService.Dispose();
        }

        [Test]
        public void Create()
        {
            //Arrange
            var analysisViewModel = new AnalysisViewModel
            {
                Hemoglobin = 150,
                Hematocrit = 0.35,
                PlateletCount = 150000,
                Alt = 5,
                Ast = 20,
                Cpk = 50,
                Proteins = 70,
                Sodium = 150,
                Potassium = 4,
                Chloride = 100
            };

            //Act
            var analysisId = _analysisService.Create(analysisViewModel);
            var analysis = _dataContext.Analyses.SingleOrDefault(a => a.Id == analysisId);

            //Assert
            Assert.That(analysisId, Is.Not.Zero);
            Assert.That(analysis, Is.Not.Null);
        }

        [Test]
        public void Update()
        {
            //Arrange
            var analysis = _dataContext.Analyses.First();
            var analysisViewModel = new AnalysisViewModel(analysis);
            analysisViewModel.Hemoglobin = analysisViewModel.Hemoglobin + 10;

            //Act
            _analysisService.Update(analysisViewModel);
            analysis = _dataContext.Analyses.FirstOrDefault(a => a.Id == analysisViewModel.Id);

            //Assert
            Assert.That(analysis, Is.Not.Null);
            Assert.That(analysis.Hemoglobin, Is.EqualTo(analysisViewModel.Hemoglobin));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var analysisViewModel = new AnalysisViewModel
            {
                Id = 123456
            };

            //Act
            _analysisService.Update(analysisViewModel);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var analysisId = _dataContext.Analyses.First().Id;

            //Act
            _analysisService.Delete(analysisId);
            var analysis = _dataContext.Analyses.FirstOrDefault(a => a.Id == analysisId);

            //Assert
            Assert.That(analysis, Is.Null);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int analysisId = 123456;

            //Act
            _analysisService.Delete(analysisId);
        }
    }
}
