using Moq;
using Kaizen.Server.Application.Services.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using Kaizen.Server.Application.Dtos.Reports;
using AutoFixture;

namespace Kaizen.Server.Tests.Application.Services.Reports
{
    [TestFixture]
    public class PayrollReportsServiceTests
    {
        private Fixture _fixture;
        private Mock<IReportsRepository> _mockReportsRepository;
        private PayrollReportsService _payrollReportsService;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mockReportsRepository = new Mock<IReportsRepository>();
            _payrollReportsService = new PayrollReportsService(_mockReportsRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockReportsRepository?.Reset();
        }

        [Test]
        public void Constructor_WithValidRepository_ShouldCreateInstance()
        {
            var service = new PayrollReportsService(_mockReportsRepository.Object);

            Assert.That(service, Is.Not.Null);
        }

        [Test]
        public async Task ExecuteAsync_WithValidCompanyId_ShouldReturnPayrollReports()
        {
            var companyId = _fixture.Create<Guid>();
            var expectedReports = _fixture.CreateMany<OwnerPayrollReport>(2).ToList();

            _mockReportsRepository
                .Setup(x => x.GetPayrollReportsByCompanyAsync(companyId))
                .ReturnsAsync(expectedReports);

            var result = await _payrollReportsService.ExecuteAsync(companyId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            _mockReportsRepository.Verify(x => x.GetPayrollReportsByCompanyAsync(companyId), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_WithEmptyResults_ShouldReturnEmptyList()
        {
            var companyId = _fixture.Create<Guid>();
            var expectedReports = new List<OwnerPayrollReport>();

            _mockReportsRepository
                .Setup(x => x.GetPayrollReportsByCompanyAsync(companyId))
                .ReturnsAsync(expectedReports);

            var result = await _payrollReportsService.ExecuteAsync(companyId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ExecuteAsync_WithEmptyGuid_ShouldThrowArgumentException()
        {
            var emptyGuid = Guid.Empty;

            var exception = Assert.ThrowsAsync<ArgumentException>(
                async () => await _payrollReportsService.ExecuteAsync(emptyGuid));

            Assert.That(exception.Message, Does.Contain("Company ID cannot be empty"));
            Assert.That(exception.ParamName, Is.EqualTo("companyId"));
        }

        [Test]
        public void ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            var companyId = Guid.NewGuid();
            _mockReportsRepository
                .Setup(x => x.GetPayrollReportsByCompanyAsync(companyId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                () => _payrollReportsService.ExecuteAsync(companyId));

            Assert.That(exception.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void CalculateLaborCharges_WithNullReport_ShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _payrollReportsService.CalculateLaborCharges(null));

            Assert.That(exception.ParamName, Is.EqualTo("report"));
        }

        [Test]
        public void CalculateLaborCharges_WithValidReport_ShouldCalculateAllCharges()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 500.00m,
                TiempoCompletoAmount = 1500.00m
            };
            var totalSalarios = 2000.00m;

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SEM, Is.EqualTo(totalSalarios * 0.0925m).Within(0.01m));
            Assert.That(result.IVM, Is.EqualTo(totalSalarios * 0.0542m).Within(0.01m));
            Assert.That(result.CuotaPatronalBancoPopular, Is.EqualTo(totalSalarios * 0.0025m).Within(0.01m));
            Assert.That(result.AsignacionesFamiliares, Is.EqualTo(totalSalarios * 0.0500m).Within(0.01m));
            Assert.That(result.IMAS, Is.EqualTo(totalSalarios * 0.0050m).Within(0.01m));
            Assert.That(result.INA, Is.EqualTo(totalSalarios * 0.0150m).Within(0.01m));
            Assert.That(result.AporteBancoPopular, Is.EqualTo(totalSalarios * 0.0025m).Within(0.01m));
            Assert.That(result.FCL, Is.EqualTo(totalSalarios * 0.0300m).Within(0.01m));
            Assert.That(result.FondoPensionesComplementarias, Is.EqualTo(totalSalarios * 0.0050m).Within(0.01m));
            Assert.That(result.INS, Is.EqualTo(totalSalarios * 0.0100m).Within(0.01m));
        }

        [Test]
        public void CalculateLaborCharges_WithZeroAmounts_ShouldCalculateZeroCharges()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 0.00m,
                TiempoCompletoAmount = 0.00m
            };

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(0.00m));
            Assert.That(result.IVM, Is.EqualTo(0.00m));
            Assert.That(result.CuotaPatronalBancoPopular, Is.EqualTo(0.00m));
            Assert.That(result.AsignacionesFamiliares, Is.EqualTo(0.00m));
            Assert.That(result.IMAS, Is.EqualTo(0.00m));
            Assert.That(result.INA, Is.EqualTo(0.00m));
            Assert.That(result.AporteBancoPopular, Is.EqualTo(0.00m));
            Assert.That(result.FCL, Is.EqualTo(0.00m));
            Assert.That(result.FondoPensionesComplementarias, Is.EqualTo(0.00m));
            Assert.That(result.INS, Is.EqualTo(0.00m));
        }

