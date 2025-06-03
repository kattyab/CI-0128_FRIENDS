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
            // Período de nómina: del 1 de mayo de 2025 al 30 de mayo de 2025
            DateTime periodoInicio = new DateTime(2025, 5, 1);
            DateTime periodoFin = new DateTime(2025, 5, 30);

            // Empleado empieza antes del periodo y no está despedido
            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 4, 15),
                FireDate = null
            };

            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            // Como no hubo despido dentro del periodo y empezó antes,
            // debe cubrir TODO el intervalo: del 1 al 30 de mayo = 30 días (cero basado a 1)
            Assert.AreEqual(30, diasTrabajados);
        }

        [Test]
        public void Calculate_PartialPeriod_WhenEmpleadoIngresaEnMedioDelPeriodo_RetornaCantidadCorrecta()
        {
            // Arrange
            // Período de nómina: del 1 de junio de 2025 al 30 de junio de 2025
            DateTime periodoInicio = new DateTime(2025, 6, 1);
            DateTime periodoFin = new DateTime(2025, 6, 30);

            // Empleado entra el 10 de junio de 2025, sin fecha de despido
            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 6, 10),
                FireDate = null
            };
            // Entonces el cálculo debería ser: desde el 10 (inclusive) hasta el 30 (inclusive).
            // 30 - 10 = 20 días, pero como contamos inclusivo sumamos +1 → 21 días en total.

            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            Assert.AreEqual(21, diasTrabajados);
        }

        [Test]
        public void Calculate_Zero_WhenEmpleadoDespedidoAntesDelPeriodo_RetornaCero()
        {
            // Arrange
            // Período de nómina: del 1 de julio de 2025 al 31 de julio de 2025
            DateTime periodoInicio = new DateTime(2025, 7, 1);
            DateTime periodoFin = new DateTime(2025, 7, 31);

            // Empleado fue despedido (FireDate) el 20 de junio de 2025, es decir, antes de que inicie el periodo.
            var empleado = new EmployeePayroll
            {
                StartDate = new DateTime(2025, 5, 1),      // Empezó con anterioridad
                FireDate = new DateTime(2025, 6, 20)
            };

            // Act
            int diasTrabajados = _calculator.Calculate(empleado, periodoInicio, periodoFin);

            // Assert
            // Puesto que FireDate < periodoInicio, el método debe detectar que no trabajó ningún día
            Assert.AreEqual(0, diasTrabajados);
        }
    }
}
