namespace Kaizen.Server.Application.Dtos.Payroll
{
    public sealed class PayrollResultSumary
    {
        /// <summary>
        /// Indicates whether the payroll processing was successful for all employees.
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// List of employee IDs for whom the payroll processing failed.
        /// </summary>
        public List<Guid> FailedPayrolls { get; set; } = default!;
        public Guid CompanyId { get; set; }
        public string Manager { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Period { get; set; } = default!;
        public decimal Gross { get; set; }
        /// <summary>
        /// Total deductions for all employees, including benefits and APIs.
        /// </summary>
        public decimal Deductions { get; set; }
        /// <summary>
        /// 26.67% of total gross salary.
        /// </summary>
        public decimal SocialCharges { get; set; }
        /// <summary>
        /// Sum of all net salaries.
        /// </summary>
        public decimal Net { get; set; }
        /// <summary>
        /// Total amount paid by company salaries and social charges.
        /// </summary>
        public decimal TotalPaid { get; set; }
    }
}