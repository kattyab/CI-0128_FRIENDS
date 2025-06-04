using NUnit.Framework;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Application.Dtos.Payroll;
using System;

namespace Kaizen.Server.Application.Tests.Payroll
{
    [TestFixture]
    public class DaysWorkedCalculatorTests
    {
        private DaysWorkedCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new DaysWorkedCalculator();
        }

        [Test]
        public void Calculate_FullPeriod_WhenEmpleadoActivoTodoElPeriodo_RetornaMaximoDias()
        {
            // Arrange
            DateTime periodoInicio = new DateTime(2025, 5, 1);
            DateTime periodoFin = new DateTime(2025, 5, 30);

            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 4, 15),
                FireDate = null
            };

            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            Assert.AreEqual(30, diasTrabajados);
        }

        [Test]
        public void Calculate_PartialPeriod_WhenEmpleadoIngresaEnMedioDelPeriodo_RetornaCantidadCorrecta()
        {
            // Arrange

            DateTime periodoInicio = new DateTime(2025, 6, 1);
            DateTime periodoFin = new DateTime(2025, 6, 30);

            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 6, 10),
                FireDate = null
            };


            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            Assert.AreEqual(21, diasTrabajados);
        }

        [Test]
        public void Calculate_Zero_WhenEmpleadoDespedidoAntesDelPeriodo_RetornaCero()
        {
            // Arrange

            DateTime periodoInicio = new DateTime(2025, 7, 1);
            DateTime periodoFin = new DateTime(2025, 7, 31);

            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 5, 1),   
                FireDate = new DateTime(2025, 6, 20)
            };

            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            Assert.AreEqual(0, diasTrabajados);
        }
    }
}
