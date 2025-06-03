namespace Kaizen.Server.Application.Dtos.Payroll
{
    public class EmployeePayroll
    {
        public Guid EmpID { get; set; }
        public decimal BruteSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FireDate { get; set; }
        public string ContractType { get; set; }
        public bool RegistersHours { get; set; }
        public string PayrollTypeDescription { get; set; }
    }
}
