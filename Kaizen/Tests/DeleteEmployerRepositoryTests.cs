using System;
using System.Collections.Generic;
using Kaizen.Server.Application.Dtos.Employers;
using Kaizen.Server.Application.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace Kaizen.Tests.Repositories
{
    [TestFixture]
    public class DeleteEmployerRepositoryTests
    {
        private Mock<IDeleteEmployerRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IDeleteEmployerRepository>();
        }

        [Test]
        public void GetEmployersWithCompanyAndPersonData_ShouldReturnList()
        {

            var expectedList = new List<DeleteEmployerDto>
            {
                new DeleteEmployerDto
                {
                    OwnerPK = Guid.NewGuid(),
                    ID = "1-1111-1111",
                    Name = "Ana",
                    LastName = "Pérez",
                    Email = "ana@example.com",
                    InCharge = null
                }
            };

            _repositoryMock.Setup(repo => repo.GetEmployersWithCompanyAndPersonData())
                .Returns(expectedList);


            var result = _repositoryMock.Object.GetEmployersWithCompanyAndPersonData();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, ((List<DeleteEmployerDto>)result).Count);
            Assert.AreEqual("Ana", ((List<DeleteEmployerDto>)result)[0].Name);
        }

        [Test]
        public void SoftDeleteEmployer_WhenCalledWithValidId_ShouldReturnTrue()
        {

            var ownerPK = Guid.NewGuid();
            _repositoryMock.Setup(r => r.SoftDeleteEmployer(ownerPK)).Returns(true);

            var result = _repositoryMock.Object.SoftDeleteEmployer(ownerPK);

            Assert.IsTrue(result);
        }

        [Test]
        public void HardDeleteEmployer_WhenCalledWithValidId_ShouldReturnTrue()
        {
            var ownerPK = Guid.NewGuid();
            _repositoryMock.Setup(r => r.HardDeleteEmployer(ownerPK)).Returns(true);

            var result = _repositoryMock.Object.HardDeleteEmployer(ownerPK);

            Assert.IsTrue(result);
        }
    }
}
