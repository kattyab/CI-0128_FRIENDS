namespace Kaizen.Server.Application.Dtos.OwnerDashboard
{
    public class ContractCountResultDto
    {
        public string ContractType { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }

    public class LastPayrollDto
    {
        public string Period { get; set; } = string.Empty;
        public DateTime ExecutedOn { get; set; }
        public decimal TotalMoneyPaid { get; set; }
    }

    public class PayrollCostBreakdownDto
    {
        public decimal Beneficios { get; set; }
        public decimal Obligatorias { get; set; }
        public decimal Cargas { get; set; }
        public decimal Salarios { get; set; }
    }
}
