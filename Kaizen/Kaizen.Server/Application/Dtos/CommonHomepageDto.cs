namespace Kaizen.Server.Application.Dtos
{
    public class CommonHomepageDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Role { get; set; }
        public string? JobPosition { get; set; }
        public required string ContractType { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public required string Email { get; set; }
        public string? Province { get; set; }
        public string? Canton { get; set; }
        public string? OtherSigns { get; set; }
        public required decimal GrossSalary { get; set; }
    }
}

