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
        public decimal Benefits { get; set; }
        public decimal ObligatoryDeductions { get; set; }
        public decimal LaborCharges { get; set; }
        public decimal Salaries { get; set; }
    }
}
