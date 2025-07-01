using NUnit.Framework;
using Moq;
using Kaizen.Server.Infrastructure.Services;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Dtos.Employees;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Emails;
using Kaizen.Server.Application.Interfaces.Employees;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Infrastructure.Repositories.Payroll;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Kaizen.Server.Application.Interfaces.Repositories;

namespace Tests.Services
{
    [TestFixture]
    public class ReportServiceTest
    {
        private IConfiguration _configuration;
        private Mock<IAuthService> _mockAuthService;
        private Mock<ICompaniesRepository> _mockCompaniesRepository;
        private Mock<IEmployeesRepository> _mockEmployeesRepository;
        private Mock<IEmployeeRepository> _mockEmployeeDetailsRepository;
        private Mock<IPayrollRepository> _mockPayrollRepository;
        private Mock<IEmailService> _mockEmailService;
        private ReportService _service;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(typeof(ReportServiceTest).Assembly, optional: true)
                .AddEnvironmentVariables()
                .Build();

            _mockAuthService = new Mock<IAuthService>();
            _mockCompaniesRepository = new Mock<ICompaniesRepository>();
            _mockEmployeesRepository = new Mock<IEmployeesRepository>();
            _mockEmployeeDetailsRepository = new Mock<IEmployeeRepository>();
            _mockPayrollRepository = new Mock<IPayrollRepository>();
            _mockEmailService = new Mock<IEmailService>();

            _service = new ReportService(
                _mockAuthService.Object,
                _mockCompaniesRepository.Object,
                _mockEmployeesRepository.Object,
                _mockEmployeeDetailsRepository.Object,
                _mockPayrollRepository.Object,
                _mockEmailService.Object
            );
        }

        [Test]
        public void GetHistoricRangeInitialDataAsync_CompanyNotFound_ThrowsArgumentException()
        {
            var companyId = Guid.NewGuid();
            _mockCompaniesRepository.Setup(r => r.GetCompany(companyId)).Returns((CompanyDto?)null);

            Assert.Throws<ArgumentException>(() => _service.GetHistoricRangeInitialDataAsync(companyId));
        }

        [Test]
        public void GetHistoricRangeInitialDataAsync_ReturnsData()
        {
            var companyId = Guid.NewGuid();
            var company = new CompanyDto { CompanyName = "Test Company" };
            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { EmpID = Guid.NewGuid(), Name = "John", LastName = "Doe" }
            };
            _mockCompaniesRepository.Setup(r => r.GetCompany(companyId)).Returns(company);
            _mockEmployeesRepository.Setup(r => r.GetEmployees(companyId)).Returns(employees);

            var result = _service.GetHistoricRangeInitialDataAsync(companyId);

            Assert.AreEqual("Test Company", result.CompanyName);
            Assert.AreEqual(1, result.Employees.Count);
            Assert.AreEqual("John", result.Employees[0].Name);
        }

        [Test]
        public void GetHistoricRangePayroll_EmptyEmployeeId_ThrowsArgumentException()
        {
            var search = new HistoricRangeSearch { EmployeeId = Guid.Empty, Start = DateTime.Now, End = DateTime.Now };
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetHistoricRangePayroll(Guid.NewGuid(), search));
        }

        [Test]
        public void GetHistoricRangePayroll_EndBeforeStart_ThrowsArgumentException()
        {
            var search = new HistoricRangeSearch { EmployeeId = Guid.NewGuid(), Start = DateTime.Now, End = DateTime.Now.AddDays(-1) };
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetHistoricRangePayroll(Guid.NewGuid(), search));
        }

        [Test]
        public async Task GetHistoricRangePayroll_ReturnsPayrolls()
        {
            var empId = Guid.NewGuid();
            var search = new HistoricRangeSearch { EmployeeId = empId, Start = DateTime.Now.AddDays(-10), End = DateTime.Now };
            var payrolls = new List<HistoricRangePayroll>
            {
                new HistoricRangePayroll { ContractType = "Full", JobPosition = "Dev", PayrollDate = DateTime.Now, BruteSalary = 1000, ObligatoryDeductions = 100, OptionalDeductions = 50, NetSalary = 850 }
            };
            _mockPayrollRepository.Setup(r => r.GetEmployeeHistoricRangePayrolls(empId, search.Start, search.End)).Returns(payrolls);

            var result = await _service.GetHistoricRangePayroll(Guid.NewGuid(), search);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Full", result[0].ContractType);
        }

        [Test]
        public void SendHistoricRangeEmail_EmptyEmployeeId_ThrowsArgumentException()
        {
            var search = new HistoricRangeSearch { EmployeeId = Guid.Empty, Start = DateTime.Now, End = DateTime.Now };
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.SendHistoricRangeEmail(Guid.NewGuid(), search));
        }

        [Test]
        public void SendHistoricRangeEmail_EndBeforeStart_ThrowsArgumentException()
        {
            var search = new HistoricRangeSearch { EmployeeId = Guid.NewGuid(), Start = DateTime.Now, End = DateTime.Now.AddDays(-1) };
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.SendHistoricRangeEmail(Guid.NewGuid(), search));
        }

        [Test]
        public void SendHistoricRangeEmail_EmployeeNotFound_ThrowsArgumentException()
        {
            var empId = Guid.NewGuid();
            var search = new HistoricRangeSearch { EmployeeId = empId, Start = DateTime.Now, End = DateTime.Now };
            _mockEmployeeDetailsRepository.Setup(r => r.GetByIdAsync(empId)).ReturnsAsync((EmployeeDetailsDto?)null);

            Assert.ThrowsAsync<ArgumentException>(async () => await _service.SendHistoricRangeEmail(Guid.NewGuid(), search));
        }

        [Test]
        public async Task SendHistoricRangeEmail_SendsEmail()
        {
            var empId = Guid.NewGuid();
            var search = new HistoricRangeSearch { EmployeeId = empId, Start = DateTime.Now.AddDays(-10), End = DateTime.Now };
            var employee = new EmployeeDetailsDto { FirstName = "John", LastName = "Doe" };
            var payrolls = new List<HistoricRangePayroll>
            {
                new HistoricRangePayroll { ContractType = "Full", JobPosition = "Dev", PayrollDate = DateTime.Now, BruteSalary = 1000, ObligatoryDeductions = 100, OptionalDeductions = 50, NetSalary = 850 }
            };
            var user = new AuthUserDto { Name = "Admin", LastName = "User", Email = "admin@company.com" };

            _mockEmployeeDetailsRepository.Setup(r => r.GetByIdAsync(empId)).ReturnsAsync(employee);
            _mockPayrollRepository.Setup(r => r.GetEmployeeHistoricRangePayrolls(empId, search.Start, search.End)).Returns(payrolls);
            _mockAuthService.Setup(r => r.GetAuthUser()).Returns(user);

            await _service.SendHistoricRangeEmail(Guid.NewGuid(), search);

            _mockEmailService.Verify(e => e.SendEmail(It.IsAny<ReportPayrollHistoricRangeEmail>()), Times.Once);
        }
    }
}
