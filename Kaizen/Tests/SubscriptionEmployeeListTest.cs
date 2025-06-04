using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Queries.Benefits;
using Kaizen.Server.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Kaizen.Server.Tests.API.Controllers
{
    [TestFixture]
    public class BenefitEmployeeListControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private BenefitEmployeeListController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new BenefitEmployeeListController(_mockMediator.Object);
        }

        [Test]
        public async Task GetEmployeeBenefitsByEmail_WithValidEmail_ReturnsOkWithBenefits()
        {
            // Arrange
            var benefits = new List<BenefitEmployeeListDto>
            {
                new BenefitEmployeeListDto
                {
                    BenefitId = Guid.NewGuid(),
                    APIId = null,
                    Name = "BenefitOne",
                    Type = "Percentage",
                    Value = 10.00M,
                    MinMonths = 1,
                    MaxBenefits = 2
                },
                new BenefitEmployeeListDto
                {
                    BenefitId = null,
                    APIId = 1,
                    Name = "MediSmart",
                    Type = "IsApi",
                    Value = 0,
                    MinMonths = 0,
                    MaxBenefits = 2
                },
                new BenefitEmployeeListDto
                {
                    BenefitId = Guid.NewGuid(),
                    APIId = null,
                    Name = "Gym",
                    Type = "Fixed",
                    Value = 1000,
                    MinMonths = 5,
                    MaxBenefits = 2
                }
            };

            var email = "johndoe@example.com";

            _mockMediator.Setup(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == email), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(benefits);

            // Act
            var result = await _controller.GetEmployeeBenefitsByEmail(email);

            // Assert
            Assert.IsInstanceOf<ActionResult<List<BenefitEmployeeListDto>>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedBenefits = okResult.Value as List<BenefitEmployeeListDto>;
            Assert.IsNotNull(returnedBenefits);
            Assert.AreEqual(3, returnedBenefits.Count);
            Assert.AreEqual("Health Insurance", returnedBenefits[0].Name);
            Assert.AreEqual("API Deduction", returnedBenefits[1].Name);
            Assert.AreEqual("Fixed Bonus", returnedBenefits[2].Name);

            _mockMediator.Verify(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == email), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployeeBenefitsByEmail_WithEmptyResult_ReturnsOkWithEmptyList()
        {
            // Arrange
            var email = "noemail@example.com";
            var emptyBenefits = new List<BenefitEmployeeListDto>();

            _mockMediator.Setup(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == email), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(emptyBenefits);

            // Act
            var result = await _controller.GetEmployeeBenefitsByEmail(email);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedBenefits = okResult.Value as List<BenefitEmployeeListDto>;
            Assert.IsNotNull(returnedBenefits);
            Assert.AreEqual(0, returnedBenefits.Count);
        }

        [Test]
        public async Task GetEmployeeBenefitsByEmail_WhenMediatorThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var email = "error@example.com";
            var exceptionMessage = "Database connection failed";

            _mockMediator.Setup(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == email), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetEmployeeBenefitsByEmail(email);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value as string;
            Assert.IsTrue(errorMessage.Contains("Error retrieving benefits"));
            Assert.IsTrue(errorMessage.Contains(exceptionMessage));
        }

        [Test]
        public async Task GetEmployeeBenefitsByEmail_WithNullEmail_CallsMediatorWithNull()
        {
            // Arrange
            string email = null;
            var benefits = new List<BenefitEmployeeListDto>();

            _mockMediator.Setup(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == null), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(benefits);

            // Act
            var result = await _controller.GetEmployeeBenefitsByEmail(email);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            _mockMediator.Verify(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == null), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetEmployeeBenefitsByEmail_WithEmptyEmail_CallsMediatorWithEmptyString()
        {
            // Arrange
            var email = string.Empty;
            var benefits = new List<BenefitEmployeeListDto>();

            _mockMediator.Setup(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == string.Empty), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(benefits);

            // Act
            var result = await _controller.GetEmployeeBenefitsByEmail(email);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            _mockMediator.Verify(m => m.Send(It.Is<BenefitEmployeeListQuery>(q => q.Email == string.Empty), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}