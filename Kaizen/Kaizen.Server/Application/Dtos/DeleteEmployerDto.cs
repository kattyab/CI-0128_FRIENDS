namespace Kaizen.Server.Application.Dtos.Employers
{
    public class DeleteEmployerDto
    {
        public Guid OwnerPK { get; set; }
        public bool OwnerIsDeleted { get; set; }
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid? UserPK { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? InCharge { get; set; }

    }
}
