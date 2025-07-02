using NUnit.Framework;
using Moq;
using System;
using System.Text;
using System.Threading.Tasks;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Emails;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Services;
using Kaizen.Server.Application.Interfaces.Employees;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Tests.Services
{
    [TestFixture]
    public class SendCompaniesHistoricEmailTest
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<ICompaniesRepository> _mockCompaniesRepository;
        private Mock<IEmployeesRepository> _mockEmployeesRepository;
        private Mock<IEmployeeRepository> _mockEmployeeDetailsRepository;
        private Mock<IPayrollRepository> _mockPayrollRepository;
        private Mock<IEmailService> _mockEmailService;

        private ReportService _reportService;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockCompaniesRepository = new Mock<ICompaniesRepository>();
            _mockEmployeesRepository = new Mock<IEmployeesRepository>();
            _mockEmployeeDetailsRepository = new Mock<IEmployeeRepository>();
            _mockPayrollRepository = new Mock<IPayrollRepository>();
            _mockEmailService = new Mock<IEmailService>();

            _reportService = new ReportService(
                _mockAuthService.Object,
                _mockCompaniesRepository.Object,
                _mockEmployeesRepository.Object,
                _mockEmployeeDetailsRepository.Object,
                _mockPayrollRepository.Object,
                _mockEmailService.Object
            );
        }

        [Test]
        public async Task SendCompaniesHistoricEmail_ShouldSendEvenIfCsvIsEmpty()
        {

            string csvContent = "";
            var user = new AuthUserDto { Name = "Ana", LastName = "Morales", Email = "ana@example.com" };

            _mockAuthService.Setup(x => x.GetAuthUser()).Returns(user);
            ReportPayrollHistoricRangeEmail capturedEmail = null;

            _mockEmailService
                .Setup(x => x.SendEmail(It.IsAny<ReportPayrollHistoricRangeEmail>()))
                .Callback<ReportPayrollHistoricRangeEmail>(email => capturedEmail = email)
                .Returns(Task.CompletedTask);

            await _reportService.SendCompaniesHistoricEmail(csvContent);


            Assert.NotNull(capturedEmail);
            Assert.That(Encoding.UTF8.GetString(capturedEmail.Attachments[0].Content), Is.EqualTo(""));
        }

        [Test]
        public void SendCompaniesHistoricEmail_ShouldThrow_WhenUserEmailIsInvalid()
        {
            string csvContent = "nombre,fecha\nX,Y";
            var user = new AuthUserDto { Name = "Mario", LastName = "López", Email = "" };

            _mockAuthService.Setup(x => x.GetAuthUser()).Returns(user);

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _reportService.SendCompaniesHistoricEmail(csvContent));
            StringAssert.Contains("invalid email", ex.Message.ToLower());
        }

        [Test]
        public void SendCompaniesHistoricEmail_ShouldThrow_WhenUserEmailIsNull()
        {
            string csvContent = "some,data";
            var user = new AuthUserDto { Name = "Jose", LastName = "Ramírez", Email = null };

            _mockAuthService.Setup(x => x.GetAuthUser()).Returns(user);

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _reportService.SendCompaniesHistoricEmail(csvContent));
            StringAssert.Contains("invalid email", ex.Message.ToLower());
        }

        [Test]
        public void SendCompaniesHistoricEmail_ShouldThrow_WhenSendEmailFails()
        {
            var user = new AuthUserDto { Name = "Luisa", LastName = "Perez", Email = "luisa@example.com" };
            string csvContent = "a,b,c";

            _mockAuthService.Setup(x => x.GetAuthUser()).Returns(user);
            _mockEmailService.Setup(x => x.SendEmail(It.IsAny<ReportPayrollHistoricRangeEmail>()))
                             .ThrowsAsync(new InvalidOperationException("SMTP failure"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _reportService.SendCompaniesHistoricEmail(csvContent));
            StringAssert.Contains("smtp failure", ex.Message.ToLower());
        }

        [Test]
        public async Task SendCompaniesHistoricEmail_ShouldUseCorrectDateFormat()
        {
            var user = new AuthUserDto { Name = "Luis", LastName = "Vega", Email = "luis@example.com" };
            string csvContent = "X,Y,Z";

            _mockAuthService.Setup(x => x.GetAuthUser()).Returns(user);

            ReportPayrollHistoricRangeEmail captured = null;
            _mockEmailService
                .Setup(x => x.SendEmail(It.IsAny<ReportPayrollHistoricRangeEmail>()))
                .Callback<ReportPayrollHistoricRangeEmail>(email => captured = email)
                .Returns(Task.CompletedTask);

            await _reportService.SendCompaniesHistoricEmail(csvContent);

            Assert.NotNull(captured);
            Assert.DoesNotThrow(() => DateTime.ParseExact(captured.Start, "yyyy-MM-dd", null));
            Assert.DoesNotThrow(() => DateTime.ParseExact(captured.End, "yyyy-MM-dd", null));
        }
    }
}
