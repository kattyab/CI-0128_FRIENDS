namespace Kaizen.Server.Models
{
    public class Employees
    {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string ContractType { get; set; }
        public int WorkHours { get; set; }
        public int ExtraHours { get; set; }
        public DateTime StartDate { get; set; }
        public string BankAccount { get; set; }
        public decimal BruteSalary { get; set; }
        public string PayCycle { get; set; }
    }

}
