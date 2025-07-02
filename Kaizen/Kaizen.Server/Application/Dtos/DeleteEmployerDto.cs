namespace Kaizen.Server.Application.Dtos.Employers
{
    public class DeleteEmployerDto
    {
        public Guid OwnerPK { get; set; }

        public bool OwnerIsDeleted { get; set; }

        public Guid? CompanyPK { get; set; }

        public bool? CompanyIsDeleted { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string ID { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public Guid? PaidBy { get; set; }
    }
}
