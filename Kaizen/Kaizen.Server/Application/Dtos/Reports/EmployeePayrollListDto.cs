namespace Kaizen.Server.Application.Dtos.Reports
{
    public class EmployeePayrollListDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string TipoEmpleado { get; set; } = string.Empty;
        public string PeriodoPago { get; set; } = string.Empty;
        public string FechaPago { get; set; } = string.Empty;
        public decimal SalarioBruto { get; set; }
        public decimal CargasSociales { get; set; }
        public decimal DeduccionesVoluntarias { get; set; }
        public decimal CostoEmpleador { get; set; }
    }
}
