namespace Kaizen.Server.Application.Dtos.BenefitDeductions
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public decimal BruteSalary { get; set; }
    }
}
