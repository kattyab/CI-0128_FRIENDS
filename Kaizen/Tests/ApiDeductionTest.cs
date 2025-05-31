using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Services.ApiDeductions;
using Moq;

namespace Kaizen.Server.Tests.Application.Services
{
    public class ApiDeductionTest
    {
        private Mock<IApiBenefitRepository> _mockRepository;
        private Mock<IExternalApiCaller> _mockApiCaller;
        private ApiDeductionService _service;
        private Guid _companyId;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IApiBenefitRepository>();
            _mockApiCaller = new Mock<IExternalApiCaller>();
            _companyId = Guid.NewGuid();

            _service = new ApiDeductionService(_companyId, _mockRepository.Object, _mockApiCaller.Object);
        }

        [Test]
        public async Task GetDeductionsForEmployeeAsyncReturnsCorrectDeductions()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var benefits = new List<APIsDto>
            {
                new APIsDto { ID = 1, Name = "BenefitA" },
                new APIsDto { ID = 2, Name = "BenefitB" }
            };

            var parameters = new List<EmployeeBenefitParameterDto>
            {
                new EmployeeBenefitParameterDto
                {
                    BenefitId = benefits[0].ID,
                    Key = "param1",
                    Value = "value1"
                },
                new EmployeeBenefitParameterDto
                {
                    BenefitId = benefits[1].ID,
                    Key = "param2",
                    Value = "value2"
                }
            };

            _mockRepository.Setup(r => r.GetBenefitsAsync(_companyId))
                .ReturnsAsync(benefits);

            _mockRepository.Setup(r => r.GetParametersForEmployeeAsync(employeeId))
                .ReturnsAsync(parameters);

            _mockApiCaller.Setup(api => api.FetchDeductionAsync(benefits[0],
                    It.Is<Dictionary<string, string>>(d => d.ContainsKey("param1") && d["param1"] == "value1")))
                .ReturnsAsync(100m);

            _mockApiCaller.Setup(api => api.FetchDeductionAsync(benefits[1],
                    It.Is<Dictionary<string, string>>(d => d.ContainsKey("param2") && d["param2"] == "value2")))
                .ReturnsAsync(200m);

            // Act
            var result = await _service.GetDeductionsForEmployeeAsync(employeeId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(100m, result["BenefitA"]);
            Assert.AreEqual(200m, result["BenefitB"]);
        }

        [Test]
        public async Task GetDeductionsForEmployeeAsyncMissingParametersOmitsBenefit()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var benefit = new APIsDto { ID = 1, Name = "BenefitA" };
            var benefits = new List<APIsDto> { benefit };

            _mockRepository.Setup(r => r.GetBenefitsAsync(_companyId))
                .ReturnsAsync(benefits);

            _mockRepository.Setup(r => r.GetParametersForEmployeeAsync(employeeId))
                .ReturnsAsync(new List<EmployeeBenefitParameterDto>());

            // Act
            var result = await _service.GetDeductionsForEmployeeAsync(employeeId);

            // Assert
            Assert.AreEqual(0, result.Count);
            Assert.IsFalse(result.ContainsKey("BenefitA"));
        }

        [Test]
        public async Task GetDeductionsForEmployeeAsyncApiCallerThrowsOmitsBenefit()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            var benefit = new APIsDto { ID = 1, Name = "BenefitA" };
            var benefits = new List<APIsDto> { benefit };

            var parameters = new List<EmployeeBenefitParameterDto>
            {
                new EmployeeBenefitParameterDto
                {
                    BenefitId = benefit.ID,
                    Key = "param1",
                    Value = "value1"
                }
            };

            _mockRepository.Setup(r => r.GetBenefitsAsync(_companyId))
                .ReturnsAsync(benefits);

            _mockRepository.Setup(r => r.GetParametersForEmployeeAsync(employeeId))
                .ReturnsAsync(parameters);

            _mockApiCaller.Setup(api => api.FetchDeductionAsync(benefit, It.IsAny<Dictionary<string, string>>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            var result = await _service.GetDeductionsForEmployeeAsync(employeeId);

            // Assert
            Assert.AreEqual(0, result.Count);
            Assert.IsFalse(result.ContainsKey("BenefitA"));
        }
    }
}
