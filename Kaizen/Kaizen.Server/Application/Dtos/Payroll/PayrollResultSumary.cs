namespace Kaizen.Server.Application.Dtos.Payroll
{
    public sealed class PayrollResultSumary
    {
        public bool IsSuccess { get; set; } = true;
        public List<Guid> FailedPayrolls { get; set; } = default!;
        public List<decimal> FailedNetSalaries { get; set; } = default!;
        public Guid CompanyId { get; set; }
        public string Manager { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Period { get; set; } = default!;
        public decimal Gross { get; set; }
        public decimal Deductions { get; set; }
        public decimal SocialCharges { get; set; }
        public decimal Net { get; set; }
        public decimal TotalPaid { get; set; }
    }
}
