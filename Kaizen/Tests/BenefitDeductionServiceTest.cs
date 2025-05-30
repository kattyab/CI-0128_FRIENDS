using Moq;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Services.BenefitDeductions;
using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Tests.Application.Services.BenefitDeductionResults;

public class BenefitDeductionResultServiceTests
{
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
    public void GetDeductionsForEmployeeWhenEmployeeHasChosenBenefitsReturnsCorrectDeductionCalculations()
    {
        var benefitId = Guid.NewGuid();
        var benefits = new List<Benefit>
    {
        new Benefit
        {
            BenefitID = benefitId,
            Name = "Dentista",
            IsFixed = true,
            FixedValue = 80m,
            IsFullTime = true,
            MinWorkDurationMonths = 6
        }
    };
        var employee = new Employee
        {
            ContractType = "Tiempo Completo",
            StartDate = DateTime.Now.AddMonths(-12),
            BruteSalary = 1000m
        };
        var chosenBenefits = new Dictionary<Guid, List<Guid>>
    {
        { _employeeId, new List<Guid> { benefitId } }
    };
        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompany(_companyId))
            .Returns(benefits);
        _mockEmployeeRepo
            .Setup(r => r.GetEmployeesByCompany(_companyId))
            .Returns(new Dictionary<Guid, Employee> { { _employeeId, employee } });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(chosenBenefits);

        var result = _service.GetDeductionsForEmployee(_employeeId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].BenefitName, Is.EqualTo("Dentista"));
        Assert.That(result[0].DeductionValue, Is.EqualTo(80m));
    }

    [Test]
    public void GetDeductionsForEmployee_WhenEmployeeHasMultipleChosenBenefits_ReturnsAllDeductionCalculations()
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
            FixedValue = 75m,
            IsFullTime = true,
            MinWorkDurationMonths = 3
        },
        new Benefit
        {
            BenefitID = visionBenefitId,
            Name = "Oftalmologo",
            IsPercetange = true,
            PercentageValue = 2.5m,
            IsFullTime = true,
            MinWorkDurationMonths = 6
        },
        new Benefit
        {
            BenefitID = healthBenefitId,
            Name = "Doctor a Casa",
            IsFixed = true,
            FixedValue = 150m,
            IsFullTime = true,
            MinWorkDurationMonths = 1
        }
    };

        var employee = new Employee
        {
            ContractType = "Tiempo Completo",
            StartDate = DateTime.Now.AddMonths(-12),
            BruteSalary = 2000m
        };

        var chosenBenefits = new Dictionary<Guid, List<Guid>>
    {
        { _employeeId, new List<Guid> { dentalBenefitId, visionBenefitId, healthBenefitId } }
    };

        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompany(_companyId))
            .Returns(benefits);
        _mockEmployeeRepo
            .Setup(r => r.GetEmployeesByCompany(_companyId))
            .Returns(new Dictionary<Guid, Employee> { { _employeeId, employee } });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(chosenBenefits);

        var result = _service.GetDeductionsForEmployee(_employeeId);

        Assert.That(result, Has.Count.EqualTo(3));

        var dentalDeduction = result.FirstOrDefault(r => r.BenefitName == "Dentista");
        Assert.That(dentalDeduction, Is.Not.Null);
        Assert.That(dentalDeduction.DeductionValue, Is.EqualTo(75m));

        var visionDeduction = result.FirstOrDefault(r => r.BenefitName == "Oftalmologo");
        Assert.That(visionDeduction, Is.Not.Null);
        Assert.That(visionDeduction.DeductionValue, Is.EqualTo(50m));

        var healthDeduction = result.FirstOrDefault(r => r.BenefitName == "Doctor a Casa");
        Assert.That(healthDeduction, Is.Not.Null);
        Assert.That(healthDeduction.DeductionValue, Is.EqualTo(150m));
    }

    [Test]
    public void GetDeductionsForEmployeeWhenEmployeeHasNoChosenBenefitsReturnsEmptyList()
    {
        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompany(_companyId))
            .Returns(new List<Benefit>
            {
            new Benefit
            {
                BenefitID = Guid.NewGuid(),
                Name = "Vision Plan",
                IsFixed = true,
                FixedValue = 40m
            }
            });
        _mockEmployeeRepo
            .Setup(r => r.GetChosenBenefitsByCompany(_companyId))
            .Returns(new Dictionary<Guid, List<Guid>>());

        var result = _service.GetDeductionsForEmployee(_employeeId);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetDeductionsForEmployeeWhenEmployeeChosenBenefitsListIsEmptyReturnsEmptyList()
    {
        var benefitId = Guid.NewGuid();
        _mockBenefitRepo
            .Setup(r => r.GetBenefitsByCompany(_companyId))
            .Returns(new List<Benefit>
            {
            new Benefit
            {
                BenefitID = benefitId,
                Name = "Extra Coverage",
                IsFixed = true,
                FixedValue = 100m
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
            .Returns(new Dictionary<Guid, Employee>
            {
            { _employeeId, new Employee { BruteSalary = 1000m } }
            });

        var result = _service.GetDeductionsForEmployee(_employeeId);

        Assert.That(result, Is.Empty);
    }
}
