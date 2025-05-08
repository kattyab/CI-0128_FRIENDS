namespace Kaizen.Server.Models;
public class EmployeeDetailsDto
{
    public string EmpID { get; set; }
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Sex { get; set; }
    public DateTime BirthDate { get; set; }
    public string Province { get; set; }
    public string Canton { get; set; }
    public string OtherSigns { get; set; }
    public string JobPosition { get; set; }
    public string ContractType { get; set; }
    public int? WorkHours { get; set; }
    public int? ExtraHours { get; set; }
    public DateTime StartDate { get; set; }
    public string BankAccount { get; set; }
    public decimal GrossSalary { get; set; }
    public string PayCycle { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool Status { get; set; }
    public List<string> PhoneNumbers { get; set; }
}