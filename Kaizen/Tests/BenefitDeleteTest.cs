using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Configuration;
using Kaizen.Server.Infrastructure.Services;
using Kaizen.Server.Application.Interfaces.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Tests
{
    internal class BenefitDeleteTest
    {
        [TestFixture]
        public class BenefitsServiceTests
        {
            private Mock<IBenefitsRepository> _mockRepository;
            private Mock<IConfiguration> _mockConfiguration;
            private BenefitsService _service;

            [SetUp]
            public void SetUp()
            {
                _mockRepository = new Mock<IBenefitsRepository>();

                var inMemorySettings = new Dictionary<string, string>
                    {
                        {"ConnectionStrings:KaizenDb", "Server=test;Database=test;"}
                    };

                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();

                _service = new BenefitsService(_mockRepository.Object, configuration);
            }

            [Test]
            public void DeleteBenefit_WithEmptyGuid_ThrowsArgumentException()
            {
                var ex = Assert.Throws<ArgumentException>(() => _service.DeleteBenefit(Guid.Empty));
                Assert.That(ex.ParamName, Is.EqualTo("benefitId"));
            }

            [Test]
            public void DeleteBenefit_WithEmptyGuidAndValidId_ThrowsArgumentException()
            {
                var ex = Assert.Throws<ArgumentException>(() => _service.DeleteBenefit(1,Guid.Empty));
                Assert.That(ex.ParamName, Is.EqualTo("companyPK"));
            }

            [Test]
            public void DeleteBenefit_WithValidGuidAndNegativeId_ThrowsArgumentException()
            {
                Guid guid = Guid.NewGuid();
                var ex = Assert.Throws<ArgumentException>(() => _service.DeleteBenefit(-1, guid));
                Assert.That(ex.ParamName, Is.EqualTo("id"));
            }
        }
    }
}
