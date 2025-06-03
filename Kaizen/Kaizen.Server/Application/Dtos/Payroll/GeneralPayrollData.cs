namespace Kaizen.Server.Application.Dtos.Payroll
{
    public class GeneralPayrollData
    {
        public decimal TotalDeductionsBenefits { get; set; }
        public decimal TotalObligatoryDeductions { get; set; }
        public decimal TotalLaborCharges { get; set; }
        public decimal TotalMoneyPaid { get; set; }
        public DateTime StartDate { get; set; }
    }
}
