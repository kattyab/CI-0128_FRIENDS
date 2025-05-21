namespace Kaizen.Server.Application.Dtos
{
    public class CompaniesListDto
    {
        public Guid CompanyPK { get; set; } = default!;
        public string CompanyID { get; set; } = default!;
        public Guid OwnerPK { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string OwnerName { get; set; } = default!;
        public int EmployeesCount { get; set; } = default!;
    }
}
