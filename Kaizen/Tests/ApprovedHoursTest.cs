using System;
using System.Threading.Tasks;
using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Kaizen.Tests.Controllers
{
    [TestFixture]
    public class ApprovedHoursControllerTests
    {
        private Mock<IApprovedHoursRepository> _repositoryMock;
        private ApprovedHoursController _controller;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IApprovedHoursRepository>();
            _controller = new ApprovedHoursController(_repositoryMock.Object);
        }

        [Test]
        public async Task UpdateStatusAsync_WithInvalidId_ReturnsNotFound()
        {

            var fakeId = Guid.NewGuid();
            var dto = new ApprovedHoursDto { Status = "Rejected" };

            _repositoryMock.Setup(r => r.UpdateStatusAsync(fakeId, dto.Status))
                .ReturnsAsync(false);

            var result = await _controller.UpdateStatus(fakeId, dto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task UpdateStatusAsync_WithValidId_ReturnsOk()
        {

            var fakeId = Guid.NewGuid();
            var dto = new ApprovedHoursDto { Status = "Approved" };

            _repositoryMock.Setup(r => r.UpdateStatusAsync(fakeId, dto.Status))
                .ReturnsAsync(true);

            var result = await _controller.UpdateStatus(fakeId, dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateStatusAndSentAsync_WithInvalidId_ReturnsNotFound()
        {
            var fakeId = Guid.NewGuid();
            var dto = new ApprovedHoursDto { Status = "Pending", IsSentForApproval = false };

            _repositoryMock.Setup(r => r.UpdateStatusAndSentAsync(fakeId, dto.Status, dto.IsSentForApproval))
                .ReturnsAsync(false);

            var result = await _controller.UpdateStatusAndSent(fakeId, dto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task UpdateStatusAndSentAsync_WithValidId_ReturnsOk()
        {
            var fakeId = Guid.NewGuid();
            var dto = new ApprovedHoursDto { Status = "Pending", IsSentForApproval = true };

            _repositoryMock.Setup(r => r.UpdateStatusAndSentAsync(fakeId, dto.Status, dto.IsSentForApproval))
                .ReturnsAsync(true);

  
            var result = await _controller.UpdateStatusAndSent(fakeId, dto);


            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
