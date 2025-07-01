namespace Kaizen.Server.Application.Dtos.Reports
{
    public class EmployeePayrollListDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string TipoEmpleado { get; set; } = string.Empty;
        public string PeriodoPago { get; set; } = string.Empty;
        public string FechaPago { get; set; } = string.Empty;
        public string SalarioBruto { get; set; } = string.Empty;
        public string CargasSociales { get; set; } = string.Empty;
        public string DeduccionesVoluntarias { get; set; } = string.Empty;
        public string CostoEmpleador { get; set; } = string.Empty;
    }
}
