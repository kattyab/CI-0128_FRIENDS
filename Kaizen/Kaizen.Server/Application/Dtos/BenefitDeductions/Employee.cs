namespace Kaizen.Server.Application.Dtos.BenefitDeductions
{
    public class Employee
    {
        public Guid EmpID { get; set; }
        public string ContractType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public decimal BruteSalary { get; set; }
        public Guid WorksFor { get; set; }
    }
}
