using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NtTracker.Controllers;
using NtTracker.Models;
using NtTracker.Services;
using NUnit.Framework;
using PagedList;

namespace NtTracker.Tests.Unit
{
    [TestFixture, Category("Controller")]
    class OperationControllerTests
    {
        //Controller services
        private Mock<IOperationService> _operationService;

        //Tested controller
        private OperationController _controller;

        [SetUp]
        public void SetUp()
        {
            //Create mock services
            _operationService = new Mock<IOperationService>();

            //Create controller
            _controller = new OperationController(_operationService.Object);
        }

        [Test]
        public void GET_List()
        {
            //Arrange
            var operation = new Operation
            {
                Id = 123,
                Action = OperationType.MonitoringCreate
            };
            var searchResult = new List<Operation> { operation };
            _operationService.Setup(
                x =>
                    x.Search(It.IsAny<string>(), It.IsAny<OperationType>(), It.IsAny<int>(), It.IsAny<string>(), 
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(searchResult.ToPagedList(1, 10));

            //Act
            var result = (ViewResult)_controller.List("", OperationType.MonitoringCreate, 1,
                DateTime.Today.AddDays(-1).ToShortDateString(), DateTime.Today.AddDays(1).ToShortDateString(), "", "id_a", null);

            //Assert
            Assert.That(((IPagedList<Operation>)result.Model)[0], Is.EqualTo(operation));
        }
    }
}
