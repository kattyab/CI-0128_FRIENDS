using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Services;
using Moq;
using NUnit.Framework;
using System;

namespace Tests.Services
{
  [TestFixture]
  public class EmployeesServiceTest
  {
    private Mock<IEmployeesRepository> _employeesRepositoryMock;
    private EmployeesService _employeesService;

    [SetUp]
    public void Setup()
    {
      _employeesRepositoryMock = new Mock<IEmployeesRepository>();
      _employeesService = new EmployeesService(_employeesRepositoryMock.Object);
    }

    [Test]
    public void GetEmployee_ReturnsEmployee_WhenExists()
    {
      var companyPK = Guid.NewGuid();
      var empID = Guid.NewGuid();
      var employeeDto = new EmployeeDto { Id = empID.ToString() };
      _employeesRepositoryMock
          .Setup(r => r.GetEmployee(companyPK, empID))
          .Returns(employeeDto);

      var result = _employeesService.GetEmployee(companyPK, empID);

      Assert.IsNotNull(result);
      Assert.AreEqual(empID.ToString(), result.Id);
    }

    [Test]
    public void GetEmployee_Throws_WhenNotFound()
    {
      var companyPK = Guid.NewGuid();
      var empID = Guid.NewGuid();
      _employeesRepositoryMock
          .Setup(r => r.GetEmployee(companyPK, empID))
          .Returns((EmployeeDto)null);

      var ex = Assert.Throws<KeyNotFoundException>(() => _employeesService.GetEmployee(companyPK, empID));
      StringAssert.Contains("Employee with ID", ex.Message);
    }

    [Test]
    public void DeleteEmployee_CallsRepository_WhenEmployeeExists()
    {
      var companyPK = Guid.NewGuid();
      var empID = Guid.NewGuid();
      var userId = Guid.NewGuid();
      var employeeDto = new EmployeeDto { Id = empID.ToString() };

      _employeesRepositoryMock
          .Setup(r => r.GetEmployee(companyPK, empID))
          .Returns(employeeDto);

      _employeesService.DeleteEmployee(companyPK, empID, userId);

      _employeesRepositoryMock.Verify(r => r.DeleteEmployee(companyPK, empID, userId), Times.Once);
    }

    [Test]
    public void DeleteEmployee_Throws_WhenEmployeeNotFound()
    {
      var companyPK = Guid.NewGuid();
      var empID = Guid.NewGuid();
      var userId = Guid.NewGuid();

      _employeesRepositoryMock
          .Setup(r => r.GetEmployee(companyPK, empID))
          .Returns((EmployeeDto)null);

      var ex = Assert.Throws<KeyNotFoundException>(() => _employeesService.DeleteEmployee(companyPK, empID, userId));
      StringAssert.Contains("Employee with ID", ex.Message);
    }
  }
}