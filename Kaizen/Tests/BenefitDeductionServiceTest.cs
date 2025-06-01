using Moq;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Services.BenefitDeductions;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos;

namespace Kaizen.Tests.Application.Services.BenefitDeductionResults;

public class BenefitDeductionResultServiceTests
{
    private const decimal BruteSalaryLow = 1000m;
    private const decimal BruteSalaryHigh = 2000m;

    private const decimal DentistFixedValue = 80m;
    private const decimal DentistFixedValueMultiple = 75m;

    private const decimal VisionPercentageValue = 2.5m;
    private const decimal VisionExpectedDeduction = 50m;

    private const decimal DoctorFixedValue = 150m;

    private const decimal VisionPlanFixedValue = 40m;
    private const decimal ExtraCoverageFixedValue = 100m;

    private const int DentistMinMonths = 6;
    private const int DentistMinMonthsMultiple = 3;
    private const int VisionMinMonths = 6;
    private const int DoctorMinMonths = 1;

    private const int EmployeeSeniorityMonths = -12;

    private Mock<IBenefitDeductionRepository> _mockBenefitRepo;
    private Mock<IEmployeeDeductionRepository> _mockEmployeeRepo;
    private BenefitDeductionService _service;
    private Guid _companyId;
    private Guid _employeeId;

    [SetUp]
    public void SetUp()
    {
        _mockBenefitRepo = new Mock<IBenefitDeductionRepository>();
        _mockEmployeeRepo = new Mock<IEmployeeDeductionRepository>();

        _companyId = Guid.NewGuid();
        _employeeId = Guid.NewGuid();

        _service = new BenefitDeductionService(
            _companyId,
            _mockBenefitRepo.Object,
            _mockEmployeeRepo.Object
        );
    }

    [Test]
    public async Task GetDeductionsForEmployeeWhenEmployeeHasChosenBenefitsReturnsCorrectDeductionCalculations()
    {
        var benefitId = Guid.NewGuid();
        var benefits = new List<Benefit>
        {
            new Benefit
            {
                BenefitID = benefitId,
                Name = "Dentista",
                IsFixed = true,
                FixedValue = DentistFixedValue,
                IsFullTime = true,
                MinWorkDurationMonths = DentistMinMonths
            }
        };
        var employee = new EmployeeDto
        {
            StartDate = DateTime.Now.AddMonths(EmployeeSeniorityMonths),
            BruteSalary = BruteSalaryLow
        };
        var chosenBenefits = new Dictionary<Guid, List<Guid>>
        {
            { _employeeId, new List<Guid> { benefitId } }
        };

        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompanyAsync(_companyId))
            .ReturnsAsync(benefits);
        _mockEmployeeRepo
            .Setup(r => r.GetEmployeesByCompany(_companyId))
            .Returns(new Dictionary<Guid, EmployeeDto> { { _employeeId, employee } });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(chosenBenefits);

        var result = await _service.GetBenefitDeductionsForEmployeeAsync(_employeeId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].BenefitName, Is.EqualTo("Dentista"));
        Assert.That(result[0].DeductionValue, Is.EqualTo(DentistFixedValue));
    }

    [Test]
    public async Task GetDeductionsForEmployee_WhenEmployeeHasMultipleChosenBenefits_ReturnsAllDeductionCalculations()
    {
        var dentalBenefitId = Guid.NewGuid();
        var visionBenefitId = Guid.NewGuid();
        var healthBenefitId = Guid.NewGuid();

        var benefits = new List<Benefit>
        {
            new Benefit
            {
                BenefitID = dentalBenefitId,
                Name = "Dentista",
                IsFixed = true,
                FixedValue = DentistFixedValueMultiple,
                IsFullTime = true,
                MinWorkDurationMonths = DentistMinMonthsMultiple
            },
            new Benefit
            {
                BenefitID = visionBenefitId,
                Name = "Oftalmologo",
                IsPercetange = true,
                PercentageValue = VisionPercentageValue,
                IsFullTime = true,
                MinWorkDurationMonths = VisionMinMonths
            },
            new Benefit
            {
                BenefitID = healthBenefitId,
                Name = "Doctor a Casa",
                IsFixed = true,
                FixedValue = DoctorFixedValue,
                IsFullTime = true,
                MinWorkDurationMonths = DoctorMinMonths
            }
        };

        var employee = new EmployeeDto
        {
            StartDate = DateTime.Now.AddMonths(EmployeeSeniorityMonths),
            BruteSalary = BruteSalaryHigh
        };

        var chosenBenefits = new Dictionary<Guid, List<Guid>>
        {
            { _employeeId, new List<Guid> { dentalBenefitId, visionBenefitId, healthBenefitId } }
        };

        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompanyAsync(_companyId))
            .ReturnsAsync(benefits);
        _mockEmployeeRepo
            .Setup(r => r.GetEmployeesByCompany(_companyId))
            .Returns(new Dictionary<Guid, EmployeeDto> { { _employeeId, employee } });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(chosenBenefits);

        var result = await _service.GetBenefitDeductionsForEmployeeAsync(_employeeId);

        Assert.That(result, Has.Count.EqualTo(3));

        var dentalDeduction = result.FirstOrDefault(r => r.BenefitName == "Dentista");
        Assert.That(dentalDeduction, Is.Not.Null);
        Assert.That(dentalDeduction.DeductionValue, Is.EqualTo(DentistFixedValueMultiple));

        var visionDeduction = result.FirstOrDefault(r => r.BenefitName == "Oftalmologo");
        Assert.That(visionDeduction, Is.Not.Null);
        Assert.That(visionDeduction.DeductionValue, Is.EqualTo(VisionExpectedDeduction));

        var healthDeduction = result.FirstOrDefault(r => r.BenefitName == "Doctor a Casa");
        Assert.That(healthDeduction, Is.Not.Null);
        Assert.That(healthDeduction.DeductionValue, Is.EqualTo(DoctorFixedValue));
    }

    [Test]
    public async Task GetDeductionsForEmployeeWhenEmployeeHasNoChosenBenefitsReturnsEmptyList()
    {
        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompanyAsync(_companyId))
            .ReturnsAsync(new List<Benefit>
            {
                new Benefit
                {
                    BenefitID = Guid.NewGuid(),
                    Name = "Vision Plan",
                    IsFixed = true,
                    FixedValue = VisionPlanFixedValue
                }
            });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(new Dictionary<Guid, List<Guid>>());

        var result = await _service.GetBenefitDeductionsForEmployeeAsync(_employeeId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetDeductionsForEmployeeWhenEmployeeChosenBenefitsListIsEmptyReturnsEmptyList()
    {
        var benefitId = Guid.NewGuid();
        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompanyAsync(_companyId))
            .ReturnsAsync(new List<Benefit>
            {
                new Benefit
                {
                    BenefitID = benefitId,
                    Name = "Extra Coverage",
                    IsFixed = true,
                    FixedValue = ExtraCoverageFixedValue
                }
            });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(new Dictionary<Guid, List<Guid>>
            {
                { _employeeId, new List<Guid>() }
            });
        _mockEmployeeRepo
            .Setup(r => r.GetEmployeesByCompany(_companyId))
            .Returns(new Dictionary<Guid, EmployeeDto>
            {
                { _employeeId, new EmployeeDto { BruteSalary = BruteSalaryLow } }
            });

        var result = await _service.GetBenefitDeductionsForEmployeeAsync(_employeeId);

        Assert.That(result, Is.Empty);
    }
}
