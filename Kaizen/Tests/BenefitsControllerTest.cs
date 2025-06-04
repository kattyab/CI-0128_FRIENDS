using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Tests.BenefitsControllerTests
{
  [TestFixture]
  public class BenefitsControllerTest
  {
    private IConfiguration _configuration;
    private Mock<IAuthService> _mockAuthService;
    private Mock<IBenefitsRepository> _mockBenefitsRepository;
    private BenefitsController _controller;

    [SetUp]
    public void Setup()
    {
      _configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
        .AddEnvironmentVariables()
        .Build();
      _mockAuthService = new Mock<IAuthService>();
      _mockBenefitsRepository = new Mock<IBenefitsRepository>();
      _controller = new BenefitsController(_mockAuthService.Object, _mockBenefitsRepository.Object);
    }

    [Test]
    public async Task Show_UnauthenticatedUser_ReturnsUnauthorized()
    {
      _mockAuthService.Setup(a => a.IsAuthenticated()).Returns(false);

      var result = _controller.Show(Guid.NewGuid());

      Assert.IsInstanceOf<UnauthorizedResult>(result);
    }

    [Test]
    public async Task Show_BenefitNotFound_ReturnsNotFound()
    {
        _mockAuthService.Setup(a => a.IsAuthenticated()).Returns(true);
        _mockAuthService.Setup(a => a.GetAuthUserCompanyPK()).Returns(Guid.NewGuid());
        _mockBenefitsRepository.Setup(r => r.GetBenefit(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((BenefitDto?)null);

        var result = _controller.Show(Guid.NewGuid());

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task Show_ValidRequest_ReturnsOkWithBenefit()
    {
        var guid = Guid.NewGuid();
        var companyGuid = Guid.NewGuid();
        var benefit = new BenefitDto { ID = guid, Name = "Test Benefit" };

        _mockAuthService.Setup(a => a.IsAuthenticated()).Returns(true);
        _mockAuthService.Setup(a => a.GetAuthUserCompanyPK()).Returns(companyGuid);
        _mockBenefitsRepository.Setup(r => r.GetBenefit(guid, companyGuid)).Returns(benefit);

        var result = _controller.Show(guid);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(benefit, okResult.Value);
    }

    [Test]
    public async Task Show_ExceptionThrown_ReturnsInternalServerError()
    {
        _mockAuthService.Setup(a => a.IsAuthenticated()).Returns(true);
        _mockAuthService.Setup(a => a.GetAuthUserCompanyPK()).Throws(new Exception("Error"));

        var result = _controller.Show(Guid.NewGuid());

        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
    }
    }
}
