namespace Kaizen.Server.Models
{
    public class EmployeeDetailsDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Gender { get; set; }
        public required DateTime BirthDate { get; set; }
        public string? Province { get; set; }
        public string? Canton { get; set; }
        public string? OtherSigns { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public required decimal GrossSalary { get; set; }
        public required string ContractType { get; set; }
        public required DateTime StartDate { get; set; }
        public required string PayCycle { get; set; }
        public string? JobPosition { get; set; }
        public string? Role { get; set; }
        public required string Status { get; set; }
        public List<string> Benefits { get; set; } = new List<string>();
        public required string Email { get; set; }
    }
}