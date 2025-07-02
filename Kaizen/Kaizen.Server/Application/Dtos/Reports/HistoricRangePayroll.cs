namespace Kaizen.Server.Application.Dtos.Reports
{
    public class HistoricRangePayroll
    {
        public string ContractType { get; set; } = default!;
        public string JobPosition { get; set; } = default!;
        public DateTime PayrollDate { get; set; } = default!;
        public decimal BruteSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal ObligatoryDeductions { get; set; }
        public decimal OptionalDeductions { get; set; }
    }
}
