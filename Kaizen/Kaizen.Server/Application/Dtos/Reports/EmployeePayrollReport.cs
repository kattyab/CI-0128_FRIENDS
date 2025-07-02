namespace Kaizen.Server.Application.Dtos.Reports
{
    public class EmployeePayrollReport
    {
        public Guid PayrollID { get; set; }
        public string Period { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeFullName { get; set; }
        public string ContractType { get; set; }
        public DateTime PayDate { get; set; }
        public decimal BruteSalary { get; set; }

        public decimal SEM { get; set; }
        public decimal IVM { get; set; }
        public decimal EmployeeAportacionBancoPopular { get; set; }
        public decimal IncomeTax { get; set; } 

        public decimal TotalObligatoryDeductions { get; set; }

        public List<OptionalDeduction> OptionalDeductions { get; set; } = new List<OptionalDeduction>();
        public decimal TotalOptionalDeductions { get; set; }
        public decimal NetPay { get; set; }
    }

    public class OptionalDeduction
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
