using Castle.Core.Logging;
using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Configuraton;
using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Emails;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tests.Services
{
    [TestFixture]
    public class EmailServiceTest
    {
        private IConfiguration _configuration;
        private IEmailService _emailService;
        private Mock<ILogger<EmailService>> _logger;

        [SetUp]
        public void Setup()
        {
            this._configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
              .AddEnvironmentVariables()
              .Build();

            this._logger = new Mock<ILogger<EmailService>>();

            IOptions<EmailConfiguration> emailConfiguration = Options.Create(new EmailConfiguration());
            this._configuration.GetSection(nameof(EmailConfiguration)).Bind(emailConfiguration.Value);

            this._emailService = new EmailService(this._logger.Object, emailConfiguration);
        }

        [Test]
        public async Task SendEmail_Success()
        {
            TestEmail testEmail = new();
            testEmail.To.Add(new MailboxAddress("User", "user@example.com"));
            await this._emailService.SendEmail(testEmail);
            Assert.Pass("Email sent");
        }
    }
}
