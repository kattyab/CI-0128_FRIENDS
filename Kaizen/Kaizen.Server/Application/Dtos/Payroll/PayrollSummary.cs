using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Dtos.Payroll
{
    public class PayrollSummary
    {
        public Guid EmployeeId { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool RegistersHours { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public Dictionary<string, decimal> ApiDeductions { get; set; } = new();
        public List<BenefitDeductionResult> BenefitDeductions { get; set; } = new();
        public decimal CCSSDeduction { get; set; }
        public decimal IncomeTax { get; set; }
        public Guid PayrollId { get; set; }
    }
}
