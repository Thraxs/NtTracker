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
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    class CnsExplorationTests
    {
        //Database context
        private DataContext _dataContext;

        //Tested service
        private ICnsExplorationService _cnsExplorationService;

        [SetUp]
        public void SetUp()
        {
            //Initialize database context
            _dataContext = new DataContext();
            _dataContext.Database.Initialize(true);

            //Initialize service
            _cnsExplorationService = new CnsExplorationService(_dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            //Drop database and dispose context
            _dataContext.Database.Delete();
            _dataContext.Dispose();

            //Dispose service
            _cnsExplorationService.Dispose();
        }

        [Test]
        public void Create()
        {
            //Arrange
            var cnsExplorationViewModel = new CnsExplorationViewModel
            {
                Behavior = Behavior.Irregular,
                CranialNerves = CranialNerves.Irregular,
                Tone = Tone.Low,
                Position = Position.Irregular,
                Reflexes = Reflexes.Normal
            };

            //Act
            var explorationId = _cnsExplorationService.Create(cnsExplorationViewModel);
            var exploration = _dataContext.CnsExplorations.SingleOrDefault(e => e.Id == explorationId);

            //Assert
            Assert.That(explorationId, Is.Not.Zero);
            Assert.That(exploration, Is.Not.Null);
        }

        [Test]
        public void Update()
        {
            //Arrange
            var exploration = _dataContext.CnsExplorations.First();
            var explorationViewModel = new CnsExplorationViewModel(exploration) {Position = Position.Irregular};

            //Act
            _cnsExplorationService.Update(explorationViewModel);
            exploration = _dataContext.CnsExplorations.FirstOrDefault(e => e.Id == explorationViewModel.Id);

            //Assert
            Assert.That(exploration, Is.Not.Null);
            Assert.That(exploration.Position, Is.EqualTo(explorationViewModel.Position));
        }

        [Test]
        public void Update_NotFound()
        {
            //Arrange
            var explorationViewModel = new CnsExplorationViewModel
            {
                Id = 123456
            };

            //Act
            _cnsExplorationService.Update(explorationViewModel);
        }

        [Test]
        public void Delete()
        {
            //Arrange
            var explorationId = _dataContext.CnsExplorations.First().Id;

            //Act
            _cnsExplorationService.Delete(explorationId);
            var exploration = _dataContext.CnsExplorations.FirstOrDefault(e => e.Id == explorationId);

            //Assert
            Assert.That(exploration, Is.Null);
        }

        [Test]
        public void Delete_NotFound()
        {
            //Arrange
            const int explorationId = 123456;

            //Act
            _cnsExplorationService.Delete(explorationId);
        }
    }
}
