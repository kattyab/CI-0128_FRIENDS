namespace Kaizen.Server.Application.Dtos
{
    public class GeneralPayrollReportDto
    {
        public Guid GeneralPayrollsID { get; set; }
        public Guid PaidBy { get; set; }
        public decimal TotalDeductionsBenefits { get; set; }
        public decimal TotalObligatoryDeductions { get; set; }
        public decimal TotalLaborCharges { get; set; }
        public decimal TotalMoneyPaid { get; set; }
        public DateTime ExecutedOn { get; set; }
        public string PayrollMode { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string InCharge { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