        [Test]
        public void CalculateLaborCharges_WithOnlyPorHorasAmount_ShouldCalculateCorrectly()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 1000.00m,
                TiempoCompletoAmount = 0.00m
            };
            var totalSalarios = 1000.00m;

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(totalSalarios * 0.0925m).Within(0.01m));
            Assert.That(result.IVM, Is.EqualTo(totalSalarios * 0.0542m).Within(0.01m));
        }

        [Test]
        public void CalculateLaborCharges_WithOnlyTiempoCompletoAmount_ShouldCalculateCorrectly()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 0.00m,
                TiempoCompletoAmount = 2000.00m
            };
            var totalSalarios = 2000.00m;

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(totalSalarios * 0.0925m).Within(0.01m));
            Assert.That(result.IVM, Is.EqualTo(totalSalarios * 0.0542m).Within(0.01m));
        }

        [Test]
        public void CalculateLaborCharges_WithLargeAmounts_ShouldCalculateCorrectly()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 500000000.00m,
                TiempoCompletoAmount = 750000000.00m
            };
            var totalSalarios = 1250000000.00m;

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(totalSalarios * 0.0925m).Within(0.01m));
            Assert.That(result.IVM, Is.EqualTo(totalSalarios * 0.0542m).Within(0.01m));
            Assert.That(result.AsignacionesFamiliares, Is.EqualTo(totalSalarios * 0.0500m).Within(0.01m));
        }

        [Test]
        public void CalculateLaborCharges_WithDecimalAmounts_ShouldHandlePrecisionCorrectly()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 333.33m,
                TiempoCompletoAmount = 666.67m
            };
            var totalSalarios = 1000.00m;

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(92.50m).Within(0.01m));
            Assert.That(result.IVM, Is.EqualTo(54.20m).Within(0.01m));
            Assert.That(result.CuotaPatronalBancoPopular, Is.EqualTo(2.50m).Within(0.01m));
        }

        [Test]
        [TestCase(1000.00, 500.00, Description = "Standard amounts")]
        [TestCase(0.00, 0.00, Description = "Zero amounts")]
        [TestCase(999999.99, 999999.99, Description = "Maximum amounts")]
        [TestCase(0.01, 0.01, Description = "Minimum amounts")]
        public void CalculateLaborCharges_WithVariousAmounts_ShouldReturnSameInstance(
            decimal porHorasAmount, decimal tiempoCompletoAmount)
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = porHorasAmount,
                TiempoCompletoAmount = tiempoCompletoAmount
            };

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result, Is.SameAs(report), "Method should return the same instance");
        }

        [Test]
        public void CalculateLaborCharges_ShouldUseCorrectRates()
        {
            var report = new OwnerPayrollReport
            {
                PorHorasAmount = 1000.00m,
                TiempoCompletoAmount = 0.00m
            };

            var result = _payrollReportsService.CalculateLaborCharges(report);

            Assert.That(result.SEM, Is.EqualTo(92.50m), "SEM rate should be 9.25%");
            Assert.That(result.IVM, Is.EqualTo(54.20m), "IVM rate should be 5.42%");
            Assert.That(result.CuotaPatronalBancoPopular, Is.EqualTo(2.50m), "Cuota Patronal rate should be 0.25%");
            Assert.That(result.AsignacionesFamiliares, Is.EqualTo(50.00m), "Asignaciones Familiares rate should be 5.00%");
            Assert.That(result.IMAS, Is.EqualTo(5.00m), "IMAS rate should be 0.50%");
            Assert.That(result.INA, Is.EqualTo(15.00m), "INA rate should be 1.50%");
            Assert.That(result.AporteBancoPopular, Is.EqualTo(2.50m), "Aporte Banco Popular rate should be 0.25%");
            Assert.That(result.FCL, Is.EqualTo(30.00m), "FCL rate should be 3.00%");
            Assert.That(result.FondoPensionesComplementarias, Is.EqualTo(5.00m), "Fondo Pensiones rate should be 0.50%");
            Assert.That(result.INS, Is.EqualTo(10.00m), "INS rate should be 1.00%");
        }
    }
}
