using System.Web.Mvc;
using NtTracker.Controllers;
using NUnit.Framework;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class AdministrationControllerTests
    {
        //Tested controller
        private AdministrationController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create controller
            _controller = new AdministrationController();
        }

        [Test]
        public void GET_Index()
        {
            //Act
            var result = (ViewResult)_controller.Index();

            //Assert
            Assert.That(result.ViewName, Is.Empty);
        }
    }
}
